using System.Runtime.InteropServices;

namespace FSyncTest;

public partial class FSyncWrapper
{
    // https://pubs.opengroup.org/onlinepubs/009695399/functions/fsync.html
    [DllImport("libc", SetLastError = true)]
    private static extern int fsync(int fd);

    // https://pubs.opengroup.org/onlinepubs/007904875/functions/open.html
    [DllImport("libc", SetLastError = true)]
    private static extern int open([MarshalAs(UnmanagedType.LPStr)] string pathname, int flags);

    // https://pubs.opengroup.org/onlinepubs/009604499/functions/close.html
    [DllImport("libc", SetLastError = true)]
    private static extern int close(int fd);

    // https://pubs.opengroup.org/onlinepubs/007904975/functions/fcntl.html
    // and https://developer.apple.com/library/archive/documentation/System/Conceptual/ManPages_iPhoneOS/man2/fcntl.2.html
    [DllImport("libc", SetLastError = true)]
    private static extern int fcntl(int fd, int cmd, int arg);

    private const int O_RDONLY = 0;

    // https://opensource.apple.com/source/xnu/xnu-6153.81.5/bsd/sys/fcntl.h.auto.html
    private const int F_FULLFSYNC = 51;

    public static void FSyncDirPosix(string dir)
    {
        int fd = open(dir, O_RDONLY);
        if (fd == -1)
        {
            throw new IOException("Unable to open directory", Marshal.GetLastPInvokeError());
        }

        // if macOS, use F_FULLFSYNC
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            if (fcntl(fd, F_FULLFSYNC, 0) == -1)
            {
                throw new IOException("fcntl failed", Marshal.GetLastPInvokeError());
            }
        }
        else if (fsync(fd) == -1)
        {
            throw new IOException("fsync failed", Marshal.GetLastPInvokeError());
        }

        if (close(fd) == -1)
        {
            throw new IOException("close failed", Marshal.GetLastPInvokeError());
        }
    }
}
