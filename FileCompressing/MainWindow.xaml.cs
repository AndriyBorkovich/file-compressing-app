using System;
using System.Windows;
using System.Windows.Controls;

namespace FileCompressing
{
    using System.IO;

    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FilePathToWrite = string.Empty;

        public bool IsFileCreated = false;
        public string CodingTable = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The create file button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// </exception>
        private void CreateFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.FirstBox.Text))
            {
                SaveFileDialog saveFileDialog =
                    new SaveFileDialog { Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" };
                try
                {
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        this.FilePathToWrite = saveFileDialog.FileName;
                        if (!Path.GetExtension(this.FilePathToWrite).Contains(".txt"))
                        {
                            throw new FileFormatException(
                                $"{Path.GetFileName(this.FilePathToWrite)} isn't a text file!\n"
                                + $" Please, choose/create another file with correct extension (.txt)");
                        }
                        else
                        {
                            File.AppendAllText(this.FilePathToWrite, this.FirstBox.Text);
                            MessageBox.Show(
                                this,
                                "File was created successfully",
                                "Congrats!",
                                MessageBoxButton.OKCancel,
                                MessageBoxImage.Information);
                            this.IsFileCreated = true;
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException("Can't find/create file!\nPlease choose/create file");
                    }
                }
                catch (Exception exception)
                {
                    XmlLogger.Log(new LoggingItem(this.FilePathToWrite, 0, Operation.Nothing, exception.Message));
                    MessageBox.Show(
                        this,
                        exception.Message,
                        "Warning!",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(
                    this,
                    "Please, enter text to create file!",
                    "Warning!",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// The first box_ on text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FirstBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.CreateFileButton.IsEnabled = true;
        }

        /// <summary>
        /// The compress file button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// </exception>
        private void CompressFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var filePath = string.Empty;
            var huffmanCompressor = new HuffmanCompressor();
            try
            {
                if (!this.IsFileCreated)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.Title = "Choose file to compress:";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        filePath = openFileDialog.FileName;
                        if (!Path.GetExtension(filePath).Contains(".txt"))
                        {
                            throw new FileFormatException(
                                $"{Path.GetFileName(filePath)} isn't a text file!\n"
                                + $" Please, choose/create another file with correct extension (.txt)");
                        }
                        else
                        {
                            filePath = openFileDialog.FileName;
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException("Can't open file!\nPlease, try again");
                    }
                }
                else
                {
                    filePath = this.FilePathToWrite;
                }

                this.CodingTable = huffmanCompressor.Compress(filePath);
                this.CodingTableMenuItem.IsEnabled = true;
                MessageBox.Show(
                    this,
                    "File was compressed successfully!",
                    "Congrats!",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                if (filePath != null)
                {
                    XmlLogger.Log(new LoggingItem(filePath, 0,  huffmanCompressor.FileOperation, exception.Message));
                }

                MessageBox.Show(
                    this,
                    exception.Message,
                    "Error!",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// The decompress file button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="FileFormatException">
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// </exception>
        private void DecompressFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var filePathToDecompress = string.Empty;
            var filePathToSave = string.Empty;
            var huffmanCompressor = new HuffmanCompressor();
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "DAT files (*.dat)|*.dat|All files (*.*)|*.*";
                openFileDialog.Title = "Choose file to decompress:";
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.Title = "Choose where to save result:";
                if (openFileDialog.ShowDialog() == true && saveFileDialog.ShowDialog() == true)
                {
                    filePathToDecompress = openFileDialog.FileName;
                    filePathToSave = saveFileDialog.FileName;
                    if (!Path.GetExtension(filePathToDecompress).Contains(".dat"))
                    {
                        throw new FileFormatException(
                            $"{Path.GetFileName(filePathToDecompress)} isn't a DAT file!\n"
                            + $" Please, choose/create another file with correct extension (.dat)");
                    }
                    else if (!Path.GetExtension(filePathToSave).Contains(".txt"))
                    {
                        throw new FileFormatException(
                            $"{Path.GetFileName(filePathToSave)} isn't a text file!\n"
                            + $" Please, choose/create another file with correct extension (.txt)");
                    }
                    else
                    {
                        huffmanCompressor.Decompress(filePathToDecompress, filePathToSave);
                        MessageBox.Show(
                            this,
                            "File was decompressed successfully!",
                            "Congrats!",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Information);
                    }
                }
                else
                {
                    throw new FileNotFoundException("Can't open file!\nPlease, try again");
                }
            }
            catch (Exception exception)
            {
                XmlLogger.Log(new LoggingItem(filePathToDecompress,  0, huffmanCompressor.FileOperation, exception.Message));

                MessageBox.Show(
                    this,
                    exception.Message,
                    "Warning!",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Open xml file on button click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OpenXmlFileButton_OnClick(object sender, RoutedEventArgs e)
        {
           var newWindow = new XmlWindow();
           newWindow.Show();
        }

        /// <summary>
        /// Opens coding table on menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CodingTableMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.ResultBox.Text = this.CodingTable;
            infoWindow.Show();
        }

        private void InstructionMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            InstructionWindow instructionWindow = new InstructionWindow();
            instructionWindow.Show();
        }
    }
}
