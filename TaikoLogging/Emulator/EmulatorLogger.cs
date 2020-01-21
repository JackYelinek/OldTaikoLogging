using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace TaikoLogging.Emulator
{
    // poorly named class, this class is to check for updated ini files in the songs folder and send the new scores to my spreadsheet
    // any ini file will be updated any time I play a chart, so I'd need to check with my spreadsheet to see if it's a high score or not, and if it is, updated, if not, don't
    class EmulatorLogger
    {
        const string MainFolderPath = @"D:\Games\Taiko\TJAPlayer3-Ver.1.5.3\songs";

        List<SongData> AllEmulatorSongData = new List<SongData>();

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
        public void SingleLoop()
        {
            GetAllSongScores();
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
            try
            {
                GetSongStats(AllEmulatorSongData[latestIndex]);
                prevTitle = title;
                prevWriteTime = latestTime;
            }
            catch
            {

            }
            // I'm not sure why I need the title here too, but it's there I guess

        }

        private void GetAllSongScores()
        {
            for (int i = 0; i < AllEmulatorSongData.Count; i++)
            {
                if (File.Exists(AllEmulatorSongData[i].IniFilePath))
                {
                    try
                    {
                        GetSongStats(AllEmulatorSongData[i]);
                    }
                    catch
                    {

                    }
                }
            }

        }

        private void GetSongStats(SongData songData)
        {
            var lines = File.ReadAllLines(songData.IniFilePath);
            int index = -1;
            int lastPlayIndex = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "[HiScore.Drums]" && index == -1)
                {
                    index = i;
                }
                else if (lines[i] == "[LastPlay.Drums]" && lastPlayIndex == -1)
                {
                    lastPlayIndex = i;
                }
                if (index != -1 && lastPlayIndex != -1)
                {
                    break;
                }
            }

            Play play = new Play();
            
            play.SongData = songData;

            play.Score = int.Parse(lines[index + 1].Remove(0, lines[index + 1].IndexOf("=") + 1));

            play.Goods = int.Parse(lines[index + 4].Remove(0, lines[index + 4].IndexOf("=") + 1));
            play.OKs = int.Parse(lines[index + 5].Remove(0, lines[index + 5].IndexOf("=") + 1));
            play.Bads = int.Parse(lines[index + 8].Remove(0, lines[index + 8].IndexOf("=") + 1));
            play.Combo = int.Parse(lines[index + 9].Remove(0, lines[index + 9].IndexOf("=") + 1));


            play.ScoreDateTime = DateTime.Parse(lines[index + 50].Remove(0, lines[index + 50].IndexOf("=") + 1));
            play.LatestDateTime = DateTime.Parse(lines[lastPlayIndex + 50].Remove(0, lines[lastPlayIndex + 50].IndexOf("=") + 1));

            play.LastGoods = int.Parse(lines[lastPlayIndex + 4].Remove(0, lines[lastPlayIndex + 4].IndexOf("=") + 1));
            play.LastOKs = int.Parse(lines[lastPlayIndex + 5].Remove(0, lines[lastPlayIndex + 5].IndexOf("=") + 1));
            play.LastBads = int.Parse(lines[lastPlayIndex + 8].Remove(0, lines[lastPlayIndex + 8].IndexOf("=") + 1));

            play = UpdateDBFile(play);

            Program.sheet.UpdateEmulatorHighScore(play);
        }

        private Play UpdateDBFile(Play play)
        {
            // This is gonna look at a .dbtja file
            int totalOKs = 0;
            int totalBads = 0;
            int plays = 0;

            if (File.Exists(play.SongData.DBTjaFilePath) == true)
            {
                // Keep the latest play at the top of them
                var lines = File.ReadAllLines(play.SongData.DBTjaFilePath);

                string newLine = play.LastOKs.ToString() + "\t" + play.LastBads.ToString();

                string[] newLines = new string[Math.Min(lines.Length + 1, 10)];
                newLines[0] = newLine;

                for (int i = 0; i < Math.Min(lines.Length, 9); i++)
                {
                    newLines[i + 1] = lines[i];
                }
                
                
                File.WriteAllLines(play.SongData.DBTjaFilePath, newLines);

                for (int i = 0; i < newLines.Length; i++)
                {
                    var splitPlay = newLines[i].Split('\t');
                    totalOKs += int.Parse(splitPlay[0]);
                    totalBads += int.Parse(splitPlay[1]);
                    plays++;
                }

                play.RecentOKs = (double)totalOKs / plays;
                play.RecentBads = (double)totalBads / plays;


            }
            else
            {
                play.RecentOKs = play.LastOKs;
                play.RecentBads = play.LastBads;
                string line = play.LastOKs.ToString() + "\t" + play.LastBads.ToString();
                File.WriteAllText(play.SongData.DBTjaFilePath, line);
            }
            Console.WriteLine("dbtja file accessed");
            return play;
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
            string message = "Adjusted " + AllEmulatorSongData[latestIndex].SongTitle + " from the ";
            if (Left == true)
            {
                message += "left.";
            }
            else
            {
                message += "right.";
            }
            Console.WriteLine(message);
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
                    SongData songData = new SongData(tjaFiles[j].FullName);
                    AllEmulatorSongData.Add(songData);
                }
            }

        }
    }
}
