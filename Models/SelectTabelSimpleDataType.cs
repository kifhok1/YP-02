using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VKR.Models;

// Класс для работы с простыми типами данных (например, справочники категорий, статусов)
// Реализует интерфейс ISelectTables<SimpleDataType>
public class SelectTabelSimpleDataType : ISelectTables<SimpleDataType>
{
    // Реализация метода из интерфейса ISelectTables
    // Выполняет SQL-запрос и возвращает коллекцию простых типов данных
    public static ObservableCollection<SimpleDataType> SelectTable(string query, string connectionString)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            ObservableCollection<SimpleDataType> simpleData = new ObservableCollection<SimpleDataType>();
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Чтение данных и создание объектов SimpleDataType
            while (reader.Read())
            {
                simpleData.Add(new SimpleDataType(
                    reader.GetInt32("ID"),
                    reader.GetString("Name")));
            }
            connection.Close();
            return simpleData;
        }
    }

    // Специальный метод для заполнения ComboBox элементов
    // Добавляет элемент "---" с ID=0 в начало списка
    public static List<SimpleDataType> SelectTableComboBox(string query, string connectionString)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            List<SimpleDataType> simpleData = new List<SimpleDataType>();
            
            // Добавляем пустой элемент для выбора по умолчанию в ComboBox
            simpleData.Add(new SimpleDataType(0, "---"));
            
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Чтение данных из базы и добавление в список
            while (reader.Read())
            {
                simpleData.Add(new SimpleDataType(
                    reader.GetInt32("ID"),
                    reader.GetString("Name")));
            }
            connection.Close();
            return simpleData;
        }
    }

    // Метод для вставки данных в таблицы простых типов
    public static void InsertData(string query)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            command.ExecuteNonQuery(); // Выполнение запроса на вставку
            connection.Close();
        }
    }

    // Метод для обновления данных в таблицах простых типов
    public static void UpdateData(string query)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            command.ExecuteNonQuery(); // Выполнение запроса на обновление
            connection.Close();
        }
    }
}