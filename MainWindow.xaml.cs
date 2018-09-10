using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleSearch
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string isSearchingButtonContent = "Cancel", isNotSearchingButtonContent = "Search";

        private bool isSearching;

        public bool IsSearching
        {
            get { return isSearching; }
            set
            {
                if (value == isSearching) return;

                isSearching = value;

                Dispatcher.Invoke(() => btnSearch.Content = isSearching ? isSearchingButtonContent : isNotSearchingButtonContent);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (isSearching)
            {
                IsSearching = false;
                return;
            }

            if (tbxKey.Text.Length == 0) return;

            string path = Path.GetFullPath(tbxPath.Text);
            string key = tbxKey.Text;
            bool caseSensetive = cbxCaseSensetive.IsChecked == true;

            Task.Factory.StartNew(() => Search(path, key, caseSensetive));
        }

        private void Search(string path, string key, bool caseSensetive)
        {
            IsSearching = true;

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

                    if (!IsSearching) return;
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

                    if (!IsSearching) return;
                }
            }

            IsSearching = false;
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
