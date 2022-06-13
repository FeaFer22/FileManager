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

        private DirectoryInfo Dir;
        private DirectoryInfo[] _Dirs;
        private FileInfo[] _Files;
        private ObservableCollection<Item> _ItemsInfo;

        #region Заголовок окна

        private string _Title = "Файловый менеджер";
        ///<summary>
        ///Заголовок окна
        ///</summary>
        public string Title
        {
            get => _Title;
            set
            {
                Set(ref _Title, value);
            }
        }

        #endregion

        #region Путь к объекту

        ///<summary>Путь к объекту</summary>

        private string _PathToItem;
        public string PathToItem
        {
            get => _PathToItem;
            set
            {   
                Set(ref _PathToItem, value);
            }
        }

        #endregion

        #region Получение информации о объекте

        ///<summary>Получение информации о объекте</summary>
        public ObservableCollection<Item> ItemsInfo
        {
            get => _ItemsInfo;
            set
            {
                Set(ref _ItemsInfo, value);
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

        #region OpenSelectedItemCommand

        public ICommand OpenSelectedItemCommand { get; }

        private bool CanOpenSelectedItemCommandExecute(object obj) => true;

        private void OnOpenSelectedItemCommandExecuted(object obj)
        {
            ItemsInfo.Clear();

            Dir = new DirectoryInfo(_PathToItem);

            if (Dir.Exists == true && Dir != null)
            {
                _Dirs = Dir.GetDirectories();
                _Files = Dir.GetFiles();
                _PathToItem = Dir.FullName;
                Dir = new DirectoryInfo(_PathToItem);
            }
            else
            {
                MessageBox.Show("Путь не найден!", "Файловый менеджер");
            }

            foreach (DirectoryInfo currentDir in _Dirs)
            {
                _ItemsInfo.Add(new Item
                {
                    ItemName = currentDir.Name,
                    ItemType = "Папка с файлами",
                    ItemDateChanged = currentDir.LastWriteTime,
                    ItemSize = 0,
                    ItemPath = currentDir.FullName
                });
            }
            foreach (FileInfo currentFile in _Files)
            {
                _ItemsInfo.Add(new Item
                {
                    ItemName = currentFile.Name,
                    ItemType = currentFile.Extension,
                    ItemDateChanged = currentFile.LastWriteTime,
                    ItemSize = currentFile.Length / 1024 / 1024 / 1024,
                    ItemPath = currentFile.FullName
                });
            }
        }


        #endregion

        #endregion

        /*---------------------------------------------------------------------------------------------------------------*/
        public MainWindowViewModel()
        {

            #region Команды

            CloseApplicationCommand = new ActionCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            OpenSelectedItemCommand = new ActionCommand(OnOpenSelectedItemCommandExecuted, CanOpenSelectedItemCommandExecute);

            #endregion

            _ItemsInfo = new ObservableCollection<Item>();

            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {
                _PathToItem = logicalDrive;
                _ItemsInfo.Add(new Item
                {
                    ItemName = logicalDrive,
                    ItemType = "Локальный диск",
                    ItemDateChanged = DateTime.Now,
                    ItemPath = logicalDrive
                });
            }
        }
    }
}
