using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;

namespace VKR.Models;

// Статический класс для выборки информации о товарах в конкретном заказе
public static class SelectTabelOrderProducts
{
    // Метод для получения списка товаров, входящих в указанный заказ
    public static ObservableCollection<OrderProducts> SelectProductOrder(int orderID)
    {
        // SQL-запрос для выборки товаров заказа с русскоязычными алиасами
        string query = $"SELECT product.ProductName AS 'Товар'," +
                       $"Count AS 'Количество', " +
                       $"CostProduct AS 'Цена товара на момент продажи' " +
                       $"FROM vkr.orderproduct " +
                       $"INNER JOIN product ON product.ID_Product = orderproduct.ID_Product " +
                       $"WHERE ID_Order = {orderID};";
        
        // Получаем строку подключения к базе данных
        string connectionString = ConnectToDB.ConnectToDBString();
        
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            ObservableCollection<OrderProducts> orderProducts = new ObservableCollection<OrderProducts>();
            connection.Open();
            
            // Выполняем запрос и читаем результаты
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Преобразуем каждую строку результата в объект OrderProducts
            while (reader.Read())
            {
                orderProducts.Add( new OrderProducts(
                    product: reader.GetString("Товар"),
                    count: reader.GetInt32("Количество"),
                    costProduct: reader.GetDouble("Цена товара на момент продажи")));
            }
            
            connection.Close();
            return orderProducts; // Возвращаем коллекцию товаров заказа
        }
    }
}