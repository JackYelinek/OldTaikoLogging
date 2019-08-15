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

namespace TaikoLogging
{
    class GoogleSheetInterface
    {

        string[] Scopes = { SheetsService.Scope.Spreadsheets };
        string ApplicationName = "Ranked Logs";

        string spreadsheetId = "15qtcBVmes43LlejxgynYoWWxqf99bVK-WmQUfGZfupo";

        SheetsService service;



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
            var values = GetValues("Ranked Logs!A2:A");
            
            info.Add(int.Parse(values.ElementAt(values.Count - 1)[0].ToString()) + 1);
            headers.Add("Match");

            if ((bool)info[headers.IndexOf("Win/Loss")] == true)
            {
                info[headers.IndexOf("Win/Loss")] = "Win";
            }
            else
            {
                info[headers.IndexOf("Win/Loss")] = "Lose";
            }

            info.Add((int)info[headers.IndexOf("My Score")] - (int)info[headers.IndexOf("Opp Score")]);
            headers.Add("Difference");

            info.Add(DateTime.Now.ToString("MM/dd/yyyy"));
            headers.Add("DateTime");

            IList<object> baseValues = new List<object>();
            for (int i = 0; i < Headers.Count; i++)
            {
                if (Headers[i] == headers[i])
                {
                    baseValues.Add(info[i]);
                }
            }

            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);

            SendData("Ranked Logs!A" + ((int)info[headers.IndexOf("Match")] - 1440).ToString() + ":Q" + ((int)info[headers.IndexOf("Match")] - 1440).ToString(), sendValues);


            string twitchMessage = "Match " + info[headers.IndexOf("Match")].ToString() + ": ";
            if ((string)info[headers.IndexOf("Win/Loss")] == "Win")
            {
                twitchMessage += "Won "+ info[headers.IndexOf("Song")] + " by " + info[headers.IndexOf("Difference")] + " RinComfy"; 
            }
            else
            {
                twitchMessage += "Lost "+ info[headers.IndexOf("Song")] + " by " + info[headers.IndexOf("Difference")] + " RinThump";
            }


            Program.rin.SendTwitchMessage(twitchMessage);

            // NOT TESTING
            bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Ranked Logs\" + info[headers.IndexOf("Match")] + ".png", ImageFormat.Png);
            Console.WriteLine("Ranked match logged");

        }

        public void RemoveLastRanked()
        {
            // Check the sheet to see the match #
            string range = "Ranked Logs!A2:A";
            var values = GetValues(range);

            int matchNumber = int.Parse(values.ElementAt(values.Count - 1)[0].ToString());

            var Headers = GetHeaders("'Ranked Logs'");

            IList<object> baseValues = new List<object>();

            for (int i = 0; i < Headers.Count; i++)
            {
                baseValues.Add("");
            }

            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);

            SendData("Ranked Logs!A" + (matchNumber - 1440).ToString() + ":R" + (matchNumber - 1440).ToString(), sendValues);

        }
        public void UpdatePS4HighScore(List<object> info, List<string> headers, Bitmap bmp)
        {
            var Headers = GetHeaders("Oni");

            // Find the song in the spreadsheet
            string range = info[headers.IndexOf("Difficulty")].ToString()+"!B2:F";
            var values = GetValues(range);
            int songIndex = -1;
            if (values != null && values.Count > 0)
            {
                foreach(var row in values)
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
                return;
            }

            string score = values.ElementAt(songIndex).ElementAt(4).ToString();
            while(true)
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
                if (int.Parse(score) == (int)info[headers.IndexOf("Score")])
                {
                    Program.rin.SendTwitchMessage("Tied high score! " + info[headers.IndexOf("Title")] + " = " + ((int)info[headers.IndexOf("Score")] - int.Parse(score)).ToString());
                }
                Program.rin.SendTwitchMessage("New high score! " + info[headers.IndexOf("Title")] + " +" + ((int)info[headers.IndexOf("Score")] - int.Parse(score)).ToString());
            }



            //send all the information onto the sheet in the correct spots
            List<IList<object>> sendValues = new List<IList<object>>();
            IList<object> baseValues = new List<object>
            {
                info[0], info[1], info[2], info[3], info[4], info[5]
            };
            sendValues.Add(baseValues);

            SendData(info[headers.IndexOf("Difficulty")].ToString() + "!F" + (songIndex + 2).ToString() + ":K" + (songIndex + 2).ToString(), sendValues);
            //List<Google.Apis.Sheets.v4.Data.ValueRange> updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            //var dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            //dataValueRange.Range = difficulty.ToString() + "!F" + (songIndex + 2).ToString() + ":K" + (songIndex + 2).ToString();
            //dataValueRange.Values = sendValues;
            //updateData.Add(dataValueRange);

            //Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            //requestBody.Data = updateData;
            //requestBody.ValueInputOption = "USER_ENTERED";

            //var updateRequest = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            //var updateResponse = updateRequest.Execute();



            baseValues = new List<object>
            {
                DateTime.Now.ToString("MM/dd/yyyy")
            };
            sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);


            SendData(info[headers.IndexOf("Difficulty")].ToString() + "!O" + (songIndex + 2).ToString(), sendValues);
            //updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            //dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            //dataValueRange.Range = difficulty.ToString()+"!O" + (songIndex + 2).ToString();
            //dataValueRange.Values = sendValues;
            //updateData.Add(dataValueRange);

            //requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            //requestBody.Data = updateData;
            //requestBody.ValueInputOption = "USER_ENTERED";

            //updateRequest = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            //updateResponse = updateRequest.Execute();


            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores");
            var result = dirInfo.GetFiles();
            int numScreenshots = 0;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].Name.Remove(result[i].Name.IndexOf('.')) == info[headers.IndexOf("Title")].ToString() && result[i].Name.Remove(0, result[i].Name.IndexOf('.') + 1).Remove(result[i].Name.Remove(0, result[i].Name.IndexOf('.') + 1).IndexOf('.')) == info[headers.IndexOf("Difficulty")].ToString())
                {
                    numScreenshots++;
                }
            }

            // NOT TESTING
            bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + info[headers.IndexOf("Title")] + "." + info[headers.IndexOf("Difficulty")].ToString() + "." + numScreenshots.ToString() + ".png", ImageFormat.Png);
            Console.WriteLine("HighScore logged");

        }

        public void UpdatePS4BestGoods(List<object> info, List<string> headers)
        {
            string range = info[headers.IndexOf("Difficulty")].ToString() + "!B2:L";
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            var values = response.Values;
            int songIndex = -1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[0].ToString() == info[headers.IndexOf("Title")])
                    {
                        songIndex = values.IndexOf(row);
                    }
                }
            }

            if (songIndex == -1)
            {
                // something got fucked up
                // probably a good idea to set up some form of logging for this sort of thing, but meh
                return;
            }

            string sheetGoods = values.ElementAt(songIndex)[10].ToString();
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


            SendData(info[headers.IndexOf("Difficulty")].ToString() + "!L" + (songIndex + 2).ToString(), sendValues);

            //List<Google.Apis.Sheets.v4.Data.ValueRange> updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            //var dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            //dataValueRange.Range = difficulty.ToString() + "!L" + (songIndex + 2).ToString();
            //dataValueRange.Values = sendValues;
            //updateData.Add(dataValueRange);

            //var requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            //requestBody.Data = updateData;
            //requestBody.ValueInputOption = "USER_ENTERED";

            //var updateRequest = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            //var updateResponse = updateRequest.Execute();

            int goodsDifference = (int)info[headers.IndexOf("GOOD")] - int.Parse(sheetGoods);
            string twitchMessage = string.Empty;
            if (goodsDifference == 1)
            {
                twitchMessage += "New best accuracy on " + info[headers.IndexOf("Title")] + ", " + goodsDifference.ToString() + " more good!";
            }
            else
            {
                twitchMessage += "New best accuracy on " + info[headers.IndexOf("Title")] + ", " + goodsDifference.ToString() + " more goods!";
            }
            Program.rin.SendTwitchMessage(twitchMessage);

        }

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


            Console.WriteLine("HighScore logged");

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

        #region Sheet Functions
        private IList<IList<object>> GetValues(string range)
        {
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            var values = response.Values;
            return values;
        }


        private List<string> GetHeaders(string sheet)
        {
            // This part would have to be updated if more headers are added
            // I don't think it'd allow me to make it go past AG if there's no cells there
            // Also note this is just for the Beatmaps sheet's headers
            string range = sheet + "!A1:Z1";
            var values = GetValues(range);
            List<string> Headers = new List<string>();
            for (int i = 0; i < values[0].Count; i++)
            {
                Headers.Add(values[0][i].ToString());
            }
            return Headers;
        }

        //public IList<IList<object>> GetSongData()
        //{
        //    var Headers = GetHeaders();
        //    string range = "Beatmaps!" + GetColumnName(Headers.IndexOf("★")) + "2:" + GetColumnName(Headers.IndexOf("Acc"));
        //    return GetValues(range);
        //}

        private void SendData(string range, IList<IList<object>> data)
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

        string GetColumnName(int index)
        {
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
