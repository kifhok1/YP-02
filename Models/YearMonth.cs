using System;

namespace VKR.Models;

// Класс для представления года и месяца в формате "YYYY MM"
public class YearMonth
{
    private int _year;
    private int _month;
    private string _showDate;

    // Год
    public int Year
    {
        get { return _year; }
        set { _year = value; }
    }

    // Месяц (число от 1 до 12)
    public int Month
    {
        get { return _month; }
        set { _month = value; }
    }

    // Отображаемая дата в формате "YYYY MM"
    public string ShowDate
    {
        get { return _showDate; }
        set { _showDate = value; }
    }

    // Конструктор, принимающий строку формата "YYYY MM"
    public YearMonth(string date)
    {
        // Разделение строки на год и месяц
        Year = Convert.ToInt32(date.Split(' ')[0]);
        Month = Convert.ToInt32(date.Split(' ')[1]);
        ShowDate = date; // Сохранение исходной строки
    }
}