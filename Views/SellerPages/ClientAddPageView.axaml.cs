using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Text;
using VKR.Models;

namespace VKR.Views.SellerPages;

public partial class ClientAddPageView : UserControl
{
    public ClientAddPageView()
    {
        InitializeComponent();
        // Подписка на событие нажатия клавиш для поля номера телефона
        PhoneNumber.AddHandler(KeyDownEvent, PhoneBox_KeyDown, RoutingStrategies.Tunnel);
    }
    
    // StringBuilder для хранения введенных цифр номера телефона
    private StringBuilder _digits = new StringBuilder(10);

    // Метод форматирования номера телефона в российском формате
    private string FormatPhone(string digits)
    {
        if (digits == null) digits = "";
        if (digits.Length > 10) digits = digits.Substring(0, 10);

        var d = digits;
        var result = new StringBuilder();
        result.Append("+7("); // Начало формата

        if (d.Length >= 1)
            result.Append(d.Substring(0, Math.Min(3, d.Length)));

        if (d.Length >= 3)
            result.Append(")");

        if (d.Length > 3)
        {
            var start = 3;
            var len = Math.Min(3, d.Length - start);
            result.Append(d.Substring(start, len));
        }

        if (d.Length > 6)
        {
            result.Append("-");
            var start = 6;
            var len = Math.Min(2, d.Length - start);
            result.Append(d.Substring(start, len));
        }

        if (d.Length > 8)
        {
            result.Append("-");
            var start = 8;
            var len = Math.Min(2, d.Length - start);
            result.Append(d.Substring(start, len));
        }

        return result.ToString();
    }

    // Обновление отображаемого текста номера телефона
    private void UpdateDisplayedText()
    {
        if (_digits.Length > 10)
            _digits.Length = 10;

        PhoneNumber.Text = FormatPhone(_digits.ToString());
        PhoneNumber.CaretIndex = PhoneNumber.Text?.Length ?? 0; // Установка курсора в конец
    }

    // Обработчик нажатия на поле номера телефона
    private void PhoneBox_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        PhoneNumber.Focus();
        PhoneNumber.CaretIndex = PhoneNumber.Text.Length;
        e.Pointer.Capture(null); // Освобождение захвата указателя
        e.Handled = true;
    }

    // Обработчик нажатия клавиш в поле номера телефона
    private async void PhoneBox_KeyDown(object? sender, KeyEventArgs e)
    {
        // Обработка клавиши Backspace (удаление последней цифры)
        if (e.Key == Key.Back)
        {
            if (_digits.Length > 0)
            {
                _digits = new StringBuilder(_digits.ToString().Substring(0, _digits.Length - 1));
                UpdateDisplayedText();
            }
            e.Handled = true;
            return;
        }

        // Обработка клавиши Delete (очистка всего номера)
        if (e.Key == Key.Delete)
        {
            if (_digits.Length > 0)
            {
                _digits.Clear();
                UpdateDisplayedText();
            }
            e.Handled = true;
            return;
        }

        char? ch = null;
        // Обработка цифровых клавиш основной клавиатуры
        if (e.Key >= Key.D0 && e.Key <= Key.D9)
            ch = (char)('0' + (e.Key - Key.D0));
        // Обработка цифровых клавиш дополнительной клавиатуры
        else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            ch = (char)('0' + (e.Key - Key.NumPad0));

        if (ch != null)
        {
            // Добавление цифры если не превышен лимит в 10 цифр
            if (_digits.Length < 10)
            {
                _digits.Append(ch.Value);
                UpdateDisplayedText();
            }
            e.Handled = true;

            return;
        }

        // Обработка служебных клавиш (курсор, табуляция и т.д.)
        if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Home || e.Key == Key.End
            || e.Key == Key.Tab || e.Key == Key.PageUp || e.Key == Key.PageDown)
        {
            PhoneNumber.CaretIndex = PhoneNumber.Text.Length; // Курсор всегда в конце
            e.Handled = true;
            return;
        }

        PhoneNumber.CaretIndex = PhoneNumber.Text.Length;
        e.Handled = true;
    }
    
    // Обработчик изменения текста в поле ФИО
    // Ограничивает ввод только русскими буквами, пробелами и дефисами
    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        FIO.Text = LineEntryRestrictions.TextChangedFio(FIO.Text);
    }
}