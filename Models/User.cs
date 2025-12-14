namespace VKR.Models;

// Класс, представляющий пользователя системы
// Использует первичный конструктор для инициализации свойств
public class User(int id, string fio, string login, string hashPassword, int rule, byte[] image)
{
    // Приватные поля для хранения данных пользователя
    private int _id = id;
    private string _login = login;
    private string _fio = fio;
    private string _hashPassword = hashPassword;
    private int _rule = rule;
    private byte[] _image = image;

    // Идентификатор пользователя (только для чтения)
    public int Id
    {
        get { return _id; }
    }

    // ФИО пользователя (только для чтения)
    public string Fio
    {
        get { return _fio; }
    }

    // Логин для входа в систему (только для чтения)
    public string Login
    {
        get { return _login; }
    }

    // Хешированный пароль пользователя (только для чтения)
    public string HashPassword
    {
        get { return _hashPassword; }
    }

    // Идентификатор роли/права доступа пользователя (только для чтения)
    public int Rule
    {
        get { return _rule; }
    }

    // Изображение профиля пользователя в виде байтового массива (только для чтения)
    public byte[] Image
    {
        get { return _image; }
    }
}