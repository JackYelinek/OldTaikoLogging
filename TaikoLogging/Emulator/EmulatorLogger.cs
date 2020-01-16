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
        const string MainFolderPath = @"D:\Games\Taiko\TJAPlayer3-Ver.1.5.3\songs";

        List<EmulatorSongData> AllEmulatorSongData = new List<EmulatorSongData>();

        string prevTitle = string.Empty;
        DateTime prevWriteTime;

        public EmulatorLogger()
        {
            GetAllSongData();

        }

        public void StandardLoop()
        {
            CheckNewScores();
            Thread.Sleep(100);
        }
        private void CheckNewScores()
        {
            DateTime latestTime = new DateTime();
            var latestIndex = -1;

            for (int i = 0; i < AllEmulatorSongData.Count; i++)
            {
                if (File.Exists(AllEmulatorSongData[i].IniFilePath))
                {
                    var iniFile = new FileInfo(AllEmulatorSongData[i].IniFilePath);
                    if (iniFile.LastWriteTime > latestTime || latestIndex == -1)
                    {
                        latestTime = iniFile.LastWriteTime;
                        latestIndex = i;
                    }
                }
            }

            var title = AllEmulatorSongData[latestIndex].SongTitle;
            if (title == prevTitle && latestTime == prevWriteTime)
            {
                return;
            }
            GetSongStats(AllEmulatorSongData[latestIndex].IniFilePath);

            // I'm not sure why I need the title here too, but it's there I guess
            prevTitle = title;
            prevWriteTime = latestTime;
        }

        //private void OldCheckNewScores()
        //{
        //    DirectoryInfo dirInfo = new DirectoryInfo(@"D:\Games\Taiko\TJAPlayer3-Ver.1.5.3\songs");
        //    var results = dirInfo.GetFiles("*.tja.score.ini");
        //    DateTime latestTime = new DateTime();
        //    var latestIndex = -1;
        //    for (int i = 0; i < results.Length; i++)
        //    {
        //        if (results[i].LastWriteTime > latestTime || latestIndex == -1)
        //        {
        //            latestTime = results[i].LastWriteTime;
        //            latestIndex = i;
        //        }
        //    }
        //    var title = results[latestIndex].Name.Remove(results[latestIndex].Name.IndexOf(".tja.score.ini"));
        //    if (title == prevTitle && latestTime == prevWriteTime)
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        GetSongStats(results[latestIndex]);
        //    }
        //    catch
        //    {

        //    }
        //}

        private void GetSongStats(string iniFilePath)
        {
            var lines = File.ReadAllLines(iniFilePath);
            int index = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "[HiScore.Drums]")
                {
                    index = i;
                    break;
                }
            }
            
            var splitFilePath = iniFilePath.Split('\\');
            var title = splitFilePath[splitFilePath.Length - 1].Remove(splitFilePath[splitFilePath.Length - 1].IndexOf(".tja"));


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
            Program.sheet.UpdateEmulatorHighScore(title, info);
        }

        private void FindSongsInSheet()
        {
            // The purpose of this function is to update the BPMs, and at the same time, check to see if every song can be found on the spreadsheet

            var listOfSheetSongs = Program.sheet.GetListofEmulatorSongs();

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


        public void AdjustPreviousSongTiming(bool Left)
        {
            // if Left == true, then Left
            // if Left == false, then right

            DateTime latestTime = new DateTime();
            var latestIndex = -1;

            for (int i = 0; i < AllEmulatorSongData.Count; i++)
            {
                if (File.Exists(AllEmulatorSongData[i].IniFilePath))
                {
                    var iniFile = new FileInfo(AllEmulatorSongData[i].IniFilePath);
                    if (iniFile.LastWriteTime > latestTime || latestIndex == -1)
                    {
                        latestTime = iniFile.LastWriteTime;
                        latestIndex = i;
                    }
                }
            }

            AdjustTiming(AllEmulatorSongData[latestIndex].TjaFilePath, Left);
        }
        private void AdjustTiming(string filePath, bool Left)
        {
            // Left is to know if I'm adjusting the timing + or -
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            FileInfo file = new FileInfo(filePath);
            var lines = File.ReadAllLines(file.FullName, Encoding.GetEncoding(932));

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOf("OFFSET:") == -1)
                {
                    continue;
                }

                string offsetString = lines[i].Remove(0, "OFFSET:".Length);

                float offset = float.Parse(offsetString);

                if (Left == false)
                {
                    offset += 0.01f;
                }
                else
                {
                    offset -= 0.01f;
                }

                lines[i] = "OFFSET:" + offset.ToString();

                break;
            }

            File.WriteAllLines(filePath, lines, Encoding.GetEncoding(932));
        }


        private List<DirectoryInfo> GetSongFolders()
        {
            DirectoryInfo mainDirInfo = new DirectoryInfo(MainFolderPath);
            var mainFolderResults = mainDirInfo.GetDirectories();

            List<DirectoryInfo> SongFolders = new List<DirectoryInfo>();
            List<DirectoryInfo> AllFolders = new List<DirectoryInfo>();

            for (int i = 0; i < mainFolderResults.Length; i++)
            {
                if (mainFolderResults[i].Name == "Challenges")
                {
                    continue;
                }
                // These folders will never be direct song folders
                var tmpFolders = mainFolderResults[i].GetDirectories();

                for (int j = 0; j < tmpFolders.Length; j++)
                {
                    AllFolders.Add(tmpFolders[j]);
                }
                for (int j = 0; j < AllFolders.Count; j++)
                {
                    if (AllFolders[j].GetFiles("*.tja").Length == 0)
                    {
                        tmpFolders = AllFolders[j].GetDirectories();
                        for (int k = 0; k < tmpFolders.Length; k++)
                        {
                            AllFolders.Add(tmpFolders[k]);
                        }
                    }
                    else
                    {
                        bool repeatFolder = false;
                        for (int k = 0; k < SongFolders.Count; k++)
                        {
                            if (AllFolders[j].FullName == SongFolders[k].FullName)
                            {
                                repeatFolder = true;
                                break;
                            }
                        }
                        if (repeatFolder == false)
                        {
                            SongFolders.Add(AllFolders[j]);
                        }
                    }
                }
            }

            return SongFolders;
        }

        private void GetAllSongData()
        {
            var SongFolders = GetSongFolders();

            for (int i = 0; i < SongFolders.Count; i++)
            {
                var tjaFiles = SongFolders[i].GetFiles("*.tja");
                for (int j = 0; j < tjaFiles.Length; j++)
                {
                    EmulatorSongData songData = new EmulatorSongData(tjaFiles[j].FullName);
                    AllEmulatorSongData.Add(songData);
                }
            }

        }
    }
}
