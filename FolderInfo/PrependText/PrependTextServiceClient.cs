/*
 * Author: Rohit Jadhav
 * Date: 3/5/2022 4:36:06 PM
 * Comments: 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderInfo.PrependText
{
    public class PrependTextServiceClient
    {
        public static void StartProcess()
        {
            var prependService = new PrependTextService();
            prependService.ConsoleInput();
            prependService.Process();
        }
    }
}
