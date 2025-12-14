using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using VKR.Models;
using VKR.Views.Dialogs;

namespace VKR.ViewModels.AdminPages;

// ViewModel для страницы управления ролями пользователей
public partial class RolePageViewModel : ViewModelBase
{
    private Window _window;
    private ObservableCollection<SimpleDataType> _simpleData;
    private bool _isVisiblePanel = false;
    private string _nameRole;
    private SimpleDataType _selectedSimpleDataType;

    // Название роли для редактирования
    public string NameRole
    {
        get => _nameRole;
        set
        {
            _nameRole = value;
            OnPropertyChanged();
        }
    }

    // Коллекция ролей для отображения в DataGrid
    public ObservableCollection<SimpleDataType> SimpleData
    {
        get { return _simpleData; }
        set { _simpleData = value; OnPropertyChanged(); }
    }

    // Свойство видимости панели редактирования роли
    public bool IsVisiblePanelUpdate
    {
        get { return _isVisiblePanel; }
        set
        {
            SetProperty(ref _isVisiblePanel, value);
            OnPropertyChanged();
        }
    }

    // Выбранная роль в DataGrid
    public SimpleDataType SelectedSimpleDataType
    {
        get { return _selectedSimpleDataType; }
        set { _selectedSimpleDataType = value; OnPropertyChanged(); }
    }

    // Конструктор ViewModel
    public RolePageViewModel(Window window)
    {
        _window = window;
        DataGridFill(); // Загрузка данных при инициализации
    }

    // Метод для загрузки данных ролей в DataGrid
    private void DataGridFill()
    {
        string query = "SELECT ID_Rule AS 'ID', Rule AS 'Name' FROM Rule";
        SimpleData = SelectTabelSimpleDataType.SelectTable(query, ConnectToDB.ConnectToDBString());
    }
    
    // Команда для редактирования существующей роли
    [RelayCommand]
    private async void RoleUpdate()
    {
        // Проверка на пустое название роли
        if (string.IsNullOrEmpty(NameRole))
        {
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Строка не должна быть пустой", "323")
            };
            await err.ShowDialog(_window);
            return;
        }
        
        // Диалог подтверждения редактирования
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel = new YesOrNotDialogViewModel("Внимание", "Вы хотите изменить роль?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        
        // Если пользователь подтвердил редактирование
        if (warningViewModel.Flag)
        {
            // Проверка на уникальность нового названия роли
            foreach (var simpleDataType in SimpleData)
            {
                if (simpleDataType.Name == NameRole)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Данная роль уже сущевствует", "323")
                    };
                    await err.ShowDialog(_window);
                    return;
                }
            }
            
            // SQL-запрос для обновления роли
            string query = $"UPDATE `vkr`.`rule` SET `Rule` = '{NameRole}' WHERE (`ID_Rule` = '{SelectedSimpleDataType.Id}');";
            
            try
            {
                // Выполнение запроса на обновление
                SelectTabelSimpleDataType.UpdateData(query);
                
                // Обновление данных и интерфейса
                DataGridFill();
                NameRole = "";
                IsVisiblePanelUpdate = false;
                
                // Уведомление об успехе
                InfoDialogWindow info = new InfoDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Успех", "Роль изменена", "323")
                };
                await info.ShowDialog(_window);
            }
            catch (Exception ex)
            {
                // Обработка ошибки при редактировании
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Во время изменения роли возникла ошибка", "323")
                };
                await err.ShowDialog(_window);
            }
        }
    }
}