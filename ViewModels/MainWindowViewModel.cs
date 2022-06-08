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
        private string _Path = "C:\\";
        ///<summary>
        ///Путь
        ///</summary>
        public string Path
        {
            get => _Path;
            set
            {
                Set(ref _Path, value);
            }
        }

        #endregion

        ///<summary>
        ///Директория
        ///</summary>
        public DirectoryInfo[] Dirs
        {
            get => _Dirs;
            set
            {
                Set(ref _Dirs, value);
            }
        }



        /*---------------------------------------------------------------------------------------------------------------*/

        #region Команды

        #region CloseApplicationCommand

        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object obj) => true;

        private void OnCloseApplicationCommandExecuted(object obj) => Application.Current.Shutdown();

        #endregion

        #endregion

        /*---------------------------------------------------------------------------------------------------------------*/
        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new ActionCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            #endregion

            Dir = new DirectoryInfo(_Path);
            _Dirs = Dir.GetDirectories();

            var Dirs = new ObservableCollection<_Directory>();

            foreach (DirectoryInfo currentDir in _Dirs)
            {
                Dirs.Add(new _Directory { Name = currentDir.Name, DateChanged = currentDir.LastWriteTime});
            }

        }
    }
}
