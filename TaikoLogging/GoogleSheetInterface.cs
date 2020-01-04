using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaikoLogging
{
    class GoogleSheetInterface
    {

        string[] Scopes = { SheetsService.Scope.Spreadsheets };
        string ApplicationName = "Ranked Logs";

        public string spreadsheetId = "15qtcBVmes43LlejxgynYoWWxqf99bVK-WmQUfGZfupo";

        SheetsService service;

        public bool test = false;

        public GoogleSheetInterface()
        {
            UserCredential credential;

            using (var stream =
                new FileStream(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public void Test()
        {
            //List<IList<object>> sendValues = new List<IList<object>>();
            //IList<object> baseValues = new List<object>();

            //baseValues.Add("test");

            //sendValues.Add(baseValues);

            //string range = "test!A1";

            //SendData(range, sendValues);

            AddSheet("test");
        }

        public void AddRankedEntry(List<object> info, List<string> headers, Bitmap bmp)
        {
            var Headers = GetHeaders("'Ranked Logs'");
            // Check the sheet to see the match #
            var values = GetValues("Ranked Logs!A2");

            info.Add(int.Parse(values[0][0].ToString()) + 1);
            headers.Add("Match");

            if ((int)info[headers.IndexOf("My Score")] == 0 && (int)info[headers.IndexOf("Opp Score")] == 0)
            {
                return;
            }

            info.Add((int)info[headers.IndexOf("My Score")] - (int)info[headers.IndexOf("Opp Score")]);
            headers.Add("Difference");

            info.Add(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
            headers.Add("DateTime");
            // Might need to be Headers.Count-1, might not actually matter
            values = GetValues("Ranked Logs!A2:" + GetColumnName(Headers.Count));
            List<IList<object>> sendValues = new List<IList<object>>();
            IList<object> baseValues = new List<object>();
            for (int i = 0; i < Headers.Count; i++)
            {
                bool headerFound = false;
                for (int j = 0; j < headers.Count; j++)
                {
                    if (Headers[i] == headers[j])
                    {
                        baseValues.Add(info[j].ToString());
                        headerFound = true;
                        break;
                    }
                }
                if (headerFound == false)
                {
                    baseValues.Add(null);
                }
            }
            sendValues.Add(baseValues);
            for (int i = 0; i < values.Count; i++)
            {
                sendValues.Add(values[i]);
            }
            


            string range = "'Ranked Logs'!A2:" + GetColumnName(Headers.Count);

            SendData(range, sendValues);

            string songTitle = info[headers.IndexOf("Title")].ToString();
            if (info[headers.IndexOf("Difficulty")].ToString() == "Ura")
            {
                songTitle += " Ura";
            }

            string twitchMessage = "Match " + info[headers.IndexOf("Match")].ToString() + ": ";
            if ((string)info[headers.IndexOf("Win/Loss")] == "Win")
            {
                twitchMessage += "Won " + songTitle + " by " + info[headers.IndexOf("Difference")] + " RinComfy";
            }
            else if ((string)info[headers.IndexOf("Win/Loss")] == "Lose")
            {
                twitchMessage += "Lost " + songTitle + " by " + info[headers.IndexOf("Difference")] + " RinThump";
            }
            else
            {
                twitchMessage += "Tied " + songTitle + " RoWOOW";
            }


            if (test == false)
            {
                Program.rin.SendTwitchMessage(twitchMessage);

                // NOT TESTING
                bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Ranked Logs\" + info[headers.IndexOf("Match")] + ".png", ImageFormat.Png);
            }
        }
        public void RemoveLastRanked()
        {
            var Headers = GetHeaders("'Ranked Logs'");
            // Check the sheet to see the match #
            string range = "Ranked Logs!A2:" + GetColumnName(Headers.Count);
            var values = GetValues(range);

            //int matchNumber = int.Parse(values.ElementAt(values.Count - 1)[0].ToString());



            List<IList<object>> sendValues = new List<IList<object>>();
            for (int i = 1; i < values.Count; i++)
            {   
                sendValues.Add(values[i]);
            }
            IList<object> baseValues = new List<object>();

            for (int i = 0; i < Headers.Count; i++)
            {
                baseValues.Add("");
            }
            sendValues.Add(baseValues);

            SendData("Ranked Logs!A2:" + GetColumnName(Headers.Count), sendValues);
        }
        public void UpdatePS4HighScore(List<object> info, List<string> headers, Bitmap bmp)
        {
            var Headers = GetHeaders("Oni");
            string range = string.Empty;
            // Find the song in the spreadsheet
            if (info[headers.IndexOf("Account")].ToString() == "RinzoP")
            {
                range += "Messy " + info[headers.IndexOf("Difficulty")].ToString() + "!";
            }
            else
            {
                range += info[headers.IndexOf("Difficulty")].ToString() + "!";
            }
            range += GetColumnName(Headers.IndexOf("Title")) + "2:" + GetColumnName(Headers.IndexOf("Score"));
            var values = GetValues(range);
            int songIndex = -1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[0].ToString() == info[headers.IndexOf("Title")].ToString())
                    {
                        songIndex = values.IndexOf(row);
                        break;
                    }
                }
            }

            if (songIndex == -1)
            {
                // something got fucked up
                // probably a good idea to set up some form of logging for this sort of thing, but meh
                Program.logger.LogManyVariables("UpdatePS4HighScore: Song Not Found", headers, info);

                return;
            }

            // This isn't very fluid
            // If something gets changed in the spreadsheet, this breaks
            string score = string.Empty;

            if (values[songIndex].Count < 5)
            {
                score = "0";
            }
            else
            {
                score = values[songIndex][4].ToString();
            }
            while (true)
            {
                if (score.IndexOf(',') == -1)
                {
                    break;
                }
                score = score.Remove(score.IndexOf(','), 1);
            }

            //double check that the new score is higher
            if (int.Parse(score) >= (int)info[headers.IndexOf("Score")])
            {
                // This'd mean the score on the spreadsheet is higher than the new score
                return;
            }
            else
            {
                if (test == false)
                {
                    string songTitle = info[headers.IndexOf("Title")].ToString();
                    if (info[headers.IndexOf("Difficulty")].ToString() == "Ura")
                    {
                        songTitle += " Ura";
                    }
                    if (info[headers.IndexOf("Account")].ToString() == "RinzoP")
                    {
                        songTitle += " Messy";
                    }
                    if (int.Parse(score) == (int)info[headers.IndexOf("Score")])
                    {
                        Program.rin.SendTwitchMessage("Tied high score! " + songTitle + " = " + ((int)info[headers.IndexOf("Score")] - int.Parse(score)).ToString());
                    }
                    else
                    {
                        Program.rin.SendTwitchMessage("New high score! " + songTitle + " +" + ((int)info[headers.IndexOf("Score")] - int.Parse(score)).ToString());
                    }
                }
            }

            info.Add(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
            headers.Add("DateTime");

            //send all the information onto the sheet in the correct spots
            IList<object> baseValues = new List<object>();
            for (int i = Headers.IndexOf("Score"); i < Headers.Count; i++)
            {
                bool headerFound = false;
                for (int j = 0; j < headers.Count; j++)
                {
                    if (Headers[i] == headers[j])
                    {
                        baseValues.Add(info[j].ToString());
                        headerFound = true;
                        break;
                    }
                }
                if (headerFound == false)
                {
                    baseValues.Add(null);
                }
            }

            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);
            if (info[headers.IndexOf("Account")].ToString() == "Deathblood")
            {
                range = info[headers.IndexOf("Difficulty")].ToString() + "!";
            }
            else
            {
                range = "Messy " + info[headers.IndexOf("Difficulty")].ToString() + "!";
            }

            range += GetColumnName(Headers.IndexOf("Score")) + (songIndex + 2).ToString() + ":"
                + GetColumnName(Headers.IndexOf("DateTime")) + (songIndex + 2).ToString();
            SendData(range, sendValues);

            if (test == false)
            {
                var fileSongTitle = Program.MakeValidFileName(info[headers.IndexOf("Title")].ToString());

                if (info[headers.IndexOf("Account")].ToString() == "Deathblood")
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Normal\" + fileSongTitle);
                    if (dirInfo.Exists == false)
                    {
                        dirInfo.Create();
                    }
                    var result = dirInfo.GetFiles();


                    bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Normal\" + fileSongTitle + "\\" + fileSongTitle + "." + info[headers.IndexOf("Difficulty")].ToString() + "." + result.Length + ".png", ImageFormat.Png);
                }
                else if (info[headers.IndexOf("Account")].ToString() == "RinzoP")
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Messy\" + fileSongTitle);
                    if (dirInfo.Exists == false)
                    {
                        dirInfo.Create();
                    }
                    var result = dirInfo.GetFiles();
                    bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Messy\" + fileSongTitle + "\\" + fileSongTitle + "." + info[headers.IndexOf("Difficulty")].ToString() + "." + result.Length + ".png", ImageFormat.Png);

                }
            }





            //DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores");
            //var result = dirInfo.GetFiles();
            //int numScreenshots = 0;
            //for (int i = 0; i < result.Length; i++)
            //{
            //    if (result[i].Name.Remove(result[i].Name.IndexOf('.')) == info[headers.IndexOf("Title")].ToString() && result[i].Name.Remove(0, result[i].Name.IndexOf('.') + 1).Remove(result[i].Name.Remove(0, result[i].Name.IndexOf('.') + 1).IndexOf('.')) == info[headers.IndexOf("Difficulty")].ToString())
            //    {
            //        numScreenshots++;
            //    }
            //}

            //if (test == false)
            //{
            //    // NOT TESTING
            //    bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + info[headers.IndexOf("Title")] + "." + info[headers.IndexOf("Difficulty")].ToString() + "." + numScreenshots.ToString() + ".png", ImageFormat.Png);
            //}

        }
        public void UpdatePS4BestGoods(List<object> info, List<string> headers)
        {
            var Headers = GetHeaders(info[headers.IndexOf("Difficulty")].ToString());

            string range = string.Empty;
            // Find the song in the spreadsheet
            if (info[headers.IndexOf("Account")].ToString() == "RinzoP")
            {
                range += "Messy " + info[headers.IndexOf("Difficulty")].ToString() + "!";
            }
            else
            {
                range += info[headers.IndexOf("Difficulty")].ToString() + "!";
            }
            range += GetColumnName(Headers.IndexOf("Title")) + "2:" + GetColumnName(Headers.IndexOf("Best Goods"));

            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            var values = response.Values;
            int songIndex = -1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[0].ToString() == (string)info[headers.IndexOf("Title")])
                    {
                        songIndex = values.IndexOf(row);
                    }
                }
            }
            if (headers.IndexOf("GOOD") == -1)
            {
                headers[headers.IndexOf("My Goods")] = "GOOD";
            }

            if (songIndex == -1)
            {
                // something got fucked up
                // probably a good idea to set up some form of logging for this sort of thing, but meh
                Program.logger.LogManyVariables("UpdatePS4BestGoods: Song Not Found", headers, info);
                return;
            }
            string sheetGoods = string.Empty;
            if (values[songIndex].Count < 11)
            {
                sheetGoods = "0";
            }
            else
            {
                sheetGoods = values.ElementAt(songIndex)[10].ToString();
            }

            if ((int)info[headers.IndexOf("GOOD")] < int.Parse(sheetGoods))
            {
                if (test == false)
                {
                    int goodsDifference = (int)info[headers.IndexOf("GOOD")] - int.Parse(sheetGoods);
                    string twitchMessage = string.Empty;
                    string songTitle = info[headers.IndexOf("Title")].ToString();
                    if (info[headers.IndexOf("Difficulty")].ToString() == "Ura")
                    {
                        songTitle += " Ura";
                    }
                    if (info[headers.IndexOf("Account")].ToString() == "RinzoP")
                    {
                        songTitle += " Messy";
                    }
                    if (goodsDifference == -1)
                    {
                        twitchMessage += "1 good away from best accuracy on " + songTitle + "!";
                    }
                    else if (goodsDifference > -11)
                    {
                        twitchMessage += (goodsDifference * -1) + " goods away from best accuracy on " + songTitle + "!";
                    }
                    else
                    {
                        return;
                    }
                    Program.rin.SendTwitchMessage(twitchMessage);

                }
                return;
            }
            IList<object> baseValues = new List<object>
            {
                info[headers.IndexOf("GOOD")]
            };
            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);


            if (info[headers.IndexOf("Account")].ToString() == "Deathblood")
            {
                range = info[headers.IndexOf("Difficulty")].ToString() + "!";
            }
            else
            {
                range = "Messy " + info[headers.IndexOf("Difficulty")].ToString() + "!";
            }

            range += GetColumnName(Headers.IndexOf("Best Goods")) + (songIndex + 2).ToString();
            SendData(range, sendValues);

            if (test == false)
            {
                int goodsDifference = (int)info[headers.IndexOf("GOOD")] - int.Parse(sheetGoods);
                string twitchMessage = string.Empty;
                string songTitle = info[headers.IndexOf("Title")].ToString();
                if (info[headers.IndexOf("Difficulty")].ToString() == "Ura")
                {
                    songTitle += " Ura";
                }
                if (info[headers.IndexOf("Account")].ToString() == "RinzoP")
                {
                    songTitle += " Messy";
                }
                if (goodsDifference == 1)
                {
                    twitchMessage += "New best accuracy on " + songTitle + ", " + goodsDifference.ToString() + " more good!";
                }
                else if (goodsDifference == 0)
                {
                    twitchMessage += "Tied accuracy on " + songTitle + "!";
                }
                else
                {
                    twitchMessage += "New best accuracy on " + songTitle + ", " + goodsDifference.ToString() + " more goods!";
                }
                Program.rin.SendTwitchMessage(twitchMessage);
            }

        }
        public void AddRecentPlay(List<object> info, List<string> headers)
        {
            var Headers = GetHeaders("'Recent Plays'");
            // Check the sheet to see the match #
            var values = GetValues("Recent Plays!A2");

            if (headers.IndexOf("My Score") != -1)
            {
                headers[headers.IndexOf("My Score")] = "Score";
            }
            if (headers.IndexOf("My Goods") != -1)
            {
                headers[headers.IndexOf("My Goods")] = "GOOD";
            }
            if (headers.IndexOf("My OKs") != -1)
            {
                headers[headers.IndexOf("My OKs")] = "OK";
            }
            if (headers.IndexOf("My Bads") != -1)
            {
                headers[headers.IndexOf("My Bads")] = "BAD";
            }
            if (headers.IndexOf("My Combo") != -1)
            {
                headers[headers.IndexOf("My Combo")] = "MAX Combo";
            }
            if (headers.IndexOf("My Drumroll") != -1)
            {
                headers[headers.IndexOf("My Drumroll")] = "Drumroll";
            }

            info.Add("=VLOOKUP(A2, Oni!B:D, 3, false)");
            headers.Add("Genre");

            info.Add(DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
            headers.Add("DateTime");
            // Might need to be Headers.Count-1, might not actually matter
            values = GetValues("Recent Plays!A2:" + GetColumnName(Headers.Count));
            List<IList<object>> sendValues = new List<IList<object>>();
            IList<object> baseValues = new List<object>();
            for (int i = 0; i < Headers.Count; i++)
            {
                bool headerFound = false;
                for (int j = 0; j < headers.Count; j++)
                {
                    if (Headers[i] == headers[j])
                    {
                        baseValues.Add(info[j].ToString());
                        headerFound = true;
                        break;
                    }
                }
                if (headerFound == false)
                {
                    baseValues.Add(null);
                }
            }
            sendValues.Add(baseValues);
            if (values != null)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    sendValues.Add(values[i]);
                }
            }




            string range = "'Recent Plays'!A2:" + GetColumnName(Headers.Count);

            SendData(range, sendValues);

            string songTitle = info[headers.IndexOf("Title")].ToString();
            if (info[headers.IndexOf("Difficulty")].ToString() == "Ura")
            {
                songTitle += " Ura";
            }
        }
        public void GetRandomSong()
        {
            var Headers = GetHeaders("Oni");

            List<string> songs = new List<string>();
            List<int> goalValues = new List<int>();

            List<int> songOKs = new List<int>();
            List<int> goalOKs = new List<int>();
            List<int> songBads = new List<int>();

            string range = "Oni!A2:" + GetColumnName(Headers.Count);
            var values = GetValues(range);

            int numOni = 0;
            for (int i = 0; i < values.Count; i++)
            {
                songs.Add(values[i][Headers.IndexOf("Title")].ToString());
                int songOK = int.Parse(values[i][Headers.IndexOf("OK")].ToString());
                int goalOK = int.Parse(values[i][Headers.IndexOf("Goal OKs")].ToString());
                int songBad = int.Parse(values[i][Headers.IndexOf("BAD")].ToString());
                int numNotes = int.Parse(values[i][Headers.IndexOf("GOOD")].ToString()) + songOK + songBad;

                //int goalValue = (songOK + (songBad * 2)) - goalOK;

                //int goalValue = ((songOK + (songBad * 2)) - goalOK) / numNotes;
                int goalValue = (int)((((songOK) - goalOK) * Math.Sqrt(songOK + (songBad * 2))) * 1000);

                if (goalValue < 0)
                {
                    goalValue = 0;
                }
                goalValues.Add(goalValue);
                songOKs.Add(songOK);
                goalOKs.Add(goalOK);
                songBads.Add(songBad);


                numOni++;
            }

            range = "Ura!A2:" + GetColumnName(Headers.Count);
            values = GetValues(range);
            for (int i = 0; i < values.Count; i++)
            {
                songs.Add(values[i][Headers.IndexOf("Title")].ToString());
                int songOK = int.Parse(values[i][Headers.IndexOf("OK")].ToString());
                int goalOK = int.Parse(values[i][Headers.IndexOf("Goal OKs")].ToString());
                int songBad = int.Parse(values[i][Headers.IndexOf("BAD")].ToString());
                //int numNotes = int.Parse(values[i][Headers.IndexOf("GOOD")].ToString()) + songOK + int.Parse(values[i][Headers.IndexOf("BAD")].ToString());
                int goalValue = (songOK + (songBad * 2)) - goalOK;
                if (goalValue < 0)
                {
                    goalValue = 0;
                }
                goalValues.Add(goalValue);
                songOKs.Add(songOK);
                goalOKs.Add(goalOK);
                songBads.Add(songBad);
            }

            int maxRand = goalValues.Sum();
            Random rand = new Random();
            float randValue = rand.Next(maxRand);

            List<float> chances = new List<float>();

            for (int i = 0; i < goalValues.Count; i++)
            {
                // I think this math is correct
                chances.Add(goalValues[i] / (float)goalValues.Sum());
            }

            for (int i = 0; i < goalValues.Count; i++)
            {
                if (goalValues[i] == 0)
                {
                    continue;
                }
                if (randValue < goalValues[i])
                {
                    // Song picked
                    string message = songs[i];

                    if(i > numOni)
                    {
                        message += " Ura";
                    }


                    message += ", " + songOKs[i] + " OKs ";
                    if (songBads[i] != 0)
                    {
                        message += songBads[i] + " Bads ";
                    }
                    message += "-> " + goalOKs[i] + " Goal OKs";
                    string consoleMessage = chances[i] * 100 + "% chance of picking this song\n";

                    if (CheckIfEnglish(songs[i]) == false)
                    {
                        if (i > numOni)
                        {
                            for (int j = 0; j < songs.Count; j++)
                            {
                                if (songs[j] == songs[i])
                                {
                                    i = j;
                                    break;
                                }
                            }
                        }
                        consoleMessage += FindClosestReadableSong(songs, i, numOni);
                    }

                    Program.rin.SendTwitchMessage(message);
                    Console.WriteLine(consoleMessage);
                    break;
                }
                else
                {
                    randValue -= goalValues[i];
                }
            }

            #region Logging
            List<object> loggingGoalDifference = new List<object>();
            for (int i = 0; i < goalValues.Count; i++)
            {
                loggingGoalDifference.Add(goalValues[i]);
            }

            Program.logger.LogManyVariables("Random", songs, loggingGoalDifference);
            Program.logger.LogVariable("Random", "randValue", randValue);
            #endregion
        }

        public string FindClosestReadableSong(List<string> songs, int i, int numOni)
        {
            string message = string.Empty;
            for (int j = 1; j < numOni; j++)
            {
                if (i - j >= 0)
                {
                    if (CheckIfEnglish(songs[i - j]) == true)
                    {
                        message += j + " songs to the right of " + songs[i - j] + "\n";
                        break;
                    }
                }
                else
                {
                    if (CheckIfEnglish(songs[i - j + numOni]) == true)
                    {
                        message += j + " songs to the right of " + songs[i - j + numOni] + "\n";
                        break;
                    }
                }
                if (i + j < songs.Count)
                {
                    if (CheckIfEnglish(songs[i + j]) == true)
                    {
                        message += j + " songs to the left of " + songs[i + j] + "\n";
                        break;
                    }
                }
                else
                {
                    if (CheckIfEnglish(songs[i + j - numOni]) == true)
                    {
                        message += j + " songs to the left of " + songs[i + j - numOni] + "\n";
                        break;
                    }
                }
            }
            return message;
        }
        private bool CheckIfEnglish(string songTitle)
        {
            int numEnglishCharacters = 0;
            string englishCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!\"'↓~";
            while(songTitle.IndexOfAny(englishCharacters.ToCharArray()) != -1)
            {
                songTitle = songTitle.Remove(songTitle.IndexOfAny(englishCharacters.ToCharArray()), 1);
                numEnglishCharacters++;
                if (numEnglishCharacters >= 2)
                {
                    return true;
                }
            }
            if (numEnglishCharacters < 5)
            {
                // I don't think this if/else statement is needed
                return false;
            }
            else
            {
                return true;
            }
        }

        public void FixRankedLogsData(List<object> info, List<string> headers, int matchNumber)
        {
            var Headers = GetHeaders("Ranked Logs");
            string range = string.Empty;

            range = "Ranked Logs!A2:" + GetColumnName(Headers.IndexOf("DateTime"));

            var values = GetValues(range);

            List<IList<object>> sendValues = new List<IList<object>>();
            int index = 0;
            for (int i = 0; i < values.Count; i++)
            {
                IList<object> baseValues = new List<object>();
                if (matchNumber != int.Parse(values[i][Headers.IndexOf("Match")].ToString()))
                {
                    continue;
                }
                index = i+2;
                for (int k = 0; k < Headers.Count; k++)
                {
                    bool headerFound = false;
                    for (int l = 0; l < headers.Count; l++)
                    {
                        if (Headers[k] == headers[l])
                        {
                            baseValues.Add(info[l].ToString());
                            headerFound = true;
                            break;
                        }
                    }
                    if (headerFound == false)
                    {
                        baseValues.Add(null);
                    }
                }
                sendValues.Add(baseValues);
                break;
            }

            range = "Ranked Logs!A" + index.ToString() + ":" + GetColumnName(Headers.Count);

            SendData(range, sendValues);
        }

        #region Emulator functions
        public void UpdateEmulatorHighScore(string title, string[] info)
        {
            // Find the song in the spreadsheet
            string range = "Emulator" + "!B2:F";
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            var values = response.Values;
            int songIndex = -1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[0].ToString() == title)
                    {
                        songIndex = values.IndexOf(row);
                    }
                }
            }

            if (songIndex == -1)
            {
                // something got fucked up
                // probably a good idea to set up some form of logging for this sort of thing, but meh
                // Set up logger that works well for my updated functions, which this is not
                //Program.logger.LogManyVariables("UpdateEmulatorHighScore: Song Not Found", )
                Console.WriteLine("Failed to update " + title + ", " + info[0].ToString() + ", " + info[1].ToString() + ", " + info[2].ToString() + ", " + info[3].ToString() + ", " + info[4].ToString() + ", ");
                return;
            }

            string score = values.ElementAt(songIndex).ElementAt(4).ToString();
            while (true)
            {
                if (score.IndexOf(',') == -1)
                {
                    break;
                }
                score = score.Remove(score.IndexOf(','), 1);
            }

            //double check that the new score is higher
            if (int.Parse(score) >= int.Parse(info[0]))
            {
                // This'd mean the score on the spreadsheet is higher than the new score
                return;
            }



            //send all the information onto the sheet in the correct spots
            IList<object> baseValues = new List<object>
            {
                info[0], info[1], info[2], info[3], info[4]
            };
            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);

            SendData("Emulator" + "!F" + (songIndex + 2).ToString() + ":J" + (songIndex + 2).ToString(), sendValues);
            //List<Google.Apis.Sheets.v4.Data.ValueRange> updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            //var dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            //dataValueRange.Range = "Emulator" + "!F" + (songIndex + 2).ToString() + ":J" + (songIndex + 2).ToString();
            //dataValueRange.Values = sendValues;
            //updateData.Add(dataValueRange);

            //Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            //requestBody.Data = updateData;
            //requestBody.ValueInputOption = "USER_ENTERED";

            //var updateRequest = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            //var updateResponse = updateRequest.Execute();

            Program.rin.SendTwitchMessage("New high score! " + title + " +" + (int.Parse(info[0]) - int.Parse(score)).ToString());


            baseValues = new List<object>
            {
                DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")
            };
            sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);

            SendData("Emulator" + "!O" + (songIndex + 2).ToString(), sendValues);
            //updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            //dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            //dataValueRange.Range = "Emulator" + "!O" + (songIndex + 2).ToString();
            //dataValueRange.Values = sendValues;
            //updateData.Add(dataValueRange);

            //requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            //requestBody.Data = updateData;
            //requestBody.ValueInputOption = "USER_ENTERED";

            //updateRequest = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            //updateResponse = updateRequest.Execute();



        }

        public IList<IList<object>> GetListofEmulatorSongs()
        {
            string range = "Emulator" + "!B2:B";
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                return values;
            }
            return null;
        }
        #endregion

        #region Sheet Functions
        public IList<IList<object>> GetValues(string range)
        {
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            var values = response.Values;
            return values;
        }


        public List<string> GetHeaders(string sheet)
        {
            string range = sheet + "!A1:Z1";
            var values = GetValues(range);
            List<string> Headers = new List<string>();
            for (int i = 0; i < values[0].Count; i++)
            {
                Headers.Add(values[0][i].ToString());
            }
            return Headers;
        }

        public void SendData(string range, IList<IList<object>> data)
        {
            List<Google.Apis.Sheets.v4.Data.ValueRange> updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            var dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            dataValueRange.Range = range;
            dataValueRange.Values = data;
            updateData.Add(dataValueRange);

            var requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            requestBody.Data = updateData;
            requestBody.ValueInputOption = "USER_ENTERED";

            var updateRequest = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            var updateResponse = updateRequest.Execute();
        }
        public void AddSheet(string sheetName)
        {
            var sheetRequest = new Google.Apis.Sheets.v4.Data.AddSheetRequest();
            sheetRequest.Properties = new SheetProperties();
            sheetRequest.Properties.Title = sheetName;
            Request newRequest = new Request()
            {
                AddSheet = sheetRequest
            };

            var requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateSpreadsheetRequest();
            requestBody.Requests = new List<Request>();
            requestBody.Requests.Add(newRequest);

            var updateRequest = service.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);

            var updateResponse = updateRequest.Execute();
        }
        public int GetSheetID(string sheetName)
        {
            var request = service.Spreadsheets.Get(spreadsheetId);
            var response = request.Execute();

            for (int i = 0; i < response.Sheets.Count; i++)
            {
                if (response.Sheets[i].Properties.Title == sheetName)
                {
                    return (int)response.Sheets[i].Properties.SheetId;
                }
            }
            return -1;
        }
        public string GetColumnName(int index)
        {
            if (index < 0)
            {
                return "";
            }
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];

            value += letters[index % letters.Length];

            return value;
        }
        #endregion
    }
}
