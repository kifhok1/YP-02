using System;
using System.Collections.Generic;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using VKR.Models;
using VKR.Views.Dialogs;

namespace VKR.ViewModels.SellerPages;

// ViewModel для страницы управления заказами (продавец)
public partial class PurchasesPageViewModel : ViewModelBase
{
    private Window _window;
    private ObservableCollection<Order> _order;
    private Order _selectedOrder;

    // Видимость панели для создания Word-отчетов
    private bool _isVisiblePanelWord = false;

    public bool IsVisiblePanelWord
    {
        get => _isVisiblePanelWord;
        set => SetProperty(ref _isVisiblePanelWord, value);
    }
    
    // Список статусов заказов
    private List<SimpleDataType> _status;
    private SimpleDataType _selectedStatus;
    
    // Даты для редактирования заказа
    private DateTime _dateCreated;
    private DateTime _deliveryDate;

    // Дата создания заказа (для редактирования)
    public DateTime DateCreated
    {
        get => _dateCreated;
        set => SetProperty(ref _dateCreated, value);
    }
    
    // Дата доставки заказа (для редактирования)
    public DateTime DeliveryDate
    {
        get => _deliveryDate;
        set => SetProperty(ref _deliveryDate, value);
    }
    
    // Список уникальных дат заказов (год-месяц) для отчетов
    private List<YearMonth> _date = SelectAllDateOrder.SelectDateOrder();
    private YearMonth _selectedDate;

    public List<YearMonth> Date
    {
        get => _date;
    }

    // Выбранная дата для отчета
    public YearMonth SelectedDate
    {
        get => _selectedDate;
        set => SetProperty(ref _selectedDate, value);
    }

    // Строка поиска для фильтрации заказов
    private string _search;

    public string Search
    {
        get => _search;
        set
        {
            SetProperty(ref _search, value); 
            DataTableSelect(); // Автоматическая фильтрация при изменении поиска
        }
    }

    // Поля для сортировки и фильтрации
    private SimpleDataType _sortSelected = new SimpleDataType(0, "---");
    private SimpleDataType _filterSelected = new SimpleDataType(0, "---");

    // Коллекция заказов для отображения в DataGrid
    public ObservableCollection<Order> Order
    {
        get => _order;
        set => SetProperty(ref _order, value);
    }
    
    // Список фильтров по статусам
    private List<SimpleDataType> _categoryFilter;
    
    // Базовый SQL-запрос для выборки заказов с объединением таблиц
    private string queryOrder = @"SELECT ID_Order AS 'ID',
                         ID_Client AS 'ClientID',
                         c.Fio AS 'ClientName',
                         CONCAT(
                                SUBSTRING_INDEX(Fio, ' ', 1), ' ',
                                LEFT(SUBSTRING_INDEX(SUBSTRING_INDEX(Fio, ' ', 2), ' ', -1), 1), '. ',
                                LEFT(SUBSTRING_INDEX(SUBSTRING_INDEX(Fio, ' ', 3), ' ', -1), 1), '.'
                            ) AS 'ClientNameHide',
                         CostOrder,
                         DateOrder,
                         DeliveryDate,
                         s.NameStatus AS 'StatusName',
                         `Status` AS 'StatusID',
                         `Code`,
                         TotalDiscount FROM vkr.order o
                        INNER JOIN clients c ON o.ID_Client = c.ID_Clients
                        INNER JOIN orderstatus s ON o.`status` = s.Id_OrderStatus";

    // SQL-запрос для получения статусов заказов
    private string queryStatus = @"SELECT Id_OrderStatus AS 'ID', NameStatus AS 'Name' FROM vkr.orderstatus;";
    
    public List<SimpleDataType> CategoryFilter
    {
        get => _categoryFilter;
        set => SetProperty(ref _categoryFilter, value);
    }

    // Список вариантов сортировки
    private List<SimpleDataType> _sort;

    public List<SimpleDataType> Sort
    {
        get => _sort;
        set => SetProperty(ref _sort, value);
    }
    
    // Выбранный тип сортировки
    public SimpleDataType SortSelected
    {
        get => _sortSelected;
        set
        {
            SetProperty(ref _sortSelected, value);
            DataTableSelect(); // Автоматическая сортировка при изменении выбора
        }
    }

    // Выбранный фильтр по статусу
    public SimpleDataType FilterSelected
    {
        get => _filterSelected;
        set
        {
            SetProperty(ref _filterSelected, value);
            DataTableSelect(); // Автоматическая фильтрация при изменении статуса
        }
    }

    // Метод для заполнения комбобоксов фильтрации и сортировки
    private void FillComboBox()
    {
        CategoryFilter = SelectTabelSimpleDataType.SelectTableComboBox(
            queryStatus, ConnectToDB.ConnectToDBString());
        FilterSelected = CategoryFilter[0]; // Значение по умолчанию
        
        // Создание списка вариантов сортировки
        Sort = new List<SimpleDataType>();
        Sort.Add(new SimpleDataType(0, "---"));
        Sort.Add(new SimpleDataType(1, "По возростанию"));
        Sort.Add(new SimpleDataType(2, "По убыванию"));
        SortSelected = Sort[0]; // Значение по умолчанию
    }
    
    // Метод для загрузки заказов по заданному запросу
    private void FillOrders(string queryOrder)
    {
        Order = SelectTabelOrder.SelectTable(ConnectToDB.ConnectToDBString(), queryOrder);
    }

    // Выбранный заказ в DataGrid
    public Order SelectedItem
    {
        get
        {
            return _selectedOrder;
        }
        set
        {
            SetProperty(ref _selectedOrder, value);
            OnPropertyChanged(nameof(SelectedItem));
        }
    }

    // Список статусов для редактирования заказа
    public List<SimpleDataType> Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    // Выбранный статус для редактирования заказа
    public SimpleDataType SelectedStatus
    {
        get => _selectedStatus;
        set => SetProperty(ref _selectedStatus, value);
    }

    // Метод для выборки заказов с учетом фильтров, поиска и сортировки
    private void DataTableSelect()
    {
        string query = queryOrder;

        // Добавление фильтра по статусу
        if (FilterSelected.Id > 0)
        {
                query += $" WHERE o.`status` = {FilterSelected.Id}";
        }

        // Добавление фильтра по ФИО клиента
        if (!string.IsNullOrEmpty(Search))
        {
            if (FilterSelected.Id > 0)
            {
                query += $" AND c.Fio LIKE '%{Search}%'";
            }
            else
            {
                query += $" WHERE c.Fio LIKE '%{Search}%'";
            }
        }

        // Добавление сортировки по стоимости заказа
        if (SortSelected.Id > 0)
        {
            if (SortSelected.Id == 1)
            {
                query += " order by CostOrder DESC"; // По убыванию
            }
            else
            {
                query += " order by CostOrder ASC"; // По возрастанию
            }
        }

        query += ";";
        FillOrders(query);
    }
    
    private DateTime? _dateTimeWord;
    private bool _buttonEnabled = false;

    // Доступность кнопки (используется для дополнительной валидации)
    public bool ButtonEnabled
    {
        get => _buttonEnabled;
        set { _buttonEnabled = value; OnPropertyChanged(); }
    }

    // Команда для создания месячного отчета о продажах в формате Word
    [RelayCommand]
    private async void CreateReport()
    {
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel =
            new YesOrNotDialogViewModel("Внимание", "Вы хотите создать отчёт?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        if (!warningViewModel.Flag)
        {
            return;
        }
        
        // Проверка выбора даты
        if (SelectedDate == null)
        {
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Дата не выбрана", "")
            };
            await err.ShowDialog(_window);
            return;
        }
        else
        {
            try
            {
                int year = SelectedDate.Year;
                int month = SelectedDate.Month;
                
                // Получение данных о продажах за выбранный месяц
                List<SalesData> numOuth = SelectSalesData.GetSalesData(year, month);
                
                // Проверка наличия данных о продажах
                if (numOuth.Count == 0)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", "В этом месяце не было продаж", "")
                    };
                    await err.ShowDialog(_window);
                    return;
                }
                else
                {
                    // Диалог сохранения файла
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "Путь сохранения документа";
                    saveFileDialog.InitialFileName = $"Отчёт_за_{year}_{month}.docx";
                    
                    var result = await saveFileDialog.ShowAsync(new Window());

                    if (result == null || result.Length == 0)
                    {
                        ErrorDialogWindow err = new ErrorDialogWindow()
                        {
                            DataContext = new OkDialogViewModel("Ошибка", "Путь сохранения не выбран", "")
                        };
                        await err.ShowDialog(_window);
                        return;
                    }

                    // Генерация отчета о продажах в формате Word
                    SalesReportGenerator.GenerateSalesReport(result, year, month, numOuth);
                    
                    InfoDialogWindow info = new InfoDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Успех", "Отчёт создан!", "")
                    };
                    await info.ShowDialog(_window);
                    IsVisiblePanelWord = false; // Скрытие панели после создания отчета
                }
            }
            catch (Exception ex)
            {
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", $"Ошибка создания отчёта", "")
                };
                await err.ShowDialog(_window);
            }
        }
    }
    
    // Команда для создания чека для выбранного заказа
    [RelayCommand]
    private async void CreateReportCheque()
    {
        // Создание объекта NewOrder на основе выбранного заказа
        NewOrder OrderNew = new NewOrder(SelectedItem.ClientId, DateCreated, DeliveryDate, SelectTabelProduct.SelectTable(SelectedItem.Id));
        
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel =
            new YesOrNotDialogViewModel("Внимание", "Вы хотите напечатать чек?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        if (!warningViewModel.Flag)
        {
            return;
        }
        try
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog(); 
            saveFileDialog.Title = "Путь сохранения документа"; 
            saveFileDialog.InitialFileName = $"Чек_для_{OrderNew.ClientOrder.Fio.Replace(' ', '_')}_от_{OrderNew.Date.ToString().Split(' ')[0]}.docx";
            
            var result = await saveFileDialog.ShowAsync(new Window());
            
            if (result == null || result.Length == 0) 
            { 
                ErrorDialogWindow err = new ErrorDialogWindow() 
                { 
                    DataContext = new OkDialogViewModel("Ошибка", "Путь сохранения не выбран", "")
                };
                await err.ShowDialog(_window); 
                return;
            }
            // Генерация чека в формате Word
            ChequeReportGenerator.GenerateOrderReceipt(OrderNew, result);
            
            InfoDialogWindow info = new InfoDialogWindow() 
            { 
                DataContext = new OkDialogViewModel("Успех", "Чек создан!", "")
            }; 
            await info.ShowDialog(_window);
        }
        catch (Exception ex)
        { 
            ErrorDialogWindow err = new ErrorDialogWindow() 
            { 
                DataContext = new OkDialogViewModel("Ошибка", $"Ошибка создания чека {ex.Message.ToString()}", "")
            }; 
            await err.ShowDialog(_window); 
        }
    }
    
    // Команда для отображения панели создания Word-отчетов
    [RelayCommand]
    private void ShowPanelWord()
    {
        IsVisiblePanelWord = true;
    }

    // Команда для обновления данных заказа (статуса и даты доставки)
    [RelayCommand]
    private async void UpdateOrder()
    {
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel =
            new YesOrNotDialogViewModel("Внимание", "Вы хотите изменить заказ?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        if (!warningViewModel.Flag)
        {
            return;
        }

        try
        {
            // Обновление заказа в базе данных
            SelectTabelOrder.UpdateTable(ConnectToDB.ConnectToDBString(), DeliveryDate, SelectedStatus.Id, SelectedItem.Id);
            
            // Обновление списка заказов
            DataTableSelect();
            
            InfoDialogWindow info = new InfoDialogWindow()
            {
                DataContext = new OkDialogViewModel("Успех", "Запись обновлена!", "")
            };
            await info.ShowDialog(_window);
        }
        catch (Exception e)
        {
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Запись не удалось обновить", "")
            };
            await err.ShowDialog(_window);
        }
        
    }
    
    // Конструктор ViewModel
    public PurchasesPageViewModel(Window window)
    {
        _window = window;
        DataTableSelect(); // Загрузка заказов
        FillComboBox();    // Загрузка фильтров и сортировок
        
        // Загрузка списка статусов для редактирования заказов
        Status = SelectTabelSimpleDataType.SelectTableComboBox(queryStatus, ConnectToDB.ConnectToDBString());
    }
}