namespace VKR.Models;

// Класс для представления простых типов данных (справочников)
// Использует первичный конструктор для инициализации свойств
public class SimpleDataType(int id, string name)
{
    // Приватные поля для хранения данных
    private int _id = id;
    private string _name = name;

    // Идентификатор элемента справочника
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    // Название элемента справочника
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
}