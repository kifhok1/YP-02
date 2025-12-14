using System;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;

namespace VKR.Models;

// Класс для работы с данными клиентов, реализует интерфейс ISelectTables<Client>
public class SelectTabelClients : ISelectTables<Client>
{
    // Реализация статического метода из интерфейса ISelectTables
    // Выполняет SQL-запрос и возвращает коллекцию клиентов
    public static ObservableCollection<Client> SelectTable(string query, string connectionString)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            ObservableCollection<Client> clients = new ObservableCollection<Client>();
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Чтение данных построчно и создание объектов Client
            while (reader.Read())
            {
                clients.Add(new Client(
                    reader.GetInt32("ID"),
                    reader.GetString("FIO"),
                    reader.GetString("PhoneNumber"),
                    reader.GetDouble("AmountOfNumber")));
            }
            connection.Close();
            return clients;
        }
    }

    // Метод для выполнения SQL-запросов на вставку данных
    public static void InsertData(string query)
    {
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery(); // Выполнение запроса без возврата данных
            connection.Close();
        }
    }

    // Метод для обновления данных клиента по его ID
    public static void UpdateData(int id, string fio, string phoneNumber)
    {
        // Формирование SQL-запроса на обновление данных клиента
        string query = $"UPDATE clients SET Fio = '{fio}', PhoneNumber = '{phoneNumber}' WHERE ID_Clients = {id};";
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    // Метод для получения клиента по его ID
    public static Client GetClientById(int id)
    {
        string query = $"SELECT * FROM clients WHERE ID_Clients = {id};";
        Client client = null;
        
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // Если найдена запись, создаем объект Client
                    if (reader.Read())
                    {
                        client = new Client(
                                Convert.ToInt32(reader["ID_Clients"]),
                                reader["Fio"].ToString(),
                                reader["PhoneNumber"].ToString(), 
                                Convert.ToDouble(reader["AmountOfNumber"]));
                    }
                }
            }
        }
        return client; // Возвращаем найденного клиента или null, если не найден
    }
}