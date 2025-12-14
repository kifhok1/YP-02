using System;
using System.Collections.ObjectModel;

namespace VKR.Models;

// Класс, представляющий существующий заказ в системе
// Использует первичный конструктор для инициализации свойств
public class Order(int id,
    int clientId,
    string client,
    DateTime date,
    int statusId,
    string status,
    DateTime deliveryDate,
    string code,
    double costOrder,
    int totalDiscount)
{
    // Приватные поля для хранения данных заказа
    private int _id = id;
    private int _clientId = clientId;
    private string _clientView = client;
    private DateTime _date = date;
    private DateTime _deliveryDate = deliveryDate;
    private string _code = code;
    private int _statusId = statusId;
    private string _status = status;
    private int _totalDiscount = totalDiscount;
    private double _costOrder = costOrder;
    
    // Коллекция товаров заказа, загружаемая из базы данных по ID заказа
    private ObservableCollection<OrderProducts> _orderProducts = SelectTabelOrderProducts.SelectProductOrder(id);

    // Идентификатор заказа
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    // Идентификатор клиента, связанного с заказом
    public int ClientId
    {
        get { return _clientId; }
        set { _clientId = value; }
    }

    // Отображаемое имя клиента (например, ФИО)
    public string ClientView
    {
        get { return _clientView; }
        set { _clientView = value; }
    }

    // Дата создания заказа в формате строки
    public string Date
    {
        get { return _date.ToString("dd/MM/yyyy"); }
        set { _date = Convert.ToDateTime(value); }
    }

    // Дата доставки заказа в формате строки
    public string DeliveryDate
    {
        get { return _deliveryDate.ToString("dd/MM/yyyy"); }
        set { _deliveryDate = Convert.ToDateTime(value); }
    }

    // Текстовое описание статуса заказа
    public string Status
    {
        get { return _status; }
        set { _status = value; }
    }

    // Идентификатор статуса заказа
    public int StatusId
    {
        get { return _statusId; }
        set { _statusId = value; }
    }

    // Уникальный код заказа
    public string Code
    {
        get { return _code; }
        set { _code = value; }
    }

    // Стоимость заказа
    public double CostOrder
    {
        get { return _costOrder; }
        set { _costOrder = value; }
    }

    // Общая скидка на заказ в процентах
    public int TotalDiscount
    {
        get => _totalDiscount;
        set => _totalDiscount = value;
    }

    // Коллекция товаров, входящих в заказ
    public ObservableCollection<OrderProducts> OrderProducts
    {
        get { return _orderProducts; }
        set { _orderProducts = value; }
    }
}