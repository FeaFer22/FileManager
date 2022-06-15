using System;

namespace FileManager.Models
{
    internal class Item : IItemInfo
    {
        public string itemName { get; set; }
        public DateTime itemDateChanged { get; set; }
        public long itemSize { get; set; }
        public string itemType { get; set; }
        public string itemPath { get; set; }
    }
}
