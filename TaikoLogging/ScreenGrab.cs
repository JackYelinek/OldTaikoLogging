using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging
{
    class ScreenGrab
    {
        public int i = 0;
        Process proc;
        User32.Rect rect = new User32.Rect();

        public int topOffset = 0;
        public int leftOffset = 0;
        public int bottomOffset = 0;
        public int rightOffset = 0;




        public ScreenGrab()
        {
            Setup();
        }

        public ScreenGrab(bool testing)
        {
            Setup();
            //FindGameWindow();
        }

        // Can I make this work without opening OBS for testing
        // Yup, with the 100% expert level code
        // I am a literal god
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
            }
            // I probably don't need to care about the outer box, just the middle box
            User32.GetWindowRect(proc.MainWindowHandle, ref rect);
            var bmp = new Bitmap(rect.right - rect.left, rect.bottom - rect.top, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            // I had rect.left = leftOffset, rect.top + topOffset here
            // That's wrong because when setting up, I need to check things from before it has any offsets
            graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(rect.right - rect.left, rect.bottom - rect.top), CopyPixelOperation.SourceCopy);
            FindGameWindow(bmp);
        }

        // Won't save the image, just returns the bitmap
        public Bitmap CaptureApplication()
        {

            User32.GetWindowRect(proc.MainWindowHandle, ref rect);
            var bmp = new Bitmap(rightOffset - leftOffset, bottomOffset - topOffset, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(rect.left + leftOffset, rect.top + topOffset, 0, 0, new Size(rightOffset - leftOffset, bottomOffset - topOffset), CopyPixelOperation.SourceCopy);
            return bmp;
        }

        // Put any bool in order to save the bitmap
        public Bitmap CaptureApplication(bool Testing)
        {
            Bitmap bmp = CaptureApplication();

            // NOT TESTING (technically testing, but this function is only for testing anyway
            bmp.Save(string.Format(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\test{0}.png", i++.ToString()), ImageFormat.Png);
            Console.WriteLine("Picture Taken");
            return bmp;
        }

        public void FindGameWindow(Bitmap bmp)
        {
            // TESTING
            //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\baseWindow.png");

            //var outerBoxColor = Color.FromArgb(240, 240, 240);
            var middleBoxColor = Color.FromArgb(76, 76, 76);


            // To find the game box, go from the middle top down in a column until I reach the middle box color, then when it stops being that color, that's the top of the box
            // Then do the same from the middle left, middle bottom, and middle right
            bool topMiddleBox = false;
            for (int i = 0; i < bmp.Height; i++)
            {
                if (CompareColors(bmp.GetPixel(bmp.Width / 2, i), middleBoxColor) != topMiddleBox)
                {
                    topMiddleBox = !topMiddleBox;
                    if (topMiddleBox == false)
                    {
                        topOffset = i;
                        break;
                    }
                }
            }
            bool leftMiddleBox = false;
            for (int i = 0; i < bmp.Width; i++)
            {
                if (CompareColors(bmp.GetPixel(i, bmp.Height / 2), middleBoxColor) != leftMiddleBox)
                {
                    leftMiddleBox = !leftMiddleBox;
                    if (leftMiddleBox == false)
                    {
                        leftOffset = i;
                        break;
                    }
                }
            }
            bool bottomMiddleBox = false;
            for (int i = bmp.Height-1; i > 0; i--)
            {
                if (CompareColors(bmp.GetPixel(bmp.Width / 2, i), middleBoxColor) != bottomMiddleBox)
                {
                    bottomMiddleBox = !bottomMiddleBox;
                    if (bottomMiddleBox == false)
                    {
                        bottomOffset = i;
                        break;
                    }
                }
            }
            bool rightMiddleBox = false;
            for (int i = bmp.Width-1; i > 0; i--)
            {
                if (CompareColors(bmp.GetPixel(i, bmp.Height / 2), middleBoxColor) != rightMiddleBox)
                {
                    rightMiddleBox = !rightMiddleBox;
                    if (rightMiddleBox == false)
                    {
                        rightOffset = i;
                        break;
                    }
                }
            }


            Console.WriteLine(topOffset);
            Console.WriteLine(leftOffset);
            Console.WriteLine(bottomOffset);
            Console.WriteLine(rightOffset);

            // top left of the outer box should always be (16,60)
            //Console.ReadLine();

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
            User32.GetWindowRect(proc.MainWindowHandle, ref rect);
            var bmp = new Bitmap(rect.right - rect.left, rect.bottom - rect.top, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(rect.right - rect.left, rect.bottom - rect.top), CopyPixelOperation.SourceCopy);

            return AnalyzeTwitch(bmp);
        }

        // I'm literally the worst at naming functions...
        public bool AnalyzeTwitch(Bitmap bmp)
        {
            // TESTING
            //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + i++.ToString() + ".png");

            // Pixel location of "Start Streaming" button:
            // x: rect.Right - (rect.Right-1137)
            // y: bottomOffset + 726-666
            int x = bmp.Width - 21;
            // y is wrong, it can vary relative to the bottomOffset
            // exit can be found at 827
            // y would be at 726
            // y is -101 compared to exit
            // -101 is the only number I need to know to find y

            int y = bmp.Height - 1;
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

            var thing = proc.MainWindowTitle;
            if (thing.IndexOf("- Profile: Twitch -") != -1 && streaming == true)
            {
                return true;
            }
            return false;
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        }
    }
}
