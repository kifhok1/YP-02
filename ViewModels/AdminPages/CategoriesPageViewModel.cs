using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using VKR.Models;
using VKR.Views.Dialogs;

namespace VKR.ViewModels.AdminPages;

// ViewModel для страницы управления категориями товаров
public partial class CategoriesPageViewModel : ViewModelBase
{
    private Window window;
    private ObservableCollection<SimpleDataType> _simpleData;
    private string _nameCategoriesAdd;
    private string _nameCategoriesUpdate;
    private bool _isVisibleAddPanel = false;
    private bool _isVisibleUpdatePanel = false;
    private SimpleDataType _selectedSimpleDataType;

    // Свойство видимости панели добавления категории
    public bool IsVisibleAddPanel
    {
        get { return _isVisibleAddPanel; }
        set { _isVisibleAddPanel = value; OnPropertyChanged(); }
    }
    
    // Свойство видимости панели редактирования категории
    public bool IsVisibleUpdatePanel
    {
        get { return _isVisibleUpdatePanel; }
        set { _isVisibleUpdatePanel = value; OnPropertyChanged(); }
    }
    
    // Название новой категории для добавления
    public string NameCategoriesAdd
    {
        get { return _nameCategoriesAdd; }
        set { _nameCategoriesAdd = value; OnPropertyChanged(); }
    } 
    
    // Название категории для редактирования
    public string NameCategoriesUpdate
    {
        get { return _nameCategoriesUpdate; }
        set { _nameCategoriesUpdate = value; OnPropertyChanged(); }
    }

    // Коллекция категорий для отображения в DataGrid
    public ObservableCollection<SimpleDataType> SimpleData
    {
        get { return _simpleData; }
        set { _simpleData = value; OnPropertyChanged(); }
    }

    // Выбранная категория в DataGrid
    public SimpleDataType SelectedSimpleDataType
    {
        get { return _selectedSimpleDataType; }
        set { _selectedSimpleDataType = value; OnPropertyChanged(); }
    }
    
    // Конструктор ViewModel
    public CategoriesPageViewModel(Window _window)
    {
        window = _window;
        DataGridFill(); // Загрузка данных при инициализации
    }

    // Метод для загрузки данных категорий в DataGrid
    private void DataGridFill()
    {
        string query = "SELECT ID_CategoryProduct AS 'ID', CategoryProduct AS 'Name' FROM categoryproduct";
        SimpleData = SelectTabelSimpleDataType.SelectTable(query, ConnectToDB.ConnectToDBString());
    }

    // Команда для отображения панели добавления категории
    [RelayCommand]
    private void ShowAddCategory()
    {
        IsVisibleAddPanel = true;
    }
    
    // Команда для скрытия панели добавления категории
    [RelayCommand]
    private void HideAddCategory()
    {
        IsVisibleAddPanel = false;
    }
    
    // Команда для добавления новой категории с проверками
    [RelayCommand]
    private async void CategoriesAdd()
    {
        // Проверка на пустое название категории
        if (string.IsNullOrEmpty(NameCategoriesAdd))
        {
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Строка не должна быть пустой", "323")
            };
            await err.ShowDialog(window);
            return;
        }
        
        // Диалог подтверждения добавления
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel = new YesOrNotDialogViewModel("Внимание", "Вы хотите добавить новую категорию?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(window);
        
        // Если пользователь подтвердил добавление
        if (warningViewModel.Flag)
        {
            // Проверка на уникальность названия категории
            foreach (var simpleDataType in SimpleData)
            {
                if (simpleDataType.Name == NameCategoriesAdd)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Данная категория уже сущевствует", "323")
                    };
                    await err.ShowDialog(window);
                    return;
                }
            }
            
            // SQL-запрос для добавления категории
            string query = $"INSERT INTO `vkr`.`categoryproduct` (`CategoryProduct`) VALUES ('{NameCategoriesAdd}');";
            
            try
            {
                // Выполнение запроса на добавление
                SelectTabelSimpleDataType.InsertData(query);
                
                // Обновление данных и интерфейса
                DataGridFill();
                IsVisibleAddPanel = false;
                NameCategoriesAdd = "";
                
                // Уведомление об успехе
                InfoDialogWindow info = new InfoDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Успех", "Категория добавлена", "323")
                };
                await info.ShowDialog(window);
            }
            catch (Exception ex)
            {
                // Обработка ошибки при добавлении
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Во время добавдения категории возникла ошибка", "323")
                };
                await err.ShowDialog(window);
            }
        }
    }

    // Команда для редактирования существующей категории
    [RelayCommand]
    private async void CategoriesUpdate()
    {
        // Проверка на пустое название категории
        if (string.IsNullOrEmpty(NameCategoriesUpdate))
        {
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Строка не должна быть пустой", "323")
            };
            await err.ShowDialog(window);
            return;
        }
        
        // Диалог подтверждения редактирования
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel = new YesOrNotDialogViewModel("Внимание", "Вы хотите изменить категорию?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(window);
        
        // Если пользователь подтвердил редактирование
        if (warningViewModel.Flag)
        {
            // Проверка на уникальность нового названия
            foreach (var simpleDataType in SimpleData)
            {
                if (simpleDataType.Name == NameCategoriesUpdate)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Данная категория уже сущевствует", "323")
                    };
                    await err.ShowDialog(window);
                    return;
                }
            }
            
            // SQL-запрос для обновления категории
            string query = $"UPDATE `vkr`.`categoryproduct` SET `CategoryProduct` = '{NameCategoriesUpdate}' WHERE (`ID_CategoryProduct` = '{SelectedSimpleDataType.Id}');";
            
            try
            {
                // Выполнение запроса на обновление
                SelectTabelSimpleDataType.UpdateData(query);
                
                // Обновление данных и интерфейса
                DataGridFill();
                IsVisibleUpdatePanel = false;
                NameCategoriesUpdate = "";
                
                // Уведомление об успехе
                InfoDialogWindow info = new InfoDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Успех", "Категория изменена", "323")
                };
                await info.ShowDialog(window);
            }
            catch (Exception ex)
            {
                // Обработка ошибки при редактировании
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Во время изменения категории возникла ошибка", "323")
                };
                await err.ShowDialog(window);
            }
        }
    }
}