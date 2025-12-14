using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using VKR.ViewModels.SellerPages;

namespace VKR.ViewModels;

// ViewModel главного окна продавца с навигацией по страницам
public partial class SellerMainViewModel : ViewModelBase
{
    // Текст для кнопок навигации
    private string _clientNavButton = "Клиенты";
    private string _clientAddNavButton = "Добавление клиента";
    private string _purchasesNavButton = "Заказы";
    private string _purchaseAddNavButton = "Создание заказа";

    private string _fio;
    private Bitmap _imageUser;
    private static Window _window;

    // Иконки для кнопок навигации (символы из шрифта иконок)
    private string _clientNavButtonIcon = "";
    private string _clientAddNavButtonIcon = "";
    private string _purchasesNavButtonIcon = "";
    private string _purchaseAddNavButtonIcon = "";

    // Свойства для текста кнопок навигации
    public string ClientNavButton
    {
        get => _clientNavButton;
        set => SetProperty(ref _clientNavButton, value);
    }

    // ФИО продавца
    public string Fio
    {
        get => _fio;
        set => SetProperty(ref _fio, value);
    }

    // Изображение профиля продавца
    public Bitmap ImageUser
    {
        get => _imageUser;
        set => SetProperty(ref _imageUser, value);
    }

    public string ClientAddNavButton
    {
        get => _clientAddNavButton;
        set => SetProperty(ref _clientAddNavButton, value);
    }

    public string PurchasesNavButton
    {
        get => _purchasesNavButton;
        set => SetProperty(ref _purchasesNavButton, value);
    }

    public string PurchaseAddNavButton
    {
        get => _purchaseAddNavButton;
        set => SetProperty(ref _purchaseAddNavButton, value);
    }

    // Свойства для иконок кнопок навигации
    public string ClientNavButtonIcon
    {
        get => _clientNavButtonIcon;
        set => SetProperty(ref _clientNavButtonIcon, value);
    }

    public string ClientAddNavButtonIcon
    {
        get => _clientAddNavButtonIcon;
        set => SetProperty(ref _clientAddNavButtonIcon, value);
    }

    public string PurchasesNavButtonIcon
    {
        get => _purchasesNavButtonIcon;
        set => SetProperty(ref _purchasesNavButtonIcon, value);
    }

    public string PurchaseAddNavButtonIcon
    {
        get => _purchaseAddNavButtonIcon;
        set => SetProperty(ref _purchaseAddNavButtonIcon, value);
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
    public SellerMainViewModel(string fio, byte[] imageUser, Window window)
    {
        _currentPage = _welcomePage; // Установка приветственной страницы по умолчанию
        Fio = fio; // Установка ФИО продавца
        _window = window; // Сохранение ссылки на главное окно
        
        // Преобразование байтового массива изображения в Bitmap
        using (MemoryStream ms = new MemoryStream(imageUser))
        {
            ImageUser = new Bitmap(ms);
        }
    }

    // Команда для перехода на страницу добавления клиента
    [RelayCommand]
    public void GoToClientAddPage()
    {
        CurrentPage = new ClientAddPageViewModel(_window);
    }

    // Команда для перехода на страницу управления клиентами
    [RelayCommand]
    public void GoToClientsPage()
    {
        CurrentPage = new ClientsPageViewModel(_window);
    }

    // Команда для перехода на страницу управления покупками (заказами)
    [RelayCommand]
    public void GoToPurchasesPage()
    {
        CurrentPage = new PurchasesPageViewModel(_window);
    }

    // Команда для перехода на страницу создания новой покупки (заказа)
    [RelayCommand]
    public void GoToPurchaseAddPage()
    {
        CurrentPage = new PurchaseAddPageViewModel(_window);
    }
}