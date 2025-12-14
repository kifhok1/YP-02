using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace VKR.Models;

// Статический класс для генерации отчетов о продажах в формате Word
public static class SalesReportGenerator
{
    // Генерирует отчет о продажах за указанный месяц и год
    public static void GenerateSalesReport(string filePath, int year, int month, List<SalesData> salesData)
    {
        // Создаем новый документ Word
        using (WordprocessingDocument document = WordprocessingDocument.Create(
            filePath, WordprocessingDocumentType.Document))
        {
            // Добавляем основную часть документа
            MainDocumentPart mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
            
            // Создаем таблицу стилей для оформления документа
            CreateStylesPart(mainPart);
            
            Body body = new Body();

            // 1. Заголовок отчета (стиль как в чеке)
            var titleParagraph = new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Title" },
                    new Justification() { Val = JustificationValues.Center }
                ),
                new Run(
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "32" },
                        new Bold(),
                        new Color() { Val = "2F5496" }
                    ),
                    new Text("ОТЧЕТ О ПРОДАЖАХ ФОТОТЕХНИКИ")
                )
            );
            body.Append(titleParagraph);

            // 2. Период отчета
            var periodParagraph = new Paragraph(
                new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                ),
                new Run(
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "24" },
                        new Color() { Val = "2F5496" }
                    ),
                    new Text($"За период: {GetMonthName(month)} {year} года")
                )
            );
            body.Append(periodParagraph);

            // Добавляем пустую строку для разделения
            body.Append(new Paragraph(new Run(new Text(""))));

            // 3. Таблица продаж
            if (salesData != null && salesData.Count > 0)
            {
                // Создаем таблицу для отображения данных о продажах
                Table table = new Table();

                // Свойства таблицы (стиль как в чеке)
                TableProperties tblProperties = new TableProperties(
                    new TableBorders(
                        new TopBorder() { Val = BorderValues.Single, Size = 8, Color = "2F5496" },
                        new BottomBorder() { Val = BorderValues.Single, Size = 8, Color = "2F5496" },
                        new LeftBorder() { Val = BorderValues.Single, Size = 8, Color = "2F5496" },
                        new RightBorder() { Val = BorderValues.Single, Size = 8, Color = "2F5496" },
                        new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4, Color = "D9D9D9" },
                        new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4, Color = "D9D9D9" }
                    ),
                    new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct },
                    new TableLayout() { Type = TableLayoutValues.Fixed },
                    new TableLook() { Val = "04A0" }
                );
                table.AppendChild(tblProperties.CloneNode(true));

                // Заголовки таблицы с синим фоном
                TableRow headerRow = new TableRow();
                string[] headers = { "№ п/п", "Наименование товара", "Категория", "Количество", "Стоимость" };
                
                // Создаем ячейки заголовков таблицы
                foreach (var header in headers)
                {
                    TableCell cell = new TableCell(
                        new TableCellProperties(
                            new TableCellWidth() { Width = "1200", Type = TableWidthUnitValues.Dxa },
                            new Shading() { Fill = "2F5496" }
                        ),
                        CreateParagraphInCell(header, 
                            new RunProperties(
                                new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                                new FontSize() { Val = "22" },
                                new Bold(),
                                new Color() { Val = "FFFFFF" }
                            ), 
                            JustificationValues.Center,
                            "2F5496")
                    );
                    headerRow.Append(cell);
                }
                table.Append(headerRow);

                // Заполняем таблицу данными с чередующимися цветами строк
                bool alternateRow = false;
                for (int i = 0; i < salesData.Count; i++)
                {
                    var data = salesData[i];
                    string rowBackgroundColor = alternateRow ? "F8F8F8" : "FFFFFF";
                    
                    TableRow row = new TableRow();
                    
                    // Номер по порядку
                    row.Append(CreateTableCell((i + 1).ToString(), 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Color() { Val = "000000" }
                        ), 
                        JustificationValues.Center,
                        rowBackgroundColor));
                    
                    // Наименование товара
                    row.Append(CreateTableCell(data.ProductName, 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Color() { Val = "000000" }
                        ), 
                        JustificationValues.Left,
                        rowBackgroundColor));
                    
                    // Категория товара
                    row.Append(CreateTableCell(data.Category, 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Color() { Val = "000000" }
                        ), 
                        JustificationValues.Left,
                        rowBackgroundColor));
                    
                    // Количество проданных единиц
                    row.Append(CreateTableCell(data.Quantity.ToString(), 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Color() { Val = "000000" }
                        ), 
                        JustificationValues.Center,
                        rowBackgroundColor));
                    
                    // Общая стоимость продаж по товару
                    row.Append(CreateTableCell($"{data.TotalCost:N2} руб.", 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Bold(),
                            new Color() { Val = "2F5496" }
                        ), 
                        JustificationValues.Right,
                        rowBackgroundColor));
                    
                    table.Append(row);
                    alternateRow = !alternateRow; // Переключаем цвет для следующей строки
                }
                
                body.Append(table);
            }

            // Добавляем пустую строку после таблицы
            body.Append(new Paragraph(new Run(new Text(""))));

            // 4. Итоговая информация
            if (salesData != null && salesData.Count > 0)
            {
                // Расчет общих показателей по всем продажам
                var totalQuantity = salesData.Sum(x => x.Quantity);
                var totalAmount = salesData.Sum(x => x.TotalCost);

                // Заголовок "Итоговая информация"
                body.Append(CreateParagraph("ИТОГОВАЯ ИНФОРМАЦИЯ", 
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "24" },
                        new Bold(),
                        new Color() { Val = "2F5496" }
                    )));
                
                // Итого продано товаров (количество)
                body.Append(CreateParagraph($"Итого продано товаров: {totalQuantity} шт.", 
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "22" },
                        new Bold(),
                        new Color() { Val = "000000" }
                    )));
                
                // Общая сумма продаж
                body.Append(CreateParagraph($"Общая сумма продаж: {totalAmount:N2} руб.", 
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "22" },
                        new Bold(),
                        new Color() { Val = "2F5496" }
                    )));
            }

            // Добавляем пустую строку перед подписью
            body.Append(new Paragraph(new Run(new Text(""))));

            // 5. Подпись и дата формирования отчета
            var signatureParagraph = new Paragraph(
                new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Left }
                )
            );
            
            var signatureRun = new Run(
                new RunProperties(
                    new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                    new FontSize() { Val = "20" },
                    new Color() { Val = "7F7F7F" }
                )
            );
            
            // Дата формирования отчета
            signatureRun.Append(new Text($"Дата формирования отчета: {DateTime.Now:dd.MM.yyyy}"));
            signatureRun.Append(new Break());
            // Место для подписи ответственного лица
            signatureRun.Append(new Text("Ответственный: _____________ / (___________________)"));
            
            signatureParagraph.Append(signatureRun);
            body.Append(signatureParagraph);

            // Добавляем тело документа в основной раздел
            mainPart.Document.Append(body);
        }
    }
    
    // Создает таблицу стилей для документа Word
    private static void CreateStylesPart(MainDocumentPart mainPart)
    {
        StyleDefinitionsPart stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
        Styles styles = new Styles();

        // Стиль заголовка (синий, жирный, выровненный по центру)
        Style titleStyle = new Style()
        {
            Type = StyleValues.Paragraph,
            StyleId = "Title",
            CustomStyle = true
        };
        StyleName styleName = new StyleName() { Val = "Title" };
        titleStyle.Append(styleName);
        
        StyleParagraphProperties titleParaProps = new StyleParagraphProperties(
            new Justification() { Val = JustificationValues.Center }
        );
        titleStyle.Append(titleParaProps);
        
        StyleRunProperties titleRunProps = new StyleRunProperties(
            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
            new FontSize() { Val = "32" },
            new Bold(),
            new Color() { Val = "2F5496" }
        );
        titleStyle.Append(titleRunProps);
        
        styles.Append(titleStyle);
        
        // Стиль обычного текста
        Style normalStyle = new Style()
        {
            Type = StyleValues.Paragraph,
            StyleId = "Normal",
            CustomStyle = true
        };
        StyleName normalName = new StyleName() { Val = "Normal" };
        normalStyle.Append(normalName);
        
        StyleRunProperties normalRunProps = new StyleRunProperties(
            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
            new FontSize() { Val = "22" },
            new Color() { Val = "000000" }
        );
        normalStyle.Append(normalRunProps);
        
        styles.Append(normalStyle);

        stylePart.Styles = styles;
    }

    // Вспомогательные методы для создания элементов документа

    // Создает параграф с заданными свойствами текста
    private static Paragraph CreateParagraph(string text, RunProperties properties)
    {
        var paragraph = new Paragraph();
        var run = new Run();
        
        if (properties != null)
        {
            var clonedProps = properties.CloneNode(true) as RunProperties;
            run.RunProperties = clonedProps;
        }
        
        run.Append(new Text(text));
        paragraph.Append(run);
        return paragraph;
    }

    // Создает параграф для ячейки таблицы с выравниванием и фоном
    private static Paragraph CreateParagraphInCell(string text, RunProperties properties, 
                                                  JustificationValues alignment, string backgroundColor = "FFFFFF")
    {
        var paragraphProperties = new ParagraphProperties(
            new Justification() { Val = alignment }
        );
        
        if (backgroundColor != "FFFFFF")
        {
            paragraphProperties.Append(new Shading() { Fill = backgroundColor });
        }
        
        var paragraph = new Paragraph(paragraphProperties);
        
        var run = new Run();
        if (properties != null)
        {
            var clonedProps = properties.CloneNode(true) as RunProperties;
            run.RunProperties = clonedProps;
        }
        
        run.Append(new Text(text));
        paragraph.Append(run);
        
        return paragraph;
    }

    // Создает ячейку таблицы с текстом и заданными свойствами
    private static TableCell CreateTableCell(string text, RunProperties properties, 
                                           JustificationValues alignment, string backgroundColor = "FFFFFF")
    {
        var cellProperties = new TableCellProperties(
            new TableCellWidth() { Width = "1200", Type = TableWidthUnitValues.Dxa }
        );
        
        if (backgroundColor != "FFFFFF")
        {
            cellProperties.Append(new Shading() { Fill = backgroundColor });
        }
        
        return new TableCell(
            cellProperties,
            CreateParagraphInCell(text, properties, alignment, backgroundColor)
        );
    }
    

    // Преобразует номер месяца в его название на русском языке
    private static string GetMonthName(int month)
    {
        return month switch
        {
            1 => "Январь",
            2 => "Февраль",
            3 => "Март",
            4 => "Апрель",
            5 => "Май",
            6 => "Июнь",
            7 => "Июль",
            8 => "Август",
            9 => "Сентябрь",
            10 => "Октябрь",
            11 => "Ноябрь",
            12 => "Декабрь",
            _ => "Неизвестный месяц"
        };
    }
}