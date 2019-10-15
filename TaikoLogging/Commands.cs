using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging
{
    class Commands
    {
        List<CommandFunction> commandFunctions = new List<CommandFunction>();
        List<List<string>> commandWords = new List<List<string>>();

        public Commands()
        {
            Setup();
        }

        private void Setup()
        {
            commandFunctions.Add(RemoveRanked);
            List<string> tmpList = new List<string>();
            tmpList.Add("!notranked");
            tmpList.Add("!removeranked");
            tmpList.Add("!removelast");
            commandWords.Add(tmpList);

            commandFunctions.Add(AddNewSong);
            tmpList = new List<string>();
            tmpList.Add("!song ");
            commandWords.Add(tmpList);

            commandFunctions.Add(ToggleRandomMode);
            tmpList = new List<string>();
            tmpList.Add("!random mode");
            commandWords.Add(tmpList);

            commandFunctions.Add(GetRandomSong);
            tmpList = new List<string>();
            tmpList.Add("!random");
            commandWords.Add(tmpList);

            commandFunctions.Add(AnalyzeResults);
            tmpList = new List<string>();
            tmpList.Add("!analyze");
            tmpList.Add("!result");
            commandWords.Add(tmpList);
        }

        public delegate void CommandFunction(string message);

        public void CheckCommands(string message)
        {
            for (int i = 0; i < commandFunctions.Count; i++)
            {
                for (int j = 0; j < commandWords[i].Count; j++)
                {
                    if (message.IndexOf(commandWords[i][j]) == 0)
                    {
                        commandFunctions[i](message);
                        return;
                    }
                }
            }
        }

        private void RemoveRanked(string message)
        {
            Program.sheet.RemoveLastRanked();
            Program.rin.SendTwitchMessage("Last ranked match removed");
        }

        bool newSongIncoming = false;
        Bitmap newSongBitmap;
        public void PrepareNewSong(Bitmap bmp)
        {
            newSongIncoming = true;
            newSongBitmap = new Bitmap(bmp);
            Program.rin.SendTwitchMessage("Couldn't figure out the song, !song <song>");
        }
        private void AddNewSong(string message)
        {
            if (newSongIncoming == true)
            {
                string songTitle = message.Remove(0, 6);
                DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Title Bitmaps\BaseTitles\");
                var result = dirInfo.GetFiles();
                int numScreenshots = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i].Name.Remove(result[i].Name.IndexOf('.')) == songTitle)
                    {
                        numScreenshots++;
                    }
                }

                newSongBitmap.Save(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Title Bitmaps\BaseTitles\" + songTitle + ".png");
                newSongIncoming = false;
                newSongBitmap = null;
                Program.rin.SendTwitchMessage(songTitle + " has been added!");
                Program.analysis.NewSongAdded();
            }
        }
        private void ToggleRandomMode(string message)
        {
            Program.analysis.RandomModeToggle();
        }
        private void GetRandomSong(string message)
        {
            Program.sheet.GetRandomSong();
        }
        private void AnalyzeResults(string message)
        {
            Program.analysis.AnalyzeResults();
        }
    }
}
