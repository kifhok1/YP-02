using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using SixLabors.ImageSharp;
using VKR.ViewModels;

namespace VKR.Views;

public partial class AdminMainWindow : Window
{
    public AdminMainWindow()
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
        Close(); // Закрытие текущего окна администратора
    }

    // Метод для сброса цветов всех кнопок навигации к стандартному черному
    private void ResetAllButtons()
    {
        SetButtonForeground(AddUser, Brushes.Black);
        SetButtonForeground(Users, Brushes.Black);
        SetButtonForeground(Categories, Brushes.Black);
        SetButtonForeground(Role, Brushes.Black);
        SetButtonForeground(Status, Brushes.Black);
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
    
    // Обработчик нажатия кнопки "Новый сотрудник"
    private void AddUser_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(AddUser, Brushes.RoyalBlue); // Установка синего цвета для активной кнопки
    }

    // Обработчик нажатия кнопки "Сотрудники"
    private void Users_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(Users, Brushes.RoyalBlue);
    }

    // Обработчик нажатия кнопки "Категории"
    private void Categories_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(Categories, Brushes.RoyalBlue);
    }

    // Обработчик нажатия кнопки "Роль"
    private void Role_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(Role, Brushes.RoyalBlue);
    }

    // Обработчик нажатия кнопки "Статус"
    private void Status_OnClick(object? sender, RoutedEventArgs e)
    {
        ResetAllButtons();
        SetButtonForeground(Status, Brushes.RoyalBlue);
    }
}