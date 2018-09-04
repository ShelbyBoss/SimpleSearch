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
            if (tbxPath.Text.Length == 0 || tbxKey.Text.Length == 0) return;

            lbxFound.Items.Clear();
            lbxError.Items.Clear();

            Folder(tbxPath.Text);
        }

        private void Folder(string folder)
        {
            if (Contains(folder)) lbxFound.Items.Add(folder);

            try
            {
                foreach (string file in Directory.GetFiles(folder))
                {
                    if (Contains(Path.GetFileName(file)))
                    {
                        lbxFound.Items.Add(file);
                    }
                }

                foreach (string sub in Directory.GetDirectories(folder)) Folder(sub);
            }
            catch (Exception e)
            {
                string error = folder + "\n" + e.Message;
                lbxError.Items.Add(error);
            }
        }

        private bool Contains(string text)
        {
            return text.ToLower().Contains(tbxKey.Text.ToLower());
        }
    }
}
