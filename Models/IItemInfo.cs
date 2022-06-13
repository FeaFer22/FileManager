﻿using System;
using System.IO.Packaging;

namespace FileManager.Models
{
    internal interface IItemInfo
    {
        public string ItemName { get; set; }
        public DateTime ItemDateChanged { get; set; }
        public long ItemSize { get; set; }
        public string ItemType { get; set; }
        public string ItemPath { get; set; }
    }
}
