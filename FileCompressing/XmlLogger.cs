namespace FileCompressing
{
    using System.Xml;
    public static class XmlLogger
    {
        public static void Log(LoggingItem loggingItem)
        {
            XmlDocument document = new XmlDocument();
            document.Load(@"C:\Users\User\Desktop\My Projects\FileCompressing\FileCompressing\log.xml");

            XmlNode logItemNode = document.CreateElement("LogItem");

            XmlNode nameOfFileNode = document.CreateElement("FileName");
            nameOfFileNode.InnerText = loggingItem.NameOfFile;
            logItemNode.AppendChild(nameOfFileNode);

            XmlNode locationNode = document.CreateElement("Location");
            locationNode.InnerText = loggingItem.LocationOfFile;
            logItemNode.AppendChild(locationNode);

            XmlNode operationNode = document.CreateElement("Operation");
            operationNode.InnerText = loggingItem.FileOperation.ToString();
            logItemNode.AppendChild(operationNode);

            XmlNode compressionRateNode = document.CreateElement("CompressionRatio");
            compressionRateNode.InnerText = loggingItem.CompressionRatio.ToString();
            logItemNode.AppendChild(compressionRateNode);

            XmlNode dateNode = document.CreateElement("Date");
            dateNode.InnerText = loggingItem.DateOfOperation;
            logItemNode.AppendChild(dateNode);

            XmlNode errorNode = document.CreateElement("Error");
            errorNode.InnerText = loggingItem.ErrorMessage;
            logItemNode.AppendChild(errorNode);

            document.DocumentElement?.AppendChild(logItemNode);
            document.Save(@"C:\Users\User\Desktop\My Projects\FileCompressing\FileCompressing\log.xml");
        }
    }
}
