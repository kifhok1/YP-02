using CommunityToolkit.Mvvm.ComponentModel;

namespace VKR.ViewModels;

// Базовый класс для всех ViewModel в приложении
// Наследуется от ObservableObject для реализации INotifyPropertyChanged
public class ViewModelBase : ObservableObject
{
    // Этот базовый класс предоставляет возможность уведомления об изменениях свойств
    // для всех производных ViewModel через механизм MVVM CommunityToolkit
}