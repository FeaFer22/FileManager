using System;
using FileManager.Infrastructure.Commands;
using FileManager.Models;
using FileManager.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ThreadState = System.Threading.ThreadState;

namespace FileManager.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /*---------------------------------------------------------------------------------------------------------------*/

        private DirectoryInfo dir;
        private DirectoryInfo[] _dirs;
        private FileInfo[] _files;

        private long catalogSize;

        private object _itemsLock;


        #region Заголовок окна

        private string _title = "Файловый менеджер";
        ///<summary>
        ///Заголовок окна
        ///</summary>
        public string Title
        {
            get => _title;
            set
            {
                Set(ref _title, value);
            }
        }

        #endregion

        #region Путь к объекту

        ///<summary>Путь к объекту</summary>

        private string _pathToItem;
        public string PathToItem
        {
            get => _pathToItem;
            set
            {
                Set(ref _pathToItem, value);
            }
        }

        #endregion

        #region Получение информации о объекте

        ///<summary>Получение информации о объекте</summary>

        private ObservableCollection<Item> _itemsInfo = new ObservableCollection<Item>();
        public ObservableCollection<Item> ItemsInfo
        {
            get => _itemsInfo;
            set
            {
                Set(ref _itemsInfo, value);
            }
        }

        #endregion

        #region Получение выделенного объекта datagrid

        ///<summary>Получение выделенного объекта datagrid</summary>
        public Item SelectedItem { get; set; }

        #endregion

        #region IsDataGridEnabled

        private bool _isDataGridEnabled = true;

        public bool IsDataGridEnabled
        {
            get => _isDataGridEnabled;
            set
            {
                Set(ref _isDataGridEnabled, value);
            }
        }

        #endregion

        #region LoadingStatus

        private string _loadingStatus;

        public string LoadingStatus
        {
            get => _loadingStatus;
            set
            {
                Set(ref _loadingStatus, value);
            }
        }

        #endregion

        /*---------------------------------------------------------------------------------------------------------------*/

        #region Команды

        #region CloseApplicationCommand

        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object obj) => true;

        private void OnCloseApplicationCommandExecuted(object obj)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region UpdateItemsInfoFromPathCommand

        public ICommand UpdateItemsInfoFromPathCommand { get; }

        private bool CanUpdateItemsInfoFromPathCommandExecute(object obj)
        {
            return true;
        }

        private void OnUpdateItemsInfoFromPathCommandExecuted(object obj)
        {
            if (ItemsInfo.Count == 0)
            {
                GetItemsInfoFromPath(_pathToItem);
            }
        }


        #endregion

        #region OpenSelectedItemCommand

        public ICommand OpenSelectedItemCommand { get; }

        private bool CanOpenSelectedItemCommandExecute(object obj)
        {
            return true;
        }

        private void OnOpenSelectedItemCommandExecuted(object obj)
        {
            OpenSelectedItem(SelectedItem);
        }

        #endregion

        #region PathBackCommand

        public ICommand PathBackCommand { get; }

        private bool CanPathBackCommandExecute(object obj) => true;

        private void OnPathBackCommandExecuted(object obj)
        {
            PathBack();
        }

        #endregion

        #endregion

        /*---------------------------------------------------------------------------------------------------------------*/
        public MainWindowViewModel()
        {

            #region Команды

            CloseApplicationCommand = new ActionCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            UpdateItemsInfoFromPathCommand = new ActionCommand(OnUpdateItemsInfoFromPathCommandExecuted, CanUpdateItemsInfoFromPathCommandExecute);

            OpenSelectedItemCommand = new ActionCommand(OnOpenSelectedItemCommandExecuted, CanOpenSelectedItemCommandExecute);

            PathBackCommand = new ActionCommand(OnPathBackCommandExecuted, CanPathBackCommandExecute);

            #endregion

            _itemsLock = new object();

            _itemsInfo = new ObservableCollection<Item>();

            BindingOperations.EnableCollectionSynchronization(_itemsInfo, _itemsLock);
            
            GetLogicalDrivesInfo();
        }
        /*---------------------------------------------------------------------------------------------------------------*/

        #region Методы

        #region GetLogicalDrivesInfo

        public void GetLogicalDrivesInfo()
        {
            Item item;
            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {
                item = new Item
                {
                    itemName = logicalDrive,
                    itemType = "Локальный диск",
                    itemDateChanged = DateTime.Today,
                    itemPath = logicalDrive
                };
                _itemsInfo.Add(item);
            }
        }

        #endregion

        #region GetItemsInfoFromPath

        public void GetItemsInfoFromPath(string path)
        {
            Thread thread;
            try
            {
                _itemsInfo.Clear();
                dir = new DirectoryInfo(path);
                _dirs = dir.GetDirectories();
                _files = dir.GetFiles();

                thread = new Thread(() =>
                {
                    string currentSizeOfDirStr;

                    Item item, itemActualSize;

                    lock (_itemsLock)
                    {
                        foreach (FileInfo currentFile in _files)
                        {
                            item = new Item()
                            {
                                itemName = currentFile.Name,
                                itemType = currentFile.Extension,
                                itemDateChanged = currentFile.LastWriteTime,
                                itemSize = SizeConversion(currentFile.Length),
                                itemPath = currentFile.DirectoryName
                            };
                            _itemsInfo.Add(item);
                        }

                        foreach (DirectoryInfo currentDir in _dirs)
                        {
                            currentSizeOfDirStr = "...";

                            item = new Item
                            {
                                itemName = currentDir.Name,
                                itemType = "Папка с файлами",
                                itemDateChanged = currentDir.LastWriteTime,
                                itemSize = currentSizeOfDirStr,
                                itemPath = currentDir.FullName
                            };

                            _itemsInfo.Add(item);

                            itemActualSize = new Item
                            {
                                itemName = currentDir.Name,
                                itemType = "Папка с файлами",
                                itemDateChanged = currentDir.LastWriteTime,
                                itemSize = currentSizeOfDirStr,
                                itemPath = currentDir.FullName
                            };

                            currentSizeOfDirStr = SizeConversion(GetDirSize(currentDir.FullName));
                            
                            
                            itemActualSize.itemSize = currentSizeOfDirStr;
                            try
                            {
                                _itemsInfo[_itemsInfo.IndexOf(item)] = itemActualSize;
                            }
                            catch (ArgumentOutOfRangeException)
                            {

                            }
                        }
                    }
                });
                thread.IsBackground = true;
                thread.Start();

                Thread thread2 = new Thread(() =>
                {
                    while (thread.IsAlive == true)
                    {
                        IsDataGridEnabled = false;
                        LoadingStatus = "Загрузка...";
                    }
                    IsDataGridEnabled = true;
                    LoadingStatus = "Завершено!";
                });
                thread2.Start();
            }
            catch (DirectoryNotFoundException directoryNotFound)
            {
                MessageBox.Show(directoryNotFound.Message, _title);
                GetLogicalDrivesInfo();
            }
            catch (IOException ioException)
            {
                MessageBox.Show(ioException.Message + "(IOException)", _title);
            }
            catch (ArgumentException)
            {
                GetLogicalDrivesInfo();
            }
            catch (UnauthorizedAccessException unauthorizedAccess)
            {
                MessageBox.Show(unauthorizedAccess.Message, _title);
                PathBack();
            }
        }

        #endregion

        #region OpenSelectedItem

        private void OpenSelectedItem(object parameter)
        {
            if (parameter is Item item)
            {

                try
                {
                    PathToItem = item.itemPath;
                    GetItemsInfoFromPath(_pathToItem);

                    if (item.itemType != "Папка с файлами" && item.itemType != "Локальный диск")
                    {
                        _pathToItem = _pathToItem + "\\" + item.itemName;
                        Process.Start(new ProcessStartInfo(_pathToItem) { UseShellExecute = true });
                    }
                    else
                    {
                        OpenSelectedItem(SelectedItem);
                    }
                }
                catch (Win32Exception exception)
                {
                    MessageBox.Show(exception.Message, _title);
                }
                   
            }
        }


        #endregion

        #region PathBack

        public string PathBack()
        {
            try
            {
                if (_pathToItem != null)
                {
                    if (_pathToItem[_pathToItem.Length - 1] == '\\' && _pathToItem[_pathToItem.Length - 2] != ':')
                    {
                        _pathToItem = _pathToItem.Remove(_pathToItem.Length - 1, 1);
                        PathToItem = PathToItem.Remove(PathToItem.Length - 1, 1);
                        while (_pathToItem[_pathToItem.Length - 1] == '\\')
                        {
                            _pathToItem = _pathToItem.Remove(_pathToItem.Length - 2, 1);
                            PathToItem = PathToItem.Remove(PathToItem.Length - 2, 1);
                        }
                    }
                    else if (_pathToItem[_pathToItem.Length - 1] != '\\')
                    {
                        while (_pathToItem[_pathToItem.Length - 1] != '\\')
                        {
                            _pathToItem = _pathToItem.Remove(_pathToItem.Length - 1, 1);
                            PathToItem = PathToItem.Remove(PathToItem.Length - 1, 1);
                        }

                        if (_pathToItem[_pathToItem.Length - 1] == '\\' && _pathToItem[_pathToItem.Length - 2] != ':')
                        {
                            _pathToItem = _pathToItem.Remove(_pathToItem.Length - 1, 1);

                        }
                        GetItemsInfoFromPath(_pathToItem);
                    }
                    else
                    {
                        _itemsInfo.Clear();
                        GetLogicalDrivesInfo();
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                GetLogicalDrivesInfo();
            }

            return _pathToItem;
        }

        #endregion
        
        #region GetDirSize

        public long GetDirSize(string path)
        {
            try
            {
                catalogSize = 0;
                dir = new DirectoryInfo(path);
                _dirs = dir.GetDirectories();
                _files = dir.GetFiles();

                foreach (FileInfo currentFile in _files)
                {
                    catalogSize = catalogSize + currentFile.Length;
                }

                foreach (DirectoryInfo currentDir in _dirs)
                {
                    catalogSize = catalogSize + GetDirSize(currentDir.FullName);
                }

            }
            catch (UnauthorizedAccessException)
            {

            }

            return catalogSize;
        }

        #endregion

        #region SizeConversion

        public string SizeConversion(long catalogSize)
        {
            if (catalogSize >= 1073741824.0)
            {
                return String.Format("{0:##.##}", catalogSize / 1073741824.0) + " Гб";
            }
            else if (catalogSize >= 1048576.0)
            {
                return String.Format("{0:##.##}", catalogSize / 1048576.0) + " Мб";
            }
            else if (catalogSize >= 1024.0)
            {
                return String.Format("{0:##.##}", catalogSize / 1024.0) + " Кб";
            }
            else if (catalogSize > 0 && catalogSize < 1024.0)
            {
                return catalogSize.ToString() + " б";
            }

            return catalogSize.ToString() + " б";
        }

        #endregion

        #endregion
        /*---------------------------------------------------------------------------------------------------------------*/
    }

}
