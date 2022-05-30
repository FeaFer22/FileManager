using System;
using System.Windows;

namespace FileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DirectoryManagement dirManage;
        public MainWindow()
        {
            InitializeComponent();
            dirManage = new DirectoryManagement();
        }

        private void Button_moveto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dirManage.DisplayDir(textBox, listBox);
            }
            catch{ }
        }

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                dirManage.RunExtension(textBox, listBox);
            }
            catch { }
        }

        private void Button_back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dirManage.PathBack(textBox, listBox);
            }
            catch { }
        }
    }
}
