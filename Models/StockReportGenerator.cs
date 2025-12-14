using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VKR.Models;

// Статический класс для генерации Excel-отчетов о запасах товаров на складе
public static class StockReportGenerator
{
    // Метод для создания Excel-отчета о количестве техники на складе
    public static void GenerateStockReport(string filePath, List<StockData> stockData)
    {
        // Установка лицензии EPPlus (бесплатной для некоммерческого использования)
        ExcelPackage.License.SetNonCommercialOrganization("Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            // Создание нового листа в Excel-документе
            var worksheet = package.Workbook.Worksheets.Add("Остатки на складе");

            // 1. Заголовок отчета
            worksheet.Cells[1, 1].Value = "ОТЧЕТ О КОЛИЧЕСТВЕ ТЕХНИКИ НА СКЛАДЕ";
            worksheet.Cells[1, 1, 1, 6].Merge = true; // Объединение ячеек для заголовка
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // 2. Дата формирования отчета
            worksheet.Cells[2, 1].Value = $"Дата формирования: {DateTime.Now:dd.MM.yyyy}";
            worksheet.Cells[2, 1, 2, 6].Merge = true;

            // 3. Заголовки столбцов таблицы
            var headers = new string[] { "№ п/п", "Наименование товара", "Категория", "Количество на складе", "Цена за единицу, руб.", "Общая стоимость, руб." };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[4, i + 1].Value = headers[i];
                worksheet.Cells[4, i + 1].Style.Font.Bold = true;
                worksheet.Cells[4, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                // Серая заливка для заголовков столбцов
                worksheet.Cells[4, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // 4. Заполнение таблицы данными
            int row = 5; // Начинаем с 5-й строки (после заголовков)
            foreach (var data in stockData.OrderBy(x => x.Category).ThenBy(x => x.ProductName))
            {
                // Порядковый номер
                worksheet.Cells[row, 1].Value = row - 4;
                // Наименование товара
                worksheet.Cells[row, 2].Value = data.ProductName;
                // Категория товара
                worksheet.Cells[row, 3].Value = data.Category;
                // Количество на складе
                worksheet.Cells[row, 4].Value = data.QuantityInStock;
                // Цена за единицу
                worksheet.Cells[row, 5].Value = data.UnitPrice;
                // Общая стоимость (вычисляемое свойство TotalValue)
                worksheet.Cells[row, 6].Value = data.TotalValue;

                // Форматирование числовых значений (денежный формат)
                worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[row, 6].Style.Numberformat.Format = "#,##0.00";

                row++;
            }

            // 5. Добавление границ для всех ячеек с данными
            using (var range = worksheet.Cells[4, 1, row - 1, 6])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }

            // 6. Автоподбор ширины столбцов для лучшего отображения
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // 7. Добавление строки с итогами
            worksheet.Cells[row + 1, 3].Value = "ИТОГО:";
            worksheet.Cells[row + 1, 3].Style.Font.Bold = true;
            // Формулы Excel для расчета итогов:
            // Сумма количества товаров на складе
            worksheet.Cells[row + 1, 4].Formula = $"=SUM(D5:D{row - 1})";
            // Сумма общей стоимости всех товаров
            worksheet.Cells[row + 1, 6].Formula = $"=SUM(F5:F{row - 1})";
            worksheet.Cells[row + 1, 6].Style.Numberformat.Format = "#,##0.00";

            // 8. Стиль для итоговой строки
            using (var range = worksheet.Cells[row + 1, 3, row + 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                // Голубая заливка для выделения итоговой строки
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            // 9. Сохранение Excel-файла по указанному пути
            package.SaveAs(new FileInfo(filePath));
        }
    }
}

