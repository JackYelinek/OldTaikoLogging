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

            //commandFunctions.Add(AddNewSong);
            //tmpList = new List<string>();
            //tmpList.Add("!song ");
            //commandWords.Add(tmpList);

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

            commandFunctions.Add(CreateNewState);
            tmpList = new List<string>();
            tmpList.Add("!state ");
            commandWords.Add(tmpList);

            commandFunctions.Add(NewSongMode);
            tmpList = new List<string>();
            tmpList.Add("!newsong");
            commandWords.Add(tmpList);



            commandFunctions.Add(HelpCommand);
            tmpList = new List<string>();
            tmpList.Add("!help");
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
        private void HelpCommand(string message)
        {
            string responseString = "\n";
            for (int i = 0; i < commandWords.Count; i++)
            {
                responseString += commandWords[i][0];
                responseString += "\n";
            }

            Console.WriteLine(responseString);
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
            //Program.rin.SendTwitchMessage("Couldn't figure out the song, !song <song>");
            Console.WriteLine("Couldn't figure out the song, !song <song>");
        }
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
