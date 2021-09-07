using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DarkDownloader
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<string> downloadedURLs = new List<string>();
            string programBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string lastPathFile = Path.Combine(programBaseDirectory, "LastPath.txt");
            Console.WriteLine(lastPathFile);
            string lastPath = "";
            if (File.Exists(lastPathFile))
            {
                lastPath = File.ReadAllText(lastPathFile);
            }
            else
            {
                File.Create(lastPathFile).Dispose();
            }
            
            Console.WriteLine($"Last download folder: {lastPath}");
            Console.WriteLine("Edit or delete LastPath.txt in the program folder to reset.");
            string downloadFolderPath;
            if (Directory.Exists(lastPath))
            {
                downloadFolderPath = lastPath;
                goto MainLoop;
            }

        OpenFile:

            var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK)
            {
                downloadFolderPath = fbd.SelectedPath;
                File.WriteAllText(lastPathFile, fbd.SelectedPath);
            }
            else
            {
                goto OpenFile;
            }
            MainLoop:
            while (true)
            {
                if (downloadedURLs.Count>1)
                {
                    downloadedURLs.RemoveAt(0);
                }
                if (Clipboard.ContainsText())
                {
                    
                    string dlLink = Clipboard.GetText();
                    if (IsValidURL(dlLink) && !downloadedURLs.Contains(dlLink))
                    {
                        string fileName = GetFileNameFromUrl(dlLink);
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        string fileExtension = Path.GetExtension(fileName); 
                        while (File.Exists(Path.Combine(downloadFolderPath, fileNameWithoutExtension + fileExtension)))
                        {
                            fileNameWithoutExtension += "_R";
                        }
                        string dlPath = Path.Combine(downloadFolderPath, fileNameWithoutExtension + fileExtension);
                        Console.WriteLine($"File is downloading, please wait warmly.\n{dlPath}");

                        using (var client = new WebClient())
                        {
                            client.DownloadFile(dlLink, dlPath);
                        }
                        Console.WriteLine($"Your file has arrived in: {downloadFolderPath}");
                        downloadedURLs.Add(dlLink);
                    }
                }
                Thread.Sleep(20);
            }
        }
        static string GetFileNameFromUrl(string url)
        {
            Uri SomeBaseUri = new Uri("http://canbeanything");
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                uri = new Uri(SomeBaseUri, url);

            return Path.GetFileName(uri.LocalPath);
        }
        static bool IsValidURL(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                return false;
            else
            {
                if (Path.HasExtension(Path.GetFileName(uri.LocalPath)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
