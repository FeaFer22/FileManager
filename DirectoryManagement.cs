using System.IO;
using System.Windows.Controls;
using System.Diagnostics;

namespace FileManager
{
    class DirectoryManagement
    {
        public void DisplayDir(TextBox textBox, ListBox listBox)
        {
            listBox.Items.Clear();

            DirectoryInfo Directory = new DirectoryInfo(textBox.Text);
            DirectoryInfo[] Dirs = Directory.GetDirectories();

            foreach (DirectoryInfo currentDir in Dirs)
            {
                listBox.Items.Add(currentDir);
            }

            FileInfo[] Files = Directory.GetFiles();

            foreach (FileInfo currentFile in Files)
            {
                listBox.Items.Add(currentFile);
            }
        }

        public void SelectItem(TextBox textBox, ListBox listBox)
        {
            textBox.Text = Path.Combine(textBox.Text, listBox.SelectedItem.ToString());
            DisplayDir(textBox, listBox);
        }
        public void PathBack(TextBox textBox, ListBox listBox)
        {
            if (textBox.Text[textBox.Text.Length - 1] == '\\')
            {
                textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1, 1);
                
                while (textBox.Text[textBox.Text.Length - 1] == '\\')
                {
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1, 1);
                }
            }
            else if (textBox.Text[textBox.Text.Length - 1] != '\\')
            {
                while (textBox.Text[textBox.Text.Length - 1] != '\\')
                {
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1, 1);
                }
            }
            DisplayDir(textBox, listBox);
        }

        public void RunExtension(TextBox textBox, ListBox listBox)
        {
            if (Path.GetExtension(Path.Combine(textBox.Text, listBox.SelectedItem.ToString())) != "")
            { 
                Process.Start(new ProcessStartInfo(Path.Combine(textBox.Text, listBox.SelectedItem.ToString())) { UseShellExecute = true });
            }
            else
            {
                SelectItem(textBox, listBox);
            }
        }
    }
}
