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

// ViewModel для страницы добавления нового товара (консультант)
public partial class ProductAddPageViewModel : ViewModelBase
{
    // Приватные поля для хранения данных о товаре
    private string _name;
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
            // Очистка ввода и преобразование в число с использованием валидации
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

    // Доступность кнопки добавления товара
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

    // Конструктор ViewModel
    public ProductAddPageViewModel(Window window)
    {
        _window = window;
        
        // Настройка видимости элементов для изображения
        if (Image is null)
        {
            BorderVisible = true;
            ImageVisible = false;
        }

        // Загрузка списка категорий из базы данных
        ListCategory = SelectTabelSimpleDataType.SelectTableComboBox(
            "SELECT ID_CategoryProduct as 'ID', CategoryProduct AS 'Name' FROM categoryproduct;",
            ConnectToDB.ConnectToDBString());
        CategorySelected = ListCategory[0]; // Установка значения по умолчанию
    }

    // Команда для добавления изображения товара
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
                DataContext = new OkDialogViewModel("Ошибка", "Изображение не удалось открыть", "")
            };
            await err.ShowDialog(_window);
        }
    }

    // Метод для проверки доступности кнопки добавления товара
    private void EnabledInsertButton()
    {
        bool isNameValid = !string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name.Trim());
        bool isCountValid = !string.IsNullOrEmpty(CountInStoke?.Trim());
        bool isDiscountValid = !string.IsNullOrEmpty(Discount?.Trim());
        bool isImageValid = Image != null;
        bool isCategoryValid = !CategorySelected.Equals(ListCategory[0]);
        bool isPriceValid = !string.IsNullOrEmpty(Price?.Trim()) && 
                            Price != "0";
        // Все поля должны быть заполнены корректно
        ButtonIsertEnable = isNameValid && 
                            isCountValid && 
                            isDiscountValid && 
                            isImageValid && 
                            isCategoryValid &&
                            isPriceValid; 
    }

    // Команда для добавления нового товара
    [RelayCommand]
    public async void InsertProduct()
    {
        // Диалог подтверждения добавления товара
        WarningYesOrNotDialogWindow warning = new WarningYesOrNotDialogWindow();
        YesOrNotDialogViewModel warningViewModel =
            new YesOrNotDialogViewModel("Внимание", "Вы хотите добавить товар?", warning);
        warning.DataContext = warningViewModel;
        await warning.ShowDialog(_window);
        
        // Если пользователь подтвердил добавление
        if (warningViewModel.Flag)
        {
            try
            {
                // Загрузка существующих товаров для проверки уникальности названия
                ObservableCollection<Product> table = SelectTabelProduct.SelectTableNoneImage(
                    "SELECT ID_Product AS 'ID', " +
                    "ProductName AS 'Name', " +
                    "CategoryProduct AS 'Category', " +
                    "ProductCategory AS 'CategoryId', " +
                    "ProductCountInStock AS 'CountInStock', " +
                    "ProductDiscount AS 'Discount', " +
                    "ProductCost AS 'Cost' " +
                    "FROM product " +
                    "JOIN vkr.categoryproduct c on c.ID_CategoryProduct = product.ProductCategory;",
                    ConnectToDB.ConnectToDBString());
                
                // Проверка уникальности названия товара
                foreach (var VARIABLE in table)
                {
                    if (VARIABLE.Name == Name)
                    {
                        ErrorDialogWindow err = new ErrorDialogWindow()
                        {
                            DataContext = new OkDialogViewModel("Ошибка", "Запись не удалось добавить", "")
                        };
                        await err.ShowDialog(_window);
                        return;
                    }
                }

                // Добавление нового товара в базу данных
                SelectTabelProduct.InsertData(Name,
                    CategorySelected.Id,
                    Convert.ToInt32(CountInStoke),
                    Convert.ToInt32(Discount),
                    Convert.ToDouble(Price),
                    _imageBlob);
                
                // Сброс полей формы после успешного добавления
                Name = "";
                Price = "0";
                CategorySelected = ListCategory[0];
                ButtonIsertEnable = false;
                ImageVisible = false;
                BorderVisible = true;
                
                // Уведомление об успешном добавлении
                InfoDialogWindow info = new InfoDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Успех", "Добавлена запись!", "")
                };
                await info.ShowDialog(_window);
            }
            catch (Exception e)
            {
                // Обработка ошибки при добавлении товара
                ErrorDialogWindow err = new ErrorDialogWindow()
                {
                    DataContext = new OkDialogViewModel("Ошибка", e.Message.ToString(), "")
                };
                await err.ShowDialog(_window);
            }
        }
    }
}