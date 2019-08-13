using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging
{
    class Program
    {
        static public RinClient rin;

        static void Main(string[] args)
        {
            Console.WriteLine("Twitch On? Y/N");
            var input = Console.ReadLine();
            if (string.Compare("y", input) == 0)
            {
                rin = new RinClient(true);
            }
            else
            {
                rin = new RinClient(false);
            }
            //Console.WriteLine("PS4 or Emulator? PS4/Emu");
            //input = Console.ReadLine();
            bool PS4 = false;
            if (string.Compare("PS4", input, true) == 0)
            {
                PS4 = true;
            }
            ImageAnalysis analysis = new ImageAnalysis();

            EmulatorLogger emulatorLogger = new EmulatorLogger();

            // TODO Set this up automatically
            PS4 = true;


            while(true)
            {
                if (PS4 == true)
                {
                    analysis.StandardLoop();
                    //analysis.GetDLCSongs();
                }
                else if (PS4 == false)
                {
                    // I need a standard loop for this
                    emulatorLogger.StandardLoop();
                }
            }


        }

    }
}
