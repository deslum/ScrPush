using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScrPush
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {


        public Settings()
        {
            InitializeComponent();
            String path = Properties.Settings.Default.Path;
            if (path == String.Empty)
                textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);
            else
                textBox1.Text = Properties.Settings.Default.Path;
            checkBox1.IsChecked = Properties.Settings.Default.Autorun;
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {

            Properties.Settings.Default.Path = textBox1.Text;
            if (checkBox1.IsChecked == true)
            {
                Microsoft.Win32.RegistryKey Key =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
                Key.SetValue("ScrPush", Environment.CurrentDirectory + "\\ScrPush.exe");
                Key.Close();
                Properties.Settings.Default.Autorun = true;
            }
            else
            {
                Microsoft.Win32.RegistryKey Key =
                   Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", false);
                Key.SetValue("ScrPush", Environment.CurrentDirectory + "\\ScrPush.exe");
                Key.Close();
                Properties.Settings.Default.Autorun = false;
                Properties.Settings.Default.Save();
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ScrPush.Properties.Settings.Default.Path = textBox1.Text;
            if (checkBox1.IsChecked == true)
            {
                Microsoft.Win32.RegistryKey Key =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
                Key.SetValue("ScrPush", Environment.CurrentDirectory + "\\ScrPush.exe");
                Key.Close();
                Properties.Settings.Default.Autorun = true;
            }
            else
            {
                Microsoft.Win32.RegistryKey Key =
                   Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", false);
                Key.SetValue("ScrPush", Environment.CurrentDirectory + "\\ScrPush.exe");
                Key.Close();
                Properties.Settings.Default.Autorun = false;
                Properties.Settings.Default.Save();
            }
            this.Hide();
        }

        private void OpenDialog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.SelectedPath = Environment.CurrentDirectory;
            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            if (result.ToString() == "OK")
                textBox1.Text = folderDialog.SelectedPath;
        }
    }
}
