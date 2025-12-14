using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using VKR.Models;
using VKR.Views.Dialogs;

namespace VKR.ViewModels.ConsultantPages;

// ViewModel для страницы управления товарами (консультант)
public partial class ProductsPageViewModel : ViewModelBase
{
    // Приватные поля для хранения данных о товаре
    private string _name = "";
    private byte[] _imageBlob;
    private double _price;
    private int _discount;
    private int _countInStoke;
    private SimpleDataType _categorySelected = new SimpleDataType(0, "---");
    private List<SimpleDataType> _listCategory;
    private bool _buttonIsertEnable;
    private bool _borderVisible = true;
    private bool _imageVisible = false;
    private Bitmap _image;
    private ObservableCollection<Product> _products;
    private Product _selectedProduct;

    private Window _window;

    // Название товара
    public string Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
            EnabledInsertButton(); // Проверка доступности кнопки при изменении названия
        }
    }

    // Скидка на товар (с преобразованием для отображения)
    public string Discount
    {
        get
        {
            if (_discount == 0)
            {
                return "";
            }
            else
            {
                return _discount.ToString();
            }
        }
        set
        {
            // Валидация и преобразование ввода с использованием статического метода
            if (string.IsNullOrEmpty(LineEntryRestrictions.TextChangedNum(value)))
            {
                SetProperty(ref _discount, 0);
                EnabledInsertButton();
            }
            else
            {
                SetProperty(ref _discount, Convert.ToInt32(LineEntryRestrictions.TextChangedNum(value)));
                EnabledInsertButton();
            }
        }
    }

    // Количество товара на складе (с преобразованием для отображения)
    public string CountInStoke
    {
        get
        {
            if (_countInStoke == 0)
            {
                return "";
            }
            else
            {
                return _countInStoke.ToString();
            }
        }
        set
        {
            // Валидация и преобразование ввода
            if (!string.IsNullOrEmpty(LineEntryRestrictions.TextChangedNum(value)))
            {
                SetProperty(ref _countInStoke, Convert.ToInt32(LineEntryRestrictions.TextChangedNum(value))); 
                EnabledInsertButton(); 
            }
            else 
            { 
                SetProperty(ref _countInStoke, 0); 
                EnabledInsertButton();
            }
        }
    }

    // Цена товара (с преобразованием для отображения)
    public string Price
    {
        get
        {
            if (_price.Equals(0.0))
            {
                return "";
            }
            else
            {
                return _price.ToString();
            }
        }
        set
        {
            // Валидация и преобразование ввода цены
            if (!string.IsNullOrEmpty(LineEntryRestrictions.TextChangedNum(value)))
            {
                SetProperty(ref _price, Convert.ToInt32(LineEntryRestrictions.TextChangedNum(value)));
                EnabledInsertButton();
            }
            else
            {
                SetProperty(ref _price, 0);
                EnabledInsertButton();
            }
        }
    }

    // Выбранная категория товара
    public SimpleDataType CategorySelected
    {
        get => _categorySelected;
        set
        {
            SetProperty(ref _categorySelected, value);
            EnabledInsertButton(); // Проверка доступности кнопки при изменении категории
        }
    }

    // Список доступных категорий товаров
    public List<SimpleDataType> ListCategory
    {
        get => _listCategory;
        set => SetProperty(ref _listCategory, value);
    }

    // Доступность кнопки сохранения изменений
    public bool ButtonIsertEnable
    {
        get => _buttonIsertEnable;
        set => SetProperty(ref _buttonIsertEnable, value);
    }

    // Видимость рамки для изображения (когда изображение не загружено)
    public bool BorderVisible
    {
        get => _borderVisible;
        set => SetProperty(ref _borderVisible, value);
    }

    // Видимость загруженного изображения товара
    public bool ImageVisible
    {
        get => _imageVisible;
        set => SetProperty(ref _imageVisible, value);
    }

    // Изображение товара
    public Bitmap Image
    {
        get => _image;
        set
        {
            SetProperty(ref _image, value);
            EnabledInsertButton(); // Проверка доступности кнопки при изменении изображения
        }
    }

    // Коллекция товаров для отображения в DataGrid
    public ObservableCollection<Product> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    // Выбранный товар в DataGrid
    public Product SelectedProduct
    {
        get => _selectedProduct;
        set => SetProperty(ref _selectedProduct, value);
    }

    // Метод для проверки доступности кнопки сохранения изменений
    private void EnabledInsertButton()
    {
        if (_price != 0 ||
            Image is null ||
            CategorySelected.Equals(ListCategory[0]))
        {
            ButtonIsertEnable = false;
        }
        else
        {
            ButtonIsertEnable = true;
        }
    }

    // Базовый SQL-запрос для выборки товаров
    private string _queryBase = "SELECT ID_Product AS 'ID', ProductName AS 'Name', CategoryProduct AS 'Category', ProductCategory AS 'CategoryId', ProductCountInStock AS 'CountInStock', ProductDiscount AS 'Discount', ProductCost AS 'Cost', ProductImage FROM product JOIN vkr.categoryproduct c on c.ID_CategoryProduct = product.ProductCategory";

    private string _query = "SELECT ID_Product AS 'ID', ProductName AS 'Name', CategoryProduct AS 'Category', ProductCategory AS 'CategoryId', ProductCountInStock AS 'CountInStock', ProductDiscount AS 'Discount', ProductCost AS 'Cost', ProductImage FROM product JOIN vkr.categoryproduct c on c.ID_CategoryProduct = product.ProductCategory;";

    // Свойство для базового запроса
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

    private List<SimpleDataType> _categoryFilter;

    // Список категорий для фильтрации
    public List<SimpleDataType> CategoryFilter
    {
        get => _categoryFilter;
        set => SetProperty(ref _categoryFilter, value);
    }

    private List<SimpleDataType> _sort;

    // Список вариантов сортировки
    public List<SimpleDataType> Sort
    {
        get => _sort;
        set => SetProperty(ref _sort, value);
    }

    // Текущий SQL-запрос (с возможными фильтрами и сортировкой)
    public string Query
    {
        get => _query;
        set => SetProperty(ref _query, value);
    }

    // Метод для первоначальной загрузки всех товаров
    private void FillProducts()
    {
        Products = SelectTabelProduct.SelectTable(Query, ConnectToDB.ConnectToDBString());
    }

    // Метод для заполнения комбобоксов фильтрации и сортировки
    private void FillComboBox()
    {
        // Загрузка категорий для фильтрации
        CategoryFilter = SelectTabelSimpleDataType.SelectTableComboBox(
            "SELECT ID_CategoryProduct AS 'ID', CategoryProduct AS 'Name' FROM categoryproduct;",
            ConnectToDB.ConnectToDBString());
        FilterSelected = CategoryFilter[0]; // Установка значения по умолчанию
        
        // Создание списка вариантов сортировки
        Sort = new List<SimpleDataType>();
        Sort.Add(new SimpleDataType(0, "---"));
        Sort.Add(new SimpleDataType(1, "По возростанию"));
        Sort.Add(new SimpleDataType(2, "По убыванию"));
        SortSelected = Sort[0]; // Установка значения по умолчанию
    }

    // Конструктор ViewModel
    public ProductsPageViewModel(Window window)
    {
        _window = window;
        FillProducts(); // Загрузка товаров
        FillComboBox(); // Загрузка фильтров и сортировок
        
        // Загрузка категорий для редактирования
        ListCategory = SelectTabelSimpleDataType.SelectTableComboBox(
            "SELECT ID_CategoryProduct as 'ID', CategoryProduct AS 'Name' FROM categoryproduct;",
            ConnectToDB.ConnectToDBString());
        CategorySelected = ListCategory[0]; // Установка значения по умолчанию
    }

    // Метод для выборки данных с учетом фильтров, поиска и сортировки
    private void DataTableSelect()
    {
        string query = QueryBase;

        // Добавление фильтра по названию товара
        if (Search != "")
        {
            query += $" WHERE product.ProductName LIKE '%{Search}%'";
        }
        
        // Добавление фильтра по категории
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
        
        // Добавление сортировки по цене
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
        Products = SelectTabelProduct.SelectTable(query, ConnectToDB.ConnectToDBString());
    }

    // Команда для добавления/изменения изображения товара
    [RelayCommand]
    public async Task AddImage()
    {
        try
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выбор изображения";
            openFileDialog.Filters.Add(new FileDialogFilter()
            {
                Name = "Images",
                Extensions = { "jpg", "jpeg", "png" }
            });

            var result = await openFileDialog.ShowAsync(new Window());

            if (result == null || result.Length == 0)
                return;

            string path = result[0];
            long maxFileSize = 15 * 1024 * 1024; // Максимальный размер файла 15 МБ
            FileInfo fileInfo = new FileInfo(path);
            
            // Проверка размера файла
            if (fileInfo.Length > maxFileSize)
            {
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", "Изображение слишком большого размера, выберите изображение меньшего размера (менее 15 МБ)", "")
                };
                
                await err.ShowDialog(_window);
                return;
            }

            // Загрузка изображения
            if (File.Exists(path))
            {
                Image = new Bitmap(path);
                _imageBlob = File.ReadAllBytes(path); // Сохранение изображения в байтовом массиве
                BorderVisible = false;
                ImageVisible = true;
            }
        }
        catch (Exception ex)
        {
            // Обработка ошибки при загрузке изображения
            ErrorDialogWindow err = new ErrorDialogWindow()
            {
                DataContext = new OkDialogViewModel("Ошибка", "Изображение не удалось добавить", "")
            };
            await err.ShowDialog(_window);
        }
    }

    // Команда для обновления данных товара
    [RelayCommand]
    private async void UpdateProduct()
    {
        // Диалог подтверждения обновления
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel = new YesOrNotDialogViewModel("Внимание", "Вы хотите изменить товар?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        
        // Если пользователь подтвердил обновление
        if (warningViewModel.Flag)
        {
            // Проверка уникальности названия товара
            foreach (Product product in Products)
            {
                if (product.Name == Name && Name != SelectedProduct.Name)
                {
                    ErrorDialogWindow err = new ErrorDialogWindow()
                    {
                        DataContext = new OkDialogViewModel("Ошибка", $"Товар с таким именем уже существует", "")
                    };
                    await err.ShowDialog(_window);
                    return;
                }
            }
            
            try
            {
                byte[] imageBytes = null;
                
                // Обработка обновления без нового изображения
                if ((_imageBlob == new byte[] { } || _imageBlob == null))
                {
                    // Обновление только данных товара без изображения
                    SelectTabelProduct.UpdateData(SelectedProduct.Id,
                        Name,
                        CategorySelected.Id,
                        Convert.ToInt32(CountInStoke),
                        Convert.ToInt32(Discount),
                        Convert.ToDouble(Price));
                }
                else
                {
                    // Обновление данных товара с новым изображением
                    SelectTabelProduct.UpdateData(SelectedProduct.Id,
                        Name,
                        CategorySelected.Id,
                        Convert.ToInt32(CountInStoke),
                        Convert.ToInt32(Discount),
                        Convert.ToDouble(Price),
                        _imageBlob);
                }

                // Уведомление об успешном обновлении
                InfoDialogWindow infoDialog = new InfoDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Успех", "Запись обновлена!", "")
                };

                await infoDialog.ShowDialog(_window);
            }
            catch (Exception e)
            {
                // Обработка ошибки при обновлении
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", $"Запись не удалось обновить", "")
                };
                await err.ShowDialog(_window);
            }

            DataTableSelect(); // Обновление списка товаров
        }
    }
}