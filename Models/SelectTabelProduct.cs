using Avalonia.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace VKR.Models;

// Класс для работы с данными продуктов, реализует интерфейс ISelectTables<Product>
public class SelectTabelProduct : ISelectTables<Product>
{
    // Реализация метода из интерфейса ISelectTables
    // Выполняет SQL-запрос и возвращает коллекцию продуктов с изображениями
    public static ObservableCollection<Product> SelectTable(string query, string connectionString)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            ObservableCollection<Product> products = new ObservableCollection<Product>();
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            Bitmap imageProduct;
            byte[] imageData = null;
            
            // Чтение данных о продуктах, включая изображения из BLOB-поля
            while (reader.Read())
            {
                // Проверяем наличие изображения в базе данных
                if (!reader.IsDBNull("ProductImage"))
                {
                    imageData = (byte[])reader["ProductImage"];
                }

                // Преобразуем байты в изображение Avalonia Bitmap
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    imageProduct = new Bitmap(ms);
                }
                
                // Создаем объект Product с изображением
                products.Add(new Product(
                    reader.GetInt32("ID"),
                    reader.GetString("Name"),
                    reader.GetString("Category"),
                    reader.GetInt32("CategoryId"),
                    reader.GetInt32("CountInStock"),
                    reader.GetInt32("Discount"),
                    reader.GetDouble("Cost"),
                    imageProduct));
            }
            connection.Close();
            return products;
        }
    } 
    
    // Метод для выборки продуктов по ID заказа (специальный запрос для заказов)
    public static ObservableCollection<Product> SelectTable(int id)
    {
        // SQL-запрос для выборки продуктов конкретного заказа с дополнительной информацией
        string query = @"SELECT 
                            p.ID_Product AS id,
                            p.ProductName AS name,
                            cp.CategoryProduct AS category,
                            p.ProductCategory AS categoryId,
                            p.ProductCountInStock AS countInStock,
                            0 AS discount,
                            CAST(p.ProductCost AS DECIMAL(10,2)) AS cost,
                            op.Count AS quantityInOrder, -- Количество в заказе
                            CAST(op.CostProduct AS DECIMAL(10,2)) AS priceInOrder -- Цена в заказе
                        FROM product p
                        JOIN orderproduct op ON p.ID_Product = op.ID_Product
                        JOIN categoryproduct cp ON p.ProductCategory = cp.ID_CategoryProduct
                        WHERE op.ID_Order = " + $"{id}" + @"
                        ORDER BY p.ID_Product;";
        
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            ObservableCollection<Product> products = new ObservableCollection<Product>();
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Чтение данных о продуктах в заказе
            while (reader.Read())
            {
                products.Add(new Product(
                    reader.GetInt32("id"),
                    reader.GetString("name"),
                    reader.GetString("category"),
                    reader.GetInt32("categoryId"),
                    reader.GetInt32("countInStock"),
                    reader.GetInt32("discount"),
                    reader.GetDouble("priceInOrder"))
                {
                    Trash = reader.GetInt32("quantityInOrder") // Устанавливаем количество из заказа
                });
            }
            connection.Close();
            return products;
        }
    }

    // Метод для выборки продуктов без изображений
    public static ObservableCollection<Product> SelectTableNoneImage(string query, string connectionString)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            ObservableCollection<Product> products = new ObservableCollection<Product>();
            connection.Open();
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Создание объектов Product без изображений
            while (reader.Read())
            {
                products.Add(new Product(
                    reader.GetInt32("ID"),
                    reader.GetString("Name"),
                    reader.GetString("Category"),
                    reader.GetInt32("CategoryId"),
                    reader.GetInt32("CountInStock"),
                    reader.GetInt32("Discount"),
                    reader.GetDouble("Cost")));
            }
            connection.Close();
            return products;
        }
    }

    // Метод для добавления нового продукта с изображением
    public static void InsertData(string name, int categoryId, int countInStoke, int discount, double price, byte[] image)
    {
        // SQL-запрос для вставки нового продукта с параметром изображения
        string query = $"INSERT INTO `vkr`.`product` (`ProductName`, `ProductCategory`, `ProductCountInStock`, `ProductDiscount`, `ProductCost`, `ProductImage`) " +
                       $"VALUES ('{name}', '{categoryId}', '{countInStoke}', '{discount}', '{price}', @ProductImage);";
        
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            
            // Создание параметра для BLOB-данных (изображения)
            MySqlParameter blob = new MySqlParameter("@ProductImage", MySqlDbType.Blob, image.Length);
            blob.Value = image;
            command.Parameters.Add(blob);
            
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    // Метод для обновления данных продукта без изображения
    public static void UpdateData(int id, string name, int categoryId, int countInStoke, int discount, double price)
    {
        string query =
            $"UPDATE `vkr`.`product` SET `ProductName` = '{name}'," +
            $" `ProductCategory` = '{categoryId}'," +
            $" `ProductCountInStock` = '{countInStoke}'," +
            $" `ProductDiscount` = '{discount}'," +
            $" `ProductCost` = '{price}'" +
            $" WHERE (`ID_Product` = '{id}');";

        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    // Метод для обновления данных продукта с изображением
    public static void UpdateData(int id, string name, int categoryId, int countInStoke, int discount, double price, byte[] image)
    {
        string query =
            $"UPDATE `vkr`.`product` SET `ProductName` = '{name}'," +
            $" `ProductCategory` = '{categoryId}'," +
            $" `ProductCountInStock` = {countInStoke}," +
            $" `ProductDiscount` = {discount}," +
            $" `ProductImage` = '@ProductImage'," +
            $" `ProductCost` = {price}" +
            $" WHERE (`ID_Product` = '{id}');";

        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            connection.Open();
            MySqlCommand command = new MySqlCommand("", connection);
            command.CommandText = query;
            
            // Создание параметра для BLOB-данных (изображения)
            MySqlParameter blob = new MySqlParameter("@ProductImage", MySqlDbType.Blob, image.Length);
            blob.Value = image;
            command.Parameters.Add(blob);
            
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}