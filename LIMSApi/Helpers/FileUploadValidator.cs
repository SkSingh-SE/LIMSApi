namespace LIMSApi.Helpers
{
    /// <summary>
    /// Centralized file upload validation. Used by FileUploadService and any controller
    /// that accepts IFormFile. All rules in one place — never hardcode validation elsewhere.
    /// </summary>
    public static class FileUploadValidator
    {
        public const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        /// <summary>
        /// Allowed extensions mapped to their expected MIME content-type prefixes.
        /// Extension check + magic-byte/content-type check = defense in depth.
        /// </summary>
        private static readonly Dictionary<string, string[]> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            // Documents
            { ".pdf",  new[] { "application/pdf" } },
            { ".doc",  new[] { "application/msword" } },
            { ".docx", new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml" } },
            { ".xls",  new[] { "application/vnd.ms-excel" } },
            { ".xlsx", new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml" } },
            { ".csv",  new[] { "text/csv", "application/vnd.ms-excel", "text/plain" } },
            { ".txt",  new[] { "text/plain" } },

            // Images
            { ".jpg",  new[] { "image/jpeg" } },
            { ".jpeg", new[] { "image/jpeg" } },
            { ".png",  new[] { "image/png" } },
            { ".gif",  new[] { "image/gif" } },
            { ".bmp",  new[] { "image/bmp" } },
            { ".tiff", new[] { "image/tiff" } },
            { ".tif",  new[] { "image/tiff" } },

            // Archives (for batch attachments)
            { ".zip",  new[] { "application/zip", "application/x-zip-compressed" } },

            // Video (SOP videos for Equipment module)
            { ".mp4",  new[] { "video/mp4" } },
        };

        /// <summary>
        /// Dangerous extensions that must NEVER be accepted, regardless of MIME type.
        /// Prevents attackers renaming malicious files.
        /// </summary>
        private static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".exe", ".bat", ".cmd", ".ps1", ".sh", ".msi", ".com", ".scr", ".pif",
            ".aspx", ".asp", ".php", ".jsp", ".cgi", ".py", ".rb",
            ".dll", ".vbs", ".vbe", ".js", ".wsf", ".wsh",
            ".reg", ".inf", ".hta", ".msp", ".mst",
            ".cpl", ".ins", ".isp", ".jse", ".lnk", ".sct", ".shb", ".shs",
            ".ws", ".wsc", ".cshtml", ".config", ".sql"
        };

        /// <summary>
        /// Magic byte signatures for common file types.
        /// Validates actual file content — not just the extension which can be faked.
        /// </summary>
        private static readonly Dictionary<string, byte[][]> MagicBytes = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".pdf",  new[] { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },                        // %PDF
            { ".png",  new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } }, // PNG header
            { ".jpg",  new[] { new byte[] { 0xFF, 0xD8, 0xFF } } },                               // JPEG
            { ".jpeg", new[] { new byte[] { 0xFF, 0xD8, 0xFF } } },                               // JPEG
            { ".gif",  new[] { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },                         // GIF8
            { ".zip",  new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },                         // PK..
            { ".xlsx", new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },                         // ZIP (OOXML)
            { ".docx", new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },                         // ZIP (OOXML)
            { ".doc",  new[] { new byte[] { 0xD0, 0xCF, 0x11, 0xE0 } } },                         // OLE Compound
            { ".xls",  new[] { new byte[] { 0xD0, 0xCF, 0x11, 0xE0 } } },                         // OLE Compound
            { ".bmp",  new[] { new byte[] { 0x42, 0x4D } } },                                     // BM
            { ".mp4",  new[] { new byte[] { 0x00, 0x00, 0x00 } } },                               // ftyp (offset 4)
        };

        /// <summary>
        /// Validates a file upload. Returns null if valid, or an error message string.
        /// Call this from any service or controller that handles file uploads.
        /// </summary>
        public static async Task<string?> ValidateAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return "File is required and cannot be empty.";

            if (file.Length > MaxFileSizeBytes)
                return $"File size ({file.Length / (1024 * 1024.0):F1} MB) exceeds the maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB.";

            // Sanitize: take only the file name, strip any path components
            var fileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            if (string.IsNullOrEmpty(extension))
                return "File must have an extension.";

            // Step 1: Block dangerous extensions
            if (BlockedExtensions.Contains(extension))
                return $"File type '{extension}' is blocked for security reasons.";

            // Step 2: Check against allowed list
            if (!AllowedTypes.ContainsKey(extension))
                return $"File type '{extension}' is not supported. Allowed types: {string.Join(", ", AllowedTypes.Keys.Order())}.";

            // Step 3: MIME content-type check (what the browser reports)
            var allowedMimes = AllowedTypes[extension];
            var contentType = file.ContentType?.ToLowerInvariant() ?? "";
            bool mimeMatch = allowedMimes.Any(m => contentType.StartsWith(m, StringComparison.OrdinalIgnoreCase));

            // Allow application/octet-stream as fallback (some browsers send this)
            if (!mimeMatch && contentType != "application/octet-stream")
                return $"File content type '{file.ContentType}' does not match expected type for '{extension}'.";

            // Step 4: Magic byte validation (actual file content check)
            if (MagicBytes.TryGetValue(extension, out var signatures))
            {
                var headerValid = await CheckMagicBytesAsync(file, signatures);
                if (!headerValid)
                    return $"File content does not match the '{extension}' format. The file may be corrupted or mislabeled.";
            }

            return null; // Valid
        }

        /// <summary>
        /// Reads the first bytes of the file and compares against known magic byte signatures.
        /// This catches renamed malicious files (e.g., malware.exe renamed to report.pdf).
        /// </summary>
        private static async Task<bool> CheckMagicBytesAsync(IFormFile file, byte[][] signatures)
        {
            var maxLen = signatures.Max(s => s.Length);
            var buffer = new byte[maxLen];

            using var stream = file.OpenReadStream();
            var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, maxLen));

            if (bytesRead < signatures.Min(s => s.Length))
                return false;

            return signatures.Any(sig =>
                bytesRead >= sig.Length &&
                buffer.AsSpan(0, sig.Length).SequenceEqual(sig));
        }

        /// <summary>
        /// Returns the list of allowed extensions for display in error messages or documentation.
        /// </summary>
        public static IReadOnlyCollection<string> GetAllowedExtensions() => AllowedTypes.Keys;
    }
}
