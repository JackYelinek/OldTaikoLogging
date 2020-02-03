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
        List<CommandFunction> ps4CommandFunctions = new List<CommandFunction>();
        List<List<string>> ps4CommandWords = new List<List<string>>();

        List<CommandFunction> emulatorCommandFunctions = new List<CommandFunction>();
        List<List<string>> emulatorCommandWords = new List<List<string>>();

        public Commands()
        {
            Setup();
        }

        private void Setup()
        {
            ps4CommandFunctions.Add(RemoveRanked);
            List<string> tmpList = new List<string>();
            tmpList.Add("!notranked");
            tmpList.Add("!removeranked");
            tmpList.Add("!removelast");
            ps4CommandWords.Add(tmpList);

            //commandFunctions.Add(AddNewSong);
            //tmpList = new List<string>();
            //tmpList.Add("!song ");
            //commandWords.Add(tmpList);

            ps4CommandFunctions.Add(ToggleRandomMode);
            tmpList = new List<string>();
            tmpList.Add("!random mode");
            ps4CommandWords.Add(tmpList);

            ps4CommandFunctions.Add(GetRandomSong);
            tmpList = new List<string>();
            tmpList.Add("!random");
            ps4CommandWords.Add(tmpList);

            ps4CommandFunctions.Add(AnalyzeResults);
            tmpList = new List<string>();
            tmpList.Add("!analyze");
            tmpList.Add("!result");
            ps4CommandWords.Add(tmpList);

            ps4CommandFunctions.Add(CreateNewState);
            tmpList = new List<string>();
            tmpList.Add("!state ");
            ps4CommandWords.Add(tmpList);

            ps4CommandFunctions.Add(NewSongMode);
            tmpList = new List<string>();
            tmpList.Add("!newsong");
            ps4CommandWords.Add(tmpList);



            ps4CommandFunctions.Add(HelpCommand);
            emulatorCommandFunctions.Add(HelpCommand);
            tmpList = new List<string>();
            tmpList.Add("!help");
            ps4CommandWords.Add(tmpList);
            emulatorCommandWords.Add(tmpList);

            emulatorCommandFunctions.Add(AdjustTimingLeft);
            tmpList = new List<string>();
            tmpList.Add("!adjust left");
            emulatorCommandWords.Add(tmpList);
            emulatorCommandFunctions.Add(AdjustTimingRight);
            tmpList = new List<string>();
            tmpList.Add("!adjust right");
            emulatorCommandWords.Add(tmpList);
        }

        private void AdjustTimingRight(string message)
        {
            Program.emulatorLogger.AdjustPreviousSongTiming(false);
        }

        private void AdjustTimingLeft(string message)
        {
            Program.emulatorLogger.AdjustPreviousSongTiming(true);
        }

        public delegate void CommandFunction(string message);

        public void CheckCommands(string message)
        {
            if (Program.currentGame == Program.Game.PS4)
            {
                for (int i = 0; i < ps4CommandFunctions.Count; i++)
                {
                    for (int j = 0; j < ps4CommandWords[i].Count; j++)
                    {
                        if (message.IndexOf(ps4CommandWords[i][j]) == 0)
                        {
                            ps4CommandFunctions[i](message);
                            return;
                        }
                    }
                }
            }
            else if (Program.currentGame == Program.Game.Emulator)
            {
                for (int i = 0; i < emulatorCommandFunctions.Count; i++)
                {
                    for (int j = 0; j < emulatorCommandWords[i].Count; j++)
                    {
                        if (message.IndexOf(emulatorCommandWords[i][j]) == 0)
                        {
                            emulatorCommandFunctions[i](message);
                            return;
                        }
                    }
                }
            }
        }
        private void HelpCommand(string message)
        {
            string responseString = "\n";
            if (Program.currentGame == Program.Game.PS4)
            {
                for (int i = 0; i < ps4CommandWords.Count; i++)
                {
                    responseString += ps4CommandWords[i][0];
                    responseString += "\n";
                }
            }
            else if (Program.currentGame == Program.Game.Emulator)
            {
                for (int i = 0; i < emulatorCommandWords.Count; i++)
                {
                    responseString += emulatorCommandWords[i][0];
                    responseString += "\n";
                }
            }
            else
            {
                responseString = "No game being played\n";
            }


            Console.WriteLine(responseString);
        }

        private void RemoveRanked(string message)
        {
            Program.sheet.RemoveLastRanked();
            Program.rin.SendTwitchMessage("Last ranked match removed");
        }

        //bool newSongIncoming = false;
        //Bitmap newSongBitmap;
        //public void PrepareNewSong(Bitmap bmp)
        //{
        //    newSongIncoming = true;
        //    newSongBitmap = new Bitmap(bmp);
        //    //Program.rin.SendTwitchMessage("Couldn't figure out the song, !song <song>");
        //    Console.WriteLine("Couldn't figure out the song, !song <song>");
        //}
        //private void AddNewSong(string message)
        //{
        //    if (newSongIncoming == true)
        //    {
        //        string songTitle = message.Remove(0, "!song ".Length);

        //        Program.analysis.AddNewSongTitleBitmap(newSongBitmap, songTitle);

        //        Console.WriteLine(songTitle + " has been added!");

        //        newSongIncoming = false;
        //        newSongBitmap = null;
        //        Program.analysis.NewSongAdded();
        //    }
        //}
        private void CreateNewState(string message)
        {
            string state = message.Remove(0, "!state ".Length);

            Program.analysis.CreateNewState(state);
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
        private void NewSongMode(string message)
        {
            Program.analysis.newSongMode = !Program.analysis.newSongMode;
            if (Program.analysis.newSongMode == true)
            {
                Console.WriteLine("NewSongMode on");
            }
            else
            {
                Console.WriteLine("NewSongMode off");
            }
        }
    }
}
