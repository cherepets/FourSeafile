namespace FourSeafile.Extensions
{
    public static class StringExt
    {
        public static bool HasExt(this string s, params string[] exts)
        {
            foreach (var ext in exts)
                if (s.EndsWith($".{ext}")) return true;
            return false;
        }
    }
}
