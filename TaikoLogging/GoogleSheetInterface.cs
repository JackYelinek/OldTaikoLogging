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

        public void AddRankedEntry(string title, int[] info, ImageAnalysis.Difficulty difficulty, bool winLoss, Bitmap bmp)
        {
            // Check the sheet to see the match #
            string range = "Ranked Logs!A2:A";
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            var values = response.Values;
            
            int matchNumber = int.Parse(values.ElementAt(values.Count - 1)[0].ToString()) + 1;
            // hard coding the value for now because my plan for it only works if there's actually stuff there
            string winLossString;
            if (winLoss == true)
            {
                winLossString = "Win";
            }
            else
            {
                winLossString = "Lose";
            }
            IList<object> baseValues = new List<object>
            {
                matchNumber, title, difficulty.ToString(), winLossString, info[0]-info[6], info[0], info[1], info[2], info[3], info[4], info[5], info[6], info[7], info[8], info[9], info[10], info[11]
            };
            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);


            SendData("Ranked Logs!A" + (matchNumber - 1440).ToString() + ":Q" + (matchNumber - 1440).ToString(), sendValues);
            //List<Google.Apis.Sheets.v4.Data.ValueRange> updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            //var dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            //dataValueRange.Range = "Ranked Logs!A" + (matchNumber - 1440).ToString() +":Q" + (matchNumber - 1440).ToString();
            //dataValueRange.Values = sendValues;
            //updateData.Add(dataValueRange);

            //Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            //requestBody.Data = updateData;
            //requestBody.ValueInputOption = "USER_ENTERED";

            //var updateRequest = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            //var updateResponse = updateRequest.Execute();

            string twitchMessage = "Match " + matchNumber.ToString() + ": ";
            if (winLoss == true)
            {
                twitchMessage += "Won "+title+" by " + (info[0] - info[6]).ToString() + " RinComfy"; 
            }
            else
            {
                twitchMessage += "Lost "+title+" by " + (info[6] - info[0]).ToString() + " RinThump";
            }


            Program.rin.SendTwitchMessage(twitchMessage);


            baseValues = new List<object>
            {
                DateTime.Now.ToString("MM/dd/yyyy")
            };
            sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);


            SendData("Ranked Logs!R" + (matchNumber - 1440).ToString(), sendValues);

            //updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            //dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            //dataValueRange.Range = "Ranked Logs!R" + (matchNumber - 1440).ToString();
            //dataValueRange.Values = sendValues;
            //updateData.Add(dataValueRange);

            //requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            //requestBody.Data = updateData;
            //requestBody.ValueInputOption = "USER_ENTERED";

            //updateRequest = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            //updateResponse = updateRequest.Execute();

            // NOT TESTING
            bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Ranked Logs\" + matchNumber.ToString() + ".png", ImageFormat.Png);
            Console.WriteLine("Ranked match logged");

        }

        public void RemoveLastRanked()
        {
            // Check the sheet to see the match #
            string range = "Ranked Logs!A2:A";
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            var values = response.Values;

            int matchNumber = int.Parse(values.ElementAt(values.Count - 1)[0].ToString());

            IList<object> baseValues = new List<object>
            {
                "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""
            };

            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);

            SendData("Ranked Logs!A" + (matchNumber - 1440).ToString() + ":R" + (matchNumber - 1440).ToString(), sendValues);

            //List<Google.Apis.Sheets.v4.Data.ValueRange> updateData = new List<Google.Apis.Sheets.v4.Data.ValueRange>();
            //var dataValueRange = new Google.Apis.Sheets.v4.Data.ValueRange();
            //dataValueRange.Range = "Ranked Logs!A" + (matchNumber - 1440).ToString() + ":R" + (matchNumber - 1440).ToString();
            //dataValueRange.Values = sendValues;
            //updateData.Add(dataValueRange);

            //Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest requestBody = new Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest();
            //requestBody.Data = updateData;
            //requestBody.ValueInputOption = "USER_ENTERED";

            //var updateRequest = service.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            //var updateResponse = updateRequest.Execute();
        }
        public void UpdatePS4HighScore(string title, int[] info, ImageAnalysis.Difficulty difficulty, Bitmap bmp)
        {
            // I want to be able to just look for the column header so even if I change things in the sheet, it'll put things in the right spot
            //string headerRange = difficulty.ToString()+"A1:1";
            //var headerRequest = service.Spreadsheets.Values.Get(spreadsheetId, headerRange);
            //ValueRange headerResponse = headerRequest.Execute();
            //var headerValues = headerResponse.Values;
            //int headerIndex = 0;
            //if (headerValues != null && headerValues.Count > 0)
            //{
            //    foreach(var row in headerValues)
            //    {
            //        foreach(var column in row)
            //        {

            //            if (column.ToString() == "Title")
            //            {
            //                break;
            //            }
            //            headerIndex++;
            //        }
            //    }
            //}

            // Find the song in the spreadsheet
            string range = difficulty.ToString()+"!B2:F";
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            var values = response.Values;
            int songIndex = -1;
            if (values != null && values.Count > 0)
            {
                foreach(var row in values)
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
            if (int.Parse(score) >= info[0])
            {
                // This'd mean the score on the spreadsheet is higher than the new score
                return;
            }



            //send all the information onto the sheet in the correct spots
            IList<object> baseValues = new List<object>
            {
                info[0], info[1], info[2], info[3], info[4], info[5]
            };
            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);

            SendData(difficulty.ToString() + "!F" + (songIndex + 2).ToString() + ":K" + (songIndex + 2).ToString(), sendValues);
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

            Program.rin.SendTwitchMessage("New high score! " + title + " +" + (info[0] - int.Parse(score)).ToString());


            baseValues = new List<object>
            {
                DateTime.Now.ToString("MM/dd/yyyy")
            };
            sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);


            SendData(difficulty.ToString() + "!O" + (songIndex + 2).ToString(), sendValues);
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
                if (result[i].Name.Remove(result[i].Name.IndexOf('.')) == title && result[i].Name.Remove(0, result[i].Name.IndexOf('.') + 1).Remove(result[i].Name.Remove(0, result[i].Name.IndexOf('.') + 1).IndexOf('.')) == difficulty.ToString())
                {
                    numScreenshots++;
                }
            }

            // NOT TESTING
            bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + title + "." + difficulty.ToString() + "." + numScreenshots.ToString() + ".png", ImageFormat.Png);
            Console.WriteLine("HighScore logged");

        }

        public void UpdatePS4BestGoods(string title, int goods, ImageAnalysis.Difficulty difficulty)
        {
            string range = difficulty.ToString() + "!B2:L";
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
                return;
            }

            string sheetGoods = values.ElementAt(songIndex)[10].ToString();
            if (goods < int.Parse(sheetGoods))
            {
                return;
            }
            IList<object> baseValues = new List<object>
            {
                goods
            };
            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);


            SendData(difficulty.ToString() + "!L" + (songIndex + 2).ToString(), sendValues);

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

            int goodsDifference = goods - int.Parse(sheetGoods);
            string twitchMessage = string.Empty;
            if (goodsDifference == 1)
            {
                twitchMessage += "New best accuracy on " + title + ", " + goodsDifference.ToString() + " more good!";
            }
            else
            {
                twitchMessage += "New best accuracy on " + title + ", " + goodsDifference.ToString() + " more goods!";
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


        private List<string> GetHeaders()
        {
            // This part would have to be updated if more headers are added
            // I don't think it'd allow me to make it go past AG if there's no cells there
            // Also note this is just for the Beatmaps sheet's headers
            string range = "Oni!B1:O1";
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
