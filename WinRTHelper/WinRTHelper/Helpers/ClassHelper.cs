using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Scanners;
using Windows.Storage.Streams;
using WinRTHelper.ScaningApi;

namespace WinRTHelper.Helpers
{
    public class ClassHelper
    {

        public async Task<MemoryStream> ConvertToMemoryStream(InMemoryRandomAccessStream inMemoryStream)
        {
            // Convert the InMemoryRandomAccessStream to a .NET Stream
            Stream netStream = inMemoryStream.AsStream();

            // Create a MemoryStream to hold the data
            MemoryStream memoryStream = new MemoryStream();

            // Copy the data from the .NET Stream to the MemoryStream
            await netStream.CopyToAsync(memoryStream);

            // Reset the position of the MemoryStream to the beginning
            memoryStream.Position = 0;

            return memoryStream;
        }

        public ImageScannerFormatHelper CastImageScannerFormatHelper(ImageScannerFormat imageScannerFormat)
        {
            return (ImageScannerFormatHelper)Enum.Parse(typeof(ImageScannerFormatHelper), imageScannerFormat.ToString());
        }

        public ImageScannerFormat CastImageScannerFormat(ImageScannerFormatHelper imageScannerFormat)
        {
            return (ImageScannerFormat)Enum.Parse(typeof(ImageScannerFormat), imageScannerFormat.ToString());
        }
    }
}
