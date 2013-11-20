/*
 * Richard Anderson
 * CS 2530
 * Final Team Project
 */

using System;
using System.Collections;
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
                //FileList.ItemsSource = files;
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
            FileList.ItemsSource = (from s in filePaths
                                    select s.ToString()).Distinct();
            FileList.SelectAll();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            FileList.ItemsSource = string.Empty;
        }

        private void ReplayGainButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedFiles = new List<string>();
            if (FileList.ItemsSource != null)
            {
                foreach (var item in FileList.ItemsSource)
                {
                    var row = FileList.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (row != null) 
                    {
                        if (row.IsSelected)
                        {
                            selectedFiles.Add(row.Item.ToString());
                        }
                    }
                }
            }
            mp3GainInterface mp3Gain = new mp3GainInterface();
            mp3Gain.runMp3Gain(selectedFiles, mp3GainInterface.GainType.Album);
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            //use openfiledialog to select an m3u file and pass path to Kurts class/method.
            ArrayList array1 = new ArrayList();
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true) // user clicked OK button
            {
                string filePath = dialog.FileName;
                foreach (var el in ParsM3UFile(filePath))
                {
                    filePaths.Add(el.ToString());
                }
                FileList.ItemsSource = (from s in filePaths
                                        select s.ToString()).Distinct();
                FileList.SelectAll();
            }
        }

        private static List<string> ParsM3UFile(string basePath)
        {
            List<string> songFilePaths = new List<String>();
            try
            {
                bool isPathCorrect;
                isPathCorrect = File.Exists(basePath);

                //If path Fails return an empty list
                if (isPathCorrect == false)
                {
                    return new List<String>();
                }
                //If path is verified, pars the m3u file
                else
                {
                    using (StreamReader reader = new StreamReader(basePath))
                    {
                        string line;
                        while (( line = reader.ReadLine() ) != null)
                        {

                            // string mp3File = getMP3file(line);
                            bool ifFilePath = File.Exists(line);
                            if (ifFilePath == true)
                            {
                                songFilePaths.Add(line);
                            }
                            else
                            {
                                //Check to see if the path can be found. 
                                string directory = System.IO.Path.GetDirectoryName(basePath);
                                if (File.Exists(directory + line))
                                {
                                    songFilePaths.Add(directory + line);
                                }
                            }
                        }
                        reader.Close();


                        return songFilePaths;
                    }
                }
            }

            catch (Exception)
            {
                Console.WriteLine("\n\nWell this is embarrassing.\nSomething went wrong, please try again.");
                Console.ReadKey();
                Environment.Exit(0);
                return new List<string>();
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
