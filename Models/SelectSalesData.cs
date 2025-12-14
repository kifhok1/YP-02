using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace VKR.Models;

// Статический класс для выборки данных о продажах из базы данных
public static class SelectSalesData
{
    // Метод для получения данных о продажах за указанный год и месяц
    public static List<SalesData> GetSalesData(int year, int month)
    {
        // Список для хранения данных о продажах
        List<SalesData> salesData = new List<SalesData>();
        
        // Получаем строку подключения к базе данных
        string connectionString = ConnectToDB.ConnectToDBString();
        
        // SQL-запрос для выборки данных о продажах
        // Объединяет данные из таблиц заказов, товаров и категорий
        string query = @"SELECT
                            p.ProductName AS Name,
                            cp.CategoryProduct as Category,
                            op.Count AS Quantity,
                            op.CostProduct AS Cost,
                            o.DateOrder
                        FROM orderproduct op
                                 INNER JOIN product p ON op.ID_Product = p.ID_Product
                                 INNER JOIN categoryproduct cp ON p.ProductCategory = cp.ID_CategoryProduct
                                 INNER JOIN `order` o ON op.ID_Order = o.ID_Order
                        WHERE YEAR(o.DateOrder) = " + $"{year}" + "  AND MONTH(o.DateOrder) = " + $"{month}" + @" 
                        ORDER BY o.DateOrder;";
        
        // Устанавливаем соединение с базой данных и выполняем запрос
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Читаем данные построчно и преобразуем в объекты SalesData
            while (reader.Read())
            {
                salesData.Add(new SalesData
                {
                    ProductName = reader.GetString("Name"),
                    Category = reader.GetString("Category"),
                    TotalCost = reader.GetDecimal("Cost"),
                    Quantity = reader.GetInt32("Quantity")
                });
            }
            connection.Close();
        }
        
        // Возвращаем список данных о продажах
        return salesData;
    }
}