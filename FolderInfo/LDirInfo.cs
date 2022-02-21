using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderInfo
{
    public class LDirInfo
    {
        public string ScanDirPath { get; set; }
        public string[] RestrictedDirForScan { get; set; }
        public long TotalDirectories { get; set; }
        public long TotalDirectoriesContainsFiles { get; set; }
        public long TotalFiles { get; set; }
        public long TotalLines { get; set; }
        public string TotalLinesByCulture { get; set; }
        public double? AverageNumberOfLinesPerFile { get; set; }
        public IList<LDirectory> Directories { get; set; } = new List<LDirectory>();
    }
}
