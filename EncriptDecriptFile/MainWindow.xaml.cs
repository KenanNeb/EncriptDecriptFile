using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;

namespace EncriptDecriptFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool check = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void fromBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileDirectory = new OpenFileDialog();
            fileDirectory.Multiselect = false;
            fileDirectory.Filter = "Text|*.txt|All|*.*";
            if (fileDirectory.ShowDialog() == true)
            {
                filenameTxtBox.Text = fileDirectory.FileName;
            }
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", filenameTxtBox.Text);
        }

        public void encdec(string str, string key)
        {

            if (str != "")
            {

                int keyIndex = 0;
                void InitIndex()
                {
                    keyIndex++;
                    if (keyIndex > key.Length - 1)
                        keyIndex = 0;
                }
                StringBuilder builder = new StringBuilder();
                string stra = "";
                this.Dispatcher.Invoke(() => { stra = filenameTxtBox.Text; });
                this.Dispatcher.Invoke(() => { prgBar.Value = 0; prgBar.Maximum = str.Length; prgBar.Minimum = 0; });
                using (StreamWriter sw = new(stra))
                {
                }
                for (int i = 0; i < str.Length; i++)
                {
                    if (check == true)
                        break;
                    using (StreamWriter sw = new(stra, true))
                    {
                        InitIndex();
                        sw.Write((char)(str[i] ^ (char)key[keyIndex]));
                    }
                    this.Dispatcher.Invoke(() => { prgBar.Value++; });
                    Thread.Sleep(50);
                }
                if (check == true)
                {
                    string reader = File.ReadAllText(stra);
                    while (reader != "")
                    {
                        using (StreamWriter sw = new(stra))
                        {
                            sw.Write(reader);
                            reader = reader.Remove(reader.Length - 1, 1);
                            this.Dispatcher.Invoke(() => { prgBar.Value--; });
                            Thread.Sleep(50);
                        }
                    }
                    using (StreamWriter sw = new(stra))
                    {
                        sw.Write(str);
                    }
                }
            }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            check = false;
            string str = "";
            try
            {
                str = File.ReadAllText(filenameTxtBox.Text);
            }
            catch (Exception)
            {
            }

            if (str != "")
            {
                string password = passwordTxtBox.Text;
                ThreadPool.QueueUserWorkItem((a) => encdec(str, password));
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            check = true;           
        }
    }
}
