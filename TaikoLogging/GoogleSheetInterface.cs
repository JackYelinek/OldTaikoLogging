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

        public void AddRankedEntry(List<object> info, List<string> headers, Bitmap bmp)
        {
            var Headers = GetHeaders("'Ranked Logs'");
            // Check the sheet to see the match #
            var values = GetValues("Ranked Logs!A2");

            info.Add(int.Parse(values[0][0].ToString()) + 1);
            headers.Add("Match");

            if ((bool)info[headers.IndexOf("Win/Loss")] == true)
            {
                info[headers.IndexOf("Win/Loss")] = "Win";
            }
            else
            {
                info[headers.IndexOf("Win/Loss")] = "Lose";
            }

            if ((int)info[headers.IndexOf("My Score")] == 0 && (int)info[headers.IndexOf("Opp Score")] == 0)
            {
                return;
            }

            info.Add((int)info[headers.IndexOf("My Score")] - (int)info[headers.IndexOf("Opp Score")]);
            headers.Add("Difference");

            info.Add(DateTime.Now.ToString("MM/dd/yyyy"));
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


            string twitchMessage = "Match " + info[headers.IndexOf("Match")].ToString() + ": ";
            if ((string)info[headers.IndexOf("Win/Loss")] == "Win")
            {
                twitchMessage += "Won " + info[headers.IndexOf("Title")] + " by " + info[headers.IndexOf("Difference")] + " RinComfy";
            }
            else
            {
                twitchMessage += "Lost " + info[headers.IndexOf("Title")] + " by " + info[headers.IndexOf("Difference")] + " RinThump";
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
                    if (int.Parse(score) == (int)info[headers.IndexOf("Score")])
                    {
                        Program.rin.SendTwitchMessage("Tied high score! " + info[headers.IndexOf("Title")] + " = " + ((int)info[headers.IndexOf("Score")] - int.Parse(score)).ToString());
                    }
                    Program.rin.SendTwitchMessage("New high score! " + info[headers.IndexOf("Title")] + " +" + ((int)info[headers.IndexOf("Score")] - int.Parse(score)).ToString());
                }
            }

            info.Add(DateTime.Now.ToString("MM/dd/yyyy"));
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

                if (info[headers.IndexOf("Account")].ToString() == "Deathblood")
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Normal\" + info[headers.IndexOf("Title")]);
                    if (dirInfo.Exists == false)
                    {
                        dirInfo.Create();
                    }
                    var result = dirInfo.GetFiles();
                    bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Normal\" + info[headers.IndexOf("Title")]  + "\\" + info[headers.IndexOf("Title")] + "." + info[headers.IndexOf("Difficulty")].ToString() + "." + result.Length + ".png", ImageFormat.Png);
                }
                else if (info[headers.IndexOf("Account")].ToString() == "RinzoP")
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Messy\" + info[headers.IndexOf("Title")]);
                    if (dirInfo.Exists == false)
                    {
                        dirInfo.Create();
                    }
                    var result = dirInfo.GetFiles();
                    bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Messy\" + info[headers.IndexOf("Title")] + "\\" + info[headers.IndexOf("Title")] + "." + info[headers.IndexOf("Difficulty")].ToString() + "." + result.Length + ".png", ImageFormat.Png);

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
        public void GetRandomSong()
        {
            var Headers = GetHeaders("Oni");

            List<string> songs = new List<string>();
            List<int> goalValues = new List<int>();

            List<int> songOKs = new List<int>();
            List<int> goalOKs = new List<int>();

            string range = "Oni!A2:" + GetColumnName(Headers.Count);
            var values = GetValues(range);

            int numOni = 0;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i][Headers.IndexOf("Goal OKs")].ToString() == "" || int.Parse(values[i][Headers.IndexOf("OK")].ToString()) == 0 || int.Parse(values[i][Headers.IndexOf("OK")].ToString()) - int.Parse(values[i][Headers.IndexOf("Goal OKs")].ToString()) <= 0)
                {
                    continue;
                }
                songs.Add(values[i][Headers.IndexOf("Title")].ToString());
                int songOK = int.Parse(values[i][Headers.IndexOf("OK")].ToString());
                int goalOK = int.Parse(values[i][Headers.IndexOf("Goal OKs")].ToString());
                //int numNotes = int.Parse(values[i][Headers.IndexOf("GOOD")].ToString()) + songOK + int.Parse(values[i][Headers.IndexOf("BAD")].ToString());
                int goalValue = (songOK + int.Parse(values[i][Headers.IndexOf("BAD")].ToString())) - goalOK;
                goalValues.Add(goalValue);
                songOKs.Add(songOK);
                goalOKs.Add(goalOK);


                numOni++;
            }

            range = "Ura!A2:" + GetColumnName(Headers.Count);
            values = GetValues(range);
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i][Headers.IndexOf("Goal OKs")].ToString() == "" || int.Parse(values[i][Headers.IndexOf("OK")].ToString()) == 0 || int.Parse(values[i][Headers.IndexOf("OK")].ToString()) - int.Parse(values[i][Headers.IndexOf("Goal OKs")].ToString()) <= 0)
                {
                    continue;
                }
                songs.Add(values[i][Headers.IndexOf("Title")].ToString());
                int songOK = int.Parse(values[i][Headers.IndexOf("OK")].ToString());
                int goalOK = int.Parse(values[i][Headers.IndexOf("Goal OKs")].ToString());
                //int numNotes = int.Parse(values[i][Headers.IndexOf("GOOD")].ToString()) + songOK + int.Parse(values[i][Headers.IndexOf("BAD")].ToString());
                int goalValue = (songOK + int.Parse(values[i][Headers.IndexOf("BAD")].ToString())) - goalOK;
                goalValues.Add(goalValue);
                songOKs.Add(songOK);
                goalOKs.Add(goalOK);
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
                if (randValue < goalValues[i])
                {
                    // Song picked
                    string message = songs[i];

                    if(i > numOni)
                    {
                        message += " Ura";
                    }


                    message += ", " + songOKs[i] + " OKs -> " + goalOKs[i] + " Goal OKs, " + chances[i] * 100 + "% chance of hitting this song";

                    Program.rin.SendTwitchMessage(message);

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
                DateTime.Now.ToString("MM/dd/yyyy")
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
