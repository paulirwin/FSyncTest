using System.Runtime.InteropServices;

namespace FSyncTest;

public partial class FSyncWrapper
{
    // https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateFileW(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile
    );

    // https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-flushfilebuffers
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FlushFileBuffers(IntPtr hFile);

    // https://learn.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-closehandle
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    public static void FSyncDirWindows(string dir)
    {
        IntPtr hFile = CreateFileW(dir,
            0x80000000, // GENERIC_READ, see https://learn.microsoft.com/en-us/windows/win32/secauthz/generic-access-rights
            0x00000001, // FILE_SHARE_READ
            IntPtr.Zero,
            3, // OPEN_EXISTING
            0x02000000, // FILE_FLAG_BACKUP_SEMANTICS required to open a directory
            IntPtr.Zero);

        if (hFile == INVALID_HANDLE_VALUE)
        {
            throw new IOException("Unable to open directory", Marshal.GetLastWin32Error());
        }

        if (!FlushFileBuffers(hFile))
        {
            throw new IOException($"FlushFileBuffers failed, 0x{Marshal.GetLastWin32Error():x8}", Marshal.GetLastWin32Error());
        }

        if (!CloseHandle(hFile))
        {
            throw new IOException("CloseHandle failed", Marshal.GetLastWin32Error());
        }
    }
}
