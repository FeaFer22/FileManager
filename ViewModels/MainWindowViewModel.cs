using FileManager.Infrastructure.Commands;
using FileManager.ViewModels.Base;
using System.Windows;
using System.Windows.Input;

namespace FileManager.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Заголовок окна

        private string _Title = "File Manager";
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
        
        #region Команды
        
        #region CloseApplicationCommand

        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object obj) => true;

        private void OnCloseApplicationCommandExecuted(object obj) 
        {
            Application.Current.Shutdown();
        }

        #endregion
        
        #endregion

        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new ActionCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            
            #endregion
        }
    }
}
