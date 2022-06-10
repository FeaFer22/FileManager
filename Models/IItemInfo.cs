using System;

namespace FileManager.Models
{
    internal interface IItemInfo
    {
        public string Name { get; set; }
        public DateTime DateChanged { get; set; }
        public long Size { get; set; }
        public string Type { get; set; }
    }
}
