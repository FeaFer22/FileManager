using System;
using System.IO.Packaging;

namespace FileManager.Models
{
    interface IItemInfo
    {
        public string itemName { get; set; }
        public DateTime itemDateChanged { get; set; }
        public string itemSize { get; set; }
        public string itemType { get; set; }
        public string itemPath { get; set; }
    }
}
