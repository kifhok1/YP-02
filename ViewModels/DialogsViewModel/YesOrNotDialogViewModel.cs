using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VKR.ViewModels;

// ViewModel для диалогового окна с выбором Да/Нет
public partial class YesOrNotDialogViewModel(string title, string message, Window window) : DialogViewModel
{
    // Заголовок диалогового окна
    [ObservableProperty] private string _title = title;
    
    // Сообщение диалогового окна
    [ObservableProperty] private string _message = message;
    
    // Текст кнопки "Да" (по умолчанию "Да")
    [ObservableProperty] private string _yesText = "Да";
    
    // Текст кнопки "Нет" (по умолчанию "Нет")
    [ObservableProperty] private string _noText = "Нет";
    
    // Ссылка на окно для управления его закрытием
    Window _window = window;

    // Флаг подтверждения (в данной реализации не используется напрямую)
    [ObservableProperty]
    private bool _confirmed;

    // Флаг выбора пользователя (true - Да, false - Нет)
    [ObservableProperty]
    private bool _flag;

    // Команда для выбора "Да"
    [RelayCommand]
    private void YES()
    {
        Flag = true;       // Установка флага выбора
        _window.Close();   // Закрытие окна диалога
    }

    // Команда для выбора "Нет"
    [RelayCommand]
    private void NO()
    {
        Flag = false;      // Установка флага выбора
        _window.Close();   // Закрытие окна диалога
    }
}