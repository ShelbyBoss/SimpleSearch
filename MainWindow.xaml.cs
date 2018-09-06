using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SimpleSearch
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tbxKey.Text.Length == 0) return;

            string path = Path.GetFullPath(tbxPath.Text);
            string key = tbxKey.Text;
            bool caseSensetive = cbxCaseSensetive.IsChecked == true;

            Task.Factory.StartNew(() => Search(path, key, caseSensetive));
        }

        private void Search(string path, string key, bool caseSensetive)
        {
            Queue<string> errors = new Queue<string>();

            if (caseSensetive)
            {
                key = key.ToLower();

                foreach (string element in GetAllFilesAndDirectories(Path.GetFullPath(path), errors))
                {
                    if (Path.GetFileName(element).ToLower().Contains(key))
                    {
                        Dispatcher.InvokeAsync(() => lbxFound.Items.Add(element));
                    }

                    Dispatcher.InvokeAsync(() =>
                    {
                        while (errors.Any()) lbxError.Items.Add(errors.Dequeue());
                    });
                }
            }
            else
            {
                foreach (string element in GetAllFilesAndDirectories(Path.GetFullPath(path), errors))
                {
                    if (Path.GetFileName(element).Contains(key))
                    {
                        Dispatcher.InvokeAsync(() => lbxFound.Items.Add(element));
                    }

                    Dispatcher.InvokeAsync(() =>
                    {
                        while (errors.Count > 0) lbxError.Items.Add(errors.Dequeue());
                    });
                }
            }
        }

        private IEnumerable<string> GetAllFilesAndDirectories(string directory, Queue<string> errors)
        {
            yield return directory;

            string[] files = new string[0];
            string[] directories = new string[0];

            try
            {
                files = Directory.GetFiles(directory);
            }
            catch (Exception e)
            {
                string error = directory + "\n" + e.Message;
                errors.Enqueue(error);
            }

            try
            {
                directories = Directory.GetDirectories(directory);
            }
            catch (Exception e)
            {
                string error = directory + "\n" + e.Message;
                errors.Enqueue(error);
            }

            foreach (string file in files) yield return file;

            foreach (string sub in directories)
            {
                foreach (string file in GetAllFilesAndDirectories(sub, errors)) yield return file;
            }
        }
    }
}
