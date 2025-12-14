using Avalonia.Controls;
using VKR.Models;

namespace VKR.Views.ConsultantPages;

public partial class ProductAddPageView : UserControl
{
    public ProductAddPageView()
    {
        InitializeComponent();
    }

    // Обработчик изменения текста в поле названия товара
    // Разрешает ввод русских и английских букв, цифр
    private void ProductName_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        ProductName.Text = LineEntryRestrictions.TextChangedRuEnNum(ProductName.Text);
    }

    // Обработчик изменения текста в поле цены
    // Ограничивает ввод только цифрами
    private void Price_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Price.Text = LineEntryRestrictions.TextChangedNum(Price.Text);
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
}