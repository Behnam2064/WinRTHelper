using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Scanners;
using Windows.Storage;

namespace WinSDKHelperNF.ScaningApi
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/uwp/api/windows.devices.scanners?view=winrt-26100
    /// https://learn.microsoft.com/en-us/windows/apps/develop/devices-sensors/scan-from-your-app
    /// </summary>
    public class ImageScannerHelper
    {
        private DeviceWatcher scannerWatcher;
        private List<DeviceInformation> Ids;
        public ImageScannerHelper()
        {
            Ids = new List<DeviceInformation>();
        }


        public void InitDeviceWatcher()
        {
            // Create a Device Watcher class for type Image Scanner for enumerating scanners
            scannerWatcher = DeviceInformation.CreateWatcher(DeviceClass.ImageScanner);
            scannerWatcher.Added += OnScannerAdded;
            scannerWatcher.Removed += OnScannerRemoved;
            scannerWatcher.EnumerationCompleted += OnScannerEnumerationComplete;
            scannerWatcher.Start();
        }

        private void OnScannerEnumerationComplete(DeviceWatcher sender, object args)
        {
            //throw new NotImplementedException();
        }

        private void OnScannerRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //throw new NotImplementedException();
        }

        private void OnScannerAdded(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            Ids.Add(deviceInfo);
            scannerWatcher.Stop();
            //Console.WriteLine((String.Format("Scanner with device id {0} has been added", deviceInfo.Id)));
        }

        public async Task Scan(string address,CancellationToken cancellationToken)
        {
            var id = Ids[0].Id;
            
            var myScanner = await ImageScanner.FromIdAsync(id);
            
            StorageFolder st = await StorageFolder.GetFolderFromPathAsync(address); //new StorageFile()
            Prog<uint> prog = new Prog<uint>();
            //Task<ImageScannerScanResult> result = 
            ImageScannerScanResult result = await myScanner.ScanFilesToFolderAsync(ImageScannerScanSource.Default,st).AsTask(cancellationToken, prog);
            
            
            
        }

        private void pro(int pr)
        {

        }
    }

    public class Prog<TP> : System.IProgress<TP> where TP : struct
    {
        public TP Progress { get; set; }
        public Prog()
        {
            
        }

        public void Report(TP value)
        {

        }
    }
}
