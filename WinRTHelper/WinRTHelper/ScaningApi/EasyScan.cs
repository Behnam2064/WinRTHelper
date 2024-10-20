using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Scanners;
using Windows.Storage;
using Windows.Storage.Streams;
using WinRTHelper.Helpers;
using WinSDKHelperNF.ScaningApi;

namespace WinRTHelper.ScaningApi
{
    public class EasyScan
    {
        #region Properties

        protected DeviceWatcher ScannerWatcher;

        protected List<DeviceInformation> DevicesInformation;

        public ReadOnlyCollection<DeviceInformation> ScannerDevicies
        {
            get => new ReadOnlyCollection<DeviceInformation>(DevicesInformation.ToList());
        }

        private DeviceInformation _SelectedDevice { get; set; }
        public ImageScanner Scanner { get; private set; }
        public DeviceInformation SelectedDevice
        {
            get => _SelectedDevice;
            set
            {
                _SelectedDevice = value;
                SetScanner();
            }
        }

        /// <summary>
        /// Call after setting the SelectedDevice variable
        /// </summary>
        public ImageScannerScanSourceHelper DefaultScanSource
        {
            get => CastImageScannerScanSourceHelper(Scanner.DefaultScanSource);
        }

        private void SetScanner()
        {
            if (SelectedDevice != null)
            {
                var tskOp = ImageScanner.FromIdAsync(SelectedDevice.Id);
                var tsk = tskOp.AsTask();
                tsk.Wait();
                Scanner = tsk.Result;
            }
            else
            {
                Scanner = null;
            }
        }

        public bool IsSearching { get; private set; }
        #endregion

        #region Constructors

        public EasyScan()
        {
            DevicesInformation = new List<DeviceInformation>();
            // Create a Device Watcher class for type Image Scanner for enumerating scanners
            ScannerWatcher = DeviceInformation.CreateWatcher(DeviceClass.ImageScanner);
            ScannerWatcher.Added += OnScannerAdded;
            //ScannerWatcher.Removed += OnScannerRemoved;
            ScannerWatcher.EnumerationCompleted += OnScannerEnumerationComplete;
        }

        #endregion

        #region Init

        public Task SearchDevicies()
        {
            return Task.Run(() =>
            {
                IsSearching = true;
                ScannerWatcher.Start();

                while (IsSearching)
                {
                    Thread.Sleep(500);
                }
            });
        }

        private void OnScannerEnumerationComplete(DeviceWatcher sender, object args)
        {
            //throw new NotImplementedException();
            ScannerWatcher.Stop();
            IsSearching = false;
        }

        private void OnScannerAdded(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            DevicesInformation.Add(deviceInfo);
        }
        #endregion

        private ImageScannerScanSource CastImageScannerScanSource(ImageScannerScanSourceHelper sourceHelper)
        {
            return (ImageScannerScanSource)Enum.Parse(typeof(ImageScannerScanSource), sourceHelper.ToString());
        }


        private ImageScannerScanSourceHelper CastImageScannerScanSourceHelper(ImageScannerScanSource sourceHelper)
        {
            return (ImageScannerScanSourceHelper)Enum.Parse(typeof(ImageScannerScanSourceHelper), sourceHelper.ToString());
        }

        #region Get / Set

        /// <summary>
        /// Call after setting the SelectedDevice variable
        /// </summary>
        /// <param name="scanSourceHelper"></param>
        /// <returns></returns>
        public bool IsPreviewSupported(ImageScannerScanSourceHelper scanSourceHelper)
        {
            return Scanner.IsPreviewSupported(CastImageScannerScanSource(scanSourceHelper));
        }


        /// <summary>
        /// Call after setting the SelectedDevice variable
        /// </summary>
        /// <param name="scanSourceHelper"></param>
        /// <returns></returns>
        public bool IsScanSourceSupported(ImageScannerScanSourceHelper scanSourceHelper)
        {

            return Scanner.IsScanSourceSupported(CastImageScannerScanSource(scanSourceHelper));
        }


        public void SetScannerDevice(int Index)
        {
            SelectedDevice = ScannerDevicies[Index];
        }

        public void SetScannerDevice(string Id)
        {
            SelectedDevice = ScannerDevicies.Where(x => x.Id == Id).First();
        }

        public IList<string> GetScannerDeviceIds()
        {
            return ScannerDevicies.Select(x => x.Id).ToList();
        }

        public IList<string> GetScannerDeviceNames()
        {
            return ScannerDevicies.Select(x => x.Name).ToList();
        }

        public bool GetScannerDeviceIsEnabled(string Id)
        {
            return ScannerDevicies.Where(x => x.Id == Id).First().IsEnabled;
        }

        public bool GetScannerDeviceIsDefault(string Id)
        {
            return ScannerDevicies.Where(x => x.Id == Id).First().IsDefault;
        }


        #endregion

        #region Scan

        /// <summary>
        /// Call after setting the SelectedDevice variable
        /// </summary>
        /// <param name="Folder"></param>
        /// <param name="scannerScanSourceHelper"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async Task<ImageScannerScanResult> ScanFilesToFolderAsync(string Folder, ImageScannerScanSourceHelper scannerScanSourceHelper, CancellationToken cancellationToken, IProgress<uint> progress)
        {
            StorageFolder st = await StorageFolder.GetFolderFromPathAsync(Folder); //new StorageFile()

            ImageScannerScanResult result = await Scanner.ScanFilesToFolderAsync(CastImageScannerScanSource(scannerScanSourceHelper), st).AsTask(cancellationToken, progress);

            return result;
        }

        /// <summary>
        /// Call after setting the SelectedDevice variable
        /// </summary>
        /// <param name="Folder"></param>
        /// <param name="scannerScanSourceHelper"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async Task<List<string>> ScanFilesToPathAsync(string Folder, ImageScannerScanSourceHelper scannerScanSourceHelper, CancellationToken cancellationToken, IProgress<uint> progress)
        {
            var result = await ScanFilesToFolderAsync(Folder, scannerScanSourceHelper, cancellationToken, progress);

            return result.ScannedFiles.Select(x => x.Path).ToList();
        }


        /// <summary>
        /// Call after setting the SelectedDevice variable
        /// </summary>
        /// <param name="Folder"></param>
        /// <param name="scannerScanSourceHelper"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async Task<ScanPreviewToStreamAsyncResult> ScanPreviewToStreamAsync(ImageScannerScanSourceHelper scannerScanSourceHelper)
        {
            //StorageFolder st = await StorageFolder.GetFolderFromPathAsync(Folder); //new StorageFile()
            //StorageFile st = await StorageFile.GetFileFromPathAsync("");
            
            var st = new Windows.Storage.Streams.InMemoryRandomAccessStream();
            var result = await Scanner
                .ScanPreviewToStreamAsync(CastImageScannerScanSource(scannerScanSourceHelper), st);//.AsTask(cancellationToken, progress);
            
            return new ScanPreviewToStreamAsyncResult()
            {
                Succeeded = result.Succeeded,
                MemoryStream = result.Succeeded ? await new ClassHelper().ConvertToMemoryStream(st) : null,
                ImageScannerFormatHelper = new ClassHelper().CastImageScannerFormatHelper(result.Format)
            };
        }

       

        #endregion

    }

    public class ScanPreviewToStreamAsyncResult
    {
        public MemoryStream MemoryStream { get; set; }
        public ImageScannerFormatHelper ImageScannerFormatHelper { get; set; }
        public bool Succeeded { get; set; }
    }
}
