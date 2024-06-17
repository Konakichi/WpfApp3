using System.Threading;
using System.Windows;

namespace WpfApp3
{
    public partial class ProgressWindow : Window
    {

        private readonly CancellationTokenSource cancellationTokenSource = new();

        public ProgressWindow()
        {
            InitializeComponent();
        }

        public void UpdateProgress(int currentFile, int totalFiles)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = (double)currentFile / totalFiles * 100;
                ProgressTextBlock.Text = $"{currentFile}/{totalFiles}";
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

        //
    }
}