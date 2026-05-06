// Infrastructure/Utils/ETagHelper.cs
using System;

namespace Infrastructure.Utils
{
    public static class ETagHelper
    {
        // Weak ETag format: W/"<base64>"
        public static string GenerateETag(byte[] rowVersion)
        {
            var b64 = Convert.ToBase64String(rowVersion ?? Array.Empty<byte>());
            return $"W/\"{b64}\"";
        }

        public static byte[]? ParseIfMatch(string? ifMatchHeader)
        {
            if (string.IsNullOrWhiteSpace(ifMatchHeader)) return null;

            // Accept: W/"xxx" or "xxx"
            var v = ifMatchHeader.Trim();
            if (v.StartsWith("W/")) v = v.Substring(2).Trim();

            if (v.Length >= 2 && v.StartsWith("\"") && v.EndsWith("\""))
                v = v.Substring(1, v.Length - 2);

            try
            {
                return Convert.FromBase64String(v);
            }
            catch
            {
                return null;
            }
        }
    }
}