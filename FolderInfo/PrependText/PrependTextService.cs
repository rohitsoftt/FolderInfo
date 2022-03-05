/*
 * Author: Rohit Jadhav
 * Date: 3/5/2022 4:26:56 PM
 * Comments: 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderInfo.PrependText
{
    public class PrependTextService
    {
        private DirectoryInfo _directoryInfo;
        private string _fileFormate;
        private string? _prependTextFilePath;
        readonly List<string> log = new List<string>();
        string []RestrictedDirForScan = Array.Empty<string>();
        List<FileInfo> _filesToPrepend = new List<FileInfo>();
        string? _startTextToSkipPrepend;
        private string _defaultPrependFilePath => Path.Combine(Directory.GetCurrentDirectory(), "prepend.txt");
        public void ConsoleInput()
        {
            Console.Write("Please enter directory location to prepend the text :: ");
            _directoryInfo = new DirectoryInfo(Console.ReadLine());
            Console.Write("Please enter file format to prepend the text(defalt *.cs) :: ");
            _fileFormate = Console.ReadLine();
            if (string.IsNullOrEmpty(_fileFormate))
            {
                _fileFormate = "*.cs";
            }
            Console.Write("Please enter prepend text file path(default : {current exe path}\\prepend.text ");
            _prependTextFilePath = Console.ReadLine();
            if (string.IsNullOrEmpty(_prependTextFilePath))
            {
                _prependTextFilePath = _defaultPrependFilePath;
            }
            Console.Write("Restricted Directory Names for scan(separate with space) :: ");
            RestrictedDirForScan = Console.ReadLine()?.Split(' ') ?? Array.Empty<string>();
            Console.Write("Start of the file text to avoid prepend operation (default : /*):: ");
            _startTextToSkipPrepend = Console.ReadLine();
            if (string.IsNullOrEmpty(_startTextToSkipPrepend))
            {
                _startTextToSkipPrepend = "/*";
            }
            //Console.WriteLine($"Info :: {Environment.NewLine} 1.use $CurrentDateTime$ in append text file to update with dateTime in dd-mon-yyyy HH:MM:SS format {Environment.NewLine} 2. use $CountryCode$ in append text file to update with country  ");
        }

        public void Process()
        {
            WalkDirectoryTree(_directoryInfo);
            foreach (var file in _filesToPrepend)
            {
                Console.WriteLine(file.FullName);
                var fileText = File.ReadAllText(file.FullName, Encoding.UTF8);
                if (_startTextToSkipPrepend is null || !fileText.StartsWith(_startTextToSkipPrepend))
                {
                    Console.WriteLine("File to prepend:: " + file.FullName);
                    var prependText = File.ReadAllText(_prependTextFilePath, Encoding.UTF8);
                    prependText = prependText.Replace("$CurrentDateTime$", DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt")).Replace("$CountryCode$", "IND");
                    var newText = prependText + Environment.NewLine + fileText;
                    File.WriteAllText(file.FullName, newText);
                }
                else
                {
                    Console.WriteLine("File to Skip :: " + file.FullName);
                }
            }
        }
        private void WalkDirectoryTree(DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles(_fileFormate);
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                log.Add(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                log.Add(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    _filesToPrepend.Add(fi);
                }
                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories().Where(i => !RestrictedDirForScan.Contains(i.Name)).ToArray();
                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                }
            }
        }

    }
}
