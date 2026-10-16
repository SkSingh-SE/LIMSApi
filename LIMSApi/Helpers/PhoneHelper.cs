using System.Text.RegularExpressions;

namespace LIMSApi.Helpers
{
    /// <summary>
    /// Centralized phone number formatting helper.
    ///
    /// The frontend phone-input component stores values as "&lt;country code&gt; &lt;number&gt;"
    /// (e.g. "+91 9876543210"), so the country code is already present in the stored value.
    /// Twilio requires the compact E.164 form (e.g. "+919876543210") with no spaces,
    /// so the space is stripped only at the sending boundary — storage keeps the space.
    /// </summary>
    public static class PhoneHelper
    {
        private const string DefaultCountryCode = "+91";

        /// <summary>
        /// Converts any stored phone value to compact E.164 (e.g. "+919876543210").
        /// Keeps the country code already present in the value instead of hardcoding one.
        /// Legacy bare Indian numbers (e.g. "9876543210") fall back to "+91".
        /// Returns null when the input is empty so callers can skip sending.
        /// </summary>
        public static string? ToE164(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            var cleaned = Regex.Replace(phone.Trim(), @"[\s\-\(\)]", "");

            if (cleaned.StartsWith("+", StringComparison.Ordinal))
                return cleaned;

            var digits = cleaned.TrimStart('+');

            // Legacy value that already carries the country code, e.g. "919876543210"
            if (digits.Length >= 12 && digits.StartsWith("91", StringComparison.Ordinal))
                return "+" + digits;

            // Legacy bare national number, e.g. "9876543210"
            return DefaultCountryCode + digits;
        }
    }
}
