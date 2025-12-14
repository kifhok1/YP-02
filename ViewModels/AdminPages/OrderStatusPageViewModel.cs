using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using VKR.Models;
using VKR.Views.Dialogs;

namespace VKR.ViewModels.AdminPages;

// ViewModel для страницы управления статусами заказов
public partial class OrderStatusPageViewModel: ViewModelBase
{
    // Конструктор ViewModel
    public OrderStatusPageViewModel(Window window)
    {
        _window = window;
        DataGridFill(); // Загрузка данных при инициализации
    }
    
    private Window _window;
    
    private ObservableCollection<SimpleDataType> _simpleData;
    private string _nameStatusAdd;
    private string _nameStatusUpdate;
    private bool _isVisibleAddPanel = false;
    private bool _isVisibleUpdatePanel = false;
    private SimpleDataType _selectedSimpleDataType;

    // Свойство видимости панели добавления статуса
    public bool IsVisibleAddPanel
    {
        get { return _isVisibleAddPanel; }
        set { _isVisibleAddPanel = value; OnPropertyChanged(); }
    }
    
    // Свойство видимости панели редактирования статуса
    public bool IsVisibleUpdatePanel
    {
        get { return _isVisibleUpdatePanel; }
        set { _isVisibleUpdatePanel = value; OnPropertyChanged(); }
    }
    
    // Название нового статуса для добавления
    public string NameStatusAdd
    {
        get { return _nameStatusAdd; }
        set { _nameStatusAdd = value; OnPropertyChanged(); }
    } 
    
    // Название статуса для редактирования
    public string NameStatusUpdate
    {
        get { return _nameStatusUpdate; }
        set { _nameStatusUpdate = value; OnPropertyChanged(); }
    }

    // Коллекция статусов для отображения в DataGrid
    public ObservableCollection<SimpleDataType> SimpleData
    {
        get { return _simpleData; }
        set { _simpleData = value; OnPropertyChanged(); }
    }

    // Выбранный статус в DataGrid
    public SimpleDataType SelectedSimpleDataType
    {
        get { return _selectedSimpleDataType; }
        set { _selectedSimpleDataType = value; OnPropertyChanged(); }
    }

    // Метод для загрузки данных статусов заказов в DataGrid
    private void DataGridFill()
    {
        string query = "SELECT Id_OrderStatus AS 'ID', NameStatus AS 'Name' FROM vkr.orderstatus;";
        SimpleData = SelectTabelSimpleDataType.SelectTable(query, ConnectToDB.ConnectToDBString());
    }

    // Команда для отображения панели добавления статуса
    [RelayCommand]
    private void ShowAddCategory()
    {
        IsVisibleAddPanel = true;
    }
    
    // Команда для добавления нового статуса заказа
    [RelayCommand]
    private async void StatusAdd()
    {
        // Проверка на пустое название статуса
        if (string.IsNullOrEmpty(NameStatusAdd))
        {
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Строка не должна быть пустой", "323")
            };
            await err.ShowDialog(_window);
            return;
        }
        
        // Диалог подтверждения добавления
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel = new YesOrNotDialogViewModel("Внимание", "Вы хотите добавить новый статус?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        
        // Если пользователь подтвердил добавление
        if (warningViewModel.Flag)
        {
            // Проверка на уникальность названия статуса
            foreach (var simpleDataType in SimpleData)
            {
                if (simpleDataType.Name == NameStatusAdd)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Данный статус уже сущевствует", "323")
                    };
                    await err.ShowDialog(_window);
                    return;
                }
            }
            
            // SQL-запрос для добавления статуса
            string query = $"INSERT INTO `vkr`.`orderstatus` (`NameStatus`) VALUES ('{NameStatusAdd}');";
            
            try
            {
                // Выполнение запроса на добавление
                SelectTabelSimpleDataType.InsertData(query);
                
                // Обновление данных и интерфейса
                DataGridFill();
                IsVisibleAddPanel = false;
                NameStatusAdd = "";
                
                // Уведомление об успехе
                InfoDialogWindow info = new InfoDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Успех", "Статус добавлен", "323")
                };
                await info.ShowDialog(_window);
            }
            catch (Exception ex)
            {
                // Обработка ошибки при добавлении
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Во время добавдения стстуса возникла ошибка", "323")
                };
                await err.ShowDialog(_window);
            }
        }
    }

    // Команда для редактирования существующего статуса
    [RelayCommand]
    private async void StatusUpdate()
    {
        // Проверка на пустое название статуса
        if (string.IsNullOrEmpty(NameStatusUpdate))
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
        YesOrNotDialogViewModel warningViewModel = new YesOrNotDialogViewModel("Внимание", "Вы хотите изменить статус?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        
        // Если пользователь подтвердил редактирование
        if (warningViewModel.Flag)
        {
            // Проверка на уникальность нового названия статуса
            foreach (var simpleDataType in SimpleData)
            {
                if (simpleDataType.Name == NameStatusUpdate)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Данный статус уже сущевствует", "323")
                    };
                    await err.ShowDialog(_window);
                    return;
                }
            }
            
            // SQL-запрос для обновления статуса
            string query = $"UPDATE `vkr`.`orderstatus` SET `NameStatus` = '{NameStatusUpdate}' WHERE (`Id_OrderStatus` = '{SelectedSimpleDataType.Id}');";
            
            try
            {
                // Выполнение запроса на обновление
                SelectTabelSimpleDataType.UpdateData(query);
                
                // Обновление данных и интерфейса
                DataGridFill();
                IsVisibleUpdatePanel = false;
                NameStatusUpdate = "";
                
                // Уведомление об успехе
                InfoDialogWindow info = new InfoDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Успех", "Статус изменён", "323")
                };
                await info.ShowDialog(_window);
            }
            catch (Exception ex)
            {
                // Обработка ошибки при редактировании
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Во время изменения статуса возникла ошибка", "323")
                };
                await err.ShowDialog(_window);
            }
        }
    }
}