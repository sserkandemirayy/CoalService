namespace Application.Common
{
    public static class RowVersionUtils
    {
        // Postgres xmin (uint) -> byte[] (endianness güvenli)
        public static byte[] ToBytes(uint value)
        {
            var b = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian) Array.Reverse(b);
            return b;
        }

        // Body/If-Match'ten gelen byte[] ile current uint karşılaştır
        public static bool Equals(byte[]? incoming, uint current)
        {
            if (incoming is null || incoming.Length == 0) return false;
            var cur = ToBytes(current);
            return incoming.AsSpan().SequenceEqual(cur);
        }

        // DTO için Base64 üretimi
        public static string ToBase64(uint value) => Convert.ToBase64String(ToBytes(value));
    }
}