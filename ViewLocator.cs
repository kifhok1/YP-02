using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using VKR.ViewModels;

namespace VKR;

// Класс ViewLocator реализует шаблон данных для автоматического сопоставления ViewModel с View
public class ViewLocator : IDataTemplate
{
    // Метод создания View на основе переданного ViewModel
    public Control? Build(object? data)
    {
        // Если data равен null, возвращаем null
        if (data == null)
        {
            return null;
        }

        // Заменяем "ViewModel" на "View" в полном имени типа ViewModel для получения имени View
        var viewName = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.InvariantCulture);

        // Получаем тип View по имени
        var type = Type.GetType(viewName);
        if (type == null)
        {
            return null;
        }
        
        // Создаем экземпляр View с помощью рефлексии
        var control = (Control)Activator.CreateInstance(type)!;
        // Устанавливаем DataContext для созданного View
        control.DataContext = data;
        return control;
    }

    // Метод проверяет, может ли ViewLocator обработать переданный объект
    public bool Match(object? data)
    {
        // Возвращаем true, если объект является экземпляром ViewModelBase или его потомком
        return data is ViewModelBase;
    }
}