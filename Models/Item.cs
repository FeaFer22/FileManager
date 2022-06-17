using System;
using System.Collections.ObjectModel;

namespace FileManager.Models
{
    internal class Item : IItemInfo
    {
        public string itemName { get; set; }
        public DateTime itemDateChanged { get; set; }
        public ObservableCollection<string> itemSize { get ; set; }
        public string itemType { get; set; }
        public string itemPath { get; set; }
    }
}
