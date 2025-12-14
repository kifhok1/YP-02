using System;
using Avalonia.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using VKR.Models;
using VKR.Views.Dialogs;

namespace VKR.ViewModels.SellerPages;

// ViewModel для страницы добавления покупки/оформления заказа (продавец)
public partial class PurchaseAddPageViewModel : ViewModelBase
{
    private Window _window;
    private NewOrder orderNew;
    private string _name = "";
    private byte[] _imageBlob;
    private double _price;
    private int _discount;
    private int _countInStoke;
    private SimpleDataType _categorySelected = new SimpleDataType(0, "---");
    private List<SimpleDataType> _listCategory;
    private bool _buttonIsertEnable;
    private ObservableCollection<Product> _products;
    private Product _selectedProduct;
    private ObservableCollection<Product> _productsToTrash;
    private DateTime _dateCreated = DateTime.Now;
    private DateTime _deliveryDate = DateTime.Now.AddDays(3);
    private bool _isVisibleAddOrder = false;
    private bool _isVisibleOrderConfirmation =  false;
    private bool _isEanabledOrderConfirmation =  false;
    
    // SQL-запрос для получения клиентов с форматом ФИО + последние 5 цифр номера телефона
    private static string _clientQuery = @"SELECT 
                                        c.ID_Clients AS 'ID',
                                        CONCAT(
                                            c.Fio, 
                                            ' ',
                                            RIGHT(c.PhoneNumber, 5)  -- Берем последние 5 символов (формат XX-XX)
	                                    ) AS 'Name'
                                    FROM clients c;";
    private List<SimpleDataType> _clients =  SelectTabelSimpleDataType.SelectTableComboBox(_clientQuery, ConnectToDB.ConnectToDBString());
    private SimpleDataType _selectedClients;

    // Видимость панели оформления заказа
    public bool IsVisibleAddOrder
    {
        get => _isVisibleAddOrder;
        set => SetProperty(ref _isVisibleAddOrder, value);
    }

    // Видимость панели подтверждения заказа
    public bool IsVisibleOrderConfirmation
    {
        get => _isVisibleOrderConfirmation;
        set => SetProperty(ref _isVisibleOrderConfirmation, value);
    }

    // Доступность кнопки подтверждения заказа
    public bool IsEnabledOrderConfirmation
    {
        get => _isEanabledOrderConfirmation;
        set => SetProperty(ref _isEanabledOrderConfirmation, value);
    }
    
    // Список клиентов для выбора
    public List<SimpleDataType> Clients
    {
        get => _clients;
        set => SetProperty(ref _clients, value);
    }

    // Выбранный клиент
    public SimpleDataType SelectedClients
    {
        get => _selectedClients;
        set => SetProperty(ref _selectedClients, value);
    }
    
    // Дата создания заказа (только для чтения)
    public DateTime DateCreated
    {
        get => _dateCreated;
    }
    
    // Дата доставки заказа
    public DateTime DeliveryDate
    {
        get => _deliveryDate;
        set => SetProperty(ref _deliveryDate, value);
    }
    
    // Минимальная дата доставки (сегодня)
    public DateTime DeliveryDateStart
    {
        get => DateCreated;
    }
    
    // Максимальная дата доставки (через месяц)
    public DateTime DeliveryDateEnd
    {
        get => DateCreated.AddMonths(1);
    }

    // Новый заказ
    public NewOrder OrderNew
    {
        get => orderNew;
        set => SetProperty(ref orderNew, value);
    }
    
    // Список категорий товаров
    public List<SimpleDataType> ListCategory
    {
        get => _listCategory;
        set => SetProperty(ref _listCategory, value);
    }

    // Доступность кнопки добавления
    public bool ButtonInsertEnable
    {
        get => _buttonIsertEnable;
        set => SetProperty(ref _buttonIsertEnable, value);
    }

    // Коллекция всех товаров
    public ObservableCollection<Product> Products
    {
        get => _products;
        set
        {
            SetProperty(ref _products, value);
            UpdateTrash(); // Обновление корзины при изменении списка товаров
        }
    }

    // Выбранный товар в списке
    public Product SelectedProduct
    {
        get => _selectedProduct;
        set => SetProperty(ref _selectedProduct, value);
    }

    // Базовый SQL-запрос для выборки товаров
    private string _queryBase = "SELECT ID_Product AS 'ID', ProductName AS 'Name', CategoryProduct AS 'Category', ProductCategory AS 'CategoryId', ProductCountInStock AS 'CountInStock', ProductDiscount AS 'Discount', ProductCost AS 'Cost', ProductImage FROM product JOIN vkr.categoryproduct c on c.ID_CategoryProduct = product.ProductCategory";

    private string _query = "SELECT ID_Product AS 'ID', ProductName AS 'Name', CategoryProduct AS 'Category', ProductCategory AS 'CategoryId', ProductCountInStock AS 'CountInStock', ProductDiscount AS 'Discount', ProductCost AS 'Cost', ProductImage FROM product JOIN vkr.categoryproduct c on c.ID_CategoryProduct = product.ProductCategory;";

    public string QueryBase
    {
        get => _queryBase;
    }

    private string _search;
    private SimpleDataType _sortSelected = new SimpleDataType(0, "---");
    private SimpleDataType _filterSelected = new SimpleDataType(0, "---");

    // Строка поиска для фильтрации товаров
    public string Search
    {
        get => _search;
        set
        {
            SetProperty(ref _search, value);
            DataTableSelect(); // Автоматическая фильтрация при изменении поискового запроса
        }
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
    
    // Выбранный фильтр по категории
    public SimpleDataType FilterSelected
    {
        get => _filterSelected;
        set
        {
            SetProperty(ref _filterSelected, value);
            DataTableSelect(); // Автоматическая фильтрация при изменении категории
        }
    }

    // Список категорий для фильтрации
    private List<SimpleDataType> _categoryFilter;

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

    // Текущий SQL-запрос
    public string Query
    {
        get => _query;
        set => SetProperty(ref _query, value);
    }

    // Товары, добавленные в корзину
    public ObservableCollection<Product> ProductsToTrash
    {
        get => _productsToTrash;
        set => SetProperty(ref _productsToTrash, value);
    }

    // Метод для загрузки товаров
    private void FillProducts()
    {
        Products = SelectTabelProduct.SelectTableNoneImage(Query, ConnectToDB.ConnectToDBString());
    }

    // Метод для заполнения комбобоксов фильтрации и сортировки
    private void FillComboBox()
    {
        CategoryFilter = SelectTabelSimpleDataType.SelectTableComboBox(
            "SELECT ID_CategoryProduct AS 'ID', CategoryProduct AS 'Name' FROM categoryproduct;",
            ConnectToDB.ConnectToDBString());
        FilterSelected = CategoryFilter[0];
        Sort = new List<SimpleDataType>();
        Sort.Add(new SimpleDataType(0, "---"));
        Sort.Add(new SimpleDataType(1, "По возростанию"));
        Sort.Add(new SimpleDataType(2, "По убыванию"));
        SortSelected = Sort[0];
    }

    // Метод для выборки товаров с учетом фильтров, поиска и сортировки
    private void DataTableSelect()
    {
        string query = QueryBase;

        if (Search != "")
        {
            query += $" WHERE product.ProductName LIKE '%{Search}%'";
        }

        if (FilterSelected.Id > 0)
        {
            if (Search != "")
            {
                query += $" AND ProductCategory = {FilterSelected.Id}";
            }
            else
            {
                query += $" WHERE ProductCategory = {FilterSelected.Id}";
            }
        }

        if (SortSelected.Id > 0)
        {
            if (SortSelected.Id == 1)
            {
                query += " order by product.ProductCost DESC"; // По убыванию
            }
            else
            {
                query += " order by product.ProductCost ASC"; // По возрастанию
            }
        }

        query += ";";
        Products = SelectTabelProduct.SelectTableNoneImage(query, ConnectToDB.ConnectToDBString());
    }

    // Обновление списка товаров в корзине
    private void UpdateTrash()
    {
        ProductsToTrash = new ObservableCollection<Product>();
        foreach (var product in Products)
        {
            if (product.Trash != 0)
            {
                ProductsToTrash.Add(product);
            }
        }
    }

    // Команда для скрытия панели подтверждения заказа
    [RelayCommand]
    private void OrderConfirmationHide()
    {
        IsVisibleOrderConfirmation = false;
    }
    
    // Команда для скрытия панели оформления заказа
    [RelayCommand]
    private void AddOrderHide()
    {
        IsVisibleAddOrder = false;
    }
    
    // Команда для перехода к оформлению заказа (проверка корзины)
    [RelayCommand]
    private async void GoToTrash()
    {
        UpdateTrash();
        int count = 0;
        foreach (var product in ProductsToTrash)
        {
            count += product.Trash;
        }
        // Проверка, что корзина не пуста
        if (ProductsToTrash.Count == 0 || count == 0)
        {
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Корзина пуста", "")
            };
            await err.ShowDialog(_window);
            return;
        }
        else
        {
            IsVisibleAddOrder =  true;
        }
    }

    // Команда для подтверждения заказа
    [RelayCommand]
    private async void OrderConfirmation()
    { 
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel =
            new YesOrNotDialogViewModel("Внимание", "Вы хотите оформить заказ?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        if (!warningViewModel.Flag)
        {
            return;
        }

        try
        {
            // Проверка выбора клиента
            if (SelectedClients == null || SelectedClients.Id == 0)
            {
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Не выбран клиент", "")
                };
                await err.ShowDialog(_window);
                return;
            }

            // Проверка, что корзина не пуста
            int count = 0;
            foreach (var product in ProductsToTrash)
            {
                count += product.Trash;
            }
            if (ProductsToTrash.Count == 0 || count == 0)
            {
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Корзина пуста", "")
                };
                await err.ShowDialog(_window);
                return;
            }
            
            // Создание нового заказа
            OrderNew = new NewOrder(SelectedClients.Id, DateCreated, DeliveryDate, ProductsToTrash);
            IsVisibleOrderConfirmation = true;
        }
        catch (Exception e)
        {
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Запись не обработать заказ!", "")
            };
            await err.ShowDialog(_window);
        }
        
    }
    
    // Команда для создания и сохранения чека в формате Word
    [RelayCommand]
    private async void CreateReport()
    {
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
            // Диалог сохранения файла
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
            IsEnabledOrderConfirmation = true;

        }
        catch (Exception ex)
        { 
            ErrorDialogWindow err = new ErrorDialogWindow() 
            { 
                DataContext = new OkDialogViewModel("Ошибка", $"Ошибка создания чека", "")
            }; 
            await err.ShowDialog(_window); 
        }
    }

    // Команда для добавления нового заказа в базу данных
    [RelayCommand]
    private async void AddingNewOrder()
    {
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel =
            new YesOrNotDialogViewModel("Внимание", "Вы хотите добавить заказ?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        if (!warningViewModel.Flag)
        {
            return;
        }
        try
        { 
            // Вставка заказа в базу данных с транзакцией
            SelectTabelOrder.InsertTable(OrderNew);
            
            // Очистка данных после успешного добавления
            OrderNew = null;
            ProductsToTrash.Clear();
            IsVisibleOrderConfirmation = false;
            IsVisibleAddOrder =  false;
            DataTableSelect();
            
            InfoDialogWindow info = new InfoDialogWindow() 
            { 
                DataContext = new OkDialogViewModel("Успех", "Заказ оформлен!", "")
            }; 
            await info.ShowDialog(_window); 
            IsEnabledOrderConfirmation = true;

        }
        catch (Exception ex)
        { 
            ErrorDialogWindow err = new ErrorDialogWindow() 
            { 
                DataContext = new OkDialogViewModel("Ошибка", $"Ошибка оформеления закза {ex.Message}", "")
            }; 
            await err.ShowDialog(_window); 
        }
    }

    // Конструктор ViewModel
    public PurchaseAddPageViewModel(Window window)
    {
        _window = window;
        FillProducts(); // Загрузка товаров
        FillComboBox(); // Загрузка фильтров и сортировок
    }
}