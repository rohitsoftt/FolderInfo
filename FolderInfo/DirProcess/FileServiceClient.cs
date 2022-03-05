/*
 * Author: Rohit Jadhav
 * Date: 3/5/2022 3:36:23 PM
 * Comments: 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderInfo
{
    public class FileServiceClient
    {
        public static void StartProcess()
        {
            var fileService = new FileService();
            fileService.TakeConsoleInput();
            fileService.CalculateResult();
            fileService.PrintResults();
            if (fileService.SaveFile.Trim() == "Y")
            {
                Console.WriteLine("File Saved at location :: " + fileService.SaveInFile());
            }
        }
    }
}
