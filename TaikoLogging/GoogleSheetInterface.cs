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

        public void AddRankedEntry(RankedPlay play)
        {
            var Headers = GetHeaders("'Ranked Logs'");
            // Check the sheet to see the match #
            var values = GetValues("Ranked Logs!A2:" + GetColumnName(Headers.Count));

            play.MatchNumber = int.Parse(values[0][0].ToString()) + 1;

            play.Difference = play.Score - play.OppScore;

            // Might need to be Headers.Count-1, might not actually matter
            List<IList<object>> sendValues = new List<IList<object>>();
            IList<object> baseValues = new List<object>();
            for (int i = 0; i < Headers.Count; i++)
            {
                if (Headers[i] == "Match")
                {
                    baseValues.Add(play.MatchNumber);
                }
                else if (Headers[i] == "Title")
                {
                    baseValues.Add(play.Title);
                }
                else if (Headers[i] == "Genre")
                {
                    baseValues.Add("=VLOOKUP(B2, PS4!B:D, 3, false)");
                }
                else if (Headers[i] == "★")
                {
                    baseValues.Add("=VLOOKUP(B2, PS4!B:E, 4, false)");
                }
                else if (Headers[i] == "Result")
                {
                    baseValues.Add(play.Result);
                }
                else if (Headers[i] == "Difference")
                {
                    baseValues.Add(play.Difference);
                }
                else if (Headers[i] == "My Score")
                {
                    baseValues.Add(play.Score);
                }
                else if (Headers[i] == "Acc")
                {
                    baseValues.Add(string.Format("{0:F2}%", play.Accuracy));
                }
                else if (Headers[i] == "My Goods")
                {
                    baseValues.Add(play.GOOD);
                }
                else if (Headers[i] == "My OKs")
                {
                    baseValues.Add(play.OK);
                }
                else if (Headers[i] == "My Bads")
                {
                    baseValues.Add(play.BAD);
                }
                else if (Headers[i] == "My Combo")
                {
                    baseValues.Add(play.Combo);
                }
                else if (Headers[i] == "My Drumroll")
                {
                    baseValues.Add(play.Drumroll);
                }
                else if (Headers[i] == "Opp Score")
                {
                    baseValues.Add(play.OppScore);
                }
                else if (Headers[i] == "Opp Acc")
                {
                    baseValues.Add(string.Format("{0:F2}%", play.OppAcc));
                }
                else if (Headers[i] == "Opp Goods")
                {
                    baseValues.Add(play.OppGOOD);
                }
                else if (Headers[i] == "Opp OKs")
                {
                    baseValues.Add(play.OppOK);
                }
                else if (Headers[i] == "Opp Bads")
                {
                    baseValues.Add(play.OppBAD);
                }
                else if (Headers[i] == "Opp Combo")
                {
                    baseValues.Add(play.OppCombo);
                }
                else if (Headers[i] == "Opp Drumroll")
                {
                    baseValues.Add(play.OppDrumroll);
                }
                else if (Headers[i] == "DateTime")
                {
                    baseValues.Add(play.Time.ToString("MM/dd/yyyy hh:mm tt"));
                }
                else
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

            string twitchMessage = "Match " + play.MatchNumber + ": ";
            if (play.Result == "Win")
            {
                twitchMessage += "Won " + play.Title + " by " + play.Difference + " RinComfy";
            }
            else if (play.Result == "Lose")
            {
                twitchMessage += "Lost " + play.Title + " by " + play.Difference + " RinThump";
            }
            else
            {
                twitchMessage += "Tied " + play.Title + " RoWOOW";
            }


            Program.rin.SendTwitchMessage(twitchMessage);

            // NOT TESTING
            play.Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Ranked Logs\" + play.MatchNumber + ".png", ImageFormat.Png);

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
        public void UpdatePS4Single(Play play)
        {
            var Headers = GetHeaders("PS4");
            string range = "PS4";

            if (play.Account == "RinzoP")
            {
                range += " Messy";
            }

            range += "!A2:" + GetColumnName(Headers.Count);

            var values = GetValues(range);

            // This part should really be a function on its own, I use it in nearly every function here
            int songIndex = -1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[Headers.IndexOf("Title")].ToString() == play.Title)
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
                //I don't even look at the logger, not gonna bother fixing this
                //Program.logger.LogManyVariables("UpdatePS4HighScore: Song Not Found", headers, info);
                Console.WriteLine("Couldn't find the song in the spreadsheet");
                return;
            }

            string oldScore;
            if (values[songIndex][Headers.IndexOf("Score")].ToString() == "")
            {
                oldScore = "0";
            }
            else
            {
                oldScore = values[songIndex][Headers.IndexOf("Score")].ToString().Replace(",", "");
            }

            bool newHighScore = false;
            if (int.Parse(oldScore) < play.Score && play.Mode != ImageAnalysis.Mode.PS4Ranked)
            {
                newHighScore = true;
            }

            string oldBestAcc;
            if (values[songIndex][Headers.IndexOf("Best Acc")].ToString() == "")
            {
                oldBestAcc = "0";
            }
            else
            {
                oldBestAcc = values[songIndex][Headers.IndexOf("Best Acc")].ToString().Replace("%", "");
            }

            float AccPerOK = (1f / (play.GOOD + play.OK + play.BAD) / 2f) * 100;

            bool newBestAcc = false;
            bool almostBestAcc = false;
            if (float.Parse(oldBestAcc) < play.Accuracy)
            {
                newBestAcc = true;
            }
            else if (((float.Parse(oldBestAcc) - play.Accuracy) / AccPerOK) <= 10)
            {
                almostBestAcc = true;
            }


            //send all the information onto the sheet in the correct spots
            IList<object> baseValues = new List<object>();
            for (int i = 0; i < Headers.Count; i++)
            {
                if (Headers[i] == "Score" && newHighScore == true)
                {
                    baseValues.Add(play.Score);
                }
                else if (Headers[i] == "Acc" && newHighScore == true)
                {
                    baseValues.Add(string.Format("{0:F2}%", play.Accuracy));
                }
                else if (Headers[i] == "GOOD" && newHighScore == true)
                {
                    baseValues.Add(play.GOOD);
                }
                else if (Headers[i] == "OK" && newHighScore == true)
                {
                    baseValues.Add(play.OK);
                }
                else if (Headers[i] == "BAD" && newHighScore == true)
                {
                    baseValues.Add(play.BAD);
                }
                else if (Headers[i] == "Combo" && newHighScore == true)
                {
                    baseValues.Add(play.Combo);
                }
                else if (Headers[i] == "Drumroll" && newHighScore == true)
                {
                    baseValues.Add(play.Drumroll);
                }
                else if (Headers[i] == "HighScore DateTime" && newHighScore == true)
                {
                    baseValues.Add(play.Time.ToString("MM/dd/yyyy hh:mm tt"));
                }
                else if (Headers[i] == "Latest Play DateTime")
                {
                    baseValues.Add(play.Time.ToString("MM/dd/yyyy hh:mm tt"));
                }
                else if (Headers[i] == "Recent Acc")
                {
                    baseValues.Add(string.Format("{0:F2}%", play.RecentAcc));
                }
                else if (Headers[i] == "Best Acc" && newBestAcc == true)
                {
                    baseValues.Add(string.Format("{0:F2}%", play.Accuracy));
                }
                else
                {
                    baseValues.Add(null);
                }
            }

            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);

            range = "PS4";
            if (play.Account == "RinzoP")
            {
                range += " Messy";
            }
            range += "!A" + (songIndex + 2) + ":" + GetColumnName(Headers.Count) + (songIndex + 2);
            SendData(range, sendValues);


            // Send twitch messages
            if (newHighScore)
            {
                string songTitle = play.Title;
                if (play.Account == "RinzoP")
                {
                    songTitle += " Messy";
                }
                if (int.Parse(oldScore) == play.Score)
                {
                    Program.rin.SendTwitchMessage("Tied high score! " + songTitle + " = " + (play.Score - int.Parse(oldScore)).ToString());
                }
                else
                {
                    Program.rin.SendTwitchMessage("New high score! " + songTitle + " +" + (play.Score - int.Parse(oldScore)).ToString());
                }
            }
            if (newBestAcc)
            {
                string songTitle = play.Title;
                if (play.Account == "RinzoP")
                {
                    songTitle += " Messy";
                }
                string twitchMessage = "New best accuracy on " + songTitle + ". +" + string.Format("{0:F2}%", play.Accuracy - (float.Parse(oldBestAcc)));
                if (play.Accuracy - (float.Parse(oldBestAcc)) < 50)
                {
                    twitchMessage += ", equivalent to " + Math.Round((play.Accuracy - float.Parse(oldBestAcc)) / AccPerOK) + " less OKs";
                }
                Program.rin.SendTwitchMessage(twitchMessage);
            }
            else if (almostBestAcc)
            {
                string songTitle = play.Title;
                if (play.Account == "RinzoP")
                {
                    songTitle += " Messy";
                }
                if (float.Parse(oldBestAcc) == play.Accuracy && play.Accuracy != 100)
                {
                    Program.rin.SendTwitchMessage("Tied best accuracy on " + songTitle + " with " + string.Format("{0:F2}%", play.Accuracy) + "!");
                }
                else // bestAcc was close, but not tied
                {
                    string twitchMessage = "Almost best accuracy on " + songTitle + ". -" + string.Format("{0:F2}%", (float.Parse(oldBestAcc) - play.Accuracy));
                    twitchMessage += ", equivalent to " + (Math.Round((play.Accuracy - float.Parse(oldBestAcc)) / AccPerOK) * -1) + " more OKs";
                    Program.rin.SendTwitchMessage(twitchMessage);
                }
            }

            // Save images

            if (newHighScore)
            {
                var fileSongTitle = Program.MakeValidFileName(play.Title);
                DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + play.Mode.ToString() + @"\" + fileSongTitle);
                if (dirInfo.Exists == false)
                {
                    dirInfo.Create();
                }
                var result = dirInfo.GetFiles();


                play.Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + play.Mode.ToString() + @"\" + fileSongTitle + "\\" + fileSongTitle + "." + play.Difficulty + "." + result.Length + ".png", ImageFormat.Png);

            }


        }
        //public void UpdatePS4HighScore(Play play)
        //{
        //    var Headers = GetHeaders("PS4");
        //    string range = string.Empty;
        //    // Find the song in the spreadsheet
        //    if (play.Account == "RinzoP")
        //    {
        //        range += "Messy " + play.Difficulty + "!";
        //    }
        //    else
        //    {
        //        range += play.Difficulty + "!";
        //    }
        //    range += GetColumnName(Headers.IndexOf("Title")) + "2:" + GetColumnName(Headers.IndexOf("Score"));
        //    var values = GetValues(range);
        //    int songIndex = -1;
        //    if (values != null && values.Count > 0)
        //    {
        //        foreach (var row in values)
        //        {
        //            if (row[0].ToString() == play.Title)
        //            {
        //                songIndex = values.IndexOf(row);
        //                break;
        //            }
        //        }
        //    }

        //    if (songIndex == -1)
        //    {
        //        // something got fucked up
        //        // probably a good idea to set up some form of logging for this sort of thing, but meh
        //        //I don't even look at the logger, not gonna bother fixing this
        //        //Program.logger.LogManyVariables("UpdatePS4HighScore: Song Not Found", headers, info);

        //        return;
        //    }

        //    // This isn't very fluid
        //    // If something gets changed in the spreadsheet, this breaks
        //    string score = string.Empty;

        //    if (values[songIndex].Count < 5)
        //    {
        //        score = "0";
        //    }
        //    else
        //    {
        //        score = values[songIndex][4].ToString();
        //    }
        //    while (true)
        //    {
        //        if (score.IndexOf(',') == -1)
        //        {
        //            break;
        //        }
        //        score = score.Remove(score.IndexOf(','), 1);
        //    }

        //    //double check that the new score is higher
        //    if (int.Parse(score) >= play.Score)
        //    {
        //        // This'd mean the score on the spreadsheet is higher than the new score
        //        return;
        //    }
        //    else
        //    {

        //        string songTitle = play.Title;
        //        if (play.Difficulty == ImageAnalysis.Difficulty.Ura)
        //        {
        //            songTitle += " Ura";
        //        }
        //        if (play.Account == "RinzoP")
        //        {
        //            songTitle += " Messy";
        //        }
        //        if (int.Parse(score) == play.Score)
        //        {
        //            Program.rin.SendTwitchMessage("Tied high score! " + songTitle + " = " + (play.Score - int.Parse(score)).ToString());
        //        }
        //        else
        //        {
        //            Program.rin.SendTwitchMessage("New high score! " + songTitle + " +" + (play.Score - int.Parse(score)).ToString());
        //        }

        //    }

        //    //send all the information onto the sheet in the correct spots
        //    IList<object> baseValues = new List<object>();
        //    for (int i = Headers.IndexOf("Score"); i < Headers.Count; i++)
        //    {
        //        if (Headers[i] == "Score")
        //        {
        //            baseValues.Add(play.Score);
        //        }
        //        else if (Headers[i] == "GOOD")
        //        {
        //            baseValues.Add(play.GOOD);
        //        }
        //        else if (Headers[i] == "OK")
        //        {
        //            baseValues.Add(play.OK);
        //        }
        //        else if (Headers[i] == "BAD")
        //        {
        //            baseValues.Add(play.BAD);
        //        }
        //        else if (Headers[i] == "Combo")
        //        {
        //            baseValues.Add(play.Combo);
        //        }
        //        else if (Headers[i] == "Drumroll")
        //        {
        //            baseValues.Add(play.Drumroll);
        //        }
        //        else if (Headers[i] == "DateTime")
        //        {
        //            baseValues.Add(play.Time.ToString("MM/dd/yyyy hh:mm tt"));
        //        }
        //        else
        //        {
        //            baseValues.Add(null);
        //        }
        //    }

        //    List<IList<object>> sendValues = new List<IList<object>>();
        //    sendValues.Add(baseValues);
        //    if (play.Account == "Deathblood")
        //    {
        //        range = play.Difficulty + "!";
        //    }
        //    else
        //    {
        //        range = "Messy " + play.Difficulty + "!";
        //    }

        //    range += GetColumnName(Headers.IndexOf("Score")) + (songIndex + 2).ToString() + ":"
        //        + GetColumnName(Headers.IndexOf("DateTime")) + (songIndex + 2).ToString();
        //    SendData(range, sendValues);


        //    var fileSongTitle = Program.MakeValidFileName(play.Title);

        //    if (play.Account == "Deathblood")
        //    {
        //        DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Normal\" + fileSongTitle);
        //        if (dirInfo.Exists == false)
        //        {
        //            dirInfo.Create();
        //        }
        //        var result = dirInfo.GetFiles();


        //        play.Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Normal\" + fileSongTitle + "\\" + fileSongTitle + "." + play.Difficulty + "." + result.Length + ".png", ImageFormat.Png);
        //    }
        //    else if (play.Account == "RinzoP")
        //    {
        //        DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Messy\" + fileSongTitle);
        //        if (dirInfo.Exists == false)
        //        {
        //            dirInfo.Create();
        //        }
        //        var result = dirInfo.GetFiles();
        //        play.Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\Messy\" + fileSongTitle + "\\" + fileSongTitle + "." + play.Difficulty + "." + result.Length + ".png", ImageFormat.Png);
        //    }






        //    //DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores");
        //    //var result = dirInfo.GetFiles();
        //    //int numScreenshots = 0;
        //    //for (int i = 0; i < result.Length; i++)
        //    //{
        //    //    if (result[i].Name.Remove(result[i].Name.IndexOf('.')) == info[headers.IndexOf("Title")].ToString() && result[i].Name.Remove(0, result[i].Name.IndexOf('.') + 1).Remove(result[i].Name.Remove(0, result[i].Name.IndexOf('.') + 1).IndexOf('.')) == info[headers.IndexOf("Difficulty")].ToString())
        //    //    {
        //    //        numScreenshots++;
        //    //    }
        //    //}

        //    //if (test == false)
        //    //{
        //    //    // NOT TESTING
        //    //    bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + info[headers.IndexOf("Title")] + "." + info[headers.IndexOf("Difficulty")].ToString() + "." + numScreenshots.ToString() + ".png", ImageFormat.Png);
        //    //}

        //}
        //public void UpdatePS4BestGoods(Play play)
        //{
        //    var Headers = GetHeaders("PS4");

        //    string range = string.Empty;
        //    // Find the song in the spreadsheet
        //    if (play.Account == "RinzoP")
        //    {
        //        range += "Messy " + play.Difficulty + "!";
        //    }
        //    else
        //    {
        //        range += play.Difficulty + "!";
        //    }
        //    range += GetColumnName(Headers.IndexOf("Title")) + "2:" + GetColumnName(Headers.IndexOf("Best Goods"));

        //    var values = GetValues(range);
        //    int songIndex = -1;
        //    if (values != null && values.Count > 0)
        //    {
        //        foreach (var row in values)
        //        {
        //            if (row[0].ToString() == play.Title)
        //            {
        //                songIndex = values.IndexOf(row);
        //            }
        //        }
        //    }

        //    if (songIndex == -1)
        //    {
        //        // something got fucked up
        //        // probably a good idea to set up some form of logging for this sort of thing, but meh
        //        // Broken and I don't feel like fixing it cuz I don't use it
        //        //Program.logger.LogManyVariables("UpdatePS4BestGoods: Song Not Found", headers, info);
        //        return;
        //    }
        //    string sheetGoods = string.Empty;
        //    if (values[songIndex].Count < 11)
        //    {
        //        sheetGoods = "0";
        //    }
        //    else
        //    {
        //        sheetGoods = values.ElementAt(songIndex)[10].ToString();
        //    }

        //    int goodsDifference = play.GOOD - int.Parse(sheetGoods);
        //    string twitchMessage = string.Empty;
        //    string songTitle = play.Title;

        //    if (play.GOOD < int.Parse(sheetGoods))
        //    {


        //        if (play.Difficulty == ImageAnalysis.Difficulty.Ura)
        //        {
        //            songTitle += " Ura";
        //        }
        //        if (play.Account == "RinzoP")
        //        {
        //            songTitle += " Messy";
        //        }
        //        if (goodsDifference == -1)
        //        {
        //            twitchMessage += "1 good away from best accuracy on " + songTitle + "!";
        //        }
        //        else if (goodsDifference > -11)
        //        {
        //            twitchMessage += (goodsDifference * -1) + " goods away from best accuracy on " + songTitle + "!";
        //        }
        //        else
        //        {
        //            return;
        //        }
        //        Program.rin.SendTwitchMessage(twitchMessage);


        //        return;
        //    }
        //    IList<object> baseValues = new List<object>
        //    {
        //        play.GOOD
        //    };
        //    List<IList<object>> sendValues = new List<IList<object>>();
        //    sendValues.Add(baseValues);


        //    if (play.Account == "Deathblood")
        //    {
        //        range = play.Difficulty + "!";
        //    }
        //    else
        //    {
        //        range = "Messy " + play.Difficulty + "!";
        //    }

        //    range += GetColumnName(Headers.IndexOf("Best Goods")) + (songIndex + 2).ToString();
        //    SendData(range, sendValues);



        //    if (play.Difficulty == ImageAnalysis.Difficulty.Ura)
        //    {
        //        songTitle += " Ura";
        //    }
        //    if (play.Account == "RinzoP")
        //    {
        //        songTitle += " Messy";
        //    }
        //    if (goodsDifference == 1)
        //    {
        //        twitchMessage += "New best accuracy on " + songTitle + ", " + goodsDifference.ToString() + " more good!";
        //    }
        //    else if (goodsDifference == 0)
        //    {
        //        twitchMessage += "Tied accuracy on " + songTitle + "!";
        //    }
        //    else
        //    {
        //        twitchMessage += "New best accuracy on " + songTitle + ", " + goodsDifference.ToString() + " more goods!";
        //    }
        //    Program.rin.SendTwitchMessage(twitchMessage);


        //}

        //public void UpdateRecentOKs(Play play)
        //{
        //    var Headers = GetHeaders("PS4");
        //    string range = string.Empty;
        //    // Find the song in the spreadsheet
        //    if (play.Account == "RinzoP")
        //    {
        //        range += "Messy " + play.Difficulty + "!";
        //    }
        //    else
        //    {
        //        range += play.Difficulty + "!";
        //    }
        //    range += "B2:B";

        //    var values = GetValues(range);

        //    int songIndex = -1;
        //    if (values != null && values.Count > 0)
        //    {
        //        foreach (var row in values)
        //        {
        //            if (row[0].ToString() == play.Title)
        //            {
        //                songIndex = values.IndexOf(row);
        //            }
        //        }
        //    }

        //    if (songIndex == -1)
        //    {
        //        // something got fucked up
        //        // probably a good idea to set up some form of logging for this sort of thing, but meh
        //        // Broken and I don't feel like fixing it cuz I don't use it
        //        //Program.logger.LogManyVariables("UpdatePS4BestGoods: Song Not Found", headers, info);
        //        return;
        //    }
        //    IList<object> baseValues = new List<object>
        //    {
        //        //play.RecentOKs, play.RecentBads
        //    };
        //    List<IList<object>> sendValues = new List<IList<object>>();
        //    sendValues.Add(baseValues);

        //    if (play.Account == "Deathblood")
        //    {
        //        range = play.Difficulty + "!";
        //    }
        //    else
        //    {
        //        range = "Messy " + play.Difficulty + "!";
        //    }

        //    range += GetColumnName(Headers.IndexOf("Recent OKs")) + (songIndex + 2).ToString() + ":" + GetColumnName(Headers.IndexOf("Recent Bads"));
        //    SendData(range, sendValues);
        //}
        public void AddRecentPlay(Play play)
        {
            var Headers = GetHeaders("'Recent Plays'");
            var values = GetValues("Recent Plays!A2");


            // Might need to be Headers.Count-1, might not actually matter
            values = GetValues("Recent Plays!A2:" + GetColumnName(Headers.Count));
            List<IList<object>> sendValues = new List<IList<object>>();
            IList<object> baseValues = new List<object>();
            for (int i = 0; i < Headers.Count; i++)
            {
                if (Headers[i] == "Title")
                {
                    baseValues.Add(play.Title);
                }
                else if (Headers[i] == "Genre")
                {
                    baseValues.Add("=VLOOKUP(A2, PS4!B:D, 3, false)");
                }
                else if (Headers[i] == "★")
                {
                    baseValues.Add("=VLOOKUP(A2, PS4!B:E, 4, false)");
                }
                else if (Headers[i] == "Mode")
                {
                    baseValues.Add(play.Mode.ToString());
                }
                else if (Headers[i] == "Score")
                {
                    baseValues.Add(play.Score);
                }
                else if (Headers[i] == "Acc")
                {
                    baseValues.Add(string.Format("{0:F2}%", play.Accuracy));
                }
                else if (Headers[i] == "GOOD")
                {
                    baseValues.Add(play.GOOD);
                }
                else if (Headers[i] == "OK")
                {
                    baseValues.Add(play.OK);
                }
                else if (Headers[i] == "BAD")
                {
                    baseValues.Add(play.BAD);
                }
                else if (Headers[i] == "Combo")
                {
                    baseValues.Add(play.Combo);
                }
                else if (Headers[i] == "Drumroll")
                {
                    baseValues.Add(play.Drumroll);
                }
                else if (Headers[i] == "Recent Acc")
                {
                    baseValues.Add(string.Format("{0:F2}%", play.RecentAcc));
                }
                else if (Headers[i] == "Recent Acc Change")
                {
                    baseValues.Add(string.Format("{0:F2}%", play.RecentAccChange));
                }
                else if (Headers[i] == "DateTime")
                {
                    baseValues.Add(play.Time.ToString("MM/dd/yyyy hh:mm tt"));
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
        }
        public void GetRandomSong()
        {
            var Headers = GetHeaders("PS4");

            List<string> songs = new List<string>();

            List<int> goalValues = new List<int>();
            List<float> songAccs = new List<float>();
            List<float> goalAccs = new List<float>();


            string range = "PS4!A2:" + GetColumnName(Headers.Count);
            var values = GetValues(range);

            for (int i = 0; i < values.Count; i++)
            {
                songs.Add(values[i][Headers.IndexOf("Title")].ToString());

                int songAcc = 0;
                int recentAcc = 0;
                int goalAcc = 0;
                int bestAcc = 0;

                int.TryParse(values[i][Headers.IndexOf("Acc")].ToString().Replace(".", "").Replace("%", ""), out songAcc);
                int.TryParse(values[i][Headers.IndexOf("Recent Acc")].ToString().Replace(".", "").Replace("%", ""), out recentAcc);
                int.TryParse(values[i][Headers.IndexOf("Goal Acc")].ToString().Replace(".", "").Replace("%", ""), out goalAcc);
                int.TryParse(values[i][Headers.IndexOf("Best Acc")].ToString().Replace(".", "").Replace("%", ""), out bestAcc);

                if (songAcc == 10000)
                {
                    continue;
                }

                int goalValue = 0;

                // This is super lazy, but I am too
                // RecentAcc is the only one that could fail right now anyway I think
                //try { songAcc = (int)(float.Parse(values[i][Headers.IndexOf("Acc")].ToString()) * 100); }
                //catch { }
                //try { recentAcc = (int)(float.Parse(values[i][Headers.IndexOf("Recent Acc")].ToString()) * 100); }
                //catch { }
                //try { goalAcc = (int)(float.Parse(values[i][Headers.IndexOf("Goal Acc")].ToString()) * 100); }
                //catch { }
                //try { bestAcc = (int)(float.Parse(values[i][Headers.IndexOf("Best Acc")].ToString()) * 100); }
                //catch { }

                goalValue = goalAcc - songAcc;


                int songLevel = int.Parse(values[i][Headers.IndexOf("★")].ToString());

                //int numNotes = int.Parse(values[i][Headers.IndexOf("GOOD")].ToString()) + songOK + songBad;
                //TimeSpan timeSinceLastPlayed;
                //if (values[i].Count > Headers.IndexOf("Latest Play DateTime"))
                //{
                //    if (values[i][Headers.IndexOf("Latest Play DateTime")].ToString() != "")
                //    {
                //        timeSinceLastPlayed = DateTime.Now - DateTime.Parse((string)values[i][Headers.IndexOf("Latest Play DateTime")]);
                //    }
                //    else
                //    {
                //        timeSinceLastPlayed = DateTime.Now - new DateTime(0);
                //    }
                //}
                //else
                //{
                //    timeSinceLastPlayed = DateTime.Now - new DateTime(0);
                //}

                goalValues.Add(goalValue);
                songAccs.Add(songAcc);
                goalAccs.Add(goalAcc);
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

                    float actualSongAcc = 0f;
                    if (songAccs[i] != 0)
                    {
                        actualSongAcc = songAccs[i] / 100;
                    }
                    float actualGoalAcc = 0f;
                    if (goalAccs[i] != 0)
                    {
                        actualGoalAcc = goalAccs[i] / 100;
                    }

                    message += ", " + string.Format("{0:F2}% Acc", actualSongAcc);
                    message += "-> " + string.Format("{0:F2}% Goal Acc", actualGoalAcc);
                    string consoleMessage = chances[i] * 100 + "% chance of picking this song\n";

                    if (CheckIfEnglish(songs[i]) == false)
                    {
                        consoleMessage += FindClosestReadableSong(songs, i);
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

            //#region Logging
            //List<object> loggingGoalDifference = new List<object>();
            //for (int i = 0; i < goalValues.Count; i++)
            //{
            //    loggingGoalDifference.Add(goalValues[i]);
            //}

            //Program.logger.LogManyVariables("Random", songs, loggingGoalDifference);
            //Program.logger.LogVariable("Random", "randValue", randValue);
            //#endregion
        }

        public string FindClosestReadableSong(List<string> songs, int i)
        {
            string message = string.Empty;
            for (int j = 1; j < songs.Count; j++)
            {
                if (i - j >= 0)
                {
                    if (CheckIfEnglish(songs[i - j]) == true)
                    {
                        message += "Right of " + songs[i - j] + "\n";
                        break;
                    }
                }
                else
                {
                    if (CheckIfEnglish(songs[i + j]) == true)
                    {
                        message += "Left of " + songs[i + j] + "\n";
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
            while (songTitle.IndexOfAny(englishCharacters.ToCharArray()) != -1)
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

        //public void FixRankedLogsData(List<object> info, List<string> headers, int matchNumber)
        //{
        //    var Headers = GetHeaders("Ranked Logs");
        //    string range = string.Empty;

        //    range = "Ranked Logs!A2:" + GetColumnName(Headers.IndexOf("DateTime"));

        //    var values = GetValues(range);

        //    List<IList<object>> sendValues = new List<IList<object>>();
        //    int index = 0;
        //    for (int i = 0; i < values.Count; i++)
        //    {
        //        IList<object> baseValues = new List<object>();
        //        if (matchNumber != int.Parse(values[i][Headers.IndexOf("Match")].ToString()))
        //        {
        //            continue;
        //        }
        //        index = i + 2;
        //        for (int k = 0; k < Headers.Count; k++)
        //        {
        //            bool headerFound = false;
        //            for (int l = 0; l < headers.Count; l++)
        //            {
        //                if (Headers[k] == headers[l])
        //                {
        //                    baseValues.Add(info[l].ToString());
        //                    headerFound = true;
        //                    break;
        //                }
        //            }
        //            if (headerFound == false)
        //            {
        //                baseValues.Add(null);
        //            }
        //        }
        //        sendValues.Add(baseValues);
        //        break;
        //    }

        //    range = "Ranked Logs!A" + index.ToString() + ":" + GetColumnName(Headers.Count);

        //    SendData(range, sendValues);
        //}

        #region Emulator functions
        public void UpdateEmulatorHighScore(Emulator.Play play)
        {
            var Headers = GetHeaders("Emulator");
            // Find the song in the spreadsheet
            string range = "Emulator" + "!A2:" + GetColumnName(Headers.Count);

            var values = GetValues(range);
            int songIndex = -1;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (row[Headers.IndexOf("Title")].ToString() == play.SongData.SongTitle)
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
                // Set up logger that works well for my updated functions, which this is not
                //Program.logger.LogManyVariables("UpdateEmulatorHighScore: Song Not Found", )
                Console.WriteLine("Failed to update " + play.SongData.SongTitle);
                Console.WriteLine("Couldn't find song in the spreadsheet");
                return;
            }

            string oldScore;
            if (values[songIndex][Headers.IndexOf("Score")].ToString() == "")
            {
                oldScore = "0";
            }
            else
            {
                oldScore = values[songIndex][Headers.IndexOf("Score")].ToString().Replace(",", "");
            }

            bool newHighScore = false;
            if (int.Parse(oldScore) < play.LastScore)
            {
                newHighScore = true;
            }


            string oldBestAcc;
            if (values[songIndex][Headers.IndexOf("Best Acc")].ToString() == "")
            {
                oldBestAcc = "0";
            }
            else
            {
                oldBestAcc = values[songIndex][Headers.IndexOf("Best Acc")].ToString().Replace("%", "");
            }

            float AccPerOK = (1f / (play.GOOD + play.OK + play.BAD) / 2f) * 100;

            bool newBestAcc = false;
            bool almostBestAcc = false;
            if (float.Parse(oldBestAcc) < play.LastAcc)
            {
                newBestAcc = true;
            }
            else if (((float.Parse(oldBestAcc) - play.LastAcc) / AccPerOK) <= 10)
            {
                almostBestAcc = true;
            }

            IList<object> baseValues = new List<object>();
            for (int i = 0; i < Headers.Count; i++)
            {
                if (Headers[i] == "Score" && newHighScore == true)
                {
                    baseValues.Add(play.Score);
                }
                else if (Headers[i] == "Acc" && newHighScore == true)
                {
                    baseValues.Add(string.Format("{0:F2}%", play.Accuracy));
                }
                else if (Headers[i] == "GOOD" && newHighScore == true)
                {
                    baseValues.Add(play.GOOD);
                }
                else if (Headers[i] == "OK" && newHighScore == true)
                {
                    baseValues.Add(play.OK);
                }
                else if (Headers[i] == "BAD" && newHighScore == true)
                {
                    baseValues.Add(play.BAD);
                }
                else if (Headers[i] == "Combo" && newHighScore == true)
                {
                    baseValues.Add(play.Combo);
                }
                else if (Headers[i] == "Best Acc" && newBestAcc == true)
                {
                    baseValues.Add(string.Format("{0:F2}%", play.LastAcc));
                }
                else if (Headers[i] == "Recent Acc" && play.RecentAcc != 0)
                {
                    baseValues.Add(string.Format("{0:F2}%", play.RecentAcc));
                }
                else if (Headers[i] == "Latest Play DateTime")
                {
                    baseValues.Add(play.LatestDateTime.ToString("MM/dd/yyyy hh:mm tt"));
                }
                else if (Headers[i] == "HighScore DateTime" && newHighScore == true)
                {
                    baseValues.Add(play.ScoreDateTime.ToString("MM/dd/yyyy hh:mm tt"));
                }
                else
                {
                    baseValues.Add(null);
                }
            }

            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);




       
            SendData("Emulator" + "!A" + (songIndex + 2).ToString() + ":" + GetColumnName(Headers.IndexOf("Update Time")) + (songIndex + 2).ToString(), sendValues);
            

            if (play.LatestDateTime < DateTime.Now - new TimeSpan(0, 5, 0))
            {
                return;
            }

            if (newHighScore)
            {
                string songTitle = play.SongData.SongTitle;
                if (int.Parse(oldScore) == play.LastScore)
                {
                    Program.rin.SendTwitchMessage("Tied high score! " + songTitle + " = " + (play.LastScore - int.Parse(oldScore)).ToString());
                }
                else
                {
                    Program.rin.SendTwitchMessage("New high score! " + songTitle + " +" + (play.LastScore - int.Parse(oldScore)).ToString());
                }
            }

            if (newBestAcc)
            {
                string songTitle = play.SongData.SongTitle;
                string twitchMessage = "New best accuracy on " + songTitle + ". +" + string.Format("{0:F2}%", play.LastAcc - (float.Parse(oldBestAcc)));
                if (play.LastAcc - (float.Parse(oldBestAcc)) < 50)
                {
                    twitchMessage += ", equivalent to " + Math.Round((play.LastAcc - float.Parse(oldBestAcc)) / AccPerOK) + " less OKs";
                }
                Program.rin.SendTwitchMessage(twitchMessage);
            }
            else if (almostBestAcc)
            {
                string songTitle = play.SongData.SongTitle;
                if (float.Parse(oldBestAcc) == Math.Round(play.LastAcc * 100f) / 100F && play.LastAcc != 100)
                {
                    Program.rin.SendTwitchMessage("Tied best accuracy on " + songTitle + " with " + string.Format("{0:F2}%", play.LastAcc) + "!");
                }
                else // bestAcc was close, but not tied
                {
                    string twitchMessage = "Almost best accuracy on " + songTitle + ". -" + string.Format("{0:F2}%", (float.Parse(oldBestAcc) - play.LastAcc));
                    twitchMessage += ", equivalent to " + (Math.Round((play.LastAcc - float.Parse(oldBestAcc)) / AccPerOK) * -1) + " more OKs";
                    Program.rin.SendTwitchMessage(twitchMessage);
                }
            }
        }

        public void AddRecentEmulatorPlay(Emulator.Play play)
        {
            var Headers = GetHeaders("'Recent Plays'");
            var values = GetValues("Recent Plays!A2");


            // Might need to be Headers.Count-1, might not actually matter
            values = GetValues("Recent Plays!A2:" + GetColumnName(Headers.Count));
            List<IList<object>> sendValues = new List<IList<object>>();
            IList<object> baseValues = new List<object>();
            for (int i = 0; i < Headers.Count; i++)
            {
                if (Headers[i] == "Title")
                {
                    baseValues.Add(play.SongData.SongTitle);
                }
                else if (Headers[i] == "Genre")
                {
                    baseValues.Add(play.SongData.Genre);
                }
                else if (Headers[i] == "★")
                {
                    baseValues.Add(play.SongData.Level);
                }
                else if (Headers[i] == "Mode")
                {
                    baseValues.Add(play.Mode.ToString());
                }
                else if (Headers[i] == "Score")
                {
                    baseValues.Add(play.LastScore);
                }
                else if (Headers[i] == "Acc")
                {
                    baseValues.Add(string.Format("{0:F2}%", play.LastAcc));
                }
                else if (Headers[i] == "GOOD")
                {
                    baseValues.Add(play.LastGoods);
                }
                else if (Headers[i] == "OK")
                {
                    baseValues.Add(play.LastOKs);
                }
                else if (Headers[i] == "BAD")
                {
                    baseValues.Add(play.LastBads);
                }
                else if (Headers[i] == "Combo")
                {
                    baseValues.Add(play.LastCombo);
                }
                else if (Headers[i] == "Drumroll")
                {
                    baseValues.Add("0");
                }
                else if (Headers[i] == "Recent Acc")
                {
                    baseValues.Add(string.Format("{0:F2}%", play.RecentAcc));
                }
                else if (Headers[i] == "Recent Acc Change")
                {
                    baseValues.Add(string.Format("{0:F2}%", play.RecentAccChange));
                }
                else if (Headers[i] == "DateTime")
                {
                    baseValues.Add(play.LatestDateTime.ToString("MM/dd/yyyy hh:mm tt"));
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
