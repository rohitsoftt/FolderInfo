/*
 * Author: Rohit Jadhav
 * Date: 3/5/2022 3:31:41 PM
 * Comments: 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderInfo
{
    public class ServiceProvider 
    {
        public FileService FileService { get
        {
            return new FileService();
        }}
    }
}
