using System.Xml;

namespace VKR.Models;

// Статический класс для получения строки подключения к базе данных
public static class ConnectToDB
{
    // Метод для получения строки подключения из XML-файла
    public static string ConnectToDBString()
    {
        // Создаем XML-документ для чтения конфигурации
        XmlDocument xDoc = new XmlDocument();
        // Загружаем XML-файл с настройками подключения
        xDoc.Load("./ConnectionString.xml");
        // Получаем корневой элемент XML-документа
        XmlElement? xRoot = xDoc.DocumentElement;
        
        // Значения по умолчанию на случай отсутствия или ошибки в XML-файле
        string host = "localhost";
        string uid = "root";
        string pwd = "root";
        string db = "VKR";
        
        // Проверяем, что корневой элемент существует и содержит атрибуты
        if (xRoot != null)
        {
            // Читаем параметры подключения из атрибутов XML-элемента
            host = xRoot.Attributes["host"].Value;
            uid = xRoot.Attributes["uid"].Value;
            pwd = xRoot.Attributes["pwd"].Value;
            db = xRoot.Attributes["db"].Value;
        }
        
        // Формируем строку подключения в формате MySQL
        // ВАЖНО: на данный момент это заглушка возвращающая постоянную строку подключения
        return $"host=localhost;uid=root;pwd=root;database=VKR";
    }
}