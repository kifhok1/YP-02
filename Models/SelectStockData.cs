using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace VKR.Models;

// Статический класс для выборки данных о запасах товаров на складе
public static class SelectStockData
{
    // Метод для получения актуальных данных о товарах на складе
    public static List<StockData> GetStockData()
    {
        // Список для хранения данных о запасах товаров
        List<StockData> stockData = new List<StockData>();
        
        // Получаем строку подключения к базе данных
        string connectionString = ConnectToDB.ConnectToDBString();
        
        // SQL-запрос для выборки данных о товарах на складе
        // Включает расчет цены с учетом скидки и объединение с категориями
        string query = @"SELECT p.ProductName AS 'Name',
                               c.CategoryProduct AS 'Category',
                               ROUND(p.ProductCost * (100 - COALESCE(p.ProductDiscount, 0))/ 100, 2) AS 'Price',
                               p.ProductCountInStock AS 'Quantity' 
                        FROM product p
                        INNER JOIN categoryproduct c ON p.ProductCategory = c.ID_CategoryProduct;";
        
        // Устанавливаем соединение с базой данных и выполняем запрос
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Читаем данные построчно и преобразуем в объекты StockData
            while (reader.Read())
            {
                stockData.Add(new StockData
                {
                    ProductName = reader.GetString("Name"),
                    Category = reader.GetString("Category"),
                    UnitPrice = reader.GetDecimal("Price"),
                    QuantityInStock = reader.GetInt32("Quantity")
                });
            }
            connection.Close();
        }
        
        // Возвращаем список данных о запасах товаров
        return stockData;
    }
}