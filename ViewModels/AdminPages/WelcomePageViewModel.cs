namespace VKR.ViewModels.AdminPages;

// ViewModel для приветственной страницы в админ-панели
public class WelcomePageViewModel : ViewModelBase
{
    // Заголовок приветственной страницы
    private string _title = "Добро пожаловать";

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}