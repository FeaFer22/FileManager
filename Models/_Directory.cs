using System;

namespace FileManager.Models
{
    internal class _File : IItemInfo
    {
        public string Name { get; set; }
        public DateTime DateChanged { get; set; }
        public long Size { get; set; }
        public string Type { get; set; }
    }
    internal class _Directory : IItemInfo
    { 
        public string Name { get; set; }
        public DateTime DateChanged { get; set; }
        public long Size { get; set; }
        public string Type { get; set; }
    }
}
