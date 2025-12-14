using Avalonia;
using System;

namespace VKR;

// Главный класс программы, содержащий точку входа
sealed class Program
{
    // Точка входа в приложение
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread] // Указывает, что для приложения требуется однопоточная модель апартаментов (STA)
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args); // Запуск приложения с классическим десктопным жизненным циклом

    // Конфигурация Avalonia приложения, не удалять; также используется визуальным дизайнером
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>() // Начинаем конфигурацию с указанием основного класса приложения
            .UsePlatformDetect()       // Автоматическое определение и настройка платформы (Windows/Linux/macOS)
            .WithInterFont()           // Использование шрифта Inter по умолчанию
            .LogToTrace();             // Настройка логирования в систему трассировки
}