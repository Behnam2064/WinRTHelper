namespace WinRTHelper.ScaningApi
{
    public enum ImageScannerFormatHelper
    {
        //
        // Summary:
        //     Exchangeable image file format/EXIF or JPEG file interchange format/JFIF Use
        //     these files only in color or grayscale modes (8 bits per channel/sample), with
        //     single page and compressed (lossy JPEG compression).
        Jpeg,
        //
        // Summary:
        //     Portable Network Graphics (PNG) image file format This value supports any color
        //     mode, with single page and compressed (loseless PNG compression).
        Png,
        //
        // Summary:
        //     Windows Device Independent Bitmap (DIB) This value supports any color mode, with
        //     single page and uncompressed. This is the only format that is supported by all
        //     compatible WIA 2.0 scanner devices.
        DeviceIndependentBitmap,
        //
        // Summary:
        //     Uncompressed Tagged Image File Format (TIFF) compatible with the TIFF 6.0 specification,
        //     either single and/or multi-page These files can be in any color mode supported
        //     by this API, always uncompressed and either single page (if only one image has
        //     to be transferred) or multi-page (if multiple images have to be transferred and
        //     the device supports this multi-page file format).
        Tiff,
        //
        // Summary:
        //     Microsoft XML Paper Specification (XPS) document file format These files can
        //     contain image data in any color mode supported by this API, compressed at the
        //     choice of the device, either single or multi-page.
        Xps,
        //
        // Summary:
        //     Open XML Paper Specification (OpenXPS) document file format These files can contain
        //     image data in any color mode supported by this API, compressed at the choice
        //     of the device, either single or multi-page.
        OpenXps,
        //
        // Summary:
        //     Portable Document Format PDF/A (PDF/A is an ISO-standardized version of the Portable
        //     Document Format/PDF specialized for the digital preservation of electronic documents)
        //     document file format These files can contain image data in any color mode supported
        //     by this API, compressed at the choice of the device, either single or multi-page.
        Pdf
    }
}
