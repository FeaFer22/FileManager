using System;
using FileManager.Infrastructure.Commands;
using FileManager.Models;
using FileManager.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using FileManager.Infrastructure.Commands.Base;

namespace FileManager.ViewModels
{
    internal class MainWindowViewModel : ViewModel
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
            get =>_pathToItem;
            set
            {
                Set(ref _pathToItem, value);
            }
        }

        #endregion

        #region Получение информации о объекте

        ///<summary>Получение информации о объекте</summary>

        private ObservableCollection<Item> _itemsInfo;
        public ObservableCollection<Item> ItemsInfo
        {
            get => _itemsInfo;
            set
            {
                Set(ref _itemsInfo, value);
            }
        }

        #endregion

        #region Размер предмета

        //private ObservableCollection<string> _itemSize;
        //public ObservableCollection<string> itemSize
        //{
        //    get => _itemSize;
        //    set
        //    {
        //        Set(ref _itemSize, value);
        //    }
        //}

        #endregion

        #region Получение выделенного объекта datagrid

        ///<summary>Получение выделенного объекта datagrid</summary>
        public Item SelectedItem { get; set; }

        #endregion


        /*---------------------------------------------------------------------------------------------------------------*/

        #region Команды

        #region CloseApplicationCommand

        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object obj) => true;

        private void OnCloseApplicationCommandExecuted(object obj) => Application.Current.Shutdown();

        #endregion

        #region UpdateItemsInfoFromPathCommand

        public ICommand UpdateItemsInfoFromPathCommand { get; }

        public bool CanUpdateItemsInfoFromPathCommandExecute(object parameter) => true;

        public void OnUpdateItemsInfoFromPathCommandExecuted(object parameter)
        {
            GetItemsInfoFromPath(_pathToItem);
        }


        #endregion

        #region OpenSelectedItemCommand

        public ICommand OpenSelectedItemCommand { get; }

        private bool CanOpenSelectedItemCommandExecute(object parameter) => true;

        private void OnOpenSelectedItemCommandExecuted(object parameter)
        {
            OpenSelectedItem(SelectedItem);
        }

        #endregion

        #region PathBackCommand

        public ICommand PathBackCommand { get; }

        private bool CanPathBackCommandExecute(object parameter) => true;

        private void OnPathBackCommandExecuted(object parameter)
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
            lock (_itemsLock)
            {
                foreach (var logicalDrive in Directory.GetLogicalDrives())
                {
                    Item item = new Item
                    {
                        itemName = logicalDrive,
                        itemType = "Локальный диск",
                        itemDateChanged = DateTime.Today,
                        itemPath = logicalDrive
                    };
                    _itemsInfo.Add(item);
                }
            }
        }

        #endregion

        #region GetItemsInfoFromPath

        public void GetItemsInfoFromPath(string path)
        {
            try
            {
                _itemsInfo.Clear();
                dir = new DirectoryInfo(path);
                SetDirs(dir);
                SetFiles(dir);
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

        #region OpenSelectedItem

        private void OpenSelectedItem(object parameter)
        {
            if (parameter is Item item)
            {
                try
                {
                    _pathToItem = item.itemPath;
                    GetItemsInfoFromPath(_pathToItem);

                    if (item.itemType != "Папка с файлами" && item.itemType != "Локальный диск")
                    {
                        _pathToItem = _pathToItem + "\\" + item.itemName;
                        Process.Start(new ProcessStartInfo(_pathToItem) {UseShellExecute = true});
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

        public void PathBack()
        {
            try
            {
                if (_pathToItem[_pathToItem.Length - 1] == '\\' && _pathToItem[_pathToItem.Length - 2] != ':')
                {
                    _pathToItem = _pathToItem.Remove(_pathToItem.Length - 1, 1);
                    while (_pathToItem[_pathToItem.Length - 1] == '\\')
                    {
                        _pathToItem = _pathToItem.Remove(_pathToItem.Length - 2, 1);
                    }
                }
                else if (_pathToItem[_pathToItem.Length - 1] != '\\')
                {
                    while (_pathToItem[_pathToItem.Length - 1] != '\\')
                    {
                        _pathToItem = _pathToItem.Remove(_pathToItem.Length - 1, 1);
                    }

                    if (_pathToItem[_pathToItem.Length - 1] == '\\' && _pathToItem[_pathToItem.Length - 2] != ':')
                    {
                        _pathToItem = _pathToItem.Remove(_pathToItem.Length - 1, 1);
                    }
                    else
                    {
                        GetLogicalDrivesInfo();
                    }
                }
                GetItemsInfoFromPath(_pathToItem);
            }
            catch (IndexOutOfRangeException)
            {
                GetLogicalDrivesInfo();
            }
        }

        #endregion

        #region SetSize
        public async void SetSize(Item item)
        {
            await Task.Run(() =>
            {
                item.itemSize[0] = Size(GetDirSize(_pathToItem));
            });
        }

        #endregion

        #region Size

        public string Size(long catalogSize)
        {
            if (catalogSize < 1024.0)
            {
                return catalogSize.ToString() + " б";
            }
            else if ((catalogSize / 1024) < 1024)
            {
                return (catalogSize / 1024).ToString() + " Кб";
            }
            else if ((catalogSize / 1024 / 1024) < 1024)
            {
                return (catalogSize / 1024 / 1024).ToString() + " Мб";
            }
            else if ((catalogSize / 1024 / 1024 / 1024) < 1024)
            {
                return (catalogSize / 1024 / 1024 / 1024).ToString() + " Гб";
            }
            else
            {
                return (catalogSize / 1024 / 1024 / 1024 / 1024).ToString() + " Тб";
            }
        }

        #endregion

        #region SetDirs

        public async void SetDirs(DirectoryInfo dir)
        {
            try
            {
                await Task.Run(() =>
                {
                    _dirs = dir.GetDirectories();

                    lock (_itemsLock)
                    {
                        foreach (DirectoryInfo currentDir in _dirs)
                        {
                            var itemSizee = new ObservableCollection<string>();

                            //await Task.Delay(10);

                            itemSizee.Add("...");


                            // Once locked, you can manipulate the collection safely from another thread

                            Item item = new Item
                            {
                                itemName = currentDir.Name,
                                itemType = "Папка с файлами",
                                itemDateChanged = currentDir.LastWriteTime,
                                itemSize = itemSizee,
                                itemPath = currentDir.FullName
                            };


                            _itemsInfo.Add(item);
                        }

                        foreach (var item in _itemsInfo)
                        {
                            //await Task.Delay(10);

                            if (item.itemType == "Папка с файлами")
                            {
                                SetSize(item);
                            }
                        }
                    }
                });
            }
            catch (InvalidOperationException)
            {

            }
        }

        #endregion

        #region SetFiles

        public async void SetFiles(DirectoryInfo dir)
        {
            await Task.Run(() =>
            {
                _files = dir.GetFiles();
                lock (_itemsLock)
                {
                    foreach (FileInfo currentFile in _files)
                    {
                        var itemSizee = new ObservableCollection<string>();

                        //await Task.Delay(10);

                        itemSizee.Add(Size(currentFile.Length));



                        Item item = new Item
                        {
                            itemName = currentFile.Name,
                            itemType = currentFile.Extension,
                            itemDateChanged = currentFile.LastWriteTime,
                            itemSize = itemSizee,
                            itemPath = currentFile.DirectoryName

                        };
                        _itemsInfo.Add(item);
                    }
                }
            });
        }

        #endregion

        #endregion
        /*---------------------------------------------------------------------------------------------------------------*/
    }
}
