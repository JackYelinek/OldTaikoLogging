using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace TaikoLogging
{
    // poorly named class, this class is to check for updated ini files in the songs folder and send the new scores to my spreadsheet
    // any ini file will be updated any time I play a chart, so I'd need to check with my spreadsheet to see if it's a high score or not, and if it is, updated, if not, don't
    class EmulatorLogger
    {
        GoogleSheetInterface sheet;

        string prevTitle = string.Empty;
        DateTime prevWriteTime;

        public EmulatorLogger()
        {
            sheet = new GoogleSheetInterface();
            //FindSongsInSheet();


        }

        public void StandardLoop()
        {
            CheckNewScores();
            Thread.Sleep(1000);
        }

        private void CheckNewScores()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\Games\Taiko\TJAPlayer3-Ver.1.5.3\songs");
            var results = dirInfo.GetFiles("*.tja.score.ini");
            DateTime latestTime = new DateTime();
            var latestIndex = -1;
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].LastWriteTime > latestTime || latestIndex == -1)
                {
                    latestTime = results[i].LastWriteTime;
                    latestIndex = i;
                }
            }
            var title = results[latestIndex].Name.Remove(results[latestIndex].Name.IndexOf(".tja.score.ini"));
            if (title == prevTitle && latestTime == prevWriteTime)
            {
                return;
            }
            try
            {
                GetSongStats(results[latestIndex]);
            }
            catch
            {

            }
        }

        private void GetSongStats(FileInfo result)
        {
            var lines = File.ReadAllLines(result.FullName);
            int index = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "[HiScore.Drums]")
                {
                    index = i;
                    break;
                }
            }

            var title = result.Name.Remove(result.Name.IndexOf(".tja.score.ini"));
            var score = lines[index + 1];


            var goods = lines[index + 4];
            var oks = lines[index + 5];
            var bads = lines[index + 8];
            var combo = lines[index + 9];
            score = score.Remove(0, score.IndexOf("=") + 1);
            goods = goods.Remove(0, goods.IndexOf("=") + 1);
            oks = oks.Remove(0, oks.IndexOf("=") + 1);
            bads = bads.Remove(0, bads.IndexOf("=") + 1);
            combo = combo.Remove(0, combo.IndexOf("=") + 1);
            string[] info = new string[5]
            {
                score, goods, oks, bads, combo
            };
            sheet.UpdateEmulatorHighScore(title, info);
            prevTitle = title;
            prevWriteTime = result.LastWriteTime;
        }

        private void FindSongsInSheet()
        {
            // The purpose of this function is to update the BPMs, and at the same time, check to see if every song can be found on the spreadsheet

            var listOfSheetSongs = sheet.GetListofEmulatorSongs();

            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\Games\Taiko\TJAPlayer3-Ver.1.5.3\songs");
            var results = dirInfo.GetFiles("*.tja");

            for (int i = 0; i < results.Length; i++)
            {
                bool canBeFound = false;
                string songTitle = results[i].Name.Remove(results[i].Name.IndexOf(".tja"));
                foreach (var row in listOfSheetSongs)
                {
                    if (row[0].ToString() == songTitle)
                    {
                        canBeFound = true;
                        break;
                    }
                }
                if (canBeFound == true)
                {
                    continue;
                }
                else
                {
                    Console.WriteLine("Couldn't find " + songTitle);
                }
            }

        }

        private void UpdateBPMs()
        {
            // This is basically going to be the opposite of FindSongsInSheet
            // I'll do this later
        }

    }
}
