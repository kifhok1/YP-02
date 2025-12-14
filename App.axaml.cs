using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using System.Linq;
using VKR.ViewModels;
using VKR.Views;

namespace VKR;

public partial class App : Application
{
    // Инициализация приложения: загрузка XAML и настройка шаблонов представлений
    public override void Initialize()
    {
        // Загрузка XAML разметки приложения
        AvaloniaXamlLoader.Load(this);
        // Добавление локатора представлений для привязки ViewModel к View
        DataTemplates.Add(new ViewLocator());
    }

    // Метод вызывается после инициализации фреймворка Avalonia
    public override void OnFrameworkInitializationCompleted()
    {
        // Проверка, что приложение запущено в классическом десктопном режиме
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Отключаем дублирующую валидацию данных от Avalonia и CommunityToolkit
            // Подробнее: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            // Создание главного окна приложения
            desktop.MainWindow = new MainWindow();
            // Создание ViewModel для главного окна, передавая ссылку на само окно
            DataContext = new MainWindowViewModel(desktop.MainWindow);
            // Установка DataContext для главного окна
            desktop.MainWindow.DataContext = DataContext;
        }

        // Вызов базовой реализации метода
        base.OnFrameworkInitializationCompleted();
    }

    // Метод для отключения валидации DataAnnotations плагина Avalonia
    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Получаем массив плагинов валидации DataAnnotations, которые нужно удалить
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // Удаляем каждый найденный плагин из коллекции валидаторов
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}