using Avalonia.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace VKR.Models;

// Класс для работы с данными сотрудников, реализует интерфейс ISelectTables<Worker>
public class SelectTabelWorkers : ISelectTables<Worker>
{
    // Реализация метода из интерфейса ISelectTables
    // Выполняет SQL-запрос и возвращает коллекцию сотрудников с изображениями
    public static ObservableCollection<Worker> SelectTable(string query, string connectionString)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            ObservableCollection<Worker> workers = new ObservableCollection<Worker>();
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            Bitmap imageUser;
            byte[] imageData = null;
            
            // Чтение данных о сотрудниках, включая изображения профиля
            while (reader.Read())
            {
                // Проверяем наличие изображения в базе данных
                if (!reader.IsDBNull("Image"))
                {
                    imageData = (byte[])reader["Image"];
                }

                // Преобразуем байты в изображение Avalonia Bitmap
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    imageUser = new Bitmap(ms);
                }
                
                // Создаем объект Worker с изображением профиля
                workers.Add(new Worker(
                    reader.GetInt32("ID"),
                    reader.GetString("Login"),
                    reader.GetString("Password"),
                    reader.GetInt32("RuleID"),
                    reader.GetString("RuleView"),
                    reader.GetString("FIO"),
                    reader.GetString("PhoneNumber"),
                    imageUser));
            }
            connection.Close();
            return workers;
        }
    }

    // Метод для выборки сотрудников без изображений профиля
    public static ObservableCollection<Worker> SelectNoneImage(string query, string connectionString)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            ObservableCollection<Worker> workers = new ObservableCollection<Worker>();
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Создание объектов Worker без изображений
            while (reader.Read())
            {
                workers.Add(new Worker(
                    reader.GetInt32("ID"),
                    reader.GetString("Login"),
                    reader.GetString("Password"),
                    reader.GetInt32("RuleID"),
                    reader.GetString("RuleView"),
                    reader.GetString("FIO"),
                    reader.GetString("PhoneNumber")));
            }
            connection.Close();
            return workers;
        }
    }

    // Метод для добавления нового сотрудника с изображением профиля
    public static void InsertData(string fio,
        string login,
        string password,
        int ruleID,
        string phoneNumber,
        byte[] userImage)
    {
        // Хеширование пароля перед сохранением в базу данных
        string hashPassword = SHA256Hasher.ComputeSHA256Hash(password);
        
        // SQL-запрос для вставки нового сотрудника
        string query = $"INSERT INTO `vkr`.`workers` (`Fio`, `PhoneNumber`, `Login`, `Password`, `rule`, `Image`) " +
                       $"VALUES ('{fio}', '{phoneNumber}', '{login}', '{hashPassword}', '{ruleID}', @UserImage)";
        
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            
            // Создание параметра для BLOB-данных (изображения профиля)
            MySqlParameter blob = new MySqlParameter("@UserImage", MySqlDbType.Blob, userImage.Length);
            blob.Value = userImage;
            command.Parameters.Add(blob);
            
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    // Перегрузки метода UpdateData для различных сценариев обновления данных сотрудника

    // Обновление данных сотрудника с изображением (без изменения пароля)
    public static void UpdateData(int id, string fio, string login, int ruleID, string phoneNumber, byte[] userImage)
    {
        string query =
            $"UPDATE `vkr`.`workers` SET `Fio` = '{fio}'," +
            $" `PhoneNumber` = '{phoneNumber}'," +
            $" `Login` = '{login}'," +
            $" `rule` = '{ruleID}'," +
            $" `Image` = @UserImage" +
            $" WHERE (`ID_Worker` = '{id}');";
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            MySqlParameter blob = new MySqlParameter("@UserImage", MySqlDbType.Blob, userImage.Length);
            blob.Value = userImage;
            command.Parameters.Add(blob);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    
    // Обновление всех данных сотрудника, включая пароль и изображение
    public static void UpdateData(int id, string fio, string login, string password, int ruleID, string phoneNumber, byte[] userImage)
    {
        string query =
            $"UPDATE `vkr`.`workers` SET `Fio` = '{fio}'," +
            $" `PhoneNumber` = '{phoneNumber}'," +
            $" `Login` = '{login}'," +
            $" `Password` = '{password}'," +
            $" `rule` = '{ruleID}'," +
            $" `Image` = @UserImage" +
            $" WHERE (`ID_Worker` = '{id}');";
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            MySqlParameter blob = new MySqlParameter("@UserImage", MySqlDbType.Blob, userImage.Length);
            blob.Value = userImage;
            command.Parameters.Add(blob);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    
    // Обновление данных сотрудника с изменением пароля, но без изображения
    public static void UpdateData(int id, string fio, string login, string password, int ruleID, string phoneNumber)
    {
        string query =
            $"UPDATE `vkr`.`workers` SET `Fio` = '{fio}'," +
            $" `PhoneNumber` = '{phoneNumber}'," +
            $" `Login` = '{login}'," +
            $" `Password` = '{password}'," +
            $" `rule` = '{ruleID}'" +
            $" WHERE (`ID_Worker` = '{id}');";
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    
    // Обновление данных сотрудника без изменения пароля и изображения
    public static void UpdateData(int id, string fio, string login, int ruleID, string phoneNumber)
    {
        string query =
            $"UPDATE `vkr`.`workers` SET `Fio` = '{fio}'," +
            $" `PhoneNumber` = '{phoneNumber}'," +
            $" `Login` = '{login}'," +
            $" `rule` = '{ruleID}'" +
            $" WHERE (`ID_Worker` = '{id}');";
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    // Метод для удаления сотрудника по ID
    public static void DeleteData(int id)
    {
        string query = $"DELETE FROM `vkr`.`workers` WHERE (`ID_Worker` = '{id}');";
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}