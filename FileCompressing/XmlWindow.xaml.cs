using System.Windows;

namespace FileCompressing
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Interaction logic for XmlWindow.xaml
    /// </summary>
    public partial class XmlWindow : Window
    {
        private List<LoggingItem> LoggingItems;
        public XmlWindow()
        {
            InitializeComponent();
            this.LoggingItems = new List<LoggingItem>();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@"C:\Users\User\Desktop\My Projects\FileCompressing\FileCompressing\log.xml");
            XmlElement rootNode = xDoc.DocumentElement;
            if (rootNode != null)
            {
                foreach (XmlElement node in rootNode)
                {
                    LoggingItem loggingItem = new LoggingItem();
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.Name == "FileName")
                        {
                            loggingItem.NameOfFile = childNode.InnerText;
                        }

                        if (childNode.Name == "Location")
                        {
                            loggingItem.LocationOfFile = childNode.InnerText;
                        }

                        if (childNode.Name == "Operation")
                        {
                            loggingItem.FileOperation = (Operation)Enum.Parse(typeof(Operation), childNode.InnerText);
                        }

                        if (childNode.Name == "CompressionRatio")
                        {
                            loggingItem.CompressionRatio = double.Parse(childNode.InnerText);
                        }

                        if (childNode.Name == "Date")
                        {
                            loggingItem.DateOfOperation = childNode.InnerText;
                        }

                        if (childNode.Name == "Error")
                        {
                            loggingItem.ErrorMessage = childNode.InnerText;
                        }
                    }

                    this.LoggingItems.Add(loggingItem);
                }
            }

            foreach (var item in this.LoggingItems)
            {
                this.XmlBox.AppendText(item.ToString());
            }
        }

        private void ShowButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.FirstDate.SelectedDate != null && this.SecondDate.SelectedDate != null)
            {
                var date1 = this.FirstDate.SelectedDate;
                var date2 = this.SecondDate.SelectedDate;
                List<LoggingItem> resultList = new List<LoggingItem>();
                this.XmlBox.Text = string.Empty;
                foreach (var item in this.LoggingItems)
                {
                    DateTime temp = DateTime.Parse(item.DateOfOperation);
                    if (temp >= date1 && temp <= date2)
                    {
                        resultList.Add(item);
                    }
                }

                if (resultList.Count == 0)
                {
                    MessageBox.Show(
                        this,
                        "No events in this time line!",
                        "Attention!",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Information);
                }
                else
                {
                    foreach (var item in resultList)
                    {
                        this.XmlBox.AppendText(item.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show(
                    this,
                    "Please, choose dates first",
                    "Warning!",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation);
            }
        }
    }
}
