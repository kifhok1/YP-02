using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace VKR.ViewModels;

// Базовый ViewModel для диалоговых окон с асинхронным ожиданием закрытия
public partial class DialogViewModel : ViewModelBase
{
    // Флаг, указывающий открыт ли диалог
    [ObservableProperty]
    private bool _isDialogOpen;

    // TaskCompletionSource для асинхронного ожидания закрытия диалога
    protected TaskCompletionSource closeTask = new TaskCompletionSource();

    // Метод для асинхронного ожидания закрытия диалога
    public async Task VoidAsync()
    {
        await closeTask.Task;
    }

    // Метод для отображения диалога
    public void Show()
    {
        // Создание новой задачи если предыдущая завершена
        if (closeTask.Task.IsCompleted)
        {
            closeTask = new TaskCompletionSource();
        }
        IsDialogOpen = true;
    }

    // Метод для закрытия диалога
    public void Close()
    {
        IsDialogOpen = false;

        // Сигнализация о завершении диалога
        closeTask.SetResult();
    }
}