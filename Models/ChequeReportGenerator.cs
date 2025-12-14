using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace VKR.Models;

public class ChequeReportGenerator
{
    // Генерирует кассовый чек в формате Word на основе данных заказа
    public static void GenerateOrderReceipt(NewOrder order, string outputPath)
    {
        // Создаем новый документ Word
        using (WordprocessingDocument document = WordprocessingDocument.Create(
            outputPath, WordprocessingDocumentType.Document))
        {
            // Добавляем основную часть документа
            MainDocumentPart mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
            
            // Создаем таблицу стилей для документа
            CreateStylesPart(mainPart);
            
            Body body = new Body();

            // 1. Заголовок чека (без фона)
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
                    new Text("КАССОВЫЙ ЧЕК")
                )
            );
            body.Append(titleParagraph);

            // Добавляем пустую строку для визуального разделения
            body.Append(new Paragraph(new Run(new Text(""))));

            // 2. Информация о клиенте
            body.Append(CreateParagraph("ИНФОРМАЦИЯ О КЛИЕНТЕ", 
                new RunProperties(
                    new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                    new FontSize() { Val = "24" },
                    new Bold(),
                    new Color() { Val = "2F5496" }
                )));
            
            if (order.ClientOrder != null)
            {
                var clientInfo = $"ФИО: {order.ClientOrder.Fio}\n" +
                                $"Телефон: {order.ClientOrder.PhoneNumber}\n"  +
                                $"Скидка постоянного покупателя: {order.TotalDiscount}%";
                body.Append(CreateParagraph(clientInfo, 
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "22" },
                        new Color() { Val = "000000" }
                    )));
            }

            // 3. Информация о заказе
            body.Append(CreateParagraph("\nИНФОРМАЦИЯ О ЗАКАЗЕ", 
                new RunProperties(
                    new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                    new FontSize() { Val = "24" },
                    new Bold(),
                    new Color() { Val = "2F5496" }
                )));
            
            var orderInfo = $"Дата оформления: {order.Date:dd.MM.yyyy}\n" +
                           $"Дата доставки: {order.DeliveryDate:dd.MM.yyyy}";
            body.Append(CreateParagraph(orderInfo, 
                new RunProperties(
                    new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                    new FontSize() { Val = "22" },
                    new Color() { Val = "000000" }
                )));

            // 4. Таблица товаров
            body.Append(CreateParagraph("\nСОСТАВ ЗАКАЗА", 
                new RunProperties(
                    new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                    new FontSize() { Val = "24" },
                    new Bold(),
                    new Color() { Val = "2F5496" }
                )));
            
            // Проверяем наличие товаров в заказе
            if (order.Products != null && order.Products.Any(p => p.Trash > 0))
            {
                // Создаем таблицу для отображения товаров
                Table table = new Table();

                // Настройка свойств таблицы: границы, ширина, внешний вид
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
                string[] headers = { "№", "Наименование", "Категория", "Кол-во", "Цена", "Сумма" };
                
                // Создаем ячейки заголовков
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
                                new FontSize() { Val = "24" },
                                new Bold(),
                                new Color() { Val = "FFFFFF" }
                            ), 
                            JustificationValues.Center,
                            "2F5496")
                    );
                    headerRow.Append(cell);
                }
                table.Append(headerRow);

                int itemNumber = 1;
                bool alternateRow = false; // Флаг для чередования цветов строк

                // Заполняем таблицу товарами с чередующимися цветами строк
                foreach (var product in order.Products.Where(p => p.Trash > 0))
                {
                    // Расчет цены с учетом скидки на товар
                    double price = product.Cost - (product.Cost * product.Discount / 100);
                    double totalForProduct = product.Trash * price;

                    TableRow row = new TableRow();
                    string rowBackgroundColor = alternateRow ? "F8F8F8" : "FFFFFF"; // Чередующиеся цвета для лучшей читаемости
                    
                    // Первая колонка - номер по порядку
                    row.Append(CreateTableCell(itemNumber.ToString(), 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Color() { Val = "000000" }
                        ), 
                        JustificationValues.Center,
                        rowBackgroundColor));
                    
                    // Название товара
                    row.Append(CreateTableCell(product.Name, 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Color() { Val = "000000" }
                        ), 
                        JustificationValues.Left,
                        rowBackgroundColor));
                    
                    // Категория товара
                    row.Append(CreateTableCell(product.CategoryView, 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Color() { Val = "000000" }
                        ), 
                        JustificationValues.Left,
                        rowBackgroundColor));
                    
                    // Количество товара
                    row.Append(CreateTableCell(product.Trash.ToString(), 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Color() { Val = "000000" }
                        ), 
                        JustificationValues.Center,
                        rowBackgroundColor));
                    
                    // Цена товара (красный цвет если есть скидка)
                    string priceColor = product.Discount > 0 ? "C00000" : "000000";
                    RunProperties priceRunProps = new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "22" },
                        new Color() { Val = priceColor }
                    );
                    if (product.Discount > 0)
                    {
                        priceRunProps.Bold = new Bold();
                    }
                    row.Append(CreateTableCell($"{price:C}", 
                        priceRunProps, 
                        JustificationValues.Right,
                        rowBackgroundColor));
                    
                    // Сумма за позицию
                    row.Append(CreateTableCell($"{totalForProduct:C}", 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Bold(),
                            new Color() { Val = "000000" }
                        ), 
                        JustificationValues.Right,
                        rowBackgroundColor));
                    
                    table.Append(row);
                    itemNumber++;
                    alternateRow = !alternateRow; // Переключаем цвет для следующей строки
                }

                // Итоговая строка (с серым фоном для выделения)
                TableRow totalRow = new TableRow();
                string totalRowColor = "E6E6E6"; // Светло-серый фон для итоговой строки
                
                // Пустые ячейки для выравнивания (4 колонки)
                for (int i = 0; i < 4; i++)
                {
                    totalRow.Append(CreateTableCell("", 
                        new RunProperties(
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "22" },
                            new Color() { Val = "000000" }
                        ), 
                        JustificationValues.Left, 
                        totalRowColor));
                }
                
                // Ячейка с текстом "ИТОГО:"
                totalRow.Append(CreateTableCell("ИТОГО:", 
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "24" },
                        new Bold(),
                        new Color() { Val = "000000" }
                    ), 
                    JustificationValues.Right, 
                    totalRowColor));
                
                // Ячейка с итоговой суммой заказа
                totalRow.Append(CreateTableCell($"{order.TotalPrice:C}", 
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "24" },
                        new Bold(),
                        new Color() { Val = "2F5496" }
                    ), 
                    JustificationValues.Right, 
                    totalRowColor));
                
                table.Append(totalRow);
                body.Append(table);
            }

            // 5. Итоговая информация о платеже
            body.Append(CreateParagraph("\nИТОГИ", 
                new RunProperties(
                    new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                    new FontSize() { Val = "24" },
                    new Bold(),
                    new Color() { Val = "2F5496" }
                )));
            
            // Расчет скидки и итоговой суммы
            double discountAmount = order.TotalPrice * order.TotalDiscount / 100;
            double finalAmount = order.TotalPrice - discountAmount;

            var summaryText = $"Общая стоимость: {order.TotalPrice:C}\n" +
                             $"Накопительная скидка ({order.TotalDiscount}%): -{discountAmount:C}\n" +
                             $"К ОПЛАТЕ: {finalAmount:C}";

            // Разделяем итоговую информацию на строки с разным форматированием
            var summaryLines = summaryText.Split('\n');
            
            // Общая стоимость
            body.Append(CreateParagraph(summaryLines[0], 
                new RunProperties(
                    new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                    new FontSize() { Val = "22" },
                    new Color() { Val = "000000" }
                )));
            
            // Скидка (красным цветом)
            body.Append(CreateParagraph(summaryLines[1], 
                new RunProperties(
                    new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                    new FontSize() { Val = "22" },
                    new Color() { Val = "C00000" }
                )));
            
            // Итоговая сумма к оплате (синим цветом)
            body.Append(CreateParagraph(summaryLines[2], 
                new RunProperties(
                    new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                    new FontSize() { Val = "24" },
                    new Bold(),
                    new Color() { Val = "2F5496" }
                )));

            // 6. Примечание (дата печати)
            var footerParagraph = new Paragraph(
                new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Right }
                ),
                new Run(
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "20" },
                        new Italic(),
                        new Color() { Val = "7F7F7F" }
                    ),
                    new Text($"Дата печати: {DateTime.Now:dd.MM.yyyy HH:mm:ss}")
                )
            );
            body.Append(footerParagraph);

            // 7. Финальное сообщение
            var finalParagraph = new Paragraph(
                new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                ),
                new Run(
                    new RunProperties(
                        new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                        new FontSize() { Val = "24" },
                        new Bold(),
                        new Color() { Val = "2F5496" }
                    ),
                    new Text("Спасибо за покупку!")
                )
            );
            body.Append(finalParagraph);

            // Добавляем тело документа в основной раздел
            mainPart.Document.Append(body);
        }
    }

    // Создает таблицу стилей для документа Word
    private static void CreateStylesPart(MainDocumentPart mainPart)
    {
        StyleDefinitionsPart stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
        Styles styles = new Styles();

        // Стиль заголовка (без фона)
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

    // Вспомогательный метод для создания параграфа с заданными свойствами
    private static Paragraph CreateParagraph(string text, RunProperties properties)
    {
        var paragraph = new Paragraph();
        var run = new Run();
        
        // Клонируем свойства, чтобы избежать изменений в оригинале
        if (properties != null)
        {
            var clonedProps = properties.CloneNode(true) as RunProperties;
            run.RunProperties = clonedProps;
        }
        
        // Обрабатываем переносы строк в тексте
        var lines = text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            run.Append(new Text(lines[i]));
            if (i < lines.Length - 1)
            {
                run.Append(new Break()); // Добавляем разрыв строки
            }
        }
        
        paragraph.Append(run);
        return paragraph;
    }

    // Создает параграф для ячейки таблицы с указанным выравниванием и фоном
    private static Paragraph CreateParagraphInCell(string text, RunProperties properties, 
                                                  JustificationValues alignment, string backgroundColor = "FFFFFF")
    {
        var paragraphProperties = new ParagraphProperties(
            new Justification() { Val = alignment }
        );
        
        // Добавляем фон для абзаца в ячейке (если не белый)
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
        
        // Добавляем фон для ячейки (если не белый)
        if (backgroundColor != "FFFFFF")
        {
            cellProperties.Append(new Shading() { Fill = backgroundColor });
        }
        
        return new TableCell(
            cellProperties,
            CreateParagraphInCell(text, properties, alignment, backgroundColor)
        );
    }
}