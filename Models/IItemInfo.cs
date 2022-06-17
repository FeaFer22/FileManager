using System;
using System.Collections.ObjectModel;
using System.IO.Packaging;

namespace FileManager.Models
{
    internal interface IItemInfo
    {
        public string itemName { get; set; }
        public DateTime itemDateChanged { get; set; }
        public ObservableCollection<string> itemSize { get; set; }
        public string itemType { get; set; }
        public string itemPath { get; set; }
    }
}
