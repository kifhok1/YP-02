using System.Collections.ObjectModel;

namespace VKR.Models;

// Обобщенный интерфейс для операций выборки данных из таблиц
// Использует статические абстрактные члены (C# 11+)
public interface ISelectTables<T>
{
    // Статический абстрактный метод для выборки данных из таблицы
    // Возвращает коллекцию объектов типа T
    static abstract ObservableCollection<T> SelectTable(string query, string connectionString);
}