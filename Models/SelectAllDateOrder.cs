using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace VKR.Models;

// Статический класс для выборки уникальных дат заказов из базы данных
public static class SelectAllDateOrder
{
    // Метод для получения списка уникальных год-месяц комбинаций из заказов
    public static List<YearMonth> SelectDateOrder()
    {
        // SQL-запрос для выборки уникальных год-месяц комбинаций из таблицы заказов
        string query = @"SELECT 
                             DATE_FORMAT(DateOrder, '%Y %m') AS OrderMonthYear
                         FROM `order`
                         GROUP BY DATE_FORMAT(DateOrder, '%Y %m')
                         ORDER BY MIN(DateOrder);";
        
        // Устанавливаем соединение с базой данных
        using (MySqlConnection connection = new MySqlConnection(ConnectToDB.ConnectToDBString()))
        {
            // Список для хранения результатов выборки
            List<YearMonth> date = new List<YearMonth>();
            connection.Open();
            
            // Выполняем SQL-запрос
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            
            // Читаем результаты построчно и добавляем в список
            while (reader.Read())
            {
                // Создаем объект YearMonth из строки формата "YYYY MM"
                date.Add(new YearMonth(reader.GetString("OrderMonthYear")));
            }
            
            connection.Close();
            return date; // Возвращаем список уникальных дат
        }
    }
}