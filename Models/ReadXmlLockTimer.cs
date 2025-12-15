using System;
using System.Xml;
using System.Xml.Linq;

namespace VKR.Models;

public static class ReadXmlLockTimer
{
    public static int ReadConfigFile()
    {
            // Загрузка XML-документа
            XmlDocument doc = new XmlDocument();

            doc.Load("./InactivityLockConfiguration.xml");
            
            // Извлечение элемента и его значения
            XmlElement timeoutElement = doc.DocumentElement;
            
            int configTime = Convert.ToInt32(timeoutElement.GetAttribute("InactivityTimeoutSeconds"));
            
            return configTime;
    }
}