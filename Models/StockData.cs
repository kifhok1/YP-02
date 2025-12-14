namespace VKR.Models;

// Класс для хранения данных о товарах на складе
// Используется для отчетов по остаткам и оценки стоимости запасов
public class StockData
{
    // Название товара
    public string ProductName { get; set; }
    
    // Категория товара
    public string Category { get; set; }
    
    // Количество товара на складе
    public int QuantityInStock { get; set; }
    
    // Цена за единицу товара
    public decimal UnitPrice { get; set; }
    
    // Вычисляемое свойство: общая стоимость товара на складе
    // Рассчитывается как произведение количества на цену за единицу
    public decimal TotalValue => QuantityInStock * UnitPrice;
}