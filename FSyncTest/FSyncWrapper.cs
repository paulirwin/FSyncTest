using System.Runtime.InteropServices;

namespace FSyncTest;

public partial class FSyncWrapper
{
    public static void FSyncDir(string dir)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            FSyncDirWindows(dir);
        }
        else
        {
            FSyncDirPosix(dir);
        }
    }
}
