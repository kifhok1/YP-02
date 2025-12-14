using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VKR.ViewModels;

// ViewModel для диалогового окна с подтверждением (OK)
public partial class OkDialogViewModel(string title, string message, string iconText) : DialogViewModel
{
    // Заголовок диалогового окна
    [ObservableProperty] private string _title = title;
    
    // Сообщение диалогового окна
    [ObservableProperty] private string _message = message;
    
    // Текст кнопки OK (по умолчанию "ОК")
    [ObservableProperty] private string _OkText = "ОК";

    // Флаг подтверждения (в данной реализации не используется напрямую)
    [ObservableProperty]
    private bool _confirmed;

    // Команда для закрытия диалога при нажатии OK
    [RelayCommand]
    public void Ok()
    {
        Close();
    }
}