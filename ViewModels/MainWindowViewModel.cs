using FileManager.Infrastructure.Commands;
using FileManager.Models;
using FileManager.ViewModels.Base;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        private ObservableCollection<_Directory> _ItemsInfo;

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

        #region Путь

        ///<summary>Путь</summary>

        private string _Path = "C:\\";
        public string Path
        {
            get => _Path;
            set
            {
                Set(ref _Path, value);
            }
        }

        #endregion

        #region Получение информации о объекте

        ///<summary>Получение информации о объекте</summary>
        public ObservableCollection<_Directory> ItemsInfo
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

        #region GetItemInfoCommand

        public ICommand GetItemInfoCommand { get; }

        private bool CanGetItemInfoCommandExecute(object obj) => true;

        private void OnGetItemInfoCommandExecuted(object obj) 
        {
            _ItemsInfo.Clear();
            foreach (DirectoryInfo currentDir in _Dirs)
            { 
                _ItemsInfo.Add(new _Directory
                {
                    Name = currentDir.Name,
                    Type = "Папка с файлами",
                    DateChanged = currentDir.LastWriteTime
                });
            }
            foreach (FileInfo currentFile in _Files)
            {
                _ItemsInfo.Add(new _Directory
                {
                    Name = currentFile.Name,
                    Type = currentFile.Extension.ToString(),
                    DateChanged = currentFile.LastWriteTime,
                    Size = currentFile.Length
                });
            }
            _Path = Dir.FullName.ToString();
        }

        #endregion

        #endregion

        /*---------------------------------------------------------------------------------------------------------------*/
        public MainWindowViewModel()
        {

            #region Команды

            CloseApplicationCommand = new ActionCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            GetItemInfoCommand = new ActionCommand(OnGetItemInfoCommandExecuted, CanGetItemInfoCommandExecute);

            #endregion

            Dir = new DirectoryInfo(_Path);
            _Dirs = Dir.GetDirectories();
            _Files = Dir.GetFiles();
            _ItemsInfo = new ObservableCollection<_Directory>();

        }
    }
}
