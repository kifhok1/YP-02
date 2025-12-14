using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using VKR.ViewModels;

namespace VKR.Views;

public partial class ConsultantMainWindow : Window
{
    public ConsultantMainWindow()
    {
        InitializeComponent();
    }

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

    // Обработчик нажатия кнопки выхода из системы
    private void ButtonExit_OnClick(object? sender, RoutedEventArgs e)
    {
        MainWindow autorisation = new MainWindow();
        DataContext = new MainWindowViewModel(autorisation);
        autorisation.DataContext = DataContext;
        autorisation.Show(); // Отображение окна авторизации
        Close(); // Закрытие текущего окна консультанта
    }
    
    // Метод для сброса цветов всех кнопок навигации к стандартному черному
    private void ResetAllButtons()
    {
        SetButtonForeground(ProductAdd, Brushes.Black);
        SetButtonForeground(Product, Brushes.Black);
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

    // Обработчик нажатия кнопки "Добавление товара"
    private void ProductAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(ProductAdd, Brushes.RoyalBlue); // Установка синего цвета для активной кнопки
    }

    // Обработчик нажатия кнопки "Товары"
    private void Product_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(Product, Brushes.RoyalBlue);
    }
}