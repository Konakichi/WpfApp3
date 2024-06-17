using System.Diagnostics;
using System.IO;
using System.IO.Hashing;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
//using R3;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string path = PathTextBox.Text;
            if (!Directory.Exists(path))
            {
                StatusTextBlock.Text = "無効なパスが入力されました。";
                return;
            }

            var progressWindow = new ProgressWindow();
            progressWindow.Show();

            await CalculateHashesAsync(path, progressWindow);

            progressWindow.Close();
            StatusTextBlock.Text = "完了しました。";
        }

        private async Task CalculateHashesAsync(string path, ProgressWindow progressWindow)
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            var totalFiles = files.Length;

            var progress = new Progress<int>(currentFile =>
            {
                progressWindow.UpdateProgress(currentFile, totalFiles);
            });

            await Task.Run(() =>
            {
                Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (file, state, index) =>
                {
                    var hashAlgorithm = new XxHash3();
                    using var stream = File.OpenRead(file);
                    hashAlgorithm.Append(stream);
                    byte[] data = Encoding.UTF8.GetBytes("Hello, World!");
                    hashAlgorithm.Append(data);
                    byte[] hashBytes = new byte[8];
                    hashAlgorithm.GetHashAndReset(hashBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                    Debug.WriteLine($"{file}: {hash}");
                    ((IProgress<int>)progress).Report((int)index);
                });
            });
        }
    }
}