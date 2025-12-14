using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using VKR.ViewModels;

namespace VKR.Views;

public partial class SellerMainWindow : Window
{
    // Переменные для реализации перемещения окна за заголовок
    private bool _mouseDownForWindowMoving = false;
    private PointerPoint _originalPoint;

    // Обработчик движения мыши для перемещения окна
    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_mouseDownForWindowMoving) return;
        PointerPoint currentPoint = e.GetCurrentPoint(this);
        // Обновление позиции окна на основе перемещения мыши
        Position = new PixelPoint(Position.X + (int)(currentPoint.Position.X - _originalPoint.Position.X),
            Position.Y + (int)(currentPoint.Position.Y - _originalPoint.Position.Y));
    }

    // Обработчик нажатия мыши для начала перемещения окна
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Не перемещать окно если оно развернуто на весь экран
        if (WindowState == WindowState.Maximized || WindowState == WindowState.FullScreen) return;

        _mouseDownForWindowMoving = true;
        _originalPoint = e.GetCurrentPoint(this); // Сохранение начальной точки
    }

    // Обработчик отпускания мыши для окончания перемещения окна
    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _mouseDownForWindowMoving = false;
    }

    public SellerMainWindow()
    {
        InitializeComponent();
    }

    // Обработчик нажатия кнопки выхода из системы
    private void ButtonExit_OnClick(object? sender, RoutedEventArgs e)
    {
        MainWindow autorisation = new MainWindow();
        DataContext = new MainWindowViewModel(autorisation);
        autorisation.DataContext = DataContext;
        autorisation.Show(); // Отображение окна авторизации
        Close(); // Закрытие текущего окна продавца
    }
    
    // Метод для сброса цветов всех кнопок навигации к стандартному черному
    private void ResetAllButtons()
    {
        SetButtonForeground(Purchases, Brushes.Black);
        SetButtonForeground(PurchasesAdd, Brushes.Black);
        SetButtonForeground(ClientAdd, Brushes.Black);
        SetButtonForeground(Client, Brushes.Black);
    }

    // Метод установки цвета текста для кнопки (внутренних элементов StackPanel)
    private void SetButtonForeground(Button button, IImmutableSolidColorBrush color)
    {
        if (button.Content is StackPanel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is Label label)
                {
                    label.Foreground = color;
                }
            }
        }
    }

    // Обработчик нажатия кнопки "Покупки" (список заказов)
    private void Purchases_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(Purchases, Brushes.RoyalBlue); // Установка синего цвета для активной кнопки
    }

    // Обработчик нажатия кнопки "Создание покупки" (новый заказ)
    private void PurchasesAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(PurchasesAdd, Brushes.RoyalBlue);
    }

    // Обработчик нажатия кнопки "Клиенты" (список клиентов)
    private void Client_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(Client, Brushes.RoyalBlue);
    }

    // Обработчик нажатия кнопки "Добавление клиента" (новый клиент)
    private void ClientAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(ClientAdd, Brushes.RoyalBlue);
    }
}