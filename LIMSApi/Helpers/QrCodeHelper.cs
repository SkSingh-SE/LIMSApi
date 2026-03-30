using QRCoder;

namespace LIMSApi.Helpers
{
    /// <summary>
    /// Generates QR code PNG images dynamically using QRCoder library.
    /// </summary>
    public static class QrCodeHelper
    {
        /// <summary>
        /// Generates a QR code PNG as byte array from the given data string (typically a URL).
        /// </summary>
        /// <param name="data">The text/URL to encode in the QR code</param>
        /// <param name="pixelsPerModule">Size of each QR module in pixels (default 10)</param>
        /// <returns>PNG image as byte array, or null if data is empty</returns>
        public static byte[]? GenerateQrPng(string? data, int pixelsPerModule = 10)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
            using var qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(pixelsPerModule);
        }
    }
}
