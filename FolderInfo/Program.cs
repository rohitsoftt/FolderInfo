using FolderInfo;
using Newtonsoft.Json;
using System.Globalization;

public class Program
{
    static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
    static string FileName;
    static LDirInfo lDirInfo = new LDirInfo();
    static List<LDirectory> lDirectories = new List<LDirectory>();
    static string[]? restrictedDir = Array.Empty<string>();
    static void Main()
    {
        #region Commented
        /*
        //Start with drives if you have to search the entire computer.
        string[] drives = System.Environment.GetLogicalDrives();
        foreach (string dr in drives)
        {
            System.IO.DriveInfo di = new System.IO.DriveInfo(dr);

            // Here we skip the drive if it is not ready to be read. This
            // is not necessarily the appropriate action in all scenarios.
            if (!di.IsReady)
            {
                Console.WriteLine("The drive {0} could not be read", di.Name);
                continue;
            }
            System.IO.DirectoryInfo rootDir = di.RootDirectory;
            WalkDirectoryTree(rootDir);
        }*/
        #endregion
        bool Isanother = false;
        do
        {
            try
            {

                Console.WriteLine("=========START===============");
                Console.WriteLine("Enter the path to scan :: ");
                var scanPath = Console.ReadLine();
                lDirInfo.ScanDirPath = scanPath;
                Console.Write("Restricted dir for scan :: ");
                var rdString = Console.ReadLine();
                if (!String.IsNullOrEmpty(rdString))
                    restrictedDir = rdString.Trim().Split((' '));
                else
                {
                    restrictedDir = Array.Empty<string>();
                }
                lDirInfo.RestrictedDirForScan = restrictedDir;
                Console.WriteLine("Enter the file name to scan :: ");
                FileName = Console.ReadLine();
                Console.Write("Save output file(Y/N):: ");
                var saveFile = Console.ReadLine() ?? "N";
                System.IO.DirectoryInfo rootDir = new DirectoryInfo(scanPath);
                Console.WriteLine("Doing analysis ...");

                WalkDirectoryTree(rootDir);

                lDirInfo.TotalDirectories = lDirectories.Count;

                lDirectories = lDirectories.Where(i => i.LFiles.Count > 0).ToList();
                var totalFiles = lDirectories.SelectMany(lDirectory => lDirectory.LFiles).Count();
                var totalLines = lDirectories.SelectMany(lDirectory => lDirectory.LFiles).Select(lFile => lFile.NumberOfLines).Sum();
                // Write out all the files that could not be processed.
                Console.WriteLine("Files with restricted access: ");
                foreach (string s in log)
                {
                    Console.WriteLine(s);
                }
                Console.WriteLine($"Total {FileName} files :: {totalFiles}");
                Console.WriteLine($"Total Lines :: {totalLines}");
                Console.WriteLine("=================END====================");
                if (saveFile.Trim() == "Y")
                {
                    lDirInfo.TotalFiles = totalFiles;
                    lDirInfo.TotalDirectoriesContainsFiles = lDirectories.Count;
                    lDirInfo.Directories = lDirectories;
                    lDirInfo.TotalLines = totalLines;
                    lDirInfo.TotalLinesByCulture = totalLines.ToString("C", CultureInfo.CreateSpecificCulture("en-IN")).Split(' ')[1].Split('.')[0];
                    if (totalFiles > 0)
                        lDirInfo.AverageNumberOfLinesPerFile = totalLines / totalFiles;

                    SaveInFile(lDirInfo);
                }
                Console.Write("Do you have anoter directory to scan(y/n) :: ");
                Isanother = (Console.ReadLine() ?? "").Trim() == "y";
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Please try again ...");
                Isanother=true;
            }
           
        }while (Isanother);
    }
    static void WalkDirectoryTree(System.IO.DirectoryInfo root)
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
            files = root.GetFiles(FileName);
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

        catch (System.IO.DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        if (files != null)
        {
            var lfiles = new List<LFile>();
            foreach (System.IO.FileInfo fi in files)
            {
                // In this example, we only access the existing FileInfo object. If we
                // want to open, delete or modify the file, then
                // a try-catch block is required here to handle the case
                // where the file has been deleted since the call to TraverseTree().
                //Console.WriteLine(fi.FullName);

                lfiles.Add(new LFile
                {
                    ModifiedDateTime = fi.CreationTime,
                    Name = fi.Name,
                    NumberOfLines = File.ReadAllLines(fi.FullName).Count(),
                });
            }
            lDirectory.LFiles = lfiles;
            lDirectories.Add(lDirectory);
            // Now find all the subdirectories under this directory.
            subDirs = root.GetDirectories().Where(i => !restrictedDir.Contains(i.Name)).ToArray();
            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                // Resursive call for each subdirectory.
                WalkDirectoryTree(dirInfo);
            }
        }
    }

    static void SaveInFile(object obj)
    {
        var jString = JsonConvert.SerializeObject(obj, Formatting.Indented);
        var fileDirname = "MyFiles";
        if (!Directory.Exists(fileDirname))
        {
            Directory.CreateDirectory(fileDirname);
        }
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), $@"{fileDirname}\{DateTime.Now.ToString("dd-mm-yyyy_hhmmss")}{FileName.Replace("*", "_all-").Replace(".", "")}.json");
        File.WriteAllText(filePath, jString);
        Console.WriteLine("File Saved at location :: " + filePath);
    }
}