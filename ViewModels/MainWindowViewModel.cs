using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using VKR.Models;
using VKR.Views;
using VKR.Views.Dialogs;

namespace VKR.ViewModels;

// ViewModel главного окна авторизации
public partial class MainWindowViewModel(Window window) : ViewModelBase
{
    // Текстовые константы для интерфейса
    public string ExitText { get; } = "Выход";
    public string AuthorizationText { get; } = "Авторизоваться";
    public string LoginWatermark { get; } = "Логин";
    public string PasswordWatermark { get; } = "Пароль";
    public string SettingsIcon { get; } = "";
    
    // Поля для ввода логина и пароля
    private string _login = "";
    private string _password = "";

    // Иконка и состояние кнопки показа/скрытия пароля
    private string _passwordButtonIcon = "";
    private bool _revealPassword = false;

    [ObservableProperty] private bool _errorVisible = false;
    [ObservableProperty] private bool _windowVisible = true;

    private Window _window = window;

    // Иконка кнопки показа/скрытия пароля
    public string PasswordButtonIcon
    {
        get => _passwordButtonIcon;
        set => SetProperty(ref _passwordButtonIcon, value);
    }

    // Флаг отображения пароля в открытом виде
    public bool RevealPassword
    {
        get => _revealPassword;
        set => SetProperty(ref _revealPassword, value);
    }

    // Логин пользователя
    public string Login
    {
        get => _login;
        set => _login = value;
    }

    // Пароль пользователя
    public string Password
    {
        get => _password;
        set => _password = value;
    }

    // Команда для показа/скрытия пароля
    [RelayCommand]
    public void ShowPassword()
    {
        if (_revealPassword)
        {
            RevealPassword = false;
            PasswordButtonIcon = ""; // Иконка скрытого пароля
        }
        else
        {
            RevealPassword = true;
            PasswordButtonIcon = ""; // Иконка открытого пароля
        }
    }

    // Команда для настроек (в данной реализации не используется)
    [RelayCommand]
    private void Settings()
    {
        // Реализация настроек может быть добавлена позже
    }

    // Команда авторизации пользователя
    [RelayCommand]
    public async void Authorisation()
    {
        try
        {
            // Получение строки подключения к базе данных
            string conn = ConnectToDB.ConnectToDBString();
            
            // Создание экземпляра для авторизации пользователей
            UserAuthorisations userAuthorisations = new UserAuthorisations(conn);
            
            // Получение пользователя по логину
            User? user = userAuthorisations.GetUserByLogin(Login);
            
            // Проверка существования пользователя
            if (user is null)
            {
                var error = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Неверный логин или пароль!", "")
                };
                await error.ShowDialog(_window);
            }
            else
            {
                // Проверка пароля (сравнение хешей)
                if (user.HashPassword == SHA256Hasher.ComputeSHA256Hash(Password))
                {
                    // Определение роли пользователя и открытие соответствующего окна
                    if (user.Rule == 1) // Администратор
                    {
                        var admin = new AdminMainWindow();
                        admin.DataContext = new AdminMainViewModel(Login, user.Fio, user.Image, admin);
                        admin.Show();
                    }
                    else if (user.Rule == 2) // Продавец
                    {
                        var seller = new SellerMainWindow();
                        seller.DataContext = new SellerMainViewModel(user.Fio, user.Image, seller);
                        seller.Show();
                    }
                    else if (user.Rule == 3) // Консультант
                    {
                        var consultant = new ConsultantMainWindow();
                        consultant.DataContext = new ConsultantMainViewModel(user.Fio, user.Image, consultant);
                        consultant.Show();
                    }
                    _window.Close(); // Закрытие окна авторизации
                }
                else
                {
                    var error = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Неверный логин или пароль!", "")
                    };
                    await error.ShowDialog(_window);
                }
            }
        }
        catch (Exception ex)
        {
            // Обработка ошибок подключения к базе данных
            var error = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", $"Ошибка подключения", "")
            };
            await error.ShowDialog(_window);
        }
    }

}