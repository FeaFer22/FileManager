using FileManager.ViewModels.Base;

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
    }
}
