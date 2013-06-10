using System.Drawing;
using System.Windows.Forms;

namespace AddStepFile
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ShellFileIcon
    {
        public enum FileIconSize
        {
            Small,    // 16x16 pixels
            Large   // 32x32 pixels
        }

        private ShellFileIcon()
        {
        }

        public static Image GetFileIcon(string fullpath)
        {
            return GetFileIcon(fullpath, FileIconSize.Large);
        }

        public static Image GetFileIcon(string fullpath, FileIconSize size)
        {
            UnsafeNativeMethods.SHFILEINFO info = new UnsafeNativeMethods.SHFILEINFO();

            uint flags = UnsafeNativeMethods.SHGFI_USEFILEATTRIBUTES | UnsafeNativeMethods.SHGFI_ICON;
            if (size == FileIconSize.Small)
            {
                flags |= UnsafeNativeMethods.SHGFI_SMALLICON;
            }

            int retval = UnsafeNativeMethods.SHGetFileInfo(fullpath, UnsafeNativeMethods.FILE_ATTRIBUTE_NORMAL,
              ref info, System.Runtime.InteropServices.Marshal.SizeOf(info), flags);
            if (retval == 0)
            {
                return null;  // error occured
            }

            System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(info.hIcon);
            ImageList imglist = new ImageList();
            imglist.ImageSize = icon.Size;
            imglist.Images.Add(icon);
            Image image = imglist.Images[0];
            icon.Dispose();
            return image;
        }
    }
}
