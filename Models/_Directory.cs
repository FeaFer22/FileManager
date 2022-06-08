using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Models
{
    internal class File
    {
        public string Name { get; set; }
        public DateTime DateChanged { get; set; }
        public string Size { get; set; }

        public string Type { get; set; }
    }
    internal class _Directory
    {
        public string Name { get; set; }
        public DateTime DateChanged { get; set; }
        public ICollection<File> Files { get; set; }
    }
}
