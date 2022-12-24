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
        CancellationTokenSource cancellationTokenSource = null;

        string Path = string.Empty;
        string Text = string.Empty;
        string Key = string.Empty;
        int KeyIndex = 0;

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

        void InitIndex()
        {
            KeyIndex++;
            if (KeyIndex > Key.Length - 1)
                KeyIndex = 0;
        }

        public void encdec()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            if (string.IsNullOrWhiteSpace(Path))
                return;


            KeyIndex = 0;
            StringBuilder builder = new StringBuilder();
            Text = string.Empty;
            this.Dispatcher.Invoke(() => { Text = filenameTxtBox.Text; });
            this.Dispatcher.Invoke(() => { prgBar.Value = 0; prgBar.Maximum = Path.Length; prgBar.Minimum = 0; });

            // For clear text
            using (StreamWriter sw = new(Text)) { }

            try
            {
                Encryption(token);
            }
            catch (OperationCanceledException)
            {
                Decryption();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }

            this.Dispatcher.Invoke(() => { prgBar.Value = 0; });
            MessageBox.Show("Finished !");

        }

        public void Encryption(CancellationToken token)
        {
            // Encrypt
            for (int i = 0; i < Path.Length; i++)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                using (StreamWriter sw = new(Text, true))
                {
                    InitIndex();
                    sw.Write((char)(Path[i] ^ (char)Key[KeyIndex]));
                };
                this.Dispatcher.Invoke(() => { prgBar.Value++; });
                Thread.Sleep(50);
            }
        }

        public void Decryption()
        {
            string reader = File.ReadAllText(Text);
            while (reader != "")
            {
                using (StreamWriter sw = new(Text))
                {
                    sw.Write(reader);
                    reader = reader.Remove(reader.Length - 1, 1);
                    this.Dispatcher.Invoke(() => { prgBar.Value--; });
                    Thread.Sleep(50);
                }
            }
            using (StreamWriter sw = new(Text))
            {
                sw.Write(Path);
            }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {

            if (encryptRbtn.IsChecked is false && decryptRbtn.IsChecked is false)
            {
                MessageBox.Show("Encryption or Decryption must be selected");
                return;
            }

            Path = string.Empty;
            try
            {
                Path = File.ReadAllText(filenameTxtBox.Text);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }

            if (string.IsNullOrWhiteSpace(Path))
            {
                MessageBox.Show("File Path can't be empty !");
                return;
            }

            Key = passwordTxtBox.Text;

            if (string.IsNullOrWhiteSpace(Key))
            {
                MessageBox.Show("Password can't be empty !");
                return;
            }

            ThreadPool.QueueUserWorkItem((a) => encdec());

        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cancellationTokenSource?.Cancel();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
