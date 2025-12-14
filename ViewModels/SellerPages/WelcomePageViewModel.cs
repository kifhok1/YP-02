namespace VKR.ViewModels.SellerPages;

// ViewModel для приветственной страницы в панели продавца
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