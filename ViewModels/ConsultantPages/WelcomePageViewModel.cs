namespace VKR.ViewModels.ConsultantPages;

// ViewModel для приветственной страницы в панели консультанта
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