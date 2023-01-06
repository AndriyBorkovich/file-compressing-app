using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCompressing
{
    using System.IO;

    public class HuffmanCompressor
    {
        private HuffmanNode rootHuffmanNode;

        private List<HuffmanNode> valueHuffmanNodes;

        public Operation FileOperation;

        private List<HuffmanNode> BuildBinaryTree(string value)
        {
            var huffmanNodes = GetInitialNodeList();

            value.ToList().ForEach(m => huffmanNodes[m].Weight++);

            huffmanNodes = huffmanNodes.Where(m => (m.Weight > 0)).OrderBy(m => (m.Weight)).ThenBy(m => (m.Value))
                .ToList();

            huffmanNodes = UpdateNodeParents(huffmanNodes);

            this.rootHuffmanNode = huffmanNodes[0];
            huffmanNodes.Clear();

            SortNodes(this.rootHuffmanNode, huffmanNodes);

            return huffmanNodes;
        }

        public string Compress(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            this.FileOperation = Operation.Compression;

            if (fileInfo.Exists)
            {
                var fileContents = string.Empty;

                using (StreamReader streamReader = new StreamReader(File.Open(fileName, FileMode.Open, FileAccess.Read)))
                {
                    fileContents = streamReader.ReadToEnd();
                }

                List<HuffmanNode> huffmanNodes = BuildBinaryTree(fileContents);

                this.valueHuffmanNodes = huffmanNodes.Where(m => (m.Value.HasValue))
                    .OrderBy(m => (m.BinaryWord)) 
                    .ToList();

                Dictionary<char, string> charToBinaryWordDictionary = new Dictionary<char, string>();
                foreach (HuffmanNode huffmanNode in this.valueHuffmanNodes)
                {
                    charToBinaryWordDictionary.Add(huffmanNode.Value.Value, huffmanNode.BinaryWord);
                }

                StringBuilder stringBuilder = new StringBuilder();
                List<byte> byteList = new List<byte>();
                for (int i = 0; i < fileContents.Length; i++)
                {
                    string word = string.Empty;

                    stringBuilder.Append(charToBinaryWordDictionary[fileContents[i]]);

                    while (stringBuilder.Length >= 8)
                    {
                        word = stringBuilder.ToString().Substring(0, 8);

                        stringBuilder.Remove(0, word.Length);
                    }

                    if (!String.IsNullOrEmpty(word))
                    {
                        byteList.Add(Convert.ToByte(word, 2));
                    }
                }

                if (stringBuilder.Length > 0)
                {
                    string word = stringBuilder.ToString();

                    if (!String.IsNullOrEmpty(word))
                    {
                        byteList.Add(Convert.ToByte(word, 2));
                    }
                }

                StringBuilder codingTable = new StringBuilder();
                foreach (var item in charToBinaryWordDictionary)
                {
                    codingTable.AppendLine($"{item.Key} {item.Value}");
                }

                string compressedFileName = Path.Combine(
                    fileInfo.Directory.FullName,
                    String.Format("{0}.dat", 
                        fileInfo.Name.Replace(fileInfo.Extension, string.Empty)));
                if (File.Exists(compressedFileName))
                {
                    File.Delete(compressedFileName);
                }

                var fileStream = File.OpenWrite(compressedFileName);
                fileStream.Write(byteList.ToArray(), 0, byteList.Count);
                fileStream.Close();
                FileInfo helpFileInfo = new FileInfo(compressedFileName);
                double uncompressedSize = fileInfo.Length;
                double compressedSize = helpFileInfo.Length;
                var spaceSaving = Math.Round(((1 - (compressedSize / uncompressedSize)) * 100), 2);
                XmlLogger.Log(new LoggingItem(fileInfo.Name, spaceSaving, Operation.Compression, "No errors"));
                return codingTable.ToString();
            }
            else
            {
                throw new FileNotFoundException("File not found!");
            }
        }

        public void Decompress(string fileNameToDecompress, string destinationFileName)
        {
            FileInfo compressedFileInfo = new FileInfo(fileNameToDecompress), destinationFileInfo = new FileInfo(destinationFileName);
            this.FileOperation = Operation.Decompression;
            if (compressedFileInfo.Exists)
            {
                string compressedFileName = String.Format("{0}.dat", compressedFileInfo.FullName.Replace(compressedFileInfo.Extension, string.Empty));

                byte[] buffer = null;
                using (FileStream fileStream = File.Open(compressedFileName, FileMode.Open, FileAccess.Read))
                {
                    buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                }

                HuffmanNode zeroHuffmanNode = this.rootHuffmanNode;
                while (this.rootHuffmanNode.Left != null)
                {
                    zeroHuffmanNode = zeroHuffmanNode.Left;
                }

                HuffmanNode currentHuffmanNode = null;
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < buffer.Length; i++)
                {
                    string binaryWord = string.Empty;
                    byte b = buffer[i];

                    if (b == 0)
                    {
                        binaryWord = zeroHuffmanNode.BinaryWord;
                    }
                    else
                    {
                        binaryWord = Convert.ToString(b, 2);
                    }

                    if ((binaryWord.Length < 8) && (i < (buffer.Length - 1)))
                    {
                        StringBuilder binaryStringBuilder = new StringBuilder(binaryWord);
                        while (binaryStringBuilder.Length < 8)
                        {
                            binaryStringBuilder.Insert(0, "0");
                        }

                        binaryWord = binaryStringBuilder.ToString();
                    }

                    for (int j = 0; j < binaryWord.Length; j++)
                    {
                        char value = binaryWord[j];

                        if (currentHuffmanNode == null)
                        {
                            currentHuffmanNode = this.rootHuffmanNode;
                        }

                        if (value == '0')
                        {
                            currentHuffmanNode = currentHuffmanNode.Left;
                        }
                        else
                        {
                            currentHuffmanNode = currentHuffmanNode.Right;
                        }

                        if ((currentHuffmanNode.Left == null) && (currentHuffmanNode.Right == null))
                        {
                            stringBuilder.Append(currentHuffmanNode.Value.Value);
                            currentHuffmanNode = null;
                        }
                    }
                }

                string uncompressedFileName = Path.Combine(destinationFileInfo.Directory.FullName,
                    String.Format("{0}.uncompressed", 
                        destinationFileInfo.Name.Replace(destinationFileInfo.Extension, string.Empty)));

                if (File.Exists(uncompressedFileName))
                {
                    File.Delete(uncompressedFileName);
                }

                using (StreamWriter streamWriter = new StreamWriter(File.Open(uncompressedFileName, FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    streamWriter.Write(stringBuilder.ToString());
                    streamWriter.Close();
                    FileInfo decompressedFileInfo = new FileInfo(uncompressedFileName);
                    double uncompressedSize = decompressedFileInfo.Length;
                    double compressedSize = decompressedFileInfo.Length;
                    var spaceSaving = Math.Round(((1 - (compressedSize / uncompressedSize)) * 100), 2);
                    XmlLogger.Log(new LoggingItem(decompressedFileInfo.Name, spaceSaving, Operation.Decompression, "No errors"));
                }
            }
            else
            {
                throw new FileNotFoundException("No file with this name");
            }
        }

        private static List<HuffmanNode> GetInitialNodeList()
        {
            List<HuffmanNode> getInitialNodeList = new List<HuffmanNode>();

            for (int i = Char.MinValue; i < Char.MaxValue; i++)
            {
                getInitialNodeList.Add(new HuffmanNode((char)(i)));
            }

            return getInitialNodeList;
        }

        private static void SortNodes(HuffmanNode Node, List<HuffmanNode> Nodes)
        {
            if (!Nodes.Contains(Node))
            {
                Nodes.Add(Node);
            }

            if (Node.Left != null)
            {
                SortNodes(Node.Left, Nodes);
            }

            if (Node.Right != null)
            {
                SortNodes(Node.Right, Nodes);
            }
        }

        private static List<HuffmanNode> UpdateNodeParents(List<HuffmanNode> Nodes)
        {
            while (Nodes.Count > 1)
            {
                int iOperations = (Nodes.Count / 2);
                for (int operation = 0, i = 0, j = 1; operation < iOperations; operation++, i += 2, j += 2)
                {
                    if (j < Nodes.Count)
                    {
                        HuffmanNode parentHuffmanNode = new HuffmanNode(Nodes[i], Nodes[j]);
                        Nodes.Add(parentHuffmanNode);

                        Nodes[i] = null;
                        Nodes[j] = null;
                    }
                }

                Nodes = Nodes.Where(m => (m != null)).OrderBy(m => (m.Weight)).ToList();
            }

            return Nodes;
        }
    }
}
