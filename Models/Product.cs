using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VKR.Models;

// Класс продукта с поддержкой привязки данных (MVVM) и команд
public partial class Product : ObservableObject
{
    // Приватные поля с данными продукта
    private int _id;
    private string _name;
    private string _categoryView;
    private int _categoryId;
    private int _countInStock;
    private int _discout;
    private double _cost;
    private Bitmap _image;
    private int trash = 0; // Количество товара в корзине

    // Идентификатор продукта
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    // Название продукта
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    // Отображаемое название категории
    public string CategoryView
    {
        get { return _categoryView; }
        set { _categoryView = value; }
    }

    // Идентификатор категории
    public int CategoryId
    {
        get { return _categoryId; }
        set { _categoryId = value; }
    }

    // Количество товара на складе
    public int CountInStock
    {
        get { return _countInStock; }
        set { _countInStock = value; }
    }

    // Скидка на товар в процентах
    public int Discount
    {
        get { return _discout; }
        set { _discout = value; }
    }

    // Базовая стоимость товара
    public double Cost
    {
        get { return _cost; }
        set { _cost = value; }
    }

    // Количество товара в корзине (выбранное для покупки)
    public int Trash
    {
        get { return trash; }
        set { trash = value; }
    }

    // Цена товара с учетом скидки (вычисляемое свойство)
    public double PriceToDiscount
    {
        get { return  Cost - (Cost * Discount / 100); }
    }

    // Изображение товара
    public Bitmap Image
    {
        get { return _image; }
        set { _image = value; }
    }

    // Конструктор без изображения
    public Product(int id,
        string name,
        string category,
        int categoryId,
        int countInStock,
        int discount,
        double cost)
    {
        _id = id;
        _name = name;
        _categoryView = category;
        _categoryId = categoryId;
        _countInStock = countInStock; _discout = discount;
        _cost = cost;
    }

    // Конструктор с изображением
    public Product(int id,
        string name,
        string category,
        int categoryId,
        int countInStock,
        int discount,
        double cost,
        Bitmap image)
    {
        _id = id;
        _name = name;
        _categoryView = category;
        _categoryId = categoryId;
        _countInStock = countInStock; _discout = discount;
        _cost = cost;
        _image = image;
    }

    // Команда для добавления товара в корзину
    [RelayCommand]
    private void AddToTrash()
    {
        // Проверяем, что не превышаем количество на складе
        if (Trash < CountInStock)
        {
            Trash++;
            // Уведомляем интерфейс об изменениях для обновления привязок
            OnPropertyChanged(nameof(IsEnabledAdd));
            OnPropertyChanged(nameof(Trash));
            OnPropertyChanged(nameof(IsEnabledRemove));
        }
    }

    // Команда для удаления товара из корзины
    [RelayCommand]
    private void RemoveFromTrash()
    {
        // Проверяем, что в корзине есть хотя бы один товар
        if (Trash > 0)
        {
            Trash--;
            // Уведомляем интерфейс об изменениях для обновления привязок
            OnPropertyChanged(nameof(IsEnabledAdd));
            OnPropertyChanged(nameof(Trash));
            OnPropertyChanged(nameof(IsEnabledRemove));
        }
    }

    // Свойство, определяющее доступность кнопки "Добавить в корзину"
    public bool IsEnabledAdd
    {
        get => Trash < CountInStock;
    }

    // Свойство, определяющее доступность кнопки "Удалить из корзины"
    public bool IsEnabledRemove
    {
        get => Trash > 0;
    }
}