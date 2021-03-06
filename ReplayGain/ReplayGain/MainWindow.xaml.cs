﻿/*
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
        DoubleCollection tickMarks = new DoubleCollection();
        mp3GainInterface.GainType gainType = mp3GainInterface.GainType.Track;
        bool track = false;
        bool album = false;
        string helpDialog = string.Empty;

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
            if (curDirectory != "")
            {
                try
                {
                    var files = Directory.EnumerateFiles(curDirectory, "*.mp3", SearchOption.AllDirectories);
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
                FileList.ItemsSource = ( from s in filePaths
                                         select s.ToString() ).Distinct();
            }
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
                bool isSelected = false;
                foreach (var item in FileList.ItemsSource)
                {
                    var row = FileList.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (row != null && row.IsSelected) 
                    {
                        isSelected = true;
                        selectedFiles.Add(row.Item.ToString());
                    }
                }
                if (isSelected == false)
                {
                    MessageBox.Show("Please select at least one row...");
                }
            }
            mp3GainInterface mp3Gain = new mp3GainInterface();
            mp3Gain.runMp3Gain(selectedFiles, gainType);

            if (track == true)
            {
                mp3Gain.runMp3Gain(selectedFiles, mp3GainInterface.GainType.Track);
            }
            else if (album == true)
            {
                mp3Gain.runMp3Gain(selectedFiles, mp3GainInterface.GainType.Album);
            }
            else
            {
                mp3Gain.runMp3Gain(selectedFiles, mp3GainInterface.GainType.Track, Convert.ToDouble(tickSlider.Value));
            }
        }

        private void Button_Helper_Click(object sender, RoutedEventArgs e)
        {
            helpDialog = "There are three options for replaygain.\n\n Track: All tracks will be set to the same decibel level of 89.\n\n Album: All tracks will maintain their individual differences while all tracks are adjusted.\n\n Custom: All tracks will be adjusted to the selected decibel value.";
            Help helper = new Help();
            helper.HelpDialog.Text = helpDialog;
            helper.Show();
        }

        private void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            //use openfiledialog to select an m3u file and pass path to Kurts class/method.
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

        private void ReplayOptionGroup_Checked(object sender, RoutedEventArgs e)
        {

            if (TrackRadioButton.IsChecked == true)
            {
                //tickSlider.IsEnabled = false;
                track = true;
                album = false;
                gainType = mp3GainInterface.GainType.Track;
            }
            else if (AlbumRadioButton.IsChecked == true)
            {
                //tickSlider.IsEnabled = false;
                album = true;
                track = false;
                gainType = mp3GainInterface.GainType.Album;
            }
            else if (CustomRadioButton.IsChecked == true)
            {
                track = false;
                album = false;

                tickSlider.IsEnabled = true;
                for (int i = 74; i <= 104; i++)
                {
                    tickMarks.Add(i);
                }
                tickSlider.Ticks = tickMarks;
            }
        }

        private void SelectGroup_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectAllRadioButton.IsChecked == true)
            {
                FileList.SelectAll();
            }

            if (UnSelectAllRadioButton.IsChecked == true)
            {
                FileList.UnselectAll();
            }
        }
    }
}
