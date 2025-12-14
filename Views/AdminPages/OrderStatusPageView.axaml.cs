using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using VKR.Models;

namespace VKR.Views.AdminPages;

public partial class OrderStatusPageView : UserControl
{
    public OrderStatusPageView()
    {
        InitializeComponent();
    }

    // Обработчик изменения текста в поле ввода названия статуса (для редактирования)
    // Ограничивает ввод только русскими буквами
    private void TextBoxUpdate_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        NameStausUpdate.Text = LineEntryRestrictions.TextChangedRu(NameStausUpdate.Text);
    }

    // Обработчик нажатия кнопки для скрытия панелей добавления/редактирования
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementAddStatus.IsVisible = false; // Скрытие панели добавления статуса
        ElementUpdateStatus.IsVisible = false; // Скрытие панели редактирования статуса
    }

    // Обработчик изменения текста в поле ввода названия статуса (для добавления)
    // Ограничивает ввод только русскими буквами
    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        NameStatus.Text = LineEntryRestrictions.TextChangedRu(NameStatus.Text);
    }

    // Обработчик двойного нажатия на элемент DataGrid для редактирования статуса
    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        SimpleDataType simpleDataTypeSelected = DataGrid.SelectedItem as SimpleDataType;
        if(simpleDataTypeSelected != null)
        {
            ElementUpdateStatus.IsVisible = true; // Показ панели редактирования
            NameStausUpdate.Text = simpleDataTypeSelected.Name; // Заполнение поля названием выбранного статуса
        }
    }
}