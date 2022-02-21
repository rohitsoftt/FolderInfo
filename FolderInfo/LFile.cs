using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderInfo
{
    public record LFile
    {
        public string Name { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public long NumberOfLines { get; set; }
    }
}
