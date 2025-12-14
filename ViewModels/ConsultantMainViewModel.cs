using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using VKR.ViewModels.ConsultantPages;

namespace VKR.ViewModels;

// ViewModel главного окна консультанта с навигацией по страницам
public partial class ConsultantMainViewModel : ViewModelBase
{
    // Текст для кнопок навигации
    private string _clientNavButton = "Клиенты";
    private string _clientAddNavButton = "Добавление клиента";
    private string _productNavButton = "Товары";
    private string _productAddNavButton = "Добавление товара";

    private string _fio;
    private Bitmap _imageUser;
    private static Window _window;

    // Иконки для кнопок навигации (символы из шрифта иконок)
    private string _clientNavButtonIcon = "";
    private string _clientAddNavButtonIcon = "";
    private string _productNavButtonIcon = "";
    private string _productAddNavButtonIcon = "";

    // ФИО консультанта
    public string Fio
    {
        get => _fio;
        set => SetProperty(ref _fio, value);
    }

    // Изображение профиля консультанта
    public Bitmap ImageUser
    {
        get => _imageUser;
        set => SetProperty(ref _imageUser, value);
    }

    // Свойства для текста кнопок навигации
    public string ClientNavButton
    {
        get => _clientNavButton;
        set => SetProperty(ref _clientNavButton, value);
    }

    public string ClientAddNavButton
    {
        get => _clientAddNavButton;
        set => SetProperty(ref _clientAddNavButton, value);
    }

    public string ProductNavButton
    {
        get => _productNavButton;
        set => SetProperty(ref _productNavButton, value);
    }

    public string ProductAddNavButton
    {
        get => _productAddNavButton;
        set => SetProperty(ref _productAddNavButton, value);
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

    public string ProductNavButtonIcon
    {
        get => _productNavButtonIcon;
        set => SetProperty(ref _productNavButtonIcon, value);
    }

    public string ProductAddNavButtonIcon
    {
        get => _productAddNavButtonIcon;
        set => SetProperty(ref _productAddNavButtonIcon, value);
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
    public ConsultantMainViewModel(string fio, byte[] imageUser, Window window)
    {
        _currentPage = _welcomePage; // Установка приветственной страницы по умолчанию
        Fio = fio; // Установка ФИО консультанта
        _window = window; // Сохранение ссылки на главное окно
        
        // Преобразование байтового массива изображения в Bitmap
        using (MemoryStream ms = new MemoryStream(imageUser))
        {
            ImageUser = new Bitmap(ms);
        }
    }
    

    // Команда для перехода на страницу добавления товара
    [RelayCommand]
    public void GoToProductAddPage()
    {
        CurrentPage = new ProductAddPageViewModel(_window);
    }
    

    // Команда для перехода на страницу управления товарами
    [RelayCommand]
    public void GoToProductsPage()
    {
        CurrentPage = new ProductsPageViewModel(_window);
    }

}