using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging
{
    class ScreenGrab
    {
        Process proc;
        IntPtr windowHandle = (IntPtr)0;
        IntPtr programWindowHandle = (IntPtr)0;

        public ScreenGrab()
        {
            Setup();
        }

        private void Setup()
        {
            var processes = Process.GetProcessesByName("obs64");
            if (processes.Length == 0)
            {
                return;
            }
            else
            {
                proc = processes[0];
                windowHandle = FindWindow("Qt5QWindowIcon", "obs64");
                programWindowHandle = FindWindow("Qt5QWindowIcon", "OBS");
            }

            while (windowHandle == (IntPtr)0)
            {
                Console.WriteLine("Open the Fullscreen Preview in OBS!\npress any key to continue...");
                Console.ReadKey();
                windowHandle = FindWindow("Qt5QWindowIcon", "obs64");
            }
            if (proc.MainWindowTitle != "obs64")
            {
                programWindowHandle = FindWindow("Qt5QWindowIcon", proc.MainWindowTitle);
            }
            while (programWindowHandle == (IntPtr)0)
            {
                Console.WriteLine("Click on the main OBS window!\npress any key to continue...");
                Console.ReadKey();
                if (proc.MainWindowTitle != "obs64")
                {
                    programWindowHandle = FindWindow("Qt5QWindowIcon", proc.MainWindowTitle);
                }
            }

            Console.WriteLine("Screen Setup Complete!");
        }

        public Bitmap CaptureApplication()
        {
            var bmp = PrintWindow(windowHandle);

            return bmp;
        }

        private bool CompareColors(Color pixelColor, Color boxColor)
        {
            // Compare the pixel to what the color should be
            // true = they are the same, false = not the same

            if (pixelColor.R == boxColor.R && pixelColor.G == boxColor.G && pixelColor.B == boxColor.B)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckTwitch()
        {
            if (programWindowHandle != (IntPtr)0)
            {
                var bmp = PrintWindow(programWindowHandle);
                return AnalyzeTwitch(bmp);
            }
            else
            {
                return false;
            }
        }

        // I'm literally the worst at naming functions...
        public bool AnalyzeTwitch(Bitmap bmp)
        {
            // Pixel location of "Start Streaming" button:
            // x: rect.Right - (rect.Right-1137)
            // y: bottomOffset + 726-666
            int x = bmp.Width - 21;
            // y is wrong, it can vary relative to the bottomOffset
            // exit can be found at 827
            // y would be at 726
            // y is -101 compared to exit
            // -101 is the only number I need to know to find y

            int y = bmp.Height - 2;
            for (; y >= 0; y--)
            {
                var exitPixel = bmp.GetPixel(x, y);
                if (CompareColors(Color.FromArgb(exitPixel.R, exitPixel.G, exitPixel.B), Color.FromArgb(76, 76, 76)))
                {
                    y -= 101;
                    break;
                }
            }


            // Color if not streaming = (76, 76, 76)
            // Color if streaming = (122,121,122)
            if (y < 0)
            {
                return Program.twitchOn;
            }
            var pixel = bmp.GetPixel(x, y);
            bool streaming = CompareColors(Color.FromArgb(pixel.R, pixel.G, pixel.B), Color.FromArgb(122, 121, 122));

            var windowTitle = proc.MainWindowTitle;
            if (windowTitle.IndexOf("- Profile: Twitch -") != -1 && streaming == true)
            {
                return true;
            }
            return false;
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        public static Bitmap PrintWindow(IntPtr hwnd)
        {
            RECT rc;
            GetWindowRect(hwnd, out rc);

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            PrintWindow(hwnd, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            private int _Left;
            private int _Top;
            private int _Right;
            private int _Bottom;

            public RECT(RECT Rectangle) : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
            {
            }
            public RECT(int Left, int Top, int Right, int Bottom)
            {
                _Left = Left;
                _Top = Top;
                _Right = Right;
                _Bottom = Bottom;
            }

            public int X
            {
                get { return _Left; }
                set { _Left = value; }
            }
            public int Y
            {
                get { return _Top; }
                set { _Top = value; }
            }
            public int Left
            {
                get { return _Left; }
                set { _Left = value; }
            }
            public int Top
            {
                get { return _Top; }
                set { _Top = value; }
            }
            public int Right
            {
                get { return _Right; }
                set { _Right = value; }
            }
            public int Bottom
            {
                get { return _Bottom; }
                set { _Bottom = value; }
            }
            public int Height
            {
                get { return _Bottom - _Top; }
                set { _Bottom = value + _Top; }
            }
            public int Width
            {
                get { return _Right - _Left; }
                set { _Right = value + _Left; }
            }
            public Point Location
            {
                get { return new Point(Left, Top); }
                set
                {
                    _Left = value.X;
                    _Top = value.Y;
                }
            }
            public Size Size
            {
                get { return new Size(Width, Height); }
                set
                {
                    _Right = value.Width + _Left;
                    _Bottom = value.Height + _Top;
                }
            }

            public static implicit operator Rectangle(RECT Rectangle)
            {
                return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
            }
            public static implicit operator RECT(Rectangle Rectangle)
            {
                return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
            }
            public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
            {
                return Rectangle1.Equals(Rectangle2);
            }
            public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
            {
                return !Rectangle1.Equals(Rectangle2);
            }

            public override string ToString()
            {
                return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public bool Equals(RECT Rectangle)
            {
                return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
            }

            public override bool Equals(object Object)
            {
                if (Object is RECT)
                {
                    return Equals((RECT)Object);
                }
                else if (Object is Rectangle)
                {
                    return Equals(new RECT((Rectangle)Object));
                }

                return false;
            }
        }
    }
}
