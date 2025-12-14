namespace VKR.Models;

// Класс, представляющий товар в заказе
// Использует первичный конструктор для инициализации свойств
public class OrderProducts(string product, int count, double costProduct)
{
    // Приватные поля для хранения информации о товаре в заказе
    private string _product =  product;
    private int _count = count;
    private double _costProduct = costProduct;

    // Наименование товара
    public string Product
    {
        get => _product;
        set => _product = value;
    }

    // Количество товара в заказе
    public int Count
    {
        get => _count;
        set => _count = value;
    }

    // Стоимость товара (может быть общей стоимостью или ценой за единицу)
    public double CostProduct
    {
        get => _costProduct;
        set => _costProduct = value;
    }
}