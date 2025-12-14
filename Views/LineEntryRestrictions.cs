using System.Text.RegularExpressions;

namespace VKR.Models;

// Статический класс для валидации и фильтрации ввода текста в различные поля
public static class LineEntryRestrictions
{
    // Метод для фильтрации только русских букв и пробелов
    public static string TextChangedRu(string text)
    {
        Regex regexCir = new Regex(@"[^а-яА-Я\s]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }

    // Метод для фильтрации только английских букв и пробелов
    public static string TextChangeEn(string text)
    {
        Regex regexCir = new Regex(@"[^a-zA-Z\s]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }

    // Метод для фильтрации русских букв, цифр и пробелов
    public static string TextChangedRuNum(string text)
    {
        Regex regexCir = new Regex(@"[^а-яА-Я0-9\s]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }

    // Метод для фильтрации английских букв, цифр и пробелов
    public static string TextChangeEnNum(string text)
    {
        Regex regexCir = new Regex(@"[^a-zA-Z0-9\s]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }

    // Метод для фильтрации логина (английские буквы, цифры, дефис, подчеркивание, пробелы)
    public static string TextChangeLogin(string text)
    {
        Regex regexCir = new Regex(@"[^a-zA-Z0-9\-_\s]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }

    // Метод для фильтрации пароля (расширенный набор символов для безопасности)
    public static string TextChangePassword(string text)
    {
        Regex regexCir = new Regex(@"[^a-zA-Z0-9_\-\+\/\*\{\}\[\]\|\s]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }

    // Метод для фильтрации русских и английских букв, цифр и пробелов
    public static string TextChangedRuEnNum(string text)
    {
        Regex regexCir = new Regex(@"[^а-яА-Яa-zA-Z0-9\s]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }

    // Метод для фильтрации русских и английских букв и пробелов
    public static string TextChangedRuEn(string text)
    {
        Regex regexCir = new Regex(@"[^а-яА-Яa-zA-Z\s]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }

    // Метод для фильтрации номеров телефонов (цифры, плюс, дефис, скобки, пробелы)
    public static string TextChangedPhoneNumber(string text)
    {
        Regex regexCir = new Regex(@"[^0-9+\-()\s]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }

    // Метод для фильтрации и форматирования ФИО
    public static string TextChangedFio(string text)
    {
        // Разрешаем только русские буквы, пробелы и дефисы
        Regex regexCir = new Regex(@"[^А-ЯЁа-яё\s-]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        // Форматирование ФИО: каждое слово с заглавной буквы
        if (!string.IsNullOrEmpty(textOut))
        {
            // Приведение к формату "Первая Заглавная, остальные строчные"
            textOut = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(textOut.ToLower());

            // Обработка составных фамилий через дефис (например, Иванов-Петров)
            string[] words = textOut.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains('-'))
                {
                    string[] parts = words[i].Split('-');
                    for (int j = 0; j < parts.Length; j++)
                    {
                        if (!string.IsNullOrEmpty(parts[j]))
                        {
                            // Каждая часть составного слова с заглавной буквы
                            parts[j] = char.ToUpper(parts[j][0]) + parts[j].Substring(1).ToLower();
                        }
                    }
                    words[i] = string.Join("-", parts);
                }
            }
            textOut = string.Join(" ", words);
        }

        return textOut;
    }

    // Метод для фильтрации цены (цифры и точка для десятичных чисел)
    public static string TextChangedPrice(string text)
    {
        Regex regexCir = new Regex(@"[^0-9\.]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }

    // Метод для фильтрации только цифр
    public static string TextChangedNum(string text)
    {
        Regex regexCir = new Regex(@"[^0-9]");
        string textOut = text;

        if (regexCir.IsMatch(text))
        {
            textOut = regexCir.Replace(text, "");
        }

        return textOut;
    }
}