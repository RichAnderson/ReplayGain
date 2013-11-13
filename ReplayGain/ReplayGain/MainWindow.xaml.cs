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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog dialog = new OpenFileDialog();
            //if (dialog.ShowDialog() == true) // user clicked OK button
            //{
            //    string filePath = dialog.FileName;
            //    FileList.ItemsSource = new DirectoryInfo(filePath).GetFiles("*.*");
            //} 
            string filePath = @"c:\MyMusic";
            string[] fileList = Directory.GetFiles(filePath,"*.*");
            foreach (string s in fileList)
            {
                FileList.Items.Add(s);
            }
        }
    }
}
