using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace TaikoLogging
{
    class ImageAnalysis
    {
        GoogleSheetInterface sheet;
        public enum State {CustomizeRoom, DifficultySelect, EventPage, MainMenu, MainMenuSettings, MenuLoading, PracticePause, PracticeSelect, PracticeSong,
            RankedEndSong, RankedLeaderboards, RankedMidSong, RankedPause, RankedPointsGain, RankedResults, RankedSelect, RankedSongFound, RankedStats,
            SingleSongEnd, SingleResults, SingleSong, SingleSessionResults, SingleSongPause, SongLoading, SongSelect, SongSelectSettings, SongSettings, TreasureBoxes};
        State previousState;
        State currentState;

        public enum Players { Single, RankedTop, RankedBottom};

        List<Bitmap> stateBitmaps = new List<Bitmap>();
        List<string> states = new List<string>();

        List<Bitmap> titleBitmaps = new List<Bitmap>();
        List<string> titles = new List<string>();

        List<Bitmap> highScoreBitmaps = new List<Bitmap>();

        List<Bitmap> bigNumberBitmaps = new List<Bitmap>();
        List<string> bigNumbers = new List<string>();
        List<Bitmap> smallNumberBitmaps = new List<Bitmap>();
        List<string> smallNumbers = new List<string>();

        public enum Difficulty { Easy, Hard, Normal, Oni, Ura };
        List<Bitmap> difficultyBitmaps = new List<Bitmap>();

        List<Bitmap> modBitmaps = new List<Bitmap>();
        List<string> mods = new List<string>();

        List<Bitmap> winLossBitmaps = new List<Bitmap>();

        List<Bitmap> accountBitmaps = new List<Bitmap>();

        private int j = 0;
        private int numStatesSaved = 0;
        public ImageAnalysis()
        {
            // expert level programming right here
            try
            {
                sheet = new GoogleSheetInterface();
            }
            catch
            {

            }

            InitializeAll();

            //TestingScreenshot();


        }

        public void StandardLoop()
        {
            using (Bitmap bmp = Program.screen.CaptureApplication())
            {
                currentState = CheckState(bmp);
                if (previousState != currentState)
                {
                    // names each file based on the number it was made in, then by the state it thought it was
                    // It saves every time it changes the state
                    if (currentState == State.SingleSongEnd)
                    {
                        Thread.Sleep(3500);
                        using (Bitmap resultsBmp = Program.screen.CaptureApplication())
                        {
                            currentState = CheckState(resultsBmp);
                            if (currentState == State.SingleResults)
                            {
                                GetSingleResults(false);
                            }
                            else if (currentState == State.SingleSessionResults)
                            {
                                GetSingleResults(true);
                            }
                        }
                    }
                    else if (currentState == State.RankedResults)
                    {
                        Thread.Sleep(4000);
                        GetRankedResults();
                    }
                }
            }
            System.GC.Collect();
        }

        public void NotStandardLoop()
        {
            var bmp = Program.screen.CaptureApplication();
            currentState = CheckState(bmp);
            GetSingleResults(false);

            if (previousState != currentState)
            {


            }

            System.GC.Collect();
        }

        private void TestingScreenshot()
        {
            Program.screen.CaptureApplication().Save(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\" + numStatesSaved++.ToString() + "." + currentState.ToString() + ".png", ImageFormat.Png);
        }

        //public void GetDLCSongs()
        //{
        //    var bmp = Program.screen.CaptureApplication();
        //    currentState = CheckState(bmp);
        //    if (previousState != currentState)
        //    {
        //        // names each file based on the number it was made in, then by the state it thought it was
        //        // It saves every time it changes the state
        //        if (currentState == State.SingleSongEnd)
        //        {
        //            TestingScreenshot();
        //            Thread.Sleep(3500);
        //            currentState = CheckState(Program.screen.CaptureApplication());
        //            if (currentState == State.SingleResults)
        //            {
        //                var dlcTitleBitmap = GetTitleBitmap(Program.screen.CaptureApplication());
        //                dlcTitleBitmap.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\DLC Songs\" + j++.ToString() + ".png");
        //            }
        //            else if (currentState == State.SingleSessionResults)
        //            {
        //                var dlcTitleBitmap = GetTitleBitmap(Program.screen.CaptureApplication());
        //                dlcTitleBitmap.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\DLC Songs\" + j++.ToString() + ".png");
        //            }
        //        }
        //    }
        //    System.GC.Collect();
        //}


        #region Initialization
        private void InitializeAll()
        {
            InitializeStateBitmaps();
            InitializeTitleBitmaps();
            InitializeHighScoreBitmaps();
            InitializeSmallNumbers();
            InitializeBigNumbers();
            InitializeDifficultyBitmaps();
            InitializeModBitmaps();
            InitializeWinLossBitmaps();
            InitializeAccountBitmaps();

            Console.WriteLine("Initialization Complete");
        }

        private void InitializeStateBitmaps()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\State Bitmaps");
            var result = dirInfo.GetFiles();
            DirectoryInfo originalInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\State Bitmaps Original");
            var originalResult = originalInfo.GetFiles();
            if (originalResult.Length != result.Length)
            {
                ScaleStateBitmaps();
                result = dirInfo.GetFiles();
            }
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                stateBitmaps.Add(bitmap);
                states.Add(result[i].Name.Remove(result[i].Name.IndexOf('.')));
            }
        }
        private void ScaleStateBitmaps()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\State Bitmaps Original");
            var result = dirInfo.GetFiles();
            DirectoryInfo smallBitmapsInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\State Bitmaps");
            var smallResults = smallBitmapsInfo.GetFiles();
            for (int i = 0; i < smallResults.Length; i++)
            {
                smallResults[i].Delete();
            }
            for (int i = 0; i < result.Length; i++)
            {
                var bmp = ScaleDown(new Bitmap(result[i].FullName), 38, 21);
                // NOT TESTING
                bmp.Save(string.Format(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\State Bitmaps\" + result[i].Name), System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        private void InitializeTitleBitmaps()
        {
            titleBitmaps = new List<Bitmap>();
            titles = new List<string>();
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Title Bitmaps");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                titleBitmaps.Add(bitmap);
                string songTitle = result[i].Name.Remove(result[i].Name.LastIndexOf('.'));
                titles.Add(songTitle.Remove(songTitle.LastIndexOf('.')));
            }
        }
        private void InitializeHighScoreBitmaps()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\HighScore");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                highScoreBitmaps.Add(bitmap);
            }
        }
        private void InitializeSmallNumbers()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Small Digits");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                smallNumberBitmaps.Add(bitmap);
                smallNumbers.Add(result[i].Name.Remove(result[i].Name.IndexOf('.')));
            }
        }
        private void InitializeBigNumbers()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Big Digits");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                bigNumberBitmaps.Add(bitmap);
                bigNumbers.Add(result[i].Name.Remove(result[i].Name.IndexOf('.')));
            }
        }
        private void InitializeDifficultyBitmaps()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Difficulty");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                difficultyBitmaps.Add(bitmap);
            }
        }
        private void InitializeModBitmaps()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Mods");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                modBitmaps.Add(bitmap);
                mods.Add(result[i].Name.Remove(result[i].Name.LastIndexOf('.')));
            }
        }
        private void InitializeWinLossBitmaps()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\WinLoss");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                winLossBitmaps.Add(bitmap);
            }
        }
        private void InitializeAccountBitmaps()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Account");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                accountBitmaps.Add(bitmap);
            }
        }
        #endregion


        public State CheckState(Bitmap bitmap)
        {
            var smallBitmap = ScaleDown(bitmap, 38, 21);
            int pixelDifferences = -1;
            int smallestIndex = 0;
            for (int i = 0; i < stateBitmaps.Count; i++)
            {

                #region State Checks
                ////if (currentState == State.CustomizeRoom && (i != (int)State.MenuLoading && i != (int)currentState))
                ////{
                ////    continue; // previousState defaults to CustomizeRoom, and since I basically never go there anyway, commenting this out shouldn't be a problem
                ////}
                //if (currentState == State.DifficultySelect && (i != (int)State.SongSelect && i != (int)State.SongSettings && i != (int)State.SongLoading && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.EventPage && (i != (int)State.RankedSelect && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.MainMenu && (i != (int)State.MenuLoading && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.MainMenuSettings && (i != (int)State.MenuLoading && i != (int)currentState))
                //{
                //    continue;
                //}
                ////if (currentState == State.MenuLoading && (i != (int)State.MenuLoading && i != (int)currentState))
                ////{
                ////    continue; // MenuLoading can go to basically anything, so this would just be a mess to make. I can make it later if it ends up being needed
                ////}
                //if (currentState == State.RankedEndSong && (i != (int)State.RankedResults && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.RankedLeaderboards && (i != (int)State.RankedSelect && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.RankedMidSong && (i != (int)State.RankedEndSong && i != (int)State.RankedResults && i != (int)State.RankedPause && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.RankedPause && (i != (int)State.MenuLoading && i != (int)State.RankedMidSong && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.RankedPointsGain && (i != (int)State.MenuLoading && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.RankedResults && (i != (int)State.RankedPointsGain && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.RankedSongFound && (i != (int)State.RankedMidSong && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.RankedStats && (i != (int)State.RankedSelect && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.SingleResults && (i != (int)State.MenuLoading && i != (int)currentState && i != (int)State.SongSelect))
                //{
                //    continue;
                //}
                //if (currentState == State.SingleSong && (i != (int)State.SingleSongPause && i != (int)State.SingleResults && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.SingleSongPause && (i != (int)State.MenuLoading && i != (int)State.SingleSong && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.SongLoading && (i != (int)State.SingleSong && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.SongSelect && (i != (int)State.MenuLoading && i != (int)State.DifficultySelect && i != (int)State.SongSelectSettings && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.SongSelectSettings && (i != (int)State.SongSelect && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.SongSettings && (i != (int)State.DifficultySelect && i != (int)currentState))
                //{
                //    continue;
                //}
                //if (currentState == State.TreasureBoxes && (i != (int)State.MenuLoading && i != (int)currentState))
                //{
                //    continue;
                //}
                #endregion

                var tmpInt = CompareBitmaps(smallBitmap, ScaleDown(stateBitmaps[i], 38, 21));
                if (tmpInt < pixelDifferences || pixelDifferences == -1)
                {
                    pixelDifferences = tmpInt;
                    smallestIndex = i;
                }
            }
            //if (previousState != currentState)
            //{
                //Console.WriteLine(pixelDifferences);
            //}
            previousState = currentState;
            Enum.TryParse(states[smallestIndex], out State state);
            return state;
        }

        public void GetSingleResults(bool isSession)
        {
            Bitmap bmp = Program.screen.CaptureApplication();

            List<object> info = new List<object>();
            List<string> headers = new List<string>();

            if (IsDeathblood(bmp, Players.Single) == false)
            {
                // I think it's safe to say that if it isn't my main account, I don't care to check the rest
                // I could eventually make a messy sheet, in which case I'd send everything to that if this is my alt
                return;
            }
            var mods = CheckMods(bmp, Players.Single);
            info.Add(CheckDifficulty(bmp, Players.Single));
            headers.Add("Difficulty");
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods.ElementAt(i) == "Shin-uchi")
                {
                    // I don't want to save any shin-uchi scores
                    return;
                }
            }
            if ((Difficulty)info[headers.IndexOf("Difficulty")] == Difficulty.Easy || (Difficulty)info[headers.IndexOf("Difficulty")] == Difficulty.Normal)
            {
                // I don't care about easy or normal for my sheet
                // I don't really care about hard either, but I have the sheet anyway, so might as well save it if I get it
                return;
            }

            Players players;

            if (isSession == true)
            {
                players = Players.RankedTop;
            }
            else
            {
                players = Players.Single;
            }


            info.Add(GetTitle(bmp));
            headers.Add("Title");

            info.Add(GetScore(bmp, players));
            headers.Add("Score");

            info.Add(GetGoods(bmp, players));
            headers.Add("GOOD");
            info.Add(GetOKs(bmp, players));
            headers.Add("OK");
            info.Add(GetBads(bmp, players));
            headers.Add("BAD");
            info.Add(GetCombo(bmp, players));
            headers.Add("MAX Combo");
            info.Add(GetDrumroll(bmp, players));
            headers.Add("Drumroll");

            if ((int)info[headers.IndexOf("Score")] % 10 != 0)
            {
                return;
            }
            bool highScore = IsHighScore(bmp);
            sheet.UpdatePS4BestGoods(info, headers);
            if (highScore == true)
            {
                sheet.UpdatePS4HighScore(info, headers, bmp);

                //DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores");
                //var result = dirInfo.GetFiles();
                // NOT USED, NOT TESTING
                //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + result.Length + ".png", ImageFormat.Png);
                Console.WriteLine("Highscore Logged");
            }
        }


        public void GetRankedResults()
        {
            Bitmap bmp = Program.screen.CaptureApplication();
            List<object> info = new List<object>();
            List<string> headers = new List<string>();

            bool account = IsDeathblood(bmp, Players.RankedTop);
            if (account == false)
            {
                // I don't care if it's ranked on my alt
                return;
            }

            // Song Data
            info.Add(GetTitle(bmp));
            headers.Add("Title");
            info.Add(CheckDifficulty(bmp, Players.RankedTop));
            headers.Add("Difficulty");

            // Top Player Data
            info.Add(GetScore(bmp, Players.RankedTop));
            headers.Add("My Score");
            info.Add(GetGoods(bmp, Players.RankedTop));
            headers.Add("My Goods");
            info.Add(GetOKs(bmp, Players.RankedTop));
            headers.Add("My OKs");
            info.Add(GetBads(bmp, Players.RankedTop));
            headers.Add("My Bads");
            info.Add(GetCombo(bmp, Players.RankedTop));
            headers.Add("My Combo");
            info.Add(GetDrumroll(bmp, Players.RankedTop));
            headers.Add("My Drumroll");

            // Bottom Player Data
            info.Add(GetScore(bmp, Players.RankedBottom));
            headers.Add("Opp Score");
            info.Add(GetGoods(bmp, Players.RankedBottom));
            headers.Add("Opp Goods");
            info.Add(GetOKs(bmp, Players.RankedBottom));
            headers.Add("Opp OKs");
            info.Add(GetBads(bmp, Players.RankedBottom));
            headers.Add("Opp Bads");
            info.Add(GetCombo(bmp, Players.RankedBottom));
            headers.Add("Opp Combo");
            info.Add(GetDrumroll(bmp, Players.RankedBottom));
            headers.Add("Opp Drumroll");

            // Result Data
            info.Add(RankedWinLoss(bmp));
            headers.Add("Win/Loss");

            // Check to see if the scores are possible, they must always end with a 0
            if ((int)info[headers.IndexOf("My Score")] % 10 != 0 || (int)info[headers.IndexOf("Opp Score")] % 10 != 0)
            {
                return;
            }
            sheet.AddRankedEntry(info, headers, bmp);
            sheet.UpdatePS4BestGoods(info, headers);
        }

        //private void GetSingleResults(Bitmap bmp, bool Testing)
        //{
        //    //Bitmap bmp = Program.screen.CaptureApplication();

        //    if (IsDeathblood(bmp, Players.Single) == false)
        //    {
        //        // I think it's safe to say that if it isn't my main account, I don't care to check the rest
        //        // I could eventually make a messy sheet, in which case I'd send everything to that if this is my alt
        //        return;
        //    }
        //    var mods = CheckMods(bmp, Players.Single);
        //    var difficulty = CheckDifficulty(bmp, Players.Single);

        //    for (int i = 0; i < mods.Count; i++)
        //    {
        //        if (mods.ElementAt(i) == "Shin-uchi")
        //        {
        //            // I don't want to save any shin-uchi scores
        //            return;
        //        }
        //    }
        //    if (difficulty == Difficulty.Easy || difficulty == Difficulty.Normal)
        //    {
        //        // I don't care about easy or normal for my sheet
        //        // I don't really care about hard either, but I have the sheet anyway, so might as well save it if I get it
        //        return;
        //    }

        //    string title = GetTitle(bmp);
        //    bool highScore = IsHighScore(bmp);
        //    int score = GetScore(bmp, Players.Single);
        //    int goods = GetGoods(bmp, Players.Single);
        //    int oks = GetOKs(bmp, Players.Single);
        //    int bads = GetBads(bmp, Players.Single);
        //    int combo = GetCombo(bmp, Players.Single);
        //    int drumroll = GetDrumroll(bmp, Players.Single);

        //    Console.WriteLine(title);
        //    Console.WriteLine(highScore);
        //    Console.WriteLine(score);
        //    Console.WriteLine(goods);
        //    Console.WriteLine(oks);
        //    Console.WriteLine(bads);
        //    Console.WriteLine(combo);
        //    Console.WriteLine(drumroll);

        //    int[] info = new int[6]
        //    {
        //        score, goods, oks, bads, combo, drumroll
        //    };
        //    if (score % 10 != 0)
        //    {
        //        return;
        //    }
        //    if (highScore == true)
        //    {
        //        //sheet.UpdateHighScore(title, info, difficulty);

        //        //DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores");
        //        //var result = dirInfo.GetFiles();
        //        // NOT USED, NOT TESTING
        //        //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + result.Length + ".png", ImageFormat.Png);
        //        //Console.WriteLine("Highscore Logged");
        //    }
        //}

        //private void GetRankedResults(Bitmap bmp, bool Testing)
        //{
        //    //Bitmap bmp = Program.Program.screen.CaptureApplication();
        //    bool account = IsDeathblood(bmp, Players.RankedTop);
        //    if (account == false)
        //    {
        //        // I don't care if it's ranked on my alt
        //        return;
        //    }

        //    string title = GetTitle(bmp);
        //    Difficulty difficulty = CheckDifficulty(bmp, Players.RankedTop);
        //    int topScore = GetScore(bmp, Players.RankedTop);
        //    int topGoods = GetGoods(bmp, Players.RankedTop);
        //    int topOks = GetOKs(bmp, Players.RankedTop);
        //    int topBads = GetBads(bmp, Players.RankedTop);
        //    int topCombo = GetCombo(bmp, Players.RankedTop);
        //    int topDrumroll = GetDrumroll(bmp, Players.RankedTop);
        //    bool winLoss = RankedWinLoss(bmp);

        //    Console.WriteLine(title);
        //    if (winLoss == true)
        //    {
        //        Console.WriteLine("Win");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Lose");
        //    }
        //    Console.WriteLine(topScore);
        //    Console.WriteLine(topGoods);
        //    Console.WriteLine(topOks);
        //    Console.WriteLine(topBads);
        //    Console.WriteLine(topCombo);
        //    Console.WriteLine(topDrumroll);
        //    Console.WriteLine("");

        //    int bottomScore = GetScore(bmp, Players.RankedBottom);
        //    int bottomGoods = GetGoods(bmp, Players.RankedBottom);
        //    int bottomOks = GetOKs(bmp, Players.RankedBottom);
        //    int bottomBads = GetBads(bmp, Players.RankedBottom);
        //    int bottomCombo = GetCombo(bmp, Players.RankedBottom);
        //    int bottomDrumroll = GetDrumroll(bmp, Players.RankedBottom);

        //    Console.WriteLine(bottomScore);
        //    Console.WriteLine(bottomGoods);
        //    Console.WriteLine(bottomOks);
        //    Console.WriteLine(bottomBads);
        //    Console.WriteLine(bottomCombo);
        //    Console.WriteLine(bottomDrumroll);
        //    Console.WriteLine("");

        //    int[] info = new int[12]
        //    {
        //        topScore, topGoods, topOks, topBads, topCombo, topDrumroll, bottomScore, bottomGoods, bottomOks, bottomBads, bottomCombo, bottomDrumroll
        //    };

        //    //sheet.AddRankedEntry(title, info, difficulty, winLoss);

        //    //DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\TaikoLogging");
        //    //var result = dirInfo.GetFiles();
        //    // NOT USED, NOT TESTING
        //    //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\TaikoLogging\" + result.Length + ".png", ImageFormat.Png);
        //    //Console.WriteLine("Ranked match logged\n");
        //}


        #region Data gathering
        //public string GetTitle(Bitmap bmp)
        //{
        //    // I have a sheet on my test taiko spreadsheet that shows how I got these values, although it's a bit of a mess
        //    // These are Relative...     Width         Height            X              Y
        //    float[] relativeValues = { 0.390625f, 0.04327666151f, 0.5590277778f, 0.05100463679f };

        //    var titleBmp = GetBitmapArea(bmp, GetWidth(bmp, relativeValues[0]), GetHeight(bmp, relativeValues[1]), GetWidth(bmp, relativeValues[2]), GetHeight(bmp, relativeValues[3]));
        //    //var titleBmp = GetBitmapArea(bmp, (int)Math.Round((bmp.Width * (0.390625))), (int)Math.Round((bmp.Height * (0.0510046367))), (int)Math.Round((bmp.Width * 0.5590277777)), (int)Math.Round((bmp.Height * 0.043276661514)));
        //    // TESTING
        //    //titleBmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\TitleTest.png");
        //    titleBmp = ScaleDown(titleBmp, 450, 28);
        //    // TESTING
        //    //titleBmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\ScaledTitle.png");

        //    //Bitmap titleBmp = new Bitmap(450, 28);
        //    //CopyRegionIntoImage(bmp, new Rectangle(644, 33, 450, 28), ref titleBmp, new Rectangle(0, 0, 450, 28));
        //    int pixelDifferences = -1;
        //    int smallestIndex = 0;
        //    for (int i = 0; i < titleBitmaps.Count; i++)
        //    {
        //        var tmpInt = CompareBitmaps(titleBmp, titleBitmaps[i]);
        //        if (tmpInt < pixelDifferences || pixelDifferences == -1)
        //        {
        //            pixelDifferences = tmpInt;
        //            // TESTING
        //            //titleBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + tmpInt.ToString() + ".png");
        //            smallestIndex = i;
        //        }
        //    }
        //    Console.WriteLine(titles[smallestIndex]);
        //    return titles[smallestIndex];
        //}

        //private void GetTitleBitmap(Bitmap bmp)
        //{
        //    var titleBmp = GetBitmapArea(bmp, GetWidth(bmp, 0.390625), GetHeight(bmp, 0.043276661), GetWidth(bmp, 0.55902777777777), GetHeight(bmp, 0.0510046367851));
        //    titleBmp = ScaleDown(titleBmp, 450, 28);
        //    //var titleBmp = GetBitmapArea(bmp, 450, 28, 644, 33);
        //    //Bitmap titleBmp = new Bitmap(450, 28);
        //    //CopyRegionIntoImage(bmp, new Rectangle(644, 33, 450, 28), ref titleBmp, new Rectangle(0, 0, 450, 28));
        //    // TESTING
        //    titleBmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\DLC Songs\SingleResults.Title" + j++.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
        //}

        public bool IsHighScore(Bitmap bmp)
        {
            var highScoreBmp = GetBitmapArea(bmp, GetWidth(bmp, 0.13454861111), GetHeight(bmp, 0.0247295208), GetWidth(bmp, 0.42447916666), GetHeight(bmp, 0.59969088098));
            highScoreBmp = ScaleDown(highScoreBmp, 155, 16);
            //Bitmap highScoreBmp = new Bitmap(644 - 489, 404 - 388);
            //CopyRegionIntoImage(bmp, new Rectangle(489, 388, 644 - 489, 404 - 388), ref highScoreBmp, new Rectangle(0, 0, 644 - 489, 404 - 388));

            return CompareBitmaps(highScoreBmp, highScoreBitmaps[0]) < CompareBitmaps(highScoreBmp, highScoreBitmaps[1]);
        }

        //public int GetScore(Bitmap bmp, Players players)
        //{
        //    double offset = 0;
        //    if (players == Players.RankedTop)
        //    {
        //        offset = -0.15610510;
        //    }
        //    else if (players == Players.RankedBottom)
        //    {
        //        offset = 0.241112828;
        //    }
        //    Bitmap[] scoreBitmaps = new Bitmap[7];

        //    int sizeX = GetWidth(bmp, 0.0225694444);
        //    int sizeY = GetHeight(bmp, 0.0556414219);
        //    int height = GetHeight(bmp, 0.53477588871 + offset);

        //    scoreBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.5503472222), height);
        //    scoreBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.5277777777), height);
        //    scoreBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.5052083333), height);
        //    scoreBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.4826388888), height);
        //    scoreBitmaps[4] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.4600694444), height);
        //    scoreBitmaps[5] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.4375), height);
        //    scoreBitmaps[6] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.4149305555), height);

        //    for (int i = 0; i < scoreBitmaps.Length; i++)
        //    {
        //        scoreBitmaps[i] = ScaleDown(scoreBitmaps[i], 26, 36);
        //        //scoreBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\scoreBitmaps." + i.ToString() + ".png");
        //    }

        //    //scoreBitmaps[0] = new Bitmap(660 - 634, 382 - 346);
        //    //CopyRegionIntoImage(bmp, new Rectangle(634, 346 + offset, 660 - 634, 382 - 346), ref scoreBitmaps[0], new Rectangle(0, 0, 660 - 634, 382 - 346));
        //    //scoreBitmaps[1] = new Bitmap(634 - 608, 382 - 346);
        //    //CopyRegionIntoImage(bmp, new Rectangle(608, 346 + offset, 634 - 608, 382 - 346), ref scoreBitmaps[1], new Rectangle(0, 0, 634 - 608, 382 - 346));
        //    //scoreBitmaps[2] = new Bitmap(608 - 582, 382 - 346);
        //    //CopyRegionIntoImage(bmp, new Rectangle(582, 346 + offset, 608 - 582, 382 - 346), ref scoreBitmaps[2], new Rectangle(0, 0, 608 - 582, 382 - 346));
        //    //scoreBitmaps[3] = new Bitmap(582 - 556, 382 - 346);
        //    //CopyRegionIntoImage(bmp, new Rectangle(556, 346 + offset, 582 - 556, 382 - 346), ref scoreBitmaps[3], new Rectangle(0, 0, 582 - 556, 382 - 346));
        //    //scoreBitmaps[4] = new Bitmap(556 - 530, 382 - 346);
        //    //CopyRegionIntoImage(bmp, new Rectangle(530, 346 + offset, 556 - 530, 382 - 346), ref scoreBitmaps[4], new Rectangle(0, 0, 556 - 530, 382 - 346));
        //    //scoreBitmaps[5] = new Bitmap(530 - 504, 382 - 346);
        //    //CopyRegionIntoImage(bmp, new Rectangle(504, 346 + offset, 530 - 504, 382 - 346), ref scoreBitmaps[5], new Rectangle(0, 0, 530 - 504, 382 - 346));
        //    //scoreBitmaps[6] = new Bitmap(504 - 478, 382 - 346);
        //    //CopyRegionIntoImage(bmp, new Rectangle(478, 346 + offset, 504 - 478, 382 - 346), ref scoreBitmaps[6], new Rectangle(0, 0, 504 - 478, 382 - 346));

        //    int score = 0;
        //    for (int i = 0; i < scoreBitmaps.Length; i++)
        //    {
        //        int pixelDifferences = -1;
        //        int smallestIndex = 0;
        //        for (int j = 0; j < bigNumberBitmaps.Count; j++)
        //        {
        //            var tmpInt = CompareBitmaps(scoreBitmaps[i], bigNumberBitmaps[j]);
        //            if (tmpInt < pixelDifferences || pixelDifferences == -1)
        //            {
        //                pixelDifferences = tmpInt;
        //                smallestIndex = j;
        //            }
        //        }
        //        if (bigNumbers[smallestIndex] == "null")
        //        {
        //            return score;
        //        }
        //        score += int.Parse(bigNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
        //    }
        //    return score;
        //}

        //public int GetGoods(Bitmap bmp, Players players)
        //{
        //    double offset = 0;
        //    if (players == Players.RankedTop)
        //    {
        //        offset = -0.156105100;
        //    }
        //    else if (players == Players.RankedBottom)
        //    {
        //        offset = 0.241112828;
        //    }
        //    Bitmap[] goodBitmaps = new Bitmap[4];

        //    int sizeX = GetWidth(bmp, 0.015625);
        //    int sizeY = GetHeight(bmp, 0.0417310664605);
        //    int height = GetHeight(bmp, 0.4868624420401 + offset);

        //    goodBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7300347222), height);
        //    goodBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7144097222), height);
        //    goodBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6987847222), height);
        //    goodBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6831597222), height);

        //    //goodBitmaps[0] = new Bitmap(859 - 841, 342 - 315);
        //    //CopyRegionIntoImage(bmp, new Rectangle(841, 315 + offset, 859 - 841, 342 - 315), ref goodBitmaps[0], new Rectangle(0, 0, 859 - 841, 342 - 315));
        //    //goodBitmaps[1] = new Bitmap(841 - 823, 342 - 315);
        //    //CopyRegionIntoImage(bmp, new Rectangle(823, 315 + offset, 841 - 823, 342 - 315), ref goodBitmaps[1], new Rectangle(0, 0, 841 - 823, 342 - 315));
        //    //goodBitmaps[2] = new Bitmap(823 - 805, 342 - 315);
        //    //CopyRegionIntoImage(bmp, new Rectangle(805, 315 + offset, 823 - 805, 342 - 315), ref goodBitmaps[2], new Rectangle(0, 0, 823 - 805, 342 - 315));
        //    //goodBitmaps[3] = new Bitmap(805 - 787, 342 - 315);
        //    //CopyRegionIntoImage(bmp, new Rectangle(787, 315 + offset, 805 - 787, 342 - 315), ref goodBitmaps[3], new Rectangle(0, 0, 805 - 787, 342 - 315));

        //    for (int i = 0; i < goodBitmaps.Length; i++)
        //    {
        //        goodBitmaps[i] = ScaleDown(goodBitmaps[i], 18, 27);
        //        //goodBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\goodBitmaps." + i.ToString() + ".png");
        //    }



        //    int goods = 0;
        //    for (int i = 0; i < goodBitmaps.Length; i++)
        //    {
        //        int pixelDifferences = -1;
        //        int smallestIndex = 0;
        //        for (int j = 0; j < smallNumberBitmaps.Count; j++)
        //        {
        //            var tmpInt = CompareBitmaps(goodBitmaps[i], smallNumberBitmaps[j]);
        //            if (tmpInt < pixelDifferences || pixelDifferences == -1)
        //            {
        //                pixelDifferences = tmpInt;
        //                smallestIndex = j;
        //            }
        //        }
        //        if (smallNumbers[smallestIndex] == "null")
        //        {
        //            return goods;
        //        }
        //        goods += int.Parse(smallNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
        //    }
        //    return goods;
        //}
        //public int GetOKs(Bitmap bmp, Players players)
        //{
        //    double offset = 0;
        //    if (players == Players.RankedTop)
        //    {
        //        offset = -0.156105100;
        //    }
        //    else if (players == Players.RankedBottom)
        //    {
        //        offset = 0.241112828;
        //    }
        //    Bitmap[] okBitmaps = new Bitmap[4];

        //    int sizeX = GetWidth(bmp, 0.015625);
        //    int sizeY = GetHeight(bmp, 0.0417310664);
        //    int height = GetHeight(bmp, 0.545595054 + offset);


        //    okBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7300347222), height);
        //    okBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7144097222), height);
        //    okBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6987847222), height);
        //    okBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6831597222), height);

        //    for (int i = 0; i < okBitmaps.Length; i++)
        //    {
        //        okBitmaps[i] = ScaleDown(okBitmaps[i], 18, 27);
        //        //okBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\okBitmaps." + i.ToString() + ".png");
        //    }

        //    //okBitmaps[0] = new Bitmap(859 - 841, 380 - 353);
        //    //CopyRegionIntoImage(bmp, new Rectangle(841, 353 + offset, 859 - 841, 380 - 353), ref okBitmaps[0], new Rectangle(0, 0, 859 - 841, 380 - 353));
        //    //okBitmaps[1] = new Bitmap(841 - 823, 380 - 353);
        //    //CopyRegionIntoImage(bmp, new Rectangle(823, 353 + offset, 841 - 823, 380 - 353), ref okBitmaps[1], new Rectangle(0, 0, 841 - 823, 380 - 353));
        //    //okBitmaps[2] = new Bitmap(823 - 805, 380 - 353);
        //    //CopyRegionIntoImage(bmp, new Rectangle(805, 353 + offset, 823 - 805, 380 - 353), ref okBitmaps[2], new Rectangle(0, 0, 823 - 805, 380 - 353));
        //    //okBitmaps[3] = new Bitmap(805 - 787, 380 - 353);
        //    //CopyRegionIntoImage(bmp, new Rectangle(787, 353 + offset, 805 - 787, 380 - 353), ref okBitmaps[3], new Rectangle(0, 0, 805 - 787, 380 - 353));

        //    int oks = 0;
        //    for (int i = 0; i < okBitmaps.Length; i++)
        //    {
        //        int pixelDifferences = -1;
        //        int smallestIndex = 0;
        //        for (int j = 0; j < smallNumberBitmaps.Count; j++)
        //        {
        //            var tmpInt = CompareBitmaps(okBitmaps[i], smallNumberBitmaps[j]);
        //            if (tmpInt < pixelDifferences || pixelDifferences == -1)
        //            {
        //                pixelDifferences = tmpInt;
        //                smallestIndex = j;
        //            }
        //        }
        //        if (smallNumbers[smallestIndex] == "null")
        //        {
        //            return oks;
        //        }
        //        oks += int.Parse(smallNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
        //    }
        //    return oks;
        //}
        //public int GetBads(Bitmap bmp, Players players)
        //{
        //    double offset = 0;
        //    if (players == Players.RankedTop)
        //    {
        //        offset = -0.156105100;
        //    }
        //    else if (players == Players.RankedBottom)
        //    {
        //        offset = 0.241112828;
        //    }
        //    Bitmap[] badBitmaps = new Bitmap[4];

        //    int sizeX = GetWidth(bmp, 0.015625);
        //    int sizeY = GetHeight(bmp, 0.04173106);
        //    int height = GetHeight(bmp, 0.602782071 + offset);

        //    badBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.730034722222), height);
        //    badBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.714409722222), height);
        //    badBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.698784722222), height);
        //    badBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.683159722222), height);

        //    for (int i = 0; i < badBitmaps.Length; i++)
        //    {
        //        badBitmaps[i] = ScaleDown(badBitmaps[i], 18, 27);
        //        //badBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\badBitmaps." + i.ToString() + ".png");
        //    }

        //    //badBitmaps[0] = new Bitmap(859 - 841, 417 - 390);
        //    //CopyRegionIntoImage(bmp, new Rectangle(841, 390 + offset, 859 - 841, 417 - 390), ref badBitmaps[0], new Rectangle(0, 0, 859 - 841, 417 - 390));
        //    //badBitmaps[1] = new Bitmap(841 - 823, 417 - 390);
        //    //CopyRegionIntoImage(bmp, new Rectangle(823, 390 + offset, 841 - 823, 417 - 390), ref badBitmaps[1], new Rectangle(0, 0, 841 - 823, 417 - 390));
        //    //badBitmaps[2] = new Bitmap(823 - 805, 417 - 390);
        //    //CopyRegionIntoImage(bmp, new Rectangle(805, 390 + offset, 823 - 805, 417 - 390), ref badBitmaps[2], new Rectangle(0, 0, 823 - 805, 417 - 390));
        //    //badBitmaps[3] = new Bitmap(805 - 787, 417 - 390);
        //    //CopyRegionIntoImage(bmp, new Rectangle(787, 390 + offset, 805 - 787, 417 - 390), ref badBitmaps[3], new Rectangle(0, 0, 805 - 787, 417 - 390));

        //    int bads = 0;
        //    for (int i = 0; i < badBitmaps.Length; i++)
        //    {
        //        int pixelDifferences = -1;
        //        int smallestIndex = 0;
        //        for (int j = 0; j < smallNumberBitmaps.Count; j++)
        //        {
        //            var tmpInt = CompareBitmaps(badBitmaps[i], smallNumberBitmaps[j]);
        //            if (tmpInt < pixelDifferences || pixelDifferences == -1)
        //            {
        //                pixelDifferences = tmpInt;
        //                smallestIndex = j;
        //            }
        //        }
        //        if (smallNumbers[smallestIndex] == "null")
        //        {
        //            return bads;
        //        }
        //        bads += int.Parse(smallNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
        //    }
        //    return bads;
        //}
        //public int GetCombo(Bitmap bmp, Players players)
        //{
        //    double offset = 0;
        //    if (players == Players.RankedTop)
        //    {
        //        offset = -0.156105100;
        //    }
        //    else if (players == Players.RankedBottom)
        //    {
        //        offset = 0.241112828;
        //    }
        //    Bitmap[] comboBitmaps = new Bitmap[4];

        //    int sizeX = GetWidth(bmp, 0.015625);
        //    int sizeY = GetHeight(bmp, 0.0417310664);
        //    int height = GetHeight(bmp, 0.4868624420 + offset);

        //    comboBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.9210069444), height);
        //    comboBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.9053819444), height);
        //    comboBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.8897569444), height);
        //    comboBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.8741319444), height);

        //    for (int i = 0; i < comboBitmaps.Length; i++)
        //    {
        //        comboBitmaps[i] = ScaleDown(comboBitmaps[i], 18, 27);
        //        //comboBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\comboBitmaps." + i.ToString() + ".png");
        //    }

        //    //comboBitmaps[0] = new Bitmap(1079 - 1061, 342 - 315);
        //    //CopyRegionIntoImage(bmp, new Rectangle(1061, 315 + offset, 1079 - 1061, 342 - 315), ref comboBitmaps[0], new Rectangle(0, 0, 1079 - 1061, 342 - 315));
        //    //comboBitmaps[1] = new Bitmap(1061 - 1043, 342 - 315);
        //    //CopyRegionIntoImage(bmp, new Rectangle(1043, 315 + offset, 1061 - 1043, 342 - 315), ref comboBitmaps[1], new Rectangle(0, 0, 1061 - 1043, 342 - 315));
        //    //comboBitmaps[2] = new Bitmap(1043 - 1025, 342 - 315);
        //    //CopyRegionIntoImage(bmp, new Rectangle(1025, 315 + offset, 1043 - 1025, 342 - 315), ref comboBitmaps[2], new Rectangle(0, 0, 1043 - 1025, 342 - 315));
        //    //comboBitmaps[3] = new Bitmap(1025 - 1007, 342 - 315);
        //    //CopyRegionIntoImage(bmp, new Rectangle(1007, 315 + offset, 1025 - 1007, 342 - 315), ref comboBitmaps[3], new Rectangle(0, 0, 1025 - 1007, 342 - 315));

        //    int combo = 0;
        //    for (int i = 0; i < comboBitmaps.Length; i++)
        //    {
        //        int pixelDifferences = -1;
        //        int smallestIndex = 0;
        //        for (int j = 0; j < smallNumberBitmaps.Count; j++)
        //        {
        //            var tmpInt = CompareBitmaps(comboBitmaps[i], smallNumberBitmaps[j]);
        //            if (tmpInt < pixelDifferences || pixelDifferences == -1)
        //            {
        //                pixelDifferences = tmpInt;
        //                smallestIndex = j;
        //            }
        //        }
        //        if (smallNumbers[smallestIndex] == "null")
        //        {
        //            return combo;
        //        }
        //        combo += int.Parse(smallNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
        //    }
        //    return combo;
        //}
        //public int GetDrumroll(Bitmap bmp, Players players)
        //{
        //    double offset = 0;
        //    if (players == Players.RankedTop)
        //    {
        //        offset = -0.156105100;
        //    }
        //    else if (players == Players.RankedBottom)
        //    {
        //        offset = 0.241112828;
        //    }
        //    Bitmap[] drumrollBitmaps = new Bitmap[4];

        //    int sizeX = GetWidth(bmp, 0.015625);
        //    int sizeY = GetHeight(bmp, 0.04173106646);
        //    int height = GetHeight(bmp, 0.545595054095 + offset);

        //    drumrollBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.92013888), height);
        //    drumrollBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.90451388), height);
        //    drumrollBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.88888888), height);
        //    drumrollBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.87326388), height);

        //    for (int i = 0; i < drumrollBitmaps.Length; i++)
        //    {
        //        drumrollBitmaps[i] = ScaleDown(drumrollBitmaps[i], 18, 27);
        //        //drumrollBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\drumrollBitmaps." + i.ToString() + ".png");
        //    }

        //    //drumrollBitmaps[0] = new Bitmap(1079 - 1061, 380 - 353);
        //    //CopyRegionIntoImage(bmp, new Rectangle(1060, 353 + offset, 1079 - 1061, 380 - 353), ref drumrollBitmaps[0], new Rectangle(0, 0, 1079 - 1061, 380 - 353));
        //    //drumrollBitmaps[1] = new Bitmap(1061 - 1043, 380 - 353);
        //    //CopyRegionIntoImage(bmp, new Rectangle(1042, 353 + offset, 1061 - 1043, 380 - 353), ref drumrollBitmaps[1], new Rectangle(0, 0, 1061 - 1043, 380 - 353));
        //    //drumrollBitmaps[2] = new Bitmap(1043 - 1025, 380 - 353);
        //    //CopyRegionIntoImage(bmp, new Rectangle(1024, 353 + offset, 1043 - 1025, 380 - 353), ref drumrollBitmaps[2], new Rectangle(0, 0, 1043 - 1025, 380 - 353));
        //    //drumrollBitmaps[3] = new Bitmap(1025 - 1007, 380 - 353);
        //    //CopyRegionIntoImage(bmp, new Rectangle(1006, 353 + offset, 1025 - 1007, 380 - 353), ref drumrollBitmaps[3], new Rectangle(0, 0, 1025 - 1007, 380 - 353));

        //    int drumroll = 0;
        //    for (int i = 0; i < drumrollBitmaps.Length; i++)
        //    {
        //        int pixelDifferences = -1;
        //        int smallestIndex = 0;
        //        for (int j = 0; j < smallNumberBitmaps.Count; j++)
        //        {
        //            var tmpInt = CompareBitmaps(drumrollBitmaps[i], smallNumberBitmaps[j]);
        //            if (tmpInt < pixelDifferences || pixelDifferences == -1)
        //            {
        //                pixelDifferences = tmpInt;
        //                smallestIndex = j;
        //            }
        //        }
        //        if (smallNumbers[smallestIndex] == "null")
        //        {
        //            return drumroll;
        //        }
        //        drumroll += int.Parse(smallNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
        //    }
        //    return drumroll;
        //}

        public Difficulty CheckDifficulty(Bitmap bmp, Players players)
        {
            double offset = 0;
            if (players == Players.RankedTop)
            {
                offset = -0.156105100;
            }
            else if (players == Players.RankedBottom)
            {
                offset = 0.241112828;
            }
            var difficultyCheckBmp = GetBitmapArea(bmp, GetWidth(bmp, 0.0095486111), GetHeight(bmp, 0.0108191653), GetWidth(bmp, 0.2725694444), GetHeight(bmp, 0.612055641421 + offset));
            difficultyCheckBmp = ScaleDown(difficultyCheckBmp, 11, 7);
            //Bitmap difficultyCheckBmp = new Bitmap(325-314, 403-396);
            //CopyRegionIntoImage(bmp, new Rectangle(314, 396 + offset, 325-314, 403-396), ref difficultyCheckBmp, new Rectangle(0, 0, 325-314, 403-396));
            int pixelDifferences = -1;
            int smallestIndex = 0;
            for (int i = 0; i < difficultyBitmaps.Count; i++)
            {
                var tmpInt = CompareBitmaps(difficultyCheckBmp, difficultyBitmaps[i]);
                if (tmpInt < pixelDifferences || pixelDifferences == -1)
                {
                    pixelDifferences = tmpInt;
                    smallestIndex = i;
                }
            }
            return (Difficulty)smallestIndex;

        }

        public List<string> CheckMods(Bitmap bmp, Players players)
        {
            double offset = 0;
            if (players == Players.RankedTop)
            {
                offset = -0.156105100;
            }
            else if (players == Players.RankedBottom)
            {
                offset = 0.241112828;
            }
            Bitmap[] checkModBitmaps = new Bitmap[7];

            int sizeX = GetWidth(bmp, 0.021701388);
            int sizeY = GetHeight(bmp, 0.037094281);
            int height = GetHeight(bmp, 0.6027820710 + offset);

            checkModBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.91840277), height);
            checkModBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.89756944), height);
            checkModBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.87586805), height);
            checkModBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.85416666), height);
            checkModBitmaps[4] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.83333333), height);
            checkModBitmaps[5] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.81163194), height);
            checkModBitmaps[6] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.79079861), height);

            for (int i = 0; i < checkModBitmaps.Length; i++)
            {
                // Sizes for scaling shouldn't change unless I change the base bitmaps
                checkModBitmaps[i] = ScaleDown(checkModBitmaps[i], 25, 24);
                //checkModBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\checkModBitmaps." + i.ToString() + ".png");
            }

            //checkModBitmaps[0] = new Bitmap(25, 414 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(1058, 390 + offset, 25, 414 - 390), ref checkModBitmaps[0], new Rectangle(0, 0, 25, 414 - 390));
            //checkModBitmaps[1] = new Bitmap(25, 414 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(1034, 390 + offset, 25, 414 - 390), ref checkModBitmaps[1], new Rectangle(0, 0, 25, 414 - 390));
            //checkModBitmaps[2] = new Bitmap(25, 414 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(1009, 390 + offset, 25, 414 - 390), ref checkModBitmaps[2], new Rectangle(0, 0, 25, 414 - 390));
            //checkModBitmaps[3] = new Bitmap(25, 414 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(984, 390 + offset, 25, 414 - 390), ref checkModBitmaps[3], new Rectangle(0, 0, 25, 414 - 390));
            //checkModBitmaps[4] = new Bitmap(25, 414 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(960, 390 + offset, 25, 414 - 390), ref checkModBitmaps[4], new Rectangle(0, 0, 25, 414 - 390));
            //checkModBitmaps[5] = new Bitmap(25, 414 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(935, 390 + offset, 25, 414 - 390), ref checkModBitmaps[5], new Rectangle(0, 0, 25, 414 - 390));
            //checkModBitmaps[6] = new Bitmap(25, 414 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(911, 390 + offset, 25, 414 - 390), ref checkModBitmaps[6], new Rectangle(0, 0, 25, 414 - 390));

            List<string> activeMods = new List<string>();
            for (int i = 0; i < checkModBitmaps.Length; i++)
            {
                int pixelDifferences = -1;
                int smallestIndex = 0;
                for (int j = 0; j < modBitmaps.Count; j++)
                {
                    var tmpInt = CompareBitmaps(checkModBitmaps[i], modBitmaps[j]);
                    if (tmpInt < pixelDifferences || pixelDifferences == -1)
                    {
                        pixelDifferences = tmpInt;
                        smallestIndex = j;
                    }
                }
                if (smallestIndex == 5)
                {
                    return activeMods;
                }
                activeMods.Add(mods[smallestIndex]);
            }
            return activeMods;
        }

        public bool RankedWinLoss(Bitmap bmp)
        {
            var winlossBmp = GetBitmapArea(bmp, GetWidth(bmp, 0.146701388), GetHeight(bmp, 0.0401854), GetWidth(bmp, 0.2465277), GetHeight(bmp, 0.26120556));
            winlossBmp = ScaleDown(winlossBmp, 169, 26);
            //Bitmap winlossBmp = new Bitmap(453 - 284, 195 - 169);
            //CopyRegionIntoImage(bmp, new Rectangle(284, 169, 453 - 284, 195 - 169), ref winlossBmp, new Rectangle(0, 0, 453 - 284, 195 - 169));
            return CompareBitmaps(winlossBmp, winLossBitmaps[1]) < CompareBitmaps(winlossBmp, winLossBitmaps[0]);
        }

        public bool IsDeathblood(Bitmap bmp, Players players)
        {
            double offset = 0;
            if (players == Players.RankedTop)
            {
                offset = -0.156105100;
            }
            else if (players == Players.RankedBottom)
            {
                offset = 0.241112828;
            }
            var accountBmp = GetBitmapArea(bmp, GetWidth(bmp, 0.08333333), GetHeight(bmp, 0.0231839), GetWidth(bmp, 0.10677083), GetHeight(bmp, 0.333848531 + offset));
            accountBmp = ScaleDown(accountBmp, 96, 15);
            return CompareBitmaps(accountBmp, accountBitmaps[0]) < CompareBitmaps(accountBmp, accountBitmaps[1]);
        }

        // OCR is a pain in the ass and I can't be bothered figuring out how to make it work
        // My main guess is that I'd have to train the data myself, but I don't even have enough data to train it
        //private string GetOpponentName(Bitmap bmp)
        //{
        //    Bitmap opponentNameBmp = new Bitmap(261 - 102, 385 - 372);
        //    CopyRegionIntoImage(bmp, new Rectangle(102, 372, 261 - 102, 385 - 372), ref opponentNameBmp, new Rectangle(0, 0, 261 - 102, 385 - 372));
        //    var Ocr = new TesseractEngine(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\tessdata", "eng");

        //    var result = Ocr.Process(new Bitmap(@"D:\My Stuff\My Programs\Taiko\Test Data 1\RankedResults_waifu2x_name.png"));
        //    Console.WriteLine(result.GetText());
        //    return "true";
        //}

        #endregion

        #region Refactored Data Gathering
        public string GetTitle(Bitmap bmp)
        {
            // bmp in this case would be the full game screen

            // First: Get a bitmap of just the title area that will be compared
            using (Bitmap titleBmp = GetTitleBitmap(bmp))
            {
                // Second: Compare that bitmap to the List of bitmaps to find the closest match
                int index = CompareBitmapToList(titleBmp, titleBitmaps);
                // Third: Return the string in the index of the closest match
                return titles[index];
            }
        }
        public Bitmap GetTitleBitmap(Bitmap bmp)
        {
            // I have a sheet on my test taiko spreadsheet that shows how I got these values, although it's a bit of a mess
            // These are Relative...     Width         Height            X              Y
            float[] relativeValues = { 0.390625f, 0.04327666151f, 0.5590277778f, 0.05100463679f };

            var titleBmp = GetBitmapArea(bmp, GetWidth(bmp, relativeValues[0]), GetHeight(bmp, relativeValues[1]), GetWidth(bmp, relativeValues[2]), GetHeight(bmp, relativeValues[3]));
            return ScaleDown(titleBmp, 450, 28);
        }

        public int GetScore(Bitmap bmp, Players player)
        {
            List<Bitmap> scoreBitmaps = GetScoreBitmaps(bmp, player);

            int score = 0;
            for (int i = 0; i < 7; i++)
            {
                int index = CompareBitmapToList(scoreBitmaps[i], bigNumberBitmaps);
                if (bigNumbers[index] == "null")
                {
                    return score;
                }
                score += int.Parse(bigNumbers[index]) * ((int)Math.Pow(10, i));
            }
            return score;
        }
        public List<Bitmap> GetScoreBitmaps(Bitmap bmp, Players player)
        {
            double offset = 0;
            if (player == Players.RankedTop)
            {
                offset = -0.15610510;
            }
            else if (player == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            int width = GetWidth(bmp, 0.02256944444);
            int height = GetHeight(bmp, 0.05564142195);
            int y = GetHeight(bmp, 0.5347758887 + offset);

            float[] relativeX = { 0.5503472222f, 0.5286458333f, 0.5052083333f, 0.4826388889f, 0.4600694444f, 0.4375f, 0.4149305556f };

            List<Bitmap> scoreBitmaps = new List<Bitmap>();
            for (int i = 0; i < 7; i++)
            {
                scoreBitmaps.Add(ScaleDown(GetBitmapArea(bmp, width, height, GetWidth(bmp, relativeX[i]), y), 26, 36));
            }
            return scoreBitmaps;
        }
        public int GetGoods(Bitmap bmp, Players player)
        {
            return GetNumbers(bmp, player, GetGoodsBitmaps);
        }
        public int GetOKs(Bitmap bmp, Players player)
        {
            return GetNumbers(bmp, player, GetOKsBitmaps);
        }
        public int GetBads(Bitmap bmp, Players player)
        {
            return GetNumbers(bmp, player, GetBadsBitmaps);
        }
        public int GetCombo(Bitmap bmp, Players player)
        {
            return GetNumbers(bmp, player, GetComboBitmaps);
        }
        public int GetDrumroll(Bitmap bmp, Players player)
        {
            return GetNumbers(bmp, player, GetDrumrollBitmaps);
        }
        public delegate List<Bitmap> GetNumberBitmaps(Bitmap bmp, Players player);
        public int GetNumbers(Bitmap bmp, Players player, GetNumberBitmaps function)
        {
            List<Bitmap> bitmaps = function(bmp, player);

            int number = 0;
            for (int i = 0; i < bitmaps.Count; i++)
            {
                int index = CompareBitmapToList(bitmaps[i], smallNumberBitmaps);
                if (smallNumbers[index] == "null")
                {
                    return number;
                }
                number += int.Parse(smallNumbers[index]) * ((int)Math.Pow(10, i));
            }
            return number;
        }
        public List<Bitmap> GetGoodsBitmaps(Bitmap bmp, Players player)
        {
            double offset = 0;
            if (player == Players.RankedTop)
            {
                offset = -0.15610510;
            }
            else if (player == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            int width = GetWidth(bmp, 0.015625);
            int height = GetHeight(bmp, 0.04173106646);
            int y = GetHeight(bmp, 0.486862442 + offset);

            float[] relativeX = { 0.7300347222f, 0.7144097222f, 0.6987847222f, 0.6831597222f };

            List<Bitmap> scoreBitmaps = new List<Bitmap>();
            for (int i = 0; i < 4; i++)
            {
                scoreBitmaps.Add(ScaleDown(GetBitmapArea(bmp, width, height, GetWidth(bmp, relativeX[i]), y), 18, 27));
            }
            return scoreBitmaps;
        }
        public List<Bitmap> GetOKsBitmaps(Bitmap bmp, Players player)
        {
            double offset = 0;
            if (player == Players.RankedTop)
            {
                offset = -0.15610510;
            }
            else if (player == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            int width = GetWidth(bmp, 0.015625);
            int height = GetHeight(bmp, 0.04173106646);
            int y = GetHeight(bmp, 0.5455950541 + offset);

            float[] relativeX = { 0.7300347222f, 0.7144097222f, 0.6987847222f, 0.6831597222f };

            List<Bitmap> scoreBitmaps = new List<Bitmap>();
            for (int i = 0; i < 4; i++)
            {
                scoreBitmaps.Add(ScaleDown(GetBitmapArea(bmp, width, height, GetWidth(bmp, relativeX[i]), y), 18, 27));
            }
            return scoreBitmaps;
        }
        public List<Bitmap> GetBadsBitmaps(Bitmap bmp, Players player)
        {
            double offset = 0;
            if (player == Players.RankedTop)
            {
                offset = -0.15610510;
            }
            else if (player == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            int width = GetWidth(bmp, 0.015625);
            int height = GetHeight(bmp, 0.04173106646);
            int y = GetHeight(bmp, 0.6027820711 + offset);

            float[] relativeX = { 0.7300347222f, 0.7144097222f, 0.6987847222f, 0.6831597222f };

            List<Bitmap> scoreBitmaps = new List<Bitmap>();
            for (int i = 0; i < 4; i++)
            {
                scoreBitmaps.Add(ScaleDown(GetBitmapArea(bmp, width, height, GetWidth(bmp, relativeX[i]), y), 18, 27));
            }
            return scoreBitmaps;
        }
        public List<Bitmap> GetComboBitmaps(Bitmap bmp, Players player)
        {
            double offset = 0;
            if (player == Players.RankedTop)
            {
                offset = -0.15610510;
            }
            else if (player == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            int width = GetWidth(bmp, 0.015625);
            int height = GetHeight(bmp, 0.04173106646);
            int y = GetHeight(bmp, 0.486862442 + offset);

            float[] relativeX = { 0.9210069444f, 0.9053819444f, 0.8897569444f, 0.8741319444f };

            List<Bitmap> scoreBitmaps = new List<Bitmap>();
            for (int i = 0; i < 4; i++)
            {
                scoreBitmaps.Add(ScaleDown(GetBitmapArea(bmp, width, height, GetWidth(bmp, relativeX[i]), y), 18, 27));
            }
            return scoreBitmaps;
        }
        public List<Bitmap> GetDrumrollBitmaps(Bitmap bmp, Players player)
        {
            double offset = 0;
            if (player == Players.RankedTop)
            {
                offset = -0.15610510;
            }
            else if (player == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            int width = GetWidth(bmp, 0.015625);
            int height = GetHeight(bmp, 0.04173106646);
            int y = GetHeight(bmp, 0.5455950541 + offset);

            float[] relativeX = { 0.9192708333f, 0.9036458333f, 0.8897569444f, 0.8741319444f };

            List<Bitmap> scoreBitmaps = new List<Bitmap>();
            for (int i = 0; i < 4; i++)
            {
                scoreBitmaps.Add(ScaleDown(GetBitmapArea(bmp, width, height, GetWidth(bmp, relativeX[i]), y), 18, 27));
            }
            return scoreBitmaps;
        }


        public int CompareBitmapToList(Bitmap bmp, List<Bitmap> bitmaps)
        {
            int pixelDifferences = -1;
            int smallestIndex = 0;

            for (int i = 0; i < bitmaps.Count; i++)
            {
                var tmpInt = CompareBitmaps(bmp, bitmaps[i]);

                if (tmpInt < pixelDifferences || pixelDifferences == -1)
                {
                    pixelDifferences = tmpInt;
                    smallestIndex = i;
                }
            }
            if (bitmaps == titleBitmaps && pixelDifferences >= 200000)
            {
                Program.rin.PrepareNewSong(bmp);
            }
            return smallestIndex;
        }

        #endregion

        public void NewSongAdded()
        {
            // Reinitialize the title bitmaps so it has the new song in them
            InitializeTitleBitmaps();

            // Go through the GetSingleResults() so it will put the score on the spreadsheet
            using (Bitmap resultsBmp = Program.screen.CaptureApplication())
            {
                currentState = CheckState(resultsBmp);
                if (currentState == State.SingleResults)
                {
                    GetSingleResults(false);
                }
                else if (currentState == State.SingleSessionResults)
                {
                    GetSingleResults(true);
                }
            }
        }

        // These were for getting the bitmaps, just here for future reference and in case they'd be needed in the future
        // Hey it's the future and they're needed
        // I'm using these to see what the failed tests are seeing, or at least it should work that way if it isn't broken which it is

        // I'm aiming to phase these out
        public void GetSmallDigits(Bitmap bmp, Players players, string folderLocation, string baseFileName)
        {
            //Bitmap bmp = Program.Program.screen.CaptureApplication();

            // Old folder location was D:\My Stuff\My Programs\Taiko\Image Data\Test Data\
            Bitmap[] goodBitmaps = new Bitmap[4];

            double offset = 0;
            if (players == Players.RankedTop)
            {
                offset = -0.156105100;
            }
            else if (players == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            int sizeX = GetWidth(bmp, 0.015625);
            int sizeY = GetHeight(bmp, 0.0417310664605);
            int height = GetHeight(bmp, 0.4868624420401 + offset);

            goodBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7300347222), height);
            goodBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7144097222), height);
            goodBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6987847222), height);
            goodBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6831597222), height);

            for (int i = 0; i < goodBitmaps.Length; i++)
            {
                goodBitmaps[i] = ScaleDown(goodBitmaps[i], 18, 27);
                goodBitmaps[i].Save(folderLocation + baseFileName + ".goods." + i.ToString() + ".png");
            }


            Bitmap[] okBitmaps = new Bitmap[4];

            offset = 0;
            if (players == Players.RankedTop)
            {
                offset = -0.156105100;
            }
            else if (players == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            sizeX = GetWidth(bmp, 0.015625);
            sizeY = GetHeight(bmp, 0.0417310664);
            height = GetHeight(bmp, 0.545595054 + offset);

            okBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7300347222), height);
            okBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7144097222), height);
            okBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6987847222), height);
            okBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6831597222), height);

            for (int i = 0; i < okBitmaps.Length; i++)
            {
                okBitmaps[i] = ScaleDown(okBitmaps[i], 18, 27);
                okBitmaps[i].Save(folderLocation + baseFileName + ".oks." + i.ToString() + ".png");
            }

            Bitmap[] badBitmaps = new Bitmap[4];

            offset = 0;
            if (players == Players.RankedTop)
            {
                offset = -0.156105100;
            }
            else if (players == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            sizeX = GetWidth(bmp, 0.015625);
            sizeY = GetHeight(bmp, 0.04173106);
            height = GetHeight(bmp, 0.602782071 + offset);

            badBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7300347222), height);
            badBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7144097222), height);
            badBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6987847222), height);
            badBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6831597222), height);

            for (int i = 0; i < badBitmaps.Length; i++)
            {
                badBitmaps[i] = ScaleDown(badBitmaps[i], 18, 27);
                badBitmaps[i].Save(folderLocation + baseFileName + ".bads." + i.ToString() + ".png");
            }

            Bitmap[] comboBitmaps = new Bitmap[4];

            offset = 0;
            if (players == Players.RankedTop)
            {
                offset = -0.156105100;
            }
            else if (players == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            sizeX = GetWidth(bmp, 0.015625);
            sizeY = GetHeight(bmp, 0.0417310664605);
            height = GetHeight(bmp, 0.4868624420401 + offset);

            comboBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.9210069444), height);
            comboBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.9053819444), height);
            comboBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.8897569444), height);
            comboBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.8741319444), height);

            for (int i = 0; i < comboBitmaps.Length; i++)
            {
                comboBitmaps[i] = ScaleDown(comboBitmaps[i], 18, 27);
                comboBitmaps[i].Save(folderLocation + baseFileName + ".combos." + i.ToString() + ".png");
            }

            Bitmap[] drumrollBitmaps = new Bitmap[4];

            offset = 0;
            if (players == Players.RankedTop)
            {
                offset = -0.156105100;
            }
            else if (players == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            sizeX = GetWidth(bmp, 0.015625);
            sizeY = GetHeight(bmp, 0.0417310664605);
            height = GetHeight(bmp, 0.545595054095 + offset);

            drumrollBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.92013888), height);
            drumrollBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.90451388), height);
            drumrollBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.88888888), height);
            drumrollBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.87326388), height);

            for (int i = 0; i < drumrollBitmaps.Length; i++)
            {
                drumrollBitmaps[i] = ScaleDown(drumrollBitmaps[i], 18, 27);
                drumrollBitmaps[i].Save(folderLocation + baseFileName + ".drumrolls." + i.ToString() + ".png");
            }
        }
        public void GetBigDigits(Bitmap bmp, Players players, string folderLocation, string baseFileName)
        {
            double offset = 0;
            if (players == Players.RankedTop)
            {
                offset = -0.15610510;
            }
            else if (players == Players.RankedBottom)
            {
                offset = 0.241112828;
            }

            Bitmap[] scoreBitmaps = new Bitmap[7];

            int sizeX = GetWidth(bmp, 0.0225694444);
            int sizeY = GetHeight(bmp, 0.0556414219);
            int height = GetHeight(bmp, 0.53477588871 + offset);

            scoreBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.5503472222), height);
            scoreBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.5277777777), height);
            scoreBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.5052083333), height);
            scoreBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.4826388888), height);
            scoreBitmaps[4] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.4600694444), height);
            scoreBitmaps[5] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.4375), height);
            scoreBitmaps[6] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.4149305555), height);

            for (int i = 0; i < scoreBitmaps.Length; i++)
            {
                scoreBitmaps[i] = ScaleDown(scoreBitmaps[i], 26, 36);
                scoreBitmaps[i].Save(folderLocation + baseFileName + ".scores." + i.ToString() + ".png");
            }

            // Old folder location was D:\My Stuff\My Programs\Taiko\Image Data\Test Data\



        }


        // These are general functions used for everything
        private int GetHeight(Bitmap bmp, double ratio)
        {
            return (int)Math.Round((float)(bmp.Height * ratio));
        }
        private int GetWidth(Bitmap bmp, double ratio)
        {
            return (int)Math.Round((float)(bmp.Width * ratio));
        }
        private Bitmap GetBitmapArea(Bitmap bmp, int width, int height, int x, int y)
        {
            Bitmap returnBmp = new Bitmap(width, height);
            CopyRegionIntoImage(bmp, new Rectangle(x, y, width, height), ref returnBmp, new Rectangle(0, 0, width, height));
            return returnBmp;
        }
        private void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
        {
            using (Graphics grD = Graphics.FromImage(destBitmap))
            {
                grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            }
        }
        private int CompareBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            // Make a difference image.
            int wid = bmp1.Width;
            int hgt = bmp1.Height;

            // Get the differences.
            int[,] diffs = new int[wid, hgt];
            int max_diff = 0;
            for (int x = 0; x < wid; x+=2)
            {
                for (int y = 0; y < hgt; y+=2)
                {
                    // Calculate the pixels' difference.
                    Color color1 = bmp1.GetPixel(x, y);
                    Color color2 = bmp2.GetPixel(x, y);
                    diffs[x, y] = (int)(
                        Math.Abs(color1.R - color2.R) +
                        Math.Abs(color1.G - color2.G) +
                        Math.Abs(color1.B - color2.B));

                        max_diff += diffs[x, y];
                }
            }

            return max_diff;
        }
        private Bitmap ScaleDown(Bitmap image, float width, float height)
        {
            var brush = new SolidBrush(Color.Black);

            //float scale = Math.Min(width / image.Width, height / image.Height);

            var bmp = new Bitmap((int)width, (int)height);


            //var scaleWidth = (int)Math.Round(image.Width * scale);
            //var scaleHeight = (int)Math.Round(image.Height * scale);

            //graph.DrawImage(image, ((int)Math.Round((width - scaleWidth) / 2)), ((int)Math.Round((height - scaleHeight) / 2)), scaleWidth, scaleHeight);
            var graph = Graphics.FromImage(bmp);
            graph.FillRectangle(brush, new RectangleF(0, 0, width, height));
            graph.DrawImage(image, (0), (0), width, height);

            return bmp;
        }
    }
}
