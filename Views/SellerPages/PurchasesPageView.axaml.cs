using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using VKR.Models;

namespace VKR.Views.SellerPages;

public partial class PurchasesPageView : UserControl
{
    public PurchasesPageView()
    {
        InitializeComponent();
    }

    // Обработчик нажатия кнопки для скрытия панели редактирования заказа
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Element.IsVisible = false; // Скрытие панели редактирования заказа
            
        // Установка ограничений для даты доставки (от даты создания до +1 месяца)
        CalendarDeliveryDatePicker.DisplayDateStart = null;
        CalendarDeliveryDatePicker.DisplayDateEnd = null;
    }

    // Обработчик двойного нажатия на элемент DataGrid для редактирования заказа
    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        Order selectedOrder = (Order)DataGridOrder.SelectedItem;
        if (selectedOrder != null)
        {
            Element.IsVisible = true; // Показ панели редактирования
            
            // Заполнение полей данными выбранного заказа
            Status.SelectedIndex = selectedOrder.StatusId; // Установка текущего статуса
            FIO.Content = $"{selectedOrder.ClientView}"; // Отображение ФИО клиента
            Code.Content = $"Код получения: {selectedOrder.Code}"; // Отображение кода заказа
            
            // Установка дат в календарях
            CalendarDatePicker.SelectedDate = Convert.ToDateTime(selectedOrder.Date);
            CalendarDeliveryDatePicker.SelectedDate = Convert.ToDateTime(selectedOrder.DeliveryDate);
            
            // Установка ограничений для даты доставки (от даты создания до +1 месяца)
            CalendarDeliveryDatePicker.DisplayDateStart = Convert.ToDateTime(selectedOrder.Date);
            CalendarDeliveryDatePicker.DisplayDateEnd = Convert.ToDateTime(selectedOrder.Date).AddMonths(1);
            
            // Отображение финансовой информации
            TotalCost.Content = $"Общая цена заказа {selectedOrder.CostOrder:C}";
            TotalDiscount.Content = $"Скидка: {selectedOrder.TotalDiscount}%";
            CostDiscount.Content = $"Цена со скидкой: {(selectedOrder.CostOrder - (selectedOrder.CostOrder * selectedOrder.TotalDiscount / 100)):C}";
        }
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
        e.Handled = true; // Обработка события завершена, дальнейшая обработка не требуется
    }

    // Обработчик нажатия кнопки для скрытия панели создания Word-отчета
    private void ButtonWord_OnClick(object? sender, RoutedEventArgs e)
    {
        ElementUpdateWord.IsVisible = false; // Скрытие панели создания отчетов
    }
}