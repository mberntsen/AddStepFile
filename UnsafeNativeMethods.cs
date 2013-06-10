using System;
using System.Runtime.InteropServices;

namespace AddStepFile
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class UnsafeNativeMethods
    {
        private UnsafeNativeMethods()
        {
        }

        public enum MenuFlags
        {
            MF_INSERT = 0x00000000,
            MF_CHANGE = 0x00000080,
            MF_APPEND = 0x00000100,
            MF_DELETE = 0x00000200,
            MF_REMOVE = 0x00001000,
            MF_BYCOMMAND = 0x00000000,
            MF_BYPOSITION = 0x00000400,
            MF_SEPARATOR = 0x00000800,
            MF_ENABLED = 0x00000000,
            MF_GRAYED = 0x00000001,
            MF_DISABLED = 0x00000002,
            MF_UNCHECKED = 0x00000000,
            MF_CHECKED = 0x00000008,
            MF_USECHECKBITMAPS = 0x00000200,
            MF_STRING = 0x00000000,
            MF_BITMAP = 0x00000004,
            MF_OWNERDRAW = 0x00000100,
            MF_POPUP = 0x00000010,
            MF_MENUBARBREAK = 0x00000020,
            MF_MENUBREAK = 0x00000040,
            MF_UNHILITE = 0x00000000,
            MF_HILITE = 0x00000080,
        }

        public const uint SHGFI_ICON = 0x100;                         // get icon
        public const uint SHGFI_DISPLAYNAME = 0x200;                  // get display name
        public const uint SHGFI_TYPENAME = 0x400;                     // get type name
        public const uint SHGFI_ATTRIBUTES = 0x800;                   // get attributes
        public const uint SHGFI_ICONLOCATION = 0x1000;                // get icon location
        public const uint SHGFI_EXETYPE = 0x2000;                     // return exe type
        public const uint SHGFI_SYSICONINDEX = 0x4000;                // get system icon index
        public const uint SHGFI_LINKOVERLAY = 0x8000;                 // put a link overlay on icon
        public const uint SHGFI_SELECTED = 0x10000;                   // show icon in selected state
        public const uint SHGFI_LARGEICON = 0x0;                      // get large icon
        public const uint SHGFI_SMALLICON = 0x1;                      // get small icon
        public const uint SHGFI_OPENICON = 0x2;                       // get open icon
        public const uint SHGFI_SHELLICONSIZE = 0x4;                  // get shell size icon
        public const uint SHGFI_PIDL = 0x8;                           // pszPath is a pidl
        public const uint SHGFI_USEFILEATTRIBUTES = 0x10;             // use passed dwFileAttribute

        public static uint FILE_ATTRIBUTE_NORMAL = 0x80;

        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public int dwAttributes;
            public string szDisplayName;
            public string szTypeName;
        }

        public const int WM_SYSCOMMAND = 0x0112;

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hwnd, int bRevert);

        [DllImport("user32.dll")]
        public static extern bool AppendMenu(IntPtr hMenu,
          MenuFlags uFlags, uint uIDNewItem, String lpNewItem);

        [DllImport("Shell32.dll")]
        public static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, int cbfileInfo, uint uFlags);

    }
}
