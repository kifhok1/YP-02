using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using VKR.Models;

namespace VKR.Views.AdminPages;

public partial class CategoriesPageView : UserControl
{
    public CategoriesPageView()
    {
        InitializeComponent();
    }

    // Обработчик нажатия кнопки для скрытия панелей добавления/редактирования
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementAddCategories.IsVisible = false; // Скрытие панели добавления категории
        ElementUpdateCategories.IsVisible = false; // Скрытие панели редактирования категории
    }

    // Обработчик изменения текста в поле ввода названия категории (для добавления)
    // Ограничивает ввод только русскими буквами
    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        NameCategories.Text = LineEntryRestrictions.TextChangedRu(NameCategories.Text);
    }

    // Обработчик двойного нажатия на элемент DataGrid для редактирования категории
    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        SimpleDataType simpleDataTypeSelected = DataGrid.SelectedItem as SimpleDataType;
        if(simpleDataTypeSelected != null)
        {
            ElementUpdateCategories.IsVisible = true; // Показ панели редактирования
            NameCategoriesUpdate.Text = simpleDataTypeSelected.Name; // Заполнение поля названием выбранной категории
        }
    }

    // Обработчик изменения текста в поле ввода названия категории (для редактирования)
    // Ограничивает ввод только русскими буквами
    private void TextBoxUpdate_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        NameCategoriesUpdate.Text = LineEntryRestrictions.TextChangedRu(NameCategoriesUpdate.Text);
    }
}