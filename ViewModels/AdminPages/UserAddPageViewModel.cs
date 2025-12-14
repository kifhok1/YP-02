using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VKR.Models;
using VKR.Views.Dialogs;

namespace VKR.ViewModels.AdminPages;

// ViewModel для страницы добавления нового пользователя (сотрудника)
public partial class UserAddPageViewModel : ViewModelBase
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
    private string _login = "";
    private string _password = "";
    private Window _window;

    // Список доступных ролей для выбора
    public List<SimpleDataType> RoleUser
    {
        get => _roleUser;
        set => SetProperty(ref _roleUser, value);
    }

    // Доступность кнопки добавления пользователя
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

    // Пароль пользователя
    public string Password
    {
        get => _password;
        set
        {
            SetProperty(ref _password, value);
            EnabledInsertButton(); // Проверка доступности кнопки при изменении пароля
        }
    }

    // Конструктор ViewModel
    public UserAddPageViewModel(Window window)
    {
        _window = window;
        // Настройка видимости элементов для изображения
        if (ImageUser is null)
        {
            IsVisibleBorder = true;
            IsVisibleImage = false;
        }
        // Загрузка списка ролей из базы данных
        RoleUser = SelectTabelSimpleDataType.SelectTableComboBox("SELECT ID_Rule as 'ID', Rule AS 'Name' FROM rule;",
            ConnectToDB.ConnectToDBString());
        RoleUserSelected = RoleUser[0]; // Установка значения по умолчанию (первый элемент)
    }

    // Команда для добавления изображения профиля
    [RelayCommand]
    public async Task AddImage()
    {
        try
        {
            // Настройка диалога выбора файла
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выбор изображения";
            openFileDialog.Filters.Add(new FileDialogFilter()
            {
                Name = "Images",
                Extensions = { "jpg", "jpeg", "png" }
            });

            // Отображение диалога выбора файла
            var result = await openFileDialog.ShowAsync(new Window());

            // Проверка, выбрал ли пользователь файл
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

    // Метод для проверки доступности кнопки добавления пользователя
    private void EnabledInsertButton()
    {
        // Проверка всех обязательных полей
        if (Password.Trim().Length < 6 ||             // Пароль не менее 6 символов
            Login.Trim().Length < 6 ||                // Логин не менее 6 символов
            PhoneNumber.Length < 16 ||                // Полный номер телефона
            ImageUser is null ||                      // Изображение должно быть загружено
            RoleUserSelected.Equals(RoleUser[0]) ||   // Роль не должна быть выбрана по умолчанию
            Fio.Trim() == string.Empty)               // ФИО не должно быть пустым
        {
            InsertButtonEnabled = false;
        }
        else
        {
            InsertButtonEnabled = true;
        }
    }

    // Команда для добавления нового пользователя
    [RelayCommand]
    public async void InsertUser()
    {
        // Диалог подтверждения добавления пользователя
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel = new YesOrNotDialogViewModel("Внимание", "Вы хотите добавить пользователя?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        
        // Если пользователь подтвердил добавление
        if (warningViewModel.Flag)
        {
            try
            {
                // Загрузка существующих пользователей для проверки уникальности
                ObservableCollection<Worker> table = SelectTabelWorkers.SelectNoneImage(
                    "SELECT ID_Worker AS 'ID', " +
                    "Login, " +
                    "Password, " +
                    "Fio AS 'FIO', " +
                    "PhoneNumber, " +
                    "workers.rule AS 'RuleID', " +
                    "r.Rule AS 'RuleView'  " +
                    "FROM workers INNER JOIN vkr.rule r on workers.rule = r.ID_Rule;",
                    ConnectToDB.ConnectToDBString());
                
                // Проверка уникальности логина и номера телефона
                foreach (var VARIABLE in table)
                {
                    if (VARIABLE.Login == Login)
                    {
                        ErrorDialogWindow err = new ErrorDialogWindow()
                        {
                            DataContext = new OkDialogViewModel("Ошибка", "Пользователь с таким логином уже сущевствует", "")
                        };
                        await err.ShowDialog(_window);
                        return;
                    }
                    if (VARIABLE.PhoneNumber == PhoneNumber)
                    {
                        ErrorDialogWindow err = new ErrorDialogWindow()
                        {
                            DataContext = new OkDialogViewModel("Ошибка", "Пользователь с таким номером телефона уже сущевствует", "")
                        };
                        await err.ShowDialog(_window);
                        return;
                    }
                }

                // Добавление нового пользователя в базу данных
                SelectTabelWorkers.InsertData(fio: Fio.Trim(),
                    login: Login.Trim(),
                    password: Password.Trim(),
                    phoneNumber: PhoneNumber,
                    ruleID: RoleUserSelected.Id,
                    userImage: _imageBlob);
                
                // Сброс полей формы после успешного добавления
                Login = "";
                Password = "";
                Fio = "";
                PhoneNumber = "";
                RoleUserSelected = RoleUser[0];
                InsertButtonEnabled = false;
                IsVisibleBorder = true;
                IsVisibleImage = false;
                
                // Уведомление об успешном добавлении
                InfoDialogWindow info = new InfoDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Успех", "Добавлена запись!", "")
                };
                await info.ShowDialog(_window);
            }
            catch (Exception e)
            {
                // Обработка ошибки при добавлении пользователя
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Запись не удалось добавить", "")
                };
                await err.ShowDialog(_window);
            }
        }
    }
}