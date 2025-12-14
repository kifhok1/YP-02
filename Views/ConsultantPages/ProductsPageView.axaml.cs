using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using VKR.Models;

namespace VKR.Views.ConsultantPages;

public partial class ProductsPageView : UserControl
{
    public ProductsPageView()
    {
        InitializeComponent();
    }

    // Обработчик нажатия кнопки для скрытия панели редактирования
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Element.IsVisible = false; // Скрытие панели редактирования товара
    }

    // Обработчик изменения текста в поле названия товара
    // Разрешает ввод русских и английских букв, цифр
    private void Name_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Name.Text = LineEntryRestrictions.TextChangedRuEnNum(Name.Text);
    }

    // Обработчик изменения текста в поле количества на складе
    // Ограничивает ввод только цифрами
    private void CountInStoke_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        CountInStoke.Text = LineEntryRestrictions.TextChangedNum(CountInStoke.Text);
    }

    // Обработчик изменения текста в поле скидки
    // Ограничивает ввод только цифрами
    private void Discount_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Discount.Text = LineEntryRestrictions.TextChangedNum(Discount.Text);
    }

    // Обработчик изменения текста в поле цены
    // Ограничивает ввод только цифрами
    private void Price_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Price.Text = LineEntryRestrictions.TextChangedNum(Price.Text);
    }

    // Обработчик нажатия кнопки обновления (скрытие панели без очистки полей)
    private void ButtonUpdate_OnClick(object? sender, RoutedEventArgs e)
    {
        Element.IsVisible = false; // Скрытие панели редактирования
    }

    // Обработчик двойного нажатия на элемент DataGrid для редактирования товара
    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        Element.IsVisible = true; // Показ панели редактирования
        if (sender is DataGrid dataGrid)
        {
            Product selectedItem = (Product)dataGrid.SelectedItem;

            if (selectedItem != null)
            {
                // Заполнение полей данными выбранного товара
                Name.Text = selectedItem.Name;
                Price.Text = Convert.ToString(selectedItem.Cost);
                Discount.Text = Convert.ToString(selectedItem.Discount);
                CountInStoke.Text = Convert.ToString(selectedItem.CountInStock);
                Category.SelectedIndex = selectedItem.CategoryId; // Установка выбранной категории
                
                // Отображение изображения товара
                ImageUser.Source = selectedItem.Image;
                NonImageBorder.IsVisible = false; // Скрытие рамки для отсутствующего изображения
                ImageBorder.IsVisible = true;     // Показ контейнера с изображением
            }
        }
    }
}