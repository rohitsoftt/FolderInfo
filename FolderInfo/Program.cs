using FolderInfo;
using FolderInfo.PrependText;
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
        Console.WriteLine($"Please select a service to use :: {Environment.NewLine}");
        Console.WriteLine("1. Folder file scanning (will give you count of files, lines, directories)");
        Console.WriteLine("2. Prepend text into file text service");
        var input = Console.ReadLine();
        switch (input)
        {
            case "1":
                FileServiceClient.StartProcess();
                break;
            case"2":
                PrependTextServiceClient.StartProcess();
                break;
            default:
                Console.WriteLine("Invalid Input...");
                break;
        }
        Console.Write("Do you want to use services again (y/n)? :: ");
        Isanother = (Console.ReadLine() ?? "").Trim() == "y";
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.ToString());
        Console.WriteLine("Please try again ...");
        Isanother=true;
    }
           
}while (Isanother);