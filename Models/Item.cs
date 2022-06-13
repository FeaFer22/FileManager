using System;

namespace FileManager.Models
{
    internal class Item : IItemInfo
    {
        public string ItemName { get; set; }
        public DateTime ItemDateChanged { get; set; }
        public long ItemSize { get; set; }
        public string ItemType { get; set; }
        public string ItemPath { get; set; }
    }
}
