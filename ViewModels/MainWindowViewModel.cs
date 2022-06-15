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
using System.Windows.Input;

namespace FileManager.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        /*---------------------------------------------------------------------------------------------------------------*/

        private DirectoryInfo dir;
        private DirectoryInfo[] _dirs;
        private FileInfo[] _files;

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


        /*---------------------------------------------------------------------------------------------------------------*/

        #region Команды

        #region CloseApplicationCommand

        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object obj) => true;

        private void OnCloseApplicationCommandExecuted(object obj) => Application.Current.Shutdown();

        #endregion

        #region UpdateItemsInfoFromPathCommand

        public ICommand UpdateItemsInfoFromPathCommand { get; }

        private bool CanUpdateItemsInfoFromPathCommandExecute(object obj) => true;

        private void OnUpdateItemsInfoFromPathCommandExecuted(object obj)
        {
            GetItemsInfoFromPath(_pathToItem);
        }


        #endregion

        #region OpenSelectedItemCommand

        public ICommand OpenSelectedItemCommand { get; }

        private bool CanOpenSelectedItemCommandExecute(object obj) => true;

        private void OnOpenSelectedItemCommandExecuted(object obj)
        {
            OpenSelectedItem(SelectedItem);
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

            #endregion

            GetLogicalDrivesInfo();
        }
        /*---------------------------------------------------------------------------------------------------------------*/

        #region Методы

        #region GetLogicalDrivesInfo

        public void GetLogicalDrivesInfo()
        {
            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {
                _itemsInfo.Add(new Item
                {
                    itemName = logicalDrive,
                    itemType = "Локальный диск",
                    itemDateChanged = DateTime.Today,
                    itemPath = logicalDrive
                });
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
                _dirs = dir.GetDirectories();
                _files = dir.GetFiles();

                foreach (FileInfo currentFile in _files)
                {
                    _itemsInfo.Add(new Item
                    {
                        itemName = currentFile.Name,
                        itemType = currentFile.Extension,
                        itemDateChanged = currentFile.LastWriteTime,
                        itemSize = currentFile.Length.ToString(),
                        itemPath = currentFile.DirectoryName
                    });
                }

                foreach (DirectoryInfo currentDir in _dirs)
                {
                    _itemsInfo.Add(new Item
                    {
                        itemName = currentDir.Name,
                        itemType = "Папка с файлами",
                        itemDateChanged = currentDir.LastWriteTime,
                        itemSize = " ",
                        itemPath = currentDir.FullName
                    });
                }
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
                GetLogicalDrivesInfo();
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


        #endregion
        /*---------------------------------------------------------------------------------------------------------------*/
    }
}
