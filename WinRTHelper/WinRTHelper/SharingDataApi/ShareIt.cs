using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WinRTHelper.SharingDataApi
{
    public enum ShareMode
    {
        Blank,
        Text,
        Weblink,
        File,
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/uwp/app-to-app/share-data
    /// https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.datapackage?view=winrt-22621
    /// </summary>
    public class ShareIt
    {

        private IntPtr hwnd { get; set; }

        private DataTransferManager dtm { get; set; }

        public ShareMode Mode { get; private set; }

        public object Content { get; private set; }

        public string Title { get; set; }

        public string Description { get; set; }

        private Window window;

        public ShareIt(IntPtr hwnd, string Title, string Description)
        {
            this.hwnd = hwnd;
            dtm = DataTransferManagerHelper.GetForWindow(hwnd);
            dtm.DataRequested += OnDataRequested;
            this.Title = Title;
            this.Description = Description;
        }

        public ShareIt(Window window, string Title, string Description) : this(new WindowInteropHelper(window).Handle, Title, Description)
        {
            this.window = window;
            //hwnd = new WindowInteropHelper(window).Handle;
            //var s = DataTransferManager.GetForCurrentView();
            //dtm = DataTransferManagerHelper.GetForWindow(hwnd);
            //dtm.ShareProvidersRequested += Dtm_ShareProvidersRequested;
            //dtm.TargetApplicationChosen += Dtm_TargetApplicationChosen;
            //dtm.DataRequested += OnDataRequested;

        }
        public ShareIt(Window window) : this(window, string.Empty, string.Empty) { }

        private void Dtm_TargetApplicationChosen(DataTransferManager sender, TargetApplicationChosenEventArgs args)
        {
            Debug.WriteLine("TargetApplicationChosen");
        }

        private void Dtm_ShareProvidersRequested(DataTransferManager sender, ShareProvidersRequestedEventArgs args)
        {
            Debug.WriteLine("ShareProvidersRequested");
        }


        async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();
            args.Request.Data.Properties.Title = Title;
            args.Request.Data.Properties.Description = Description;


            DataPackage dp = args.Request.Data;
            //dp.Properties.Title = DataPackageTitle.Text;
            Debug.WriteLine("IsSupported: " + DataTransferManager.IsSupported());

            switch (Mode)
            {
                case ShareMode.Text:
                    dp.SetText(Content.ToString());
                    break;

                case ShareMode.Weblink:
                    dp.SetWebLink(new System.Uri(Content.ToString()));
                    break;

                /*               case ShareMode.StorageItem:
                                   var filesToShare = new List<IStorageItem>();
                                   StorageFile imageFile = await StorageFile.GetFileFromPathAsync(AppDomain.CurrentDomain.BaseDirectory + Content.ToString());
                                   filesToShare.Add(imageFile);

                                   dp.SetStorageItems(filesToShare);
                                   break;
           */
                case ShareMode.File:
                    List<IStorageItem> filesToShare = new List<IStorageItem>();
                    if (Content is string)
                    {
                        if (System.IO.File.Exists(Content.ToString()))
                        {
                            filesToShare.Add(await GetPathAsync(Content.ToString()));
                        }
                        else
                            throw new System.IO.FileNotFoundException();
                    }
                    else if (Content is IEnumerable<string>)
                    {
                        var ls = Content as List<string>;
                        if (ls != null && ls.Count > 0)
                        {
                            foreach (string item in ls)
                                if (System.IO.File.Exists(item))
                                {
                                    filesToShare.Add(await GetPathAsync(item));
                                }
                                else
                                    throw new System.IO.FileNotFoundException();
                        }
                    }
                    //https://stackoverflow.com/questions/54570111/how-can-i-pass-an-implementation-of-istorageitem-to-datapackage-setstorageitem
                    if (filesToShare.Count > 0)
                        dp.SetStorageItems(filesToShare, false);

                    break;
            }

            deferral.Complete();
        }


        private async Task<StorageFile> GetPathAsync(string path)
        {
            if (Path.IsPathRooted(Content.ToString()))
            {
                //Absolute Path => "C:\Temp\myfile.txt"
                return await StorageFile.GetFileFromPathAsync(path);
            }
            else
            {
                //Relative Path => "myfile.txt" or "..\temp\mytext.txt"
                return await StorageFile.GetFileFromPathAsync(Path.GetFullPath(path));
            }
        }

        public void Share(object content, ShareMode shareMode)
        {
            Mode = shareMode;
            Content = content;
            //IntPtr hwnd = new WindowInteropHelper(window).Handle;
            //Debug.WriteLine("IntPtr window:" + hwnd.ToString());
            DataTransferManagerHelper.ShowShareUIForWindow(this.hwnd);
        }

    }
}
