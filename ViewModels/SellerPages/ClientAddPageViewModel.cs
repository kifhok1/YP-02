using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using VKR.Models;
using VKR.Views.Dialogs;

namespace VKR.ViewModels.SellerPages;

// ViewModel для страницы добавления нового клиента (продавец)
public partial class ClientAddPageViewModel : ViewModelBase
{
    private string _fio = "";
    private string _phoneNumber = "+7("; // Начальный формат номера телефона
    private bool _buttonIsertEnable = false;
    private Window _window;

    // ФИО клиента
    public string Fio
    {
        get { return _fio; }
        set
        {
            _fio = value;
            OnPropertyChanged();
            EnabledInsertButton(); // Проверка доступности кнопки при изменении ФИО
        }
    }

    // Доступность кнопки добавления клиента
    public bool ButtonIsertEnable
    {
        get { return _buttonIsertEnable; }
        set
        {
            _buttonIsertEnable = value;
            OnPropertyChanged();
        }
    }

    // Номер телефона клиента
    public string PhoneNumber
    {
        get { return _phoneNumber; }
        set
        {
            _phoneNumber = value;
            OnPropertyChanged();
            EnabledInsertButton(); // Проверка доступности кнопки при изменении номера
        }
    }

    // Метод для проверки доступности кнопки добавления клиента
    private void EnabledInsertButton()
    {
        if (Fio.Trim() == "" ||            // ФИО не должно быть пустым
            PhoneNumber.Length < 16)       // Полный номер телефона (формат +7(XXX)XXX-XX-XX)
        {
            ButtonIsertEnable = false;
        }
        else
        {
            ButtonIsertEnable = true;
        }
    }

    // Конструктор ViewModel
    public ClientAddPageViewModel(Window window)
    {
        _window = window;
    }

    // Команда для добавления нового клиента
    [RelayCommand]
    public async void InsertUser()
    {
        // Диалог подтверждения добавления клиента
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel =
            new YesOrNotDialogViewModel("Внимание", "Вы хотите добавить клиета?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        if (!warningViewModel.Flag)
        {
            return;
        }
        try
        {
            // Загрузка существующих клиентов для проверки уникальности номера телефона
            ObservableCollection<Client> table = SelectTabelClients.SelectTable(
                @"SELECT ID_Clients AS 'ID',
                                 Fio AS 'FIO',
                                 PhoneNumber, 
                                    CONCAT(
                                        SUBSTRING_INDEX(Fio, ' ', 1), ' ',
                                        LEFT(SUBSTRING_INDEX(SUBSTRING_INDEX(Fio, ' ', 2), ' ', -1), 1), '. ',
                                        LEFT(SUBSTRING_INDEX(SUBSTRING_INDEX(Fio, ' ', 3), ' ', -1), 1), '.'
                                    ) AS 'FIO_Hide',
                                    CONCAT(
                                        SUBSTRING(PhoneNumber, 1, 7),
                                        'xxx-xx-xx'
                                    ) AS 'PhoneNumber_Hide',
                                 AmountOfNumber
                                 FROM clients;", 
                ConnectToDB.ConnectToDBString());
            
            // Проверка уникальности номера телефона
            foreach (var VARIABLE in table)
            {
                if (VARIABLE.PhoneNumber == PhoneNumber)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Клиент с данным номером телефона уже сущевствует", "")
                    };
                    await err.ShowDialog(_window);
                    return;
                }
            }
            
            // SQL-запрос для добавления нового клиента (сумма накоплений = 0)
            string query = $"INSERT INTO `vkr`.`clients` (`Fio`, `PhoneNumber`, `AmountOfNumber`) VALUES ('{Fio.Trim()}', '{PhoneNumber.Trim()}', '0');";
            
            // Выполнение запроса на добавление
            SelectTabelClients.InsertData(query);
            
            // Сброс полей формы после успешного добавления
            Fio = "";
            PhoneNumber = "";
            ButtonIsertEnable = false;
            
            // Уведомление об успешном добавлении
            InfoDialogWindow info = new InfoDialogWindow()
            {
                DataContext = new OkDialogViewModel("Успех", "Добавлена запись!", "")
            };
            await info.ShowDialog(_window);
        }
        catch (Exception e)
        {
            // Обработка ошибки при добавлении клиента
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Запись не удалось добавить", "")
            };
            await err.ShowDialog(_window);
        }
    }
}