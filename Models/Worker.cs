using Avalonia.Media.Imaging;

namespace VKR.Models;

// Класс, представляющий сотрудника/работника в системе
public class Worker
{
    // Приватные поля для хранения данных о сотруднике
    private int _id;
    private string _login;
    private string _hashPassword;
    private int _ruleId;
    private string _ruleView;
    private string _fio;
    private string _phoneNumber;
    private Bitmap _image;

    // Идентификатор сотрудника
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    // Логин для входа в систему
    public string Login
    {
        get { return _login; }
        set { _login = value; }
    }

    // Хешированный пароль сотрудника
    public string HashPassword
    {
        get { return _hashPassword; }
        set { _hashPassword = value; }
    }

    // Идентификатор роли/уровня доступа сотрудника
    public int RuleId
    {
        get { return _ruleId; }
        set { _ruleId = value; }
    }

    // Текстовое представление роли сотрудника
    public string RuleView
    {
        get { return _ruleView; }
        set { _ruleView = value; }
    }

    // ФИО сотрудника
    public string Fio
    {
        get { return _fio; }
        set { _fio = value; }
    }

    // Номер телефона сотрудника
    public string PhoneNumber
    {
        get { return _phoneNumber; }
        set { _phoneNumber = value; }
    }

    // Изображение профиля сотрудника (Avalonia Bitmap)
    public Bitmap Image
    {
        get { return _image; }
        set { _image = value; }
    }

    // Конструктор без изображения профиля
    public Worker(int id, string login, string hashPassword, int ruleId, string ruleView, string fio, string phoneNumber)
    {
        _id = id;
        _login = login;
        _hashPassword = hashPassword;
        _ruleId = ruleId;
        _ruleView = ruleView;
        _fio = fio;
        _phoneNumber = phoneNumber;
    }
    
    // Конструктор с изображением профиля
    public Worker(int id, string login, string hashPassword, int ruleId, string ruleView, string fio, string phoneNumber, Bitmap image)
    {
        _id = id;
        _login = login;
        _hashPassword = hashPassword;
        _ruleId = ruleId;
        _ruleView = ruleView;
        _fio = fio;
        _phoneNumber = phoneNumber;
        _image = image;
    }
}