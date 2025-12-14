using MySql.Data.MySqlClient;
using System.Data;

namespace VKR.Models;

// Класс для авторизации пользователей, реализует интерфейс IUserAuthorisations
public class UserAuthorisations(string connString) : IUserAuthorisations
{
    // Строка подключения к базе данных, передается через конструктор
    private readonly string _connString = connString;

    // Метод для получения пользователя по логину
    public User? GetUserByLogin(string login)
    {
        using MySqlConnection connection = new MySqlConnection(_connString);

        // SQL-запрос для поиска пользователя по логину
        const string query = @"
        SELECT ID_Worker, Fio, Login, Password, rule, Image 
        FROM workers 
        WHERE Login = @Login;";

        // Создание команды с параметризованным запросом для защиты от SQL-инъекций
        using MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Login", login);

        connection.Open();

        // Выполнение запроса и чтение результата
        using MySqlDataReader reader = command.ExecuteReader();
        
        // Если пользователь не найден, возвращаем null
        if (!reader.Read())
        {
            return null;
        }

        // Чтение данных пользователя из результата запроса
        int idWorker = reader.GetInt32("ID_Worker");
        string fio = reader.IsDBNull("Fio") ? string.Empty : reader.GetString("Fio");
        string password = reader.IsDBNull("Password") ? string.Empty : reader.GetString("Password");
        int rule = reader.GetInt32("rule");

        // Чтение изображения профиля пользователя (может быть null)
        byte[] image = new byte[] { };
        if (!reader.IsDBNull("Image"))
        {
            // Определение размера изображения и чтение BLOB-данных
            long imageSize = reader.GetBytes("Image", 0, null, 0, 0);
            image = new byte[imageSize];
            reader.GetBytes("Image", 0, image, 0, (int)imageSize);
        }

        // Создание и возврат объекта пользователя
        return new User(
            id: idWorker,
            fio: fio,
            login: login,
            hashPassword: password,
            rule: rule,
            image: image
        );
    }
}