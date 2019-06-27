using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EncodingConverter
{

    class Program
    {
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();


        static void Main(string[] args)
        {
            string path = @"D:\Tortoise\MailProcessor-hg";

            SearchFiles(path);
            Console.WriteLine($"Найдено файлов - {dictionary.Count}");

            int n = 1;
            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
            {
                Console.WriteLine($"{n} - {keyValuePair.Value} - {keyValuePair.Key}");
                n++;
            }

            //ConvertEngoding(dictionary);

            Console.ReadKey();
        }

        public static void SearchFiles(string path)
        {
            string[] attachments = Directory.GetFileSystemEntries(path);

            foreach (string attachment in attachments)
            {
                FileAttributes attributes = File.GetAttributes(attachment);
                if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    SearchFiles(attachment);
                }
                else
                {
                    using (FileStream fs = new FileStream(attachment, FileMode.Open))
                    {
                        string encoding = DetectEncoding(fs);
                        if (Path.GetExtension(attachment)==".cs" && !encoding.Contains("UTF-8") && !attachment.Contains(@"\obj\"))
                            dictionary.Add(attachment, encoding);
                        fs.Close();
                    }
                }
            }
        }

        public static string DetectEncoding(FileStream fs)
        {
            Ude.CharsetDetector cdet = new Ude.CharsetDetector();
            cdet.Feed(fs);
            cdet.DataEnd();
            if (cdet.Charset != null)
            {
                return cdet.Charset;
            }
            else
            {
                return "Error";
            }
        }

        public static void ConvertEngoding(Dictionary<string,string> dic)
        {
            foreach (KeyValuePair<string, string> keyValuePair in dic)
            {
                string temp;

                switch (keyValuePair.Value)
                {
                    case "ASCII":
                        {
                            using (StreamReader sr = new StreamReader(keyValuePair.Key, Encoding.ASCII))
                            {
                                temp = sr.ReadToEnd();
                                sr.Close();
                            }
                            break;
                        }
                    default:
                        {
                            using (StreamReader sr = new StreamReader(keyValuePair.Key, Encoding.GetEncoding(1251)))
                            {
                                temp = sr.ReadToEnd();
                                sr.Close();
                            }
                            break;
                        }
                }

                using (StreamWriter sw = new StreamWriter(keyValuePair.Key, false, Encoding.UTF8))
                {
                    sw.WriteLine(temp);
                    sw.Close();
                }

                Console.WriteLine($"{keyValuePair.Key} ---> UTF-8");
            }
        }
    }
}
