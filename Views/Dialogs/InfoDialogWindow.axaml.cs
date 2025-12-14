using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace VKR.Views.Dialogs;

public partial class InfoDialogWindow : Window
{
    public InfoDialogWindow()
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

    // Обработчик нажатия кнопки OK для закрытия окна
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}