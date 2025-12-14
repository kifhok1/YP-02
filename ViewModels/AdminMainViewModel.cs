using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using VKR.ViewModels.AdminPages;

namespace VKR.ViewModels;

// ViewModel главного окна администратора с навигацией по страницам
public partial class AdminMainViewModel : ViewModelBase
{
    private string _fio;
    private string loginUser;
    private Bitmap _imageUser;
    private static Window _window;

    // Текст и иконки для кнопок навигации
    private string _roleNavButton = "Роль";
    private string _roleNavButtonIcon = "";    
    private string _statusNavButton = "Статус";
    private string _statusNavButtonIcon = "";
    private string _categoryNavButton = "Категория";
    private string _categoryNavButtonIcon = "";
    private string _userButton = "Сотрудники";
    private string _userButtonIcon = "";
    private string _addUserButton = "Новый сотрудник";
    private string _addUserButtonIcon = "";

    // ФИО администратора
    public string Fio
    {
        get => _fio;
        set => SetProperty(ref _fio, value);
    }

    // Изображение профиля администратора
    public Bitmap ImageUser
    {
        get => _imageUser;
        set => SetProperty(ref _imageUser, value);
    }

    // Свойства для текста кнопок навигации (только для чтения)
    public string RoleNavButton
    {
        get => _roleNavButton;
    }
    public string RoleNavButtonIcon
    {
        get => _roleNavButtonIcon;
    }
    public string CategoryNavButton
    {
        get => _categoryNavButton;
    }
    public string CategoryNavButtonIcon
    {
        get => _categoryNavButtonIcon;
    }
    public string UserButton
    {
        get => _userButton;
    }
    public string UserButtonIcon
    {
        get => _userButtonIcon;
    }
    public string AddUserButton
    {
        get => _addUserButton;
    }
    public string AddUserButtonIcon
    {
        get => _addUserButtonIcon;
    }

    public string StatusNavButton
    {
        get => _statusNavButton;
    }

    public string StatusNavButtonIcon
    {
        get => _statusNavButtonIcon;
    }

    // Текущая открытая страница в главном окне
    private ViewModelBase _currentPage;

    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }
    
    // Приветственная страница по умолчанию
    private readonly WelcomePageViewModel _welcomePage = new WelcomePageViewModel();

    // Конструктор ViewModel
    public AdminMainViewModel(string _loginUser, string fio, byte[] imageUser, Window window)
    {
        loginUser = _loginUser; // Сохранение логина текущего пользователя
        _currentPage = _welcomePage; // Установка приветственной страницы по умолчанию
        Fio = fio; // Установка ФИО администратора
        _window = window; // Сохранение ссылки на главное окно
        
        // Преобразование байтового массива изображения в Bitmap
        using (MemoryStream ms = new MemoryStream(imageUser))
        {
            ImageUser = new Bitmap(ms);
        }
    }

    // Команда для перехода на страницу управления ролями
    [RelayCommand]
    public void GoToRolePage()
    {
        CurrentPage = new RolePageViewModel(_window);
    }

    // Команда для перехода на страницу управления статусами заказов
    [RelayCommand]
    public void GoToStatusPage()
    {
        CurrentPage = new OrderStatusPageViewModel(_window);
    }

    // Команда для перехода на страницу управления категориями товаров
    [RelayCommand]
    public void GoToCategoriesPage()
    {
        CurrentPage = new CategoriesPageViewModel(_window);
    }

    // Команда для перехода на страницу управления сотрудниками
    [RelayCommand]
    public void GoToUsersPage()
    {
        CurrentPage = new UsersPageViewModel(loginUser, _window);
    }

    // Команда для перехода на страницу добавления нового сотрудника
    [RelayCommand]
    public void GoToAddUserPage()
    {
        CurrentPage = new UserAddPageViewModel(_window);
    }
}