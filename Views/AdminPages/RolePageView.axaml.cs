using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using VKR.Models;

namespace VKR.Views.AdminPages;

public partial class RolePageView : UserControl
{
    public RolePageView()
    {
        InitializeComponent();
    }

    // Обработчик нажатия кнопки для скрытия панели редактирования роли
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementUpdateRole.IsVisible = false; // Скрытие панели редактирования роли
    }

    // Обработчик изменения текста в поле ввода названия роли (для редактирования)
    // Ограничивает ввод только русскими буквами
    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        NameRoleUpdate.Text = LineEntryRestrictions.TextChangedRu(NameRoleUpdate.Text);
    }

    // Обработчик двойного нажатия на элемент DataGrid для редактирования роли
    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        SimpleDataType selected = DataGrid.SelectedItem as SimpleDataType;
        if (selected != null)
        {
            ElementUpdateRole.IsVisible = true; // Показ панели редактирования
            NameRoleUpdate.Text = selected.Name; // Заполнение поля названием выбранной роли
        }
    }
}