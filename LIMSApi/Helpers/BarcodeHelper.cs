using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LIMSApi.Helpers
{
    /// <summary>
    /// Generates ISO/IEC 15417 Code 128 (Subset B) barcode images and SVG markup.
    /// Pure C# implementation with zero external dependencies.
    /// Compatible with QuestPDF image rendering and HTML/SVG web previews.
    /// </summary>
    public static class BarcodeHelper
    {
        // Standard Code 128 patterns (Indices 0 to 106)
        // Each pattern consists of alternating widths of bars (B) and spaces (S)
        private static readonly string[] Patterns = new[]
        {
            "212222", "222122", "222221", "121223", "121322", "131222", "122213", "122312", "132212", "221213",
            "221312", "231212", "112232", "122132", "122231", "113222", "123122", "123221", "223211", "221132",
            "221231", "213212", "223112", "312131", "311222", "321122", "321221", "312212", "322112", "322211",
            "212123", "212321", "232121", "111323", "131123", "131321", "112313", "132113", "132311", "211313",
            "231113", "231311", "112133", "112331", "132131", "113123", "113321", "133121", "313121", "211331",
            "231131", "213113", "213311", "213131", "311123", "311321", "331121", "312113", "312311", "332111",
            "314111", "221411", "431111", "111224", "111422", "121124", "121421", "141122", "141221", "112214",
            "112412", "122114", "122411", "142112", "142211", "241211", "221114", "413111", "241112", "134111",
            "111242", "121142", "121241", "114212", "124112", "124211", "411212", "421112", "421211", "212141",
            "214121", "412121", "111143", "111341", "131141", "114113", "114311", "411113", "411311", "113141",
            "114131", "311141", "411131", "211412", "211214", "211232", "2331112"
        };

        private const int StartCodeB = 104;
        private const int StopCode = 106;

        /// <summary>
        /// Encodes text into Code128 modules (boolean array where true = black bar, false = white space).
        /// Includes quiet zones (10 modules on each side), Start Code B, payload, checksum, and Stop Code.
        /// </summary>
        public static List<bool> EncodeCode128B(string text)
        {
            var modules = new List<bool>();
            if (string.IsNullOrEmpty(text))
                text = "0";

            // Quiet zone left (10 modules)
            for (int i = 0; i < 10; i++) modules.Add(false);

            // Start Code B (Pattern 104)
            AppendPattern(modules, Patterns[StartCodeB]);

            int checksum = StartCodeB;
            int position = 1;

            foreach (char c in text)
            {
                int code = (int)c - 32;
                if (code < 0 || code > 95)
                    code = 0; // Fallback space

                checksum += code * position;
                position++;

                AppendPattern(modules, Patterns[code]);
            }

            // Checksum symbol
            checksum %= 103;
            AppendPattern(modules, Patterns[checksum]);

            // Stop Code (Pattern 106)
            AppendPattern(modules, Patterns[StopCode]);

            // Quiet zone right (10 modules)
            for (int i = 0; i < 10; i++) modules.Add(false);

            return modules;
        }

        private static void AppendPattern(List<bool> modules, string pattern)
        {
            bool isBar = true;
            foreach (char digit in pattern)
            {
                int width = digit - '0';
                for (int w = 0; w < width; w++)
                {
                    modules.Add(isBar);
                }
                isBar = !isBar;
            }
        }

        /// <summary>
        /// Generates an uncompressed 24-bit BMP image byte array for the barcode.
        /// Decodable by SkiaSharp and QuestPDF Image().
        /// </summary>
        public static byte[] GenerateBarcodeBmp(string text, int height = 40, int moduleWidth = 2)
        {
            var modules = EncodeCode128B(text);
            int width = modules.Count * moduleWidth;
            if (height <= 0) height = 40;

            int rowSize = ((width * 3 + 3) / 4) * 4; // 4-byte aligned
            int imageSize = rowSize * height;
            int fileSize = 54 + imageSize;

            byte[] bmp = new byte[fileSize];

            // BMP Header (14 bytes)
            bmp[0] = 0x42; // 'B'
            bmp[1] = 0x4D; // 'M'
            WriteInt32(bmp, 2, fileSize);
            WriteInt32(bmp, 6, 0); // Reserved
            WriteInt32(bmp, 10, 54); // Offset to pixel data

            // DIB Header (BITMAPINFOHEADER - 40 bytes)
            WriteInt32(bmp, 14, 40); // Header size
            WriteInt32(bmp, 18, width); // Width
            WriteInt32(bmp, 22, height); // Height (positive = bottom-up)
            WriteInt16(bmp, 26, 1); // Color planes
            WriteInt16(bmp, 28, 24); // Bits per pixel (24-bit RGB)
            WriteInt32(bmp, 30, 0); // BI_RGB (no compression)
            WriteInt32(bmp, 34, imageSize); // Image data size
            WriteInt32(bmp, 38, 2835); // Pixels/meter horizontal (~72 DPI)
            WriteInt32(bmp, 42, 2835); // Pixels/meter vertical
            WriteInt32(bmp, 46, 0); // Palette colors
            WriteInt32(bmp, 50, 0); // Important colors

            // Pixel Data (Bottom-Up)
            for (int y = 0; y < height; y++)
            {
                int rowOffset = 54 + (y * rowSize);
                int colIdx = 0;

                foreach (bool isBar in modules)
                {
                    byte color = isBar ? (byte)0x00 : (byte)0xFF;
                    for (int w = 0; w < moduleWidth; w++)
                    {
                        int pixelOffset = rowOffset + (colIdx * 3);
                        bmp[pixelOffset] = color;     // Blue
                        bmp[pixelOffset + 1] = color; // Green
                        bmp[pixelOffset + 2] = color; // Red
                        colIdx++;
                    }
                }
            }

            return bmp;
        }

        /// <summary>
        /// Generates a vector SVG barcode markup for HTML/Angular rendering.
        /// </summary>
        public static string GenerateBarcodeSvg(string text, int height = 30, int moduleWidth = 2)
        {
            var modules = EncodeCode128B(text);
            int width = modules.Count * moduleWidth;

            var sb = new StringBuilder();
            sb.Append($"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {width} {height}\" width=\"100%\" height=\"{height}\" preserveAspectRatio=\"none\">");
            sb.Append($"<rect width=\"{width}\" height=\"{height}\" fill=\"#ffffff\"/>");

            int runStart = -1;
            int runLength = 0;

            for (int i = 0; i < modules.Count; i++)
            {
                bool isBar = modules[i];
                if (isBar)
                {
                    if (runStart == -1)
                    {
                        runStart = i * moduleWidth;
                        runLength = moduleWidth;
                    }
                    else
                    {
                        runLength += moduleWidth;
                    }
                }
                else
                {
                    if (runStart != -1)
                    {
                        sb.Append($"<rect x=\"{runStart}\" y=\"0\" width=\"{runLength}\" height=\"{height}\" fill=\"#000000\"/>");
                        runStart = -1;
                        runLength = 0;
                    }
                }
            }

            if (runStart != -1)
            {
                sb.Append($"<rect x=\"{runStart}\" y=\"0\" width=\"{runLength}\" height=\"{height}\" fill=\"#000000\"/>");
            }

            sb.Append("</svg>");
            return sb.ToString();
        }

        private static void WriteInt32(byte[] buffer, int offset, int value)
        {
            buffer[offset] = (byte)(value & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
        }

        private static void WriteInt16(byte[] buffer, int offset, short value)
        {
            buffer[offset] = (byte)(value & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
        }
    }
}
