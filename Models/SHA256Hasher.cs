using System.Security.Cryptography;
using System.Text;

// Статический класс для хеширования строк с использованием алгоритма SHA256
public static class SHA256Hasher
{
    // Метод для вычисления SHA256 хеша входной строки
    public static string ComputeSHA256Hash(string input)
    {
        // Проверка на пустую или null строку
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Создание экземпляра SHA256 для вычисления хеша
        using (SHA256 sha256 = SHA256.Create())
        {
            // Преобразование строки в байтовый массив с кодировкой UTF-8
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // Вычисление хеша SHA256 для входных данных
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // Преобразование байтового массива хеша в шестнадцатеричную строку
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                // Форматирование каждого байта как двузначного шестнадцатеричного числа
                sb.Append(b.ToString("x2"));
            }

            // Возврат итоговой хеш-строки
            return sb.ToString();
        }
    }
}