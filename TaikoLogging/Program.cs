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
        static public bool twitchOn;
        static public ScreenGrab screen = new ScreenGrab();
        static public ImageAnalysis analysis = new ImageAnalysis();
        static public DebugLogging logger = new DebugLogging();
        static public GoogleSheetInterface sheet = new GoogleSheetInterface();
        static public EmulatorLogger emulatorLogger = new EmulatorLogger();

        static void Main(string[] args)
        {
            // TODO Set this up automatically

            bool PS4 = false;
            bool emulator = false;

            Thread inputThread = new Thread(ReadInput);
            inputThread.Start();

            while(true)
            {
                // I'm not sure if this is the best way to check for it, maybe i could just have it check every 10 loops, or 100
                // Events would be nice to set up, but I'm not sure if I could do that, or how I'd do that
                // This isn't a bad way of doing things without events though I think
                // It prevents me from having to wait each time I open the program to input what it's for
                var processes = Process.GetProcessesByName("obs64");
                if (processes.Length == 0)
                {
                    PS4 = false;
                }
                else
                {
                    twitchOn = screen.CheckTwitch();
                    // This isn't technically accurate
                    PS4 = true;
                }
                processes = Process.GetProcessesByName("TJAPlayer3");
                if (processes.Length == 0)
                {
                    emulator = false;
                }
                else
                {
                    // This isn't technically accurate, but it helps to make the previous if/else more accurate
                    // Ideally, I'd check to see if obs looks like what PS4 should look like, and if it's close enough, send it to PS4
                    // That wouldn't really work, since emulator looks like PS4 on obs
                    // I could check to see if it starts to fail a bunch of the data checks I guess? But that would require a lot of checks, and some failed data gathering
                    emulator = true;
                    PS4 = true;
                }
                if (PS4 == true)
                {
                    //analysis.NotStandardLoop();
                    analysis.StandardLoop();
                    
                }
                else if (emulator == true)
                {
                    // I need a standard loop for this
                    emulatorLogger.StandardLoop();
                }
            }


        }

        static void ReadInput()
        {
            List<string> commandWord = new List<string>()
            {
                "analyze", "result",
                "random",
                "help"
            };
            while(true)
            {
                var input = Console.ReadLine();

                if (string.CompareOrdinal(input, "analyze") == 0 || string.CompareOrdinal(input, "result") == 0)
                {
                    analysis.AnalyzeResults();
                }
                else if (string.CompareOrdinal(input, "random") == 0)
                {
                    sheet.GetRandomSong();
                }
                else if (string.CompareOrdinal(input, "help") == 0)
                {
                    string message = "List of commands:\n";
                    for (int i = 0; i < commandWord.Count; i++)
                    {
                        message += commandWord[i] + "\n";
                    }
                    message += "\n";

                    Console.WriteLine(message);
                }
            }
        }
    }
}
