/*
 * Richard Anderson
 * CS 2530
 * Final Team Project
 */

using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace ReplayGain
{
    public partial class MainWindow : Window
    {
        List<string> filePaths = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
            folderDialog.ShowDialog();
            string curDirectory = folderDialog.SelectedPath;
            try
            { 
                var files = Directory.EnumerateFiles(curDirectory, "*.mp3",SearchOption.AllDirectories);
                FileList.ItemsSource = files;
                foreach (var f in files)
                {
                    filePaths.Add(f.ToString());
                }
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Console.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Console.WriteLine(PathEx.Message);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            FileList.ItemsSource = string.Empty;
        }

        private void ReplayGainButton_Click(object sender, RoutedEventArgs e)
        {
            var itemsSource = FileList.ItemsSource as IEnumerable<string>;
            if (FileList.ItemsSource != null)
            {
                foreach (var item in FileList.ItemsSource)
                {
                    var row = FileList.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (row != null) 
                    {
                        MessageBox.Show(row.Item.ToString());
                    }
                }
            }
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            //use openfiledialog to select an m3u file and pass path to Kurts class/method.
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true) // user clicked OK button
            {
                string filePath = dialog.FileName;
            } 
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
