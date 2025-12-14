using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using VKR.Models;
using VKR.Views.Dialogs;

namespace VKR.ViewModels.AdminPages;

// ViewModel для страницы управления пользователями (сотрудниками)
public partial class UsersPageViewModel : ViewModelBase
{
    // Приватные поля для хранения данных
    private bool _isVisibleImage = false;
    private bool _isVisibleBorder = true;
    private bool _insertButtonEnabled = false;
    private Bitmap? _imageUser = null;
    private byte[] _imageBlob;
    private SimpleDataType _roleUserSelected;
    private List<SimpleDataType> _roleUser = new List<SimpleDataType>();
    private string _phoneNumber = "+7("; // Начальный формат номера телефона
    private string _fio = "";
    private Worker _selectedWorker;
    private string _login = "";
    private string _password = "";
    private bool _elementVisible = false;
    private string loginUser; // Логин текущего пользователя для проверки удаления самого себя
    private Window _window;

    // Выбранный сотрудник в DataGrid
    public Worker SelectedWorker
    {
        get { return _selectedWorker; }
        set { _selectedWorker = value; }
    }

    // Видимость элемента управления (панели редактирования)
    public bool ElementVisible
    {
        get { return _elementVisible; }
        set { _elementVisible = value; }
    }
    
    // Список доступных ролей для выбора
    public List<SimpleDataType> RoleUser
    {
        get => _roleUser;
        set => SetProperty(ref _roleUser, value);
    }

    // Доступность кнопки сохранения изменений
    public bool InsertButtonEnabled
    {
        get => _insertButtonEnabled;
        set => SetProperty(ref _insertButtonEnabled, value);
    }

    // Видимость загруженного изображения
    public bool IsVisibleImage
    {
        get => _isVisibleImage;
        set => SetProperty(ref _isVisibleImage, value);
    }

    // Видимость рамки для изображения (когда изображение не загружено)
    public bool IsVisibleBorder
    {
        get => _isVisibleBorder;
        set => SetProperty(ref _isVisibleBorder, value);
    }

    // Изображение профиля пользователя
    public Bitmap? ImageUser
    {
        get => _imageUser;
        set
        {
            SetProperty(ref _imageUser, value);
            EnabledInsertButton(); // Проверка доступности кнопки при изменении изображения
        }
    }

    // Выбранная роль пользователя
    public SimpleDataType RoleUserSelected
    {
        get => _roleUserSelected;
        set
        {
            SetProperty(ref _roleUserSelected, value);
            EnabledInsertButton(); // Проверка доступности кнопки при изменении роли
        }
    }

    // Номер телефона пользователя
    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            SetProperty(ref _phoneNumber, value);
            EnabledInsertButton(); // Проверка доступности кнопки при изменении номера
        }
    }

    // ФИО пользователя
    public string Fio
    {
        get => _fio;
        set
        {
            SetProperty(ref _fio, value);
            EnabledInsertButton(); // Проверка доступности кнопки при изменении ФИО
        }
    }

    // Логин пользователя
    public string Login
    {
        get => _login;
        set
        {
            SetProperty(ref _login, value);
            EnabledInsertButton(); // Проверка доступности кнопки при изменении логина
        }
    }

    // Пароль пользователя (необязательное поле при обновлении)
    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            EnabledInsertButton(); // Проверка доступности кнопки при изменении пароля
        }
    }

    // Базовый SQL-запрос для выборки сотрудников
    private string _queryBase = "SELECT ID_Worker AS 'ID', Login, Password, Fio AS 'FIO', PhoneNumber, workers.rule AS 'RuleID', r.Rule AS 'RuleView', Image  FROM workers INNER JOIN vkr.rule r on workers.rule = r.ID_Rule";

    // Текущий SQL-запрос (с возможными фильтрами)
    private string _query = "SELECT ID_Worker AS 'ID', Login, Password, Fio AS 'FIO', PhoneNumber, workers.rule AS 'RuleID', r.Rule AS 'RuleView', Image  FROM workers INNER JOIN vkr.rule r on workers.rule = r.ID_Rule;";

    // Свойство для базового запроса
    public string QueryBase
    {
        get => _queryBase;
    }
    
    // Свойство для текущего запроса
    public string Query
    {
        get => _query;
        set => SetProperty(ref _query, value);
    }

    // Строка поиска для фильтрации сотрудников
    private string _search;

    public string Search
    {
        get => _search;
        set
        {
            SetProperty(ref _search, value);
            DataTableSelect(); // Автоматическая фильтрация при изменении поискового запроса
        }
    }

    // Коллекция сотрудников для отображения в DataGrid
    private ObservableCollection<Worker> _workers;

    public ObservableCollection<Worker> Workers
    {
        get => _workers;
        set => SetProperty(ref _workers, value);
    }

    // Метод для выборки данных с учетом фильтра поиска
    private void DataTableSelect()
    {
        string query = QueryBase;

        // Добавление фильтра по ФИО если задан поисковый запрос
        if (Search != "")
        {
            query += $" WHERE fio LIKE '%{Search}%'";
        }
        query += ";";
        Workers = SelectTabelWorkers.SelectTable(query, ConnectToDB.ConnectToDBString());
    }

    // Метод для проверки доступности кнопки сохранения изменений
    private void EnabledInsertButton()
    {
        if (Login.Trim().Length < 6 ||           // Логин не менее 6 символов
            Fio.Trim() == string.Empty ||        // ФИО не должно быть пустым
            PhoneNumber.Length < 16 ||           // Номер телефона должен быть заполнен
            ImageUser is null ||                 // Изображение должно быть загружено
            RoleUserSelected.Equals(RoleUser[0])) // Роль не должна быть выбрана по умолчанию
        {
            InsertButtonEnabled = false;
        }
        else
        {
            InsertButtonEnabled = true;
        }
    }

    // Метод для первоначальной загрузки всех сотрудников
    private void FillWorkers()
    {
        Workers = SelectTabelWorkers.SelectTable(Query, ConnectToDB.ConnectToDBString());
    }

    // Команда для добавления/изменения изображения профиля
    [RelayCommand]
    public async Task AddImage()
    {
        try
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выбор изображения";
            openFileDialog.Filters.Add(new FileDialogFilter()
            {
                Name = "Images",
                Extensions = { "jpg", "jpeg", "png" }
            });

            var result = await openFileDialog.ShowAsync(new Window());

            if (result == null || result.Length == 0)
                return;

            string path = result[0];
            long maxFileSize = 15 * 1024 * 1024; // Максимальный размер файла 15 МБ
            FileInfo fileInfo = new FileInfo(path);
            
            // Проверка размера файла
            if (fileInfo.Length > maxFileSize)
            {
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Изображение слишком большого размера, выберите изображение меньшего размера (менее 15 МБ)", "")
                };
                await err.ShowDialog(_window);
                return;
            }

            // Загрузка изображения
            if (File.Exists(path))
            {
                ImageUser = new Bitmap(path);
                _imageBlob = File.ReadAllBytes(path); // Сохранение изображения в байтовом массиве
                IsVisibleBorder = false;
                IsVisibleImage = true;
            }
        }
        catch (Exception ex)
        {
            // Обработка ошибки при загрузке изображения
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Изображение не удалось добавить", "")
            };
            await err.ShowDialog(_window);
        }
    }

    // Команда для обновления данных пользователя
    [RelayCommand]
    public async void UpdateUser()
    {
        // Диалог подтверждения обновления
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel = new YesOrNotDialogViewModel("Внимание", "Вы хотите изменитить данные пользователя?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        
        // Если пользователь подтвердил обновление
        if (warningViewModel.Flag)
        {
            // Проверка уникальности логина и номера телефона
            foreach (Worker worker in Workers)
            {
                if (worker.Login == Login && Login != SelectedWorker.Login)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Пользователь с таким логином существует", "")
                    };
                    await err.ShowDialog(_window);
                    return;
                }
                if (worker.PhoneNumber == PhoneNumber && PhoneNumber != SelectedWorker.PhoneNumber)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Пользователь с таким номером телефона существует", "")
                    };
                    await err.ShowDialog(_window);
                    return;
                }
            }
            
            try
            {
                string heshPassword = "";
                byte[] imageBytes = null;
                
                // Обработка обновления с изменением пароля
                if (!string.IsNullOrEmpty(Password))
                {
                    heshPassword = SHA256Hasher.ComputeSHA256Hash(Password);
                    if (!(_imageBlob == new byte[] { } || _imageBlob == null))
                    {
                        // Обновление с паролем и изображением
                        SelectTabelWorkers.UpdateData(SelectedWorker.Id,
                            Fio,
                            Login,
                            heshPassword,
                            RoleUserSelected.Id,
                            PhoneNumber,
                            _imageBlob);
                    }
                    else
                    {
                        // Обновление только с паролем
                        SelectTabelWorkers.UpdateData(SelectedWorker.Id,
                            Fio,
                            Login,
                            heshPassword,
                            RoleUserSelected.Id,
                            PhoneNumber);
                    }
                }
                else
                {
                    // Обработка обновления без изменения пароля
                    if (!(_imageBlob == new byte[] { } || _imageBlob == null))
                    {
                        // Обновление без пароля, но с изображением
                        SelectTabelWorkers.UpdateData(SelectedWorker.Id,
                            Fio,
                            Login,
                            RoleUserSelected.Id,
                            PhoneNumber,
                            _imageBlob);
                    }
                    else
                    {
                        // Обновление только основных данных
                        SelectTabelWorkers.UpdateData(SelectedWorker.Id,
                            Fio,
                            Login,
                            RoleUserSelected.Id,
                            PhoneNumber);
                    }
                }
                
                // Уведомление об успешном обновлении
                InfoDialogWindow infoDialog = new InfoDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Успех", "Запись обновлена!", "")
                };
                infoDialog.Show();
                DataTableSelect(); // Обновление списка сотрудников
            }
            catch (Exception e)
            {
                // Обработка ошибки при обновлении
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Запись не удалось обновить", "")
                };
                await err.ShowDialog(_window);
            }
        }
    }

    // Команда для удаления пользователя
    [RelayCommand]
    public async void DeleteUser()
    {
        // Диалог подтверждения удаления
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel = new YesOrNotDialogViewModel("Внимание", "Вы хотите удалить пользователя?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        
        // Если пользователь подтвердил удаление
        if (warningViewModel.Flag)
        {
            try
            {
                // Проверка, что пользователь не удаляет самого себя
                if (SelectedWorker.Login != loginUser)
                {
                    SelectTabelWorkers.DeleteData(SelectedWorker.Id);
                    
                    // Уведомление об успешном удалении
                    InfoDialogWindow infoDialog = new InfoDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Успех", "Запись удалена!", "")
                    };
                    await infoDialog.ShowDialog(_window);
                }
                else
                {
                    // Ошибка при попытке удалить самого себя
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Запись не удалось удалить, так как вы пытаетесь удалить самого себя!", "")
                    };
                    
                    await err.ShowDialog(_window);
                }
                DataTableSelect(); // Обновление списка сотрудников
            }
            catch (Exception ex)
            {
                // Обработка ошибки при удалении
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Запись не удалось удалить " + ex.Message.ToString(),
                        "")
                };
                
                await err.ShowDialog(_window);
            }
        }
    }

    // Конструктор ViewModel
    public UsersPageViewModel(string _loginUser, Window window)
    {
        _window = window;
        loginUser = _loginUser; // Сохранение логина текущего пользователя
        
        FillWorkers(); // Загрузка списка сотрудников
        
        // Настройка видимости элементов для изображения
        if (ImageUser is null)
        {
            IsVisibleBorder = true;
            IsVisibleImage = false;
        }
        
        // Загрузка списка ролей из базы данных
        RoleUser = SelectTabelSimpleDataType.SelectTableComboBox(
            "SELECT ID_Rule as 'ID', Rule AS 'Name' FROM rule;",
            ConnectToDB.ConnectToDBString());
        RoleUserSelected = RoleUser[0]; // Установка значения по умолчанию
    }
}