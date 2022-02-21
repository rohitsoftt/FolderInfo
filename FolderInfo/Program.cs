using FolderInfo;
using Newtonsoft.Json;
using System.Globalization;


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
        var fileService = new FileService();
        fileService.TakeConsoleInput();
        fileService.CalculateResult();
        fileService.PrintResults();
        if (fileService.SaveFile.Trim() == "Y")
        {
            Console.WriteLine("File Saved at location :: " + fileService.SaveInFile()); 
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