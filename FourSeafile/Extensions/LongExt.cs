using System;

namespace FourSeafile.Extensions
{
    public static class LongExt
    {
        public static string ToSizeString(this long size)
        {
            if (size < 1024) return $"{size} B";
            size /= 1024;
            if (size < 1024) return $"{size} KB";
            size /= 1024;
            if (size < 1024) return $"{size} MB";
            size /= 1024;
            return $"{size} GB";
        }

        private static double GetRounded(long size)
            => Math.Round((double)size, 2);
    }
}
