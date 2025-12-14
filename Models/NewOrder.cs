using System;
using System.Collections.ObjectModel;

namespace VKR.Models;

// Класс, представляющий новый заказ в системе
public class NewOrder
{
    // Генератор случайных чисел для создания кода заказа
    private Random random = new Random();
    private int _clientId; 
    private int _totalDiscount; 
    private Client _clientOrder;
    private double _totalPrice; 
    private DateTime _date; 
    private DateTime _deliveryDate; 
    private string _code;
    private ObservableCollection<Product> _products; 

    // Идентификатор клиента, сделавшего заказ
    public int ClientId
    {
        get { return _clientId; }
        set { _clientId = value; }
    }

    // Скидка постоянного клиента в процентах
    public int TotalDiscount
    {
        get { return _totalDiscount; }
        set { _totalDiscount = value; }
    }

    // Общая стоимость заказа без учета скидки
    public double TotalPrice
    {
        get { return _totalPrice; }
        set { _totalPrice = value; }
    }

    // Дата создания заказа
    public DateTime Date
    {
        get { return _date; }
        set { _date = value; }
    }

    // Дата доставки/выдачи заказа
    public DateTime DeliveryDate
    {
        get { return _deliveryDate; }
        set { _deliveryDate = value; }
    }

    // Коллекция товаров в заказе
    public ObservableCollection<Product> Products
    {
        get { return _products; }
        set { _products = value; }
    }

    // Объект клиента, связанный с заказом
    public Client ClientOrder
    {
        get { return _clientOrder; }
        set { _clientOrder = value; }
    }

    // Уникальный код заказа
    public string Code
    {
        get {  return _code; }
        set { _code = value; }
    }

    // Форматированная строка с кодом выдачи
    public string StringCode
    {
        get { return $"Код выдачи: {Code}"; }
    }
    
    // Форматированная строка с датой создания заказа
    public string StringCreateDate
    {
        get { return $"Дата создания заказа: {Date.Year}.{Date.Month}.{Date.Day}"; }
    }

    // Форматированная строка с датой выдачи заказа
    public string StringDeliveryDate
    {
        get { return $"Дата выдачи заказа: {DeliveryDate.Year}.{DeliveryDate.Month}.{DeliveryDate.Day}"; }
    }

    // Форматированная строка с информацией о клиенте
    public string StringClient
    {
        get { return $"Покупатель: {ClientOrder.Fio} {ClientOrder.PhoneNumber}"; }
    }

    // Форматированная строка с общей суммой заказа
    public string StringTotalPrice
    {
        get { return $"Общая сумма заказа: {TotalPrice:C}"; }
    }

    // Форматированная строка с информацией о скидке
    public string StringTotalDiscount
    {
        get { return $"Скидка постоянного клиента: {TotalDiscount}%"; }
    }

    // Форматированная строка с итоговой суммой с учетом скидки
    public string StringTotalPriceWithDiscount
    {
        get { return $"Общая сумма заказа с учётом скидки: {(TotalPrice - (TotalDiscount * TotalPrice / 100)):C}"; }
    }

    // Конструктор класса NewOrder
    public NewOrder(int clientId, DateTime date, DateTime deliveryDate, ObservableCollection<Product> products)
    {
        _clientId = clientId;
        _date = date;
        _deliveryDate = deliveryDate;
        _products = products;
        // Генерация случайного трехзначного кода заказа
        _code = $"{random.Next(0, 10)}{random.Next(0, 10)}{random.Next(0, 10)}";
        
        // Расчет общей стоимости заказа на основе товаров
        foreach (Product product in products)
        {
            // Суммируем стоимость каждого товара с учетом его скидки и количества
            _totalPrice += product.Trash * (product.Cost - (product.Cost * product.Discount / 100));
        }
        
        // Получение информации о клиенте по его ID
        _clientOrder = SelectTabelClients.GetClientById(clientId);
        
        // Определение размера скидки в зависимости от суммы накоплений клиента
        if (_clientOrder.AmountOfNumber > 250000)
        {
            _totalDiscount = 10; // 10% скидка при накоплениях свыше 250,000
        } 
        else if (_clientOrder.AmountOfNumber > 150000)
        {
            _totalDiscount = 5;  // 5% скидка при накоплениях свыше 150,000
        }
        else
        {
            _totalDiscount = 0;  // Без скидки
        }
    }
}