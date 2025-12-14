using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace VKR.Views.SellerPages;

public partial class PurchaseAddPageView : UserControl
{
    public PurchaseAddPageView()
    {
        InitializeComponent();
    }

    // Обработчик нажатия кнопки для скрытия панели оформления заказа
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Element.IsVisible = false; // Скрытие панели оформления заказа
    }

    // Обработчик нажатия клавиш на элементе ввода
    // Блокирует все клавиши для предотвращения нежелательного ввода
    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        e.Handled = true; // Обработка события завершена, дальнейшая обработка не требуется
    }

    // Обработчик ввода текста на элементе ввода
    // Блокирует весь текстовый ввод
    private void InputElement_OnTextInput(object? sender, TextInputEventArgs e)
    {
        e.Handled =  true; // Обработка события завершена, дальнейшая обработка не требуется
    }

    // Обработчик перехода к корзине (оформление заказа)
    private void GoToTrash(object? sender, RoutedEventArgs e)
    {
        Element.IsVisible = true; // Показ панели оформления заказа
        ComboBoxClients.SelectedIndex = 0; // Сброс выбора клиента на значение по умолчанию
    }
}