namespace VKR.Models;

// Класс для хранения данных о продажах товаров
// Используется для отчетов и аналитики
public class SalesData
{
    // Название проданного товара
    public string ProductName { get; set; }
    
    // Категория товара
    public string Category { get; set; }
    
    // Количество проданных единиц
    public int Quantity { get; set; }
    
    // Общая стоимость проданного товара
    public decimal TotalCost { get; set; }
}