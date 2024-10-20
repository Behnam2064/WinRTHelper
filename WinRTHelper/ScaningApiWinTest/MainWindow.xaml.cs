using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using WinRTHelper.ScaningApi;
using WinSDKHelperNF.ScaningApi;
using static System.Net.Mime.MediaTypeNames;

namespace ScaningApiWinTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScanProgress<uint> ScanProgress;
        public MainWindow()
        {
            ScanProgress = new ScanProgress<uint>();
            ScanProgress.OnReport += OnReportChanged;
            InitializeComponent();
        }

        private void OnReportChanged(object sender, EventArgs eventArgs)
        {
            Dispatcher.Invoke(() =>
            {
                pb.Maximum = 1;
                pb.Value = ScanProgress.Progress;
            });
        }

        private async void OnScan1(object sender, RoutedEventArgs e)
        {
            EasyScan easyScan = new EasyScan();
            await easyScan.SearchDevicies();

            easyScan.SetScannerDevice(easyScan.GetScannerDeviceIds().First());

            var result = await easyScan.ScanFilesToPathAsync(@"C:\Users\Behnam\Downloads\Documents", ImageScannerScanSourceHelper.Default, CancellationToken.None, ScanProgress);

            foreach (var item in result)
            {
                System.Diagnostics.Process.Start(item);
            }
        }

        private async void OnScan2(object sender, RoutedEventArgs e)
        {
            EasyScan easyScan = new EasyScan();
            await easyScan.SearchDevicies();

            easyScan.SetScannerDevice(easyScan.GetScannerDeviceIds().First());

            if (easyScan.IsPreviewSupported(ImageScannerScanSourceHelper.Default))
            {
                var result = await easyScan.ScanPreviewToStreamAsync(ImageScannerScanSourceHelper.Default);
                if (result.Succeeded)
                {
                    img1.Source = GetImageSource(result.MemoryStream);

                    System.IO.File.WriteAllBytes(@"C:\temp\image1.bmp", result.MemoryStream.ToArray());
                }
            }else
            {
                MessageBox.Show("The scanner not support of preview!");
            }


        }

        private ImageSource GetImageSource(MemoryStream memoryStream)
        {
            using (memoryStream)
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = memoryStream;
                imageSource.EndInit();

                // Assign the Source property of your image
                return imageSource;
            }
        }
    }


}
