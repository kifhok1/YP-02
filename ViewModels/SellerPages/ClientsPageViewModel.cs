using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using VKR.Models;
using VKR.Views.Dialogs;

namespace VKR.ViewModels.SellerPages;

// ViewModel для страницы управления клиентами (продавец)
public partial class ClientsPageViewModel : ViewModelBase
{
    // Базовый SQL-запрос для выборки клиентов
    private string _queryBase = "SELECT ID_Clients AS 'ID', Fio AS 'FIO', PhoneNumber, AmountOfNumber FROM clients";

    private string _query = "SELECT ID_Clients AS 'ID', Fio AS 'FIO', PhoneNumber, AmountOfNumber FROM clients;";

    private Client _selectedClient;
    private string _fio;
    private string _phoneNumber = "+7("; // Начальный формат номера телефона
    private bool _isVisiblePanel = false;
    private Window _window;

    // Видимость панели редактирования клиента
    public bool IsVisiblePanel
    {
        get => _isVisiblePanel;
        set => SetProperty(ref _isVisiblePanel, value);
    }

    // Выбранный клиент в DataGrid
    public Client SelectedClient
    {
        get => _selectedClient;
        set => _selectedClient = value;
    }

    // ФИО клиента для редактирования
    public string Fio
    {
        get => _fio;
        set => _fio = value;
    }

    // Номер телефона клиента для редактирования
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = value;
    }

    // Свойство для базового запроса
    public string QueryBase
    {
        get => _queryBase;
    }
    
    // Текущий SQL-запрос (с возможными фильтрами)
    public string Query
    {
        get => _query;
        set => SetProperty(ref _query, value);
    }

    // Строка поиска для фильтрации клиентов
    private string _search;

    public string Search
    {
        get => _search;
        set
        {
            SetProperty(ref _search, value);
            DataTableSelect(); // Автоматическая фильтрация при изменении поискового запроса
        }
    }

    // Коллекция клиентов для отображения в DataGrid
    private ObservableCollection<Client> _clients;

    public ObservableCollection<Client> Clients
    {
        get => _clients;
        set => SetProperty(ref _clients, value);
    }

    // Метод для выборки данных с учетом фильтра поиска
    private void DataTableSelect()
    {
        string query = QueryBase;

        // Добавление фильтра по ФИО если задан поисковый запрос
        if (Search != "")
        {
            query += $" WHERE fio LIKE '%{Search}%'";
        }
        query += ";";
        Clients = SelectTabelClients.SelectTable(query, ConnectToDB.ConnectToDBString());
    }

    // Метод для первоначальной загрузки всех клиентов
    private void FillWorkers()
    {
        Clients = SelectTabelClients.SelectTable(Query, ConnectToDB.ConnectToDBString());
    }

    // Конструктор ViewModel
    public ClientsPageViewModel(Window window)
    {
        _window = window;
        FillWorkers(); // Загрузка клиентов при инициализации
    }

    // Команда для обновления данных клиента
    [RelayCommand]
    public async void UpdateClient()
    {
        // Диалог подтверждения обновления клиента
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel =
            new YesOrNotDialogViewModel("Внимание", "Вы хотите изменить клиета?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        
        try
        {
            // Проверка уникальности номера телефона
            foreach (Client client in Clients)
            {
                if (client.PhoneNumber == PhoneNumber && PhoneNumber != SelectedClient.PhoneNumber)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "Данный номер телефона уже существует", "")
                    };
                    await err.ShowDialog(_window);
                    return;
                }
            }
            
            // Обновление данных клиента в базе данных
            SelectTabelClients.UpdateData(SelectedClient.Id, Fio.Trim(), PhoneNumber.Trim());
            
            // Уведомление об успешном обновлении
            InfoDialogWindow infoDialog = new InfoDialogWindow()
            {
                DataContext = new OkDialogViewModel("Успех", "Запись обновлена!", "")
            };
            await infoDialog.ShowDialog(_window);
            
            // Сброс полей формы
            Fio = "";
            PhoneNumber = "";
            
            // Обновление списка клиентов
            FillWorkers();
        }
        catch (Exception e)
        {
            // Обработка ошибки при обновлении
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Запись не удалось обновить", "")
            };
            await err.ShowDialog(_window);
        }
    }
}