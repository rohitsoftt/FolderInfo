using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderInfo
{
    public record LDirectory
    {
        public string Name { get; set; }
        public  string FullPath { get; set; }
        public DateTime LastModified { get; set; }
        public IList<LFile> LFiles { get; set; } = new List<LFile>();
    }
}
