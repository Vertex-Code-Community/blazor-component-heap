namespace Bch.Modules.Files;

public static class FileSizeFormatter
{
    public static string FormatFileSize(long bytes)
    {
        const long kb = 1024;
        const long mb = kb * 1024;
        const long gb = mb * 1024;
        const long tb = gb * 1024;

        if (bytes < kb)
            return $"{bytes}b";
        else if (bytes < mb)
            return $"{((double)bytes / kb):0.00}KB";
        else if (bytes < gb)
            return $"{((double)bytes / mb):0.00}MB";
        else if (bytes < tb)
            return $"{((double)bytes / gb):0.00}GB";
        else
            return $"{((double)bytes / tb):0.00}TB";
    }
}