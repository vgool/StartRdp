using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StartRdp
{
    public class Program
    {
        private static string _downloadFolder = "C:\\Users\\vince\\Downloads\\";

        static void Main(string[] args)
        {
            var rdpFile = GetNewestRdpFile();
            if (string.IsNullOrEmpty(rdpFile))
            {
                Console.WriteLine("Could not find RDP file... please try again when you've downloaded the file");
                return;
            }

            ChangeRdpFile(rdpFile);
            StartRdpConnection(rdpFile);
        }

        private static string GetNewestRdpFile()
        {
            var info = new DirectoryInfo(_downloadFolder);

            return info.GetFiles("*.rdp").OrderByDescending(p => p.CreationTime).FirstOrDefault()?.FullName;
        }

        private static void ChangeRdpFile(string rdpFile)
        {
            // Read lines from the (original) RDP file
            var allLines = File.ReadAllLines(rdpFile);

            using StreamWriter writer = new StreamWriter(rdpFile);
            var isConnectionbarLineFound = false;

            foreach (var line in allLines)
            {
                if (line.Contains("use multimon"))
                {
                    writer.WriteLine("use multimon:i:1");
                }
                else if (line.Contains("displayconnectionbar"))
                {
                    writer.WriteLine("displayconnectionbar:i:0");
                    isConnectionbarLineFound = true;
                }
                else
                {
                    writer.WriteLine(line);
                }
            }

            if (!isConnectionbarLineFound)
            {
                writer.WriteLine("displayconnectionbar:i:0");
            }

            writer.WriteLine("selectedmonitors:s:3,4");

            writer.Close();
        }

        private static void StartRdpConnection(string rdpFile)
        {
            Process.Start("mstsc", "\"" + rdpFile + "\"");
        }
    }
}
