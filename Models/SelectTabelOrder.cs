using System;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;

namespace VKR.Models;

// Класс для работы с данными заказов (выборка, обновление, вставка)
public class SelectTabelOrder
{
    // Метод для выборки заказов из базы данных по заданному SQL-запросу
    public static ObservableCollection<Order> SelectTable(string connectionString, string query)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            ObservableCollection<Order> orders = new ObservableCollection<Order>();
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Чтение данных о заказах построчно и создание объектов Order
            while (reader.Read())
            {
                orders.Add(new Order(
                    id: reader.GetInt32("ID"),
                    clientId: reader.GetInt32("ClientID"),
                    client: reader.GetString("ClientName"),
                    date: reader.GetDateTime("DateOrder"),
                    statusId: reader.GetInt32("StatusID"),
                    status: reader.GetString("StatusName"),
                    deliveryDate: reader.GetDateTime("DeliveryDate"),
                    code: reader.GetString("Code"),
                    costOrder: reader.GetDouble("CostOrder"),
                    totalDiscount: reader.GetInt32("TotalDiscount")));
            }
            connection.Close();
            return orders;
        }
    }
    
    // Метод для обновления данных заказа (статуса и даты доставки)
    public static void UpdateTable(string? connectionString, DateTime date, int statusId, int orderId)
    {
        // Формирование SQL-запроса для обновления статуса и даты доставки заказа
        string query = $"UPDATE `vkr`.`order` SET `Status` = '{statusId}', `DeliveryDate` = '{date.Year}-{date.Month}-{date.Day}' WHERE (`ID_Order` = '{orderId}');";
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            command.ExecuteNonQuery(); // Выполнение запроса обновления
            connection.Close();
        }
    }

    // Метод для создания нового заказа (с использованием транзакции)
    public static void InsertTable(NewOrder newOrder)
    {
        // SQL-запрос для вставки основного заказа
        string queryOrder = $"INSERT INTO `vkr`.`order` (`ID_Client`, `CostOrder`, `Status`, `Code`, `TotalDiscount`, `DateOrder`, `DeliveryDate`) " +
                            $"VALUES ('{newOrder.ClientId}', '{newOrder.TotalPrice.ToString().Replace(',', '.')}', '{2}', '{newOrder.Code}', '{newOrder.TotalDiscount}', " +
                            $"'{newOrder.Date.Year}-{newOrder.Date.Month}-{newOrder.Date.Day}', '" +
                            $"{newOrder.DeliveryDate.Year}-{newOrder.DeliveryDate.Month}-{newOrder.DeliveryDate.Day}');";
        
        // SQL-запрос для обновления суммы накоплений клиента (с учетом скидки)
        string queryClient = $"UPDATE `clients` SET AmountOfNumber = AmountOfNumber + " +
                           $"{(newOrder.TotalPrice - (newOrder.TotalDiscount * newOrder.TotalPrice / 100)).ToString().Replace(',', '.')}" +
                           $" WHERE ID_Clients = {newOrder.ClientId};";
        
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            
            // Начинаем транзакцию для обеспечения атомарности операций
            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    // 1. Создание записи о заказе
                    MySqlCommand commandOrder = new MySqlCommand("", connection, transaction);
                    commandOrder.CommandText = queryOrder;
                    commandOrder.ExecuteNonQuery();
                    long lastId = commandOrder.LastInsertedId; // Получаем ID созданного заказа
                    
                    // 2. Добавление товаров в заказ
                    foreach (Product product in newOrder.Products)
                    {
                        // Запрос для добавления каждого товара в заказ
                        string queryProduct =
                            $"INSERT INTO `vkr`.`orderproduct` (`ID_Order`, `ID_Product`, `Count`, `CostProduct`) " +
                            $"VALUES ('{lastId}', '{product.Id}', '{product.Trash}', '{(product.Cost - (product.Cost * product.Discount / 100)).ToString().Replace(',', '.')}');";
                        MySqlCommand commandAddProduct = new MySqlCommand("", connection, transaction);
                        commandAddProduct.CommandText = queryProduct;
                        commandAddProduct.ExecuteNonQuery();
                    }
                    
                    // 3. Обновление суммы накоплений клиента
                    MySqlCommand commandClient = new MySqlCommand("", connection, transaction);
                    commandClient.CommandText = queryClient;
                    commandClient.ExecuteNonQuery();
                    
                    // 4. Обновление остатков товаров на складе
                    foreach (Product product in newOrder.Products)
                    {
                        string queryProduct =
                            $"UPDATE product SET ProductCountInStock = ProductCountInStock - {product.Trash} WHERE ID_Product = {product.Id};";
                        MySqlCommand commandAddProduct = new MySqlCommand("", connection, transaction);
                        commandAddProduct.CommandText = queryProduct;
                        commandAddProduct.ExecuteNonQuery();
                    }
                    
                    // Если все операции успешны - подтверждаем транзакцию
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // При ошибке - откатываем все изменения
                    transaction.Rollback();
                    throw new Exception($"Ошибка при создании заказа: {ex.Message}", ex);
                }
            }
            
            connection.Close();
        }
    }
}