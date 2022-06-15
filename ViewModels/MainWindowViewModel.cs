using System;
using FileManager.Infrastructure.Commands;
using FileManager.Models;
using FileManager.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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
        private ObservableCollection<Item> _itemsInfo;

        private double catalogSize = 0;

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
        public ObservableCollection<Item> ItemsInfo
        {
            get => _itemsInfo;
            set
            {
                Set(ref _itemsInfo, value);
            }
        }

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
            _itemsInfo.Clear();
            GetItemsInfoFromPath();
        }


        #endregion

        #endregion

        /*---------------------------------------------------------------------------------------------------------------*/
        public MainWindowViewModel()
        {

            #region Команды

            CloseApplicationCommand = new ActionCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            UpdateItemsInfoFromPathCommand = new ActionCommand(OnUpdateItemsInfoFromPathCommandExecuted, CanUpdateItemsInfoFromPathCommandExecute);

            #endregion

            _itemsInfo = new ObservableCollection<Item>();

            GetLogicalDrivesInfo();
        }
        /*---------------------------------------------------------------------------------------------------------------*/

        #region Методы

        #region GetLogicalDrivesInfo

        public string GetLogicalDrivesInfo()
        {
            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {
                _pathToItem = logicalDrive;
                _itemsInfo.Add(new Item
                {
                    itemName = logicalDrive,
                    itemType = "Локальный диск",
                    itemDateChanged = DateTime.Now,
                    itemPath = logicalDrive
                });
            }

            return _pathToItem;
        }

        #endregion

        #region GetItemsInfoFromPath

        public void GetItemsInfoFromPath()
        {
            try
            {
                dir = new DirectoryInfo(_pathToItem);

                _dirs = dir.GetDirectories();
                _files = dir.GetFiles();
                _pathToItem = dir.FullName;

                foreach (DirectoryInfo currentDir in _dirs)
                {

                    _itemsInfo.Add(new Item
                    {
                        itemName = currentDir.Name,
                        itemType = "Папка с файлами",
                        itemDateChanged = currentDir.LastWriteTime,
                        itemSize = 0,
                        itemPath = currentDir.FullName
                    });
                }

                foreach (FileInfo currentFile in _files)
                {
                    _itemsInfo.Add(new Item
                    {
                        itemName = currentFile.Name,
                        itemType = currentFile.Extension,
                        itemDateChanged = currentFile.LastWriteTime,
                        itemSize = currentFile.Length
                    });
                }
            }
            catch (DirectoryNotFoundException directoryNotFound)
            {
                MessageBox.Show(directoryNotFound.Message, _title);
            }

        }

        #endregion

        #endregion
        /*---------------------------------------------------------------------------------------------------------------*/
    }
}
