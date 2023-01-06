using System;

namespace FileCompressing
{
    using System.IO;

    public enum Operation
    {
        Compression, Decompression, Nothing
    }
    public class LoggingItem
    {
        public string NameOfFile { get; set; }

        public string LocationOfFile { get; set; }

        public Operation FileOperation { get; set; }

        public double CompressionRatio { get; set; }

        public string DateOfOperation { get; set; }

        public string ErrorMessage { get; set; }

        public LoggingItem()
        {
            this.NameOfFile = string.Empty;
            this.LocationOfFile = string.Empty;
            this.CompressionRatio = 0;
            this.DateOfOperation = string.Empty;
            this.ErrorMessage = string.Empty;
            this.FileOperation = Operation.Nothing;
        }

        public LoggingItem(string fileName, double compressionRatio, Operation operation, string errorMessage)
        {
            this.NameOfFile = fileName;
            this.LocationOfFile = fileName.Length > 0 ? Path.GetFullPath(fileName) : string.Empty;
            this.CompressionRatio = compressionRatio;
            this.FileOperation = operation;
            this.DateOfOperation = $"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}";
            this.ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            return $"File name: {this.NameOfFile}\n" + $"Location: {this.LocationOfFile}\n"
                                                     + $"Operation: {this.FileOperation}\n"
                                                     + $"Compress ratio: {this.CompressionRatio}\n"
                                                     + $"Date of operation: {this.DateOfOperation}\n"
                                                     + $"Errors: {this.ErrorMessage}\n\n";
        }
    }
}
