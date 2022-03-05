/*
 * Author: Rohit Jadhav
 * Date: 05-Mar-2022 06:25:57 PM IND
 * Comments: 
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderInfo
{
    
    public class FileService
    {
        private readonly LDirInfo lDirInfo;
        private readonly StringCollection log;
        private string saveFile = "N";
        public string SaveFile => saveFile;
        public FileService()
        {
            lDirInfo = new LDirInfo();
            log = new StringCollection();
        }
        public void TakeConsoleInput()
        {
            Console.WriteLine("=========START===============");
            Console.Write("Enter the path to scan :: ");
            lDirInfo.ScanDirPath = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Restricted dir for scan :: ");
            var rdString = Console.ReadLine();
            if (!String.IsNullOrEmpty(rdString))
                lDirInfo.RestrictedDirForScan = rdString.Trim().Split((' '));
            else
            {
                lDirInfo.RestrictedDirForScan = Array.Empty<string>();
            }
            Console.WriteLine();
            Console.Write("Enter the file name to scan :: ");
            lDirInfo.ScanFileName = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Save output file(Y/N):: ");
            saveFile = Console.ReadLine() ?? "N";
            Console.WriteLine("Doing analysis ...");
        }
        public void CalculateResult()
        {
            WalkDirectoryTree(new DirectoryInfo(lDirInfo.ScanDirPath));

            lDirInfo.TotalDirectories = lDirInfo.Directories.Count;
            lDirInfo.Directories = lDirInfo.Directories.Where(i => i.LFiles.Count > 0).ToList();
            var totalFiles = lDirInfo.Directories.SelectMany(lDirectory => lDirectory.LFiles).Count();
            var totalLines = lDirInfo.Directories.SelectMany(lDirectory => lDirectory.LFiles).Select(lFile => lFile.NumberOfLines).Sum();

            lDirInfo.TotalFiles = totalFiles;
            lDirInfo.TotalDirectoriesContainsFiles = lDirInfo.Directories.Count;
            lDirInfo.TotalLines = totalLines;
            lDirInfo.TotalLinesByCulture = totalLines.ToString("C", CultureInfo.CreateSpecificCulture("en-IN")).Split(' ')[1].Split('.')[0];
            if (totalFiles > 0)
                lDirInfo.AverageNumberOfLinesPerFile = totalLines / totalFiles;
        }
        public void PrintResults()
        {
            Console.WriteLine("Files with restricted access: ");
            foreach (string s in log)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine($"Total {lDirInfo.ScanFileName} files :: {lDirInfo.TotalFiles}");
            Console.WriteLine($"Total Lines :: {lDirInfo.TotalLines}");
            Console.WriteLine("=================END====================");
        }
        private void WalkDirectoryTree(DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            LDirectory lDirectory = new LDirectory();
            lDirectory.FullPath = root.FullName;
            lDirectory.Name = root.Name;
            lDirectory.LastModified = root.LastWriteTime;
            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles(lDirInfo.ScanFileName);
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
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                var lfiles = new List<LFile>();
                foreach (System.IO.FileInfo fi in files)
                {
                    lfiles.Add(new LFile
                    {
                        ModifiedDateTime = fi.CreationTime,
                        Name = fi.Name,
                        NumberOfLines = File.ReadAllLines(fi.FullName).Count(),
                    });
                }
                lDirectory.LFiles = lfiles;
                lDirInfo.Directories.Add(lDirectory);
                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories().Where(i => !lDirInfo.RestrictedDirForScan.Contains(i.Name)).ToArray();
                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                }
            }
        }

        /// <summary>
        /// Save File
        /// </summary>
        /// <returns>Path of the file</returns>
        public string SaveInFile()
        {
            var jString = JsonConvert.SerializeObject(lDirInfo, Formatting.Indented);
            var fileDirname = "MyFiles";
            if (!Directory.Exists(fileDirname))
            {
                Directory.CreateDirectory(fileDirname);
            }
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $@"{fileDirname}\{DateTime.Now.ToString("dd-mm-yyyy_hhmmss")}{lDirInfo.ScanFileName.Replace("*", "_all-").Replace(".", "")}.json");
            File.WriteAllText(filePath, jString);
            return filePath;
        }
    }
}
