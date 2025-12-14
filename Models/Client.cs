namespace VKR.Models;

// Класс, представляющий клиента в системе
// Использует первичный конструктор для инициализации свойств
public class Client(int id, string fio, string phoneNumber, double amountOfNumber)
{
    // Поля класса с приватным доступом для инкапсуляции данных
    private int _id = id;
    private string _fio = fio;
    private string _phoneNumber = phoneNumber;
    private double _amountOfNumber = amountOfNumber;

    // Свойство для идентификатора клиента
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    // Свойство для ФИО клиента
    public string Fio
    {
        get { return _fio; }
        set { _fio = value; }
    }

    // Свойство для номера телефона клиента
    public string PhoneNumber
    {
        get { return _phoneNumber; }
        set { _phoneNumber = value; }
    }

    // Свойство для суммы накоплений клиента (используется для расчета скидки)
    public double AmountOfNumber
    {
        get { return _amountOfNumber; }
        set { _amountOfNumber = value; }
    }
}