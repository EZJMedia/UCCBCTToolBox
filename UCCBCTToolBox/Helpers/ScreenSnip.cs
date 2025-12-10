using System;
using System.Drawing;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace UCCBCTToolBox.Helpers
{
    class ScreenSnip
    {
        public static Bitmap CopyScreen(int left, int top, int right, int bottom)
        {
            //System.Windows.Forms.MessageBox.Show("Left: " + left.ToString() + ", Top: " + top.ToString() + ", Right: " + right.ToString() + ", Bottom: " + bottom.ToString());
            Bitmap screenBmp = new((right - left),
                                             (bottom - top),
                                             PixelFormat.Format32bppArgb);
            Graphics bmpGraphics = Graphics.FromImage(screenBmp);
            bmpGraphics.CopyFromScreen(left, top, 0, 0, screenBmp.Size);
            return screenBmp;
        }
    }
}
