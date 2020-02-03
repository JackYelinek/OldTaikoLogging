using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaikoLogging
{
    class Program
    {
        static public RinClient rin = new RinClient();
        static public GoogleSheetInterface sheet = new GoogleSheetInterface();
        static public bool twitchOn;
        static public ScreenGrab screen = new ScreenGrab();
        static public ImageAnalysis analysis = new ImageAnalysis();
        static public DebugLogging logger = new DebugLogging();
        static public Emulator.EmulatorLogger emulatorLogger = new Emulator.EmulatorLogger();
        static public Commands commands = new Commands();

        public enum Game { PS4, Emulator, None };
        // PS4 or Emulator, so I can know at any place
        static public Game currentGame = Game.None;

        static void Main(string[] args)
        {
            Thread inputThread = new Thread(ReadInput);
            inputThread.Start();

            GetGame();

            //PlayRecordingSheetsInterface playRecordingSheetsInterface = new PlayRecordingSheetsInterface();
            //ScreenGrab newScreenGrab = new ScreenGrab();

            //var bmp = newScreenGrab.CaptureApplication();
            //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\TestingRecording\CaptureApplicationTest.png");

            while (true)
            {
                GetGame();

                twitchOn = screen.CheckTwitch();

                if (currentGame == Game.PS4)
                {
                    // This part is just temporarily if I'm streaming and the function's broken (which it currently isn't)
                    //twitchOn = false;

                    analysis.StandardLoop();
                    //analysis.NotStandardLoop();

                    //{
                    //    analysis.SingleLoop();
                    //    Thread.Sleep(100000000);
                    //    break;
                    //}
                }
                else if (currentGame == Game.Emulator)
                {
                    emulatorLogger.StandardLoop();
                    //{
                    //    emulatorLogger.SingleLoop();
                    //    Thread.Sleep(100000000);
                    //    break;
                    //}
                }
            }


        }

        static void GetGame()
        {
            var obsProcesses = Process.GetProcessesByName("obs64");
            var tjaProcesses = Process.GetProcessesByName("TJAPlayer3");
            if (tjaProcesses.Length != 0)
            {
                currentGame = Game.Emulator;
            }
            else if (obsProcesses.Length != 0)
            {
                currentGame = Game.PS4;
            }
            else
            {
                currentGame = Game.None;
            }
        }

        static void ReadInput()
        {
            while(true)
            {
                var input = Console.ReadLine();

                commands.CheckCommands(input);
            }
        }

        public static string MakeValidFileName(string name)
        {
            // This isn't really a sheet function, but I didn't know where to put it that made sense
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
