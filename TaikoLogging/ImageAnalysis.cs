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
        ScreenGrab screen;
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
        private bool obsOpen = false;
        public ImageAnalysis()
        {
            // expert level programming right here
            try
            {
                screen = new ScreenGrab();
                obsOpen = true;
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
            var bmp = screen.CaptureApplication();
            currentState = CheckState(bmp);
            if (previousState != currentState)
            {
                // names each file based on the number it was made in, then by the state it thought it was
                // It saves every time it changes the state
                if (currentState == State.SingleSongEnd)
                {
                    TestingScreenshot();
                    Thread.Sleep(3500);
                    currentState = CheckState(screen.CaptureApplication());
                    if (currentState == State.SingleResults)
                    {
                        TestingScreenshot();

                        GetSingleResults(false);
                    }
                    else if (currentState == State.SingleSessionResults)
                    {
                        TestingScreenshot();

                        GetSingleResults(true);
                    }
                }
                else if (currentState == State.RankedResults)
                {
                    TestingScreenshot();

                    Thread.Sleep(4000);
                    TestingScreenshot();

                    GetRankedResults();
                }
            }

            System.GC.Collect();
        }

        public void NotStandardLoop()
        {
            var bmp = screen.CaptureApplication();
            currentState = CheckState(bmp);
            GetSingleResults(false);

            if (previousState != currentState)
            {


            }

            System.GC.Collect();
        }

        private void TestingScreenshot()
        {
            screen.CaptureApplication().Save(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\" + numStatesSaved++.ToString() + "." + currentState.ToString() + ".png", ImageFormat.Png);
        }

        public void GetDLCSongs()
        {
            var bmp = screen.CaptureApplication();
            currentState = CheckState(bmp);
            if (previousState != currentState)
            {
                // names each file based on the number it was made in, then by the state it thought it was
                // It saves every time it changes the state
                if (currentState == State.SingleSongEnd)
                {
                    TestingScreenshot();
                    Thread.Sleep(3500);
                    currentState = CheckState(screen.CaptureApplication());
                    if (currentState == State.SingleResults)
                    {
                        GetTitleBitmap(true, screen.CaptureApplication());
                    }
                    else if (currentState == State.SingleSessionResults)
                    {
                        GetTitleBitmap(true, screen.CaptureApplication());
                    }
                }
            }
            System.GC.Collect();
        }


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
                bmp.Save(string.Format(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\State Bitmaps\" + result[i].Name), System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        private void InitializeTitleBitmaps()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Title Bitmaps");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                titleBitmaps.Add(bitmap);
                titles.Add(result[i].Name.Remove(result[i].Name.LastIndexOf('.')));
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
            Bitmap bmp = screen.CaptureApplication();

            if (IsDeathblood(bmp, Players.Single) == false)
            {
                // I think it's safe to say that if it isn't my main account, I don't care to check the rest
                // I could eventually make a messy sheet, in which case I'd send everything to that if this is my alt
                return;
            }
            var mods = CheckMods(bmp, Players.Single);
            var difficulty = CheckDifficulty(bmp, Players.Single);

            for (int i = 0; i < mods.Count; i++)
            {
                if (mods.ElementAt(i) == "Shin-uchi")
                {
                    // I don't want to save any shin-uchi scores
                    return;
                }
            }
            if (difficulty == Difficulty.Easy || difficulty == Difficulty.Normal)
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

            string title = GetTitle(bmp);
            bool highScore = IsHighScore(bmp);
            int score = GetScore(bmp, players);
            int goods = GetGoods(bmp, players);
            int oks = GetOKs(bmp, players);
            int bads = GetBads(bmp, players);
            int combo = GetCombo(bmp, players);
            int drumroll = GetDrumroll(bmp, players);

            int[] info = new int[6]
            {
                score, goods, oks, bads, combo, drumroll
            };
            if (score % 10 != 0)
            {
                return;
            }
            sheet.UpdatePS4BestGoods(title, goods, difficulty);
            if (highScore == true)
            {
                sheet.UpdatePS4HighScore(title, info, difficulty, bmp);

                //DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores");
                //var result = dirInfo.GetFiles();
                //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + result.Length + ".png", ImageFormat.Png);
                Console.WriteLine("Highscore Logged");
            }
        }
        private void GetSingleResults(Bitmap bmp, bool Testing)
        {
            //Bitmap bmp = screen.CaptureApplication();

            if (IsDeathblood(bmp, Players.Single) == false)
            {
                // I think it's safe to say that if it isn't my main account, I don't care to check the rest
                // I could eventually make a messy sheet, in which case I'd send everything to that if this is my alt
                return;
            }
            var mods = CheckMods(bmp, Players.Single);
            var difficulty = CheckDifficulty(bmp, Players.Single);

            for (int i = 0; i < mods.Count; i++)
            {
                if (mods.ElementAt(i) == "Shin-uchi")
                {
                    // I don't want to save any shin-uchi scores
                    return;
                }
            }
            if (difficulty == Difficulty.Easy || difficulty == Difficulty.Normal)
            {
                // I don't care about easy or normal for my sheet
                // I don't really care about hard either, but I have the sheet anyway, so might as well save it if I get it
                return;
            }

            string title = GetTitle(bmp);
            bool highScore = IsHighScore(bmp);
            int score = GetScore(bmp, Players.Single);
            int goods = GetGoods(bmp, Players.Single);
            int oks = GetOKs(bmp, Players.Single);
            int bads = GetBads(bmp, Players.Single);
            int combo = GetCombo(bmp, Players.Single);
            int drumroll = GetDrumroll(bmp, Players.Single);

            Console.WriteLine(title);
            Console.WriteLine(highScore);
            Console.WriteLine(score);
            Console.WriteLine(goods);
            Console.WriteLine(oks);
            Console.WriteLine(bads);
            Console.WriteLine(combo);
            Console.WriteLine(drumroll);

            int[] info = new int[6]
            {
                score, goods, oks, bads, combo, drumroll
            };
            if (score % 10 != 0)
            {
                return;
            }
            if (highScore == true)
            {
                //sheet.UpdateHighScore(title, info, difficulty);

                //DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores");
                //var result = dirInfo.GetFiles();
                //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + result.Length + ".png", ImageFormat.Png);
                //Console.WriteLine("Highscore Logged");
            }
        }

        public void GetRankedResults()
        {
            Bitmap bmp = screen.CaptureApplication();
            bool account = IsDeathblood(bmp, Players.RankedTop);
            if (account == false)
            {
                // I don't care if it's ranked on my alt
                return;
            }

            string title = GetTitle(bmp);
            Difficulty difficulty = CheckDifficulty(bmp, Players.RankedTop);
            int topScore = GetScore(bmp, Players.RankedTop);
            int topGoods = GetGoods(bmp, Players.RankedTop);
            int topOks = GetOKs(bmp, Players.RankedTop);
            int topBads = GetBads(bmp, Players.RankedTop);
            int topCombo = GetCombo(bmp, Players.RankedTop);
            int topDrumroll = GetDrumroll(bmp, Players.RankedTop);

            //Console.WriteLine(title);
            //Console.WriteLine(topScore);
            //Console.WriteLine(topGoods);
            //Console.WriteLine(topOks);
            //Console.WriteLine(topBads);
            //Console.WriteLine(topCombo);
            //Console.WriteLine(topDrumroll);
            //Console.WriteLine("");

            int bottomScore = GetScore(bmp, Players.RankedBottom);
            int bottomGoods = GetGoods(bmp, Players.RankedBottom);
            int bottomOks = GetOKs(bmp, Players.RankedBottom);
            int bottomBads = GetBads(bmp, Players.RankedBottom);
            int bottomCombo = GetCombo(bmp, Players.RankedBottom);
            int bottomDrumroll = GetDrumroll(bmp, Players.RankedBottom);

            //Console.WriteLine(bottomScore);
            //Console.WriteLine(bottomGoods);
            //Console.WriteLine(bottomOks);
            //Console.WriteLine(bottomBads);
            //Console.WriteLine(bottomCombo);
            //Console.WriteLine(bottomDrumroll);
            //Console.WriteLine("");

            bool winLoss = RankedWinLoss(bmp);
            int[] info = new int[12]
            {
                topScore, topGoods, topOks, topBads, topCombo, topDrumroll, bottomScore, bottomGoods, bottomOks, bottomBads, bottomCombo, bottomDrumroll
            };
            if (topScore % 10 != 0 || bottomScore % 10 != 0)
            {
                return;
            }
            sheet.AddRankedEntry(title, info, difficulty, winLoss, bmp);
            sheet.UpdatePS4BestGoods(title, topGoods, difficulty);


        }


        private void GetRankedResults(Bitmap bmp, bool Testing)
        {
            //Bitmap bmp = screen.CaptureApplication();
            bool account = IsDeathblood(bmp, Players.RankedTop);
            if (account == false)
            {
                // I don't care if it's ranked on my alt
                return;
            }

            string title = GetTitle(bmp);
            Difficulty difficulty = CheckDifficulty(bmp, Players.RankedTop);
            int topScore = GetScore(bmp, Players.RankedTop);
            int topGoods = GetGoods(bmp, Players.RankedTop);
            int topOks = GetOKs(bmp, Players.RankedTop);
            int topBads = GetBads(bmp, Players.RankedTop);
            int topCombo = GetCombo(bmp, Players.RankedTop);
            int topDrumroll = GetDrumroll(bmp, Players.RankedTop);
            bool winLoss = RankedWinLoss(bmp);

            Console.WriteLine(title);
            if (winLoss == true)
            {
                Console.WriteLine("Win");
            }
            else
            {
                Console.WriteLine("Lose");
            }
            Console.WriteLine(topScore);
            Console.WriteLine(topGoods);
            Console.WriteLine(topOks);
            Console.WriteLine(topBads);
            Console.WriteLine(topCombo);
            Console.WriteLine(topDrumroll);
            Console.WriteLine("");

            int bottomScore = GetScore(bmp, Players.RankedBottom);
            int bottomGoods = GetGoods(bmp, Players.RankedBottom);
            int bottomOks = GetOKs(bmp, Players.RankedBottom);
            int bottomBads = GetBads(bmp, Players.RankedBottom);
            int bottomCombo = GetCombo(bmp, Players.RankedBottom);
            int bottomDrumroll = GetDrumroll(bmp, Players.RankedBottom);

            Console.WriteLine(bottomScore);
            Console.WriteLine(bottomGoods);
            Console.WriteLine(bottomOks);
            Console.WriteLine(bottomBads);
            Console.WriteLine(bottomCombo);
            Console.WriteLine(bottomDrumroll);
            Console.WriteLine("");

            int[] info = new int[12]
            {
                topScore, topGoods, topOks, topBads, topCombo, topDrumroll, bottomScore, bottomGoods, bottomOks, bottomBads, bottomCombo, bottomDrumroll
            };

            //sheet.AddRankedEntry(title, info, difficulty, winLoss);

            //DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\TaikoLogging");
            //var result = dirInfo.GetFiles();
            //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\TaikoLogging\" + result.Length + ".png", ImageFormat.Png);
            //Console.WriteLine("Ranked match logged\n");
        }


        #region Data gathering
        public string GetTitle(Bitmap bmp)
        {
            var titleBmp = GetBitmapArea(bmp, GetWidth(bmp, 0.390625), GetHeight(bmp, 0.0510046367), GetWidth(bmp, 0.5590277777), GetHeight(bmp, 0.043276661514));
            //var titleBmp = GetBitmapArea(bmp, (int)Math.Round((bmp.Width * (0.390625))), (int)Math.Round((bmp.Height * (0.0510046367))), (int)Math.Round((bmp.Width * 0.5590277777)), (int)Math.Round((bmp.Height * 0.043276661514)));
            //titleBmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\TitleTest.png");
            titleBmp = ScaleDown(titleBmp, 450, 28);
            titleBmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\ScaledTitleTest.png");

            //Bitmap titleBmp = new Bitmap(450, 28);
            //CopyRegionIntoImage(bmp, new Rectangle(644, 33, 450, 28), ref titleBmp, new Rectangle(0, 0, 450, 28));
            int pixelDifferences = -1;
            int smallestIndex = 0;
            for (int i = 0; i < titleBitmaps.Count; i++)
            {
                var tmpInt = CompareBitmaps(titleBmp, titleBitmaps[i]);
                if (tmpInt < pixelDifferences || pixelDifferences == -1)
                {
                    pixelDifferences = tmpInt;
                    smallestIndex = i;
                }
            }
            return titles[smallestIndex];
        }

        private void GetTitleBitmap(bool Testing, Bitmap bmp)
        {
            var titleBmp = GetBitmapArea(bmp, GetWidth(bmp, 0.390625), GetHeight(bmp, 0.043276661), GetWidth(bmp, 0.55902777777777), GetHeight(bmp, 0.0510046367851));
            titleBmp = ScaleDown(titleBmp, 450, 28);
            //var titleBmp = GetBitmapArea(bmp, 450, 28, 644, 33);
            //Bitmap titleBmp = new Bitmap(450, 28);
            //CopyRegionIntoImage(bmp, new Rectangle(644, 33, 450, 28), ref titleBmp, new Rectangle(0, 0, 450, 28));
            titleBmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.Title" + j++.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }

        public bool IsHighScore(Bitmap bmp)
        {
            var highScoreBmp = GetBitmapArea(bmp, GetWidth(bmp, 0.13454861111), GetHeight(bmp, 0.0247295208), GetWidth(bmp, 0.42447916666), GetHeight(bmp, 0.59969088098));
            highScoreBmp = ScaleDown(highScoreBmp, 155, 16);
            //Bitmap highScoreBmp = new Bitmap(644 - 489, 404 - 388);
            //CopyRegionIntoImage(bmp, new Rectangle(489, 388, 644 - 489, 404 - 388), ref highScoreBmp, new Rectangle(0, 0, 644 - 489, 404 - 388));

            return CompareBitmaps(highScoreBmp, highScoreBitmaps[0]) < CompareBitmaps(highScoreBmp, highScoreBitmaps[1]);
        }

        public int GetScore(Bitmap bmp, Players players)
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
                scoreBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\scoreBitmaps." + i.ToString() + ".png");
            }

            //scoreBitmaps[0] = new Bitmap(660 - 634, 382 - 346);
            //CopyRegionIntoImage(bmp, new Rectangle(634, 346 + offset, 660 - 634, 382 - 346), ref scoreBitmaps[0], new Rectangle(0, 0, 660 - 634, 382 - 346));
            //scoreBitmaps[1] = new Bitmap(634 - 608, 382 - 346);
            //CopyRegionIntoImage(bmp, new Rectangle(608, 346 + offset, 634 - 608, 382 - 346), ref scoreBitmaps[1], new Rectangle(0, 0, 634 - 608, 382 - 346));
            //scoreBitmaps[2] = new Bitmap(608 - 582, 382 - 346);
            //CopyRegionIntoImage(bmp, new Rectangle(582, 346 + offset, 608 - 582, 382 - 346), ref scoreBitmaps[2], new Rectangle(0, 0, 608 - 582, 382 - 346));
            //scoreBitmaps[3] = new Bitmap(582 - 556, 382 - 346);
            //CopyRegionIntoImage(bmp, new Rectangle(556, 346 + offset, 582 - 556, 382 - 346), ref scoreBitmaps[3], new Rectangle(0, 0, 582 - 556, 382 - 346));
            //scoreBitmaps[4] = new Bitmap(556 - 530, 382 - 346);
            //CopyRegionIntoImage(bmp, new Rectangle(530, 346 + offset, 556 - 530, 382 - 346), ref scoreBitmaps[4], new Rectangle(0, 0, 556 - 530, 382 - 346));
            //scoreBitmaps[5] = new Bitmap(530 - 504, 382 - 346);
            //CopyRegionIntoImage(bmp, new Rectangle(504, 346 + offset, 530 - 504, 382 - 346), ref scoreBitmaps[5], new Rectangle(0, 0, 530 - 504, 382 - 346));
            //scoreBitmaps[6] = new Bitmap(504 - 478, 382 - 346);
            //CopyRegionIntoImage(bmp, new Rectangle(478, 346 + offset, 504 - 478, 382 - 346), ref scoreBitmaps[6], new Rectangle(0, 0, 504 - 478, 382 - 346));

            int score = 0;
            for (int i = 0; i < scoreBitmaps.Length; i++)
            {
                int pixelDifferences = -1;
                int smallestIndex = 0;
                for (int j = 0; j < bigNumberBitmaps.Count; j++)
                {
                    var tmpInt = CompareBitmaps(scoreBitmaps[i], bigNumberBitmaps[j]);
                    if (tmpInt < pixelDifferences || pixelDifferences == -1)
                    {
                        pixelDifferences = tmpInt;
                        smallestIndex = j;
                    }
                }
                if (bigNumbers[smallestIndex] == "null")
                {
                    return score;
                }
                score += int.Parse(bigNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
            }
            return score;
        }

        public int GetGoods(Bitmap bmp, Players players)
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
            Bitmap[] goodBitmaps = new Bitmap[4];

            int sizeX = GetWidth(bmp, 0.015625);
            int sizeY = GetHeight(bmp, 0.0417310664605);
            int height = GetHeight(bmp, 0.4868624420401 + offset);

            goodBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7300347222), height);
            goodBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7144097222), height);
            goodBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6987847222), height);
            goodBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6831597222), height);

            //goodBitmaps[0] = new Bitmap(859 - 841, 342 - 315);
            //CopyRegionIntoImage(bmp, new Rectangle(841, 315 + offset, 859 - 841, 342 - 315), ref goodBitmaps[0], new Rectangle(0, 0, 859 - 841, 342 - 315));
            //goodBitmaps[1] = new Bitmap(841 - 823, 342 - 315);
            //CopyRegionIntoImage(bmp, new Rectangle(823, 315 + offset, 841 - 823, 342 - 315), ref goodBitmaps[1], new Rectangle(0, 0, 841 - 823, 342 - 315));
            //goodBitmaps[2] = new Bitmap(823 - 805, 342 - 315);
            //CopyRegionIntoImage(bmp, new Rectangle(805, 315 + offset, 823 - 805, 342 - 315), ref goodBitmaps[2], new Rectangle(0, 0, 823 - 805, 342 - 315));
            //goodBitmaps[3] = new Bitmap(805 - 787, 342 - 315);
            //CopyRegionIntoImage(bmp, new Rectangle(787, 315 + offset, 805 - 787, 342 - 315), ref goodBitmaps[3], new Rectangle(0, 0, 805 - 787, 342 - 315));

            for (int i = 0; i < goodBitmaps.Length; i++)
            {
                goodBitmaps[i] = ScaleDown(goodBitmaps[i], 18, 27);
                goodBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\goodBitmaps." + i.ToString() + ".png");
            }



            int goods = 0;
            for (int i = 0; i < goodBitmaps.Length; i++)
            {
                int pixelDifferences = -1;
                int smallestIndex = 0;
                for (int j = 0; j < smallNumberBitmaps.Count; j++)
                {
                    var tmpInt = CompareBitmaps(goodBitmaps[i], smallNumberBitmaps[j]);
                    if (tmpInt < pixelDifferences || pixelDifferences == -1)
                    {
                        pixelDifferences = tmpInt;
                        smallestIndex = j;
                    }
                }
                if (smallNumbers[smallestIndex] == "null")
                {
                    return goods;
                }
                goods += int.Parse(smallNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
            }
            return goods;
        }
        public int GetOKs(Bitmap bmp, Players players)
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
            Bitmap[] okBitmaps = new Bitmap[4];

            int sizeX = GetWidth(bmp, 0.015625);
            int sizeY = GetHeight(bmp, 0.0417310664);
            int height = GetHeight(bmp, 0.545595054 + offset);


            okBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7300347222), height);
            okBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.7144097222), height);
            okBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6987847222), height);
            okBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.6831597222), height);

            for (int i = 0; i < okBitmaps.Length; i++)
            {
                okBitmaps[i] = ScaleDown(okBitmaps[i], 18, 27);
                okBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\okBitmaps." + i.ToString() + ".png");
            }

            //okBitmaps[0] = new Bitmap(859 - 841, 380 - 353);
            //CopyRegionIntoImage(bmp, new Rectangle(841, 353 + offset, 859 - 841, 380 - 353), ref okBitmaps[0], new Rectangle(0, 0, 859 - 841, 380 - 353));
            //okBitmaps[1] = new Bitmap(841 - 823, 380 - 353);
            //CopyRegionIntoImage(bmp, new Rectangle(823, 353 + offset, 841 - 823, 380 - 353), ref okBitmaps[1], new Rectangle(0, 0, 841 - 823, 380 - 353));
            //okBitmaps[2] = new Bitmap(823 - 805, 380 - 353);
            //CopyRegionIntoImage(bmp, new Rectangle(805, 353 + offset, 823 - 805, 380 - 353), ref okBitmaps[2], new Rectangle(0, 0, 823 - 805, 380 - 353));
            //okBitmaps[3] = new Bitmap(805 - 787, 380 - 353);
            //CopyRegionIntoImage(bmp, new Rectangle(787, 353 + offset, 805 - 787, 380 - 353), ref okBitmaps[3], new Rectangle(0, 0, 805 - 787, 380 - 353));

            int oks = 0;
            for (int i = 0; i < okBitmaps.Length; i++)
            {
                int pixelDifferences = -1;
                int smallestIndex = 0;
                for (int j = 0; j < smallNumberBitmaps.Count; j++)
                {
                    var tmpInt = CompareBitmaps(okBitmaps[i], smallNumberBitmaps[j]);
                    if (tmpInt < pixelDifferences || pixelDifferences == -1)
                    {
                        pixelDifferences = tmpInt;
                        smallestIndex = j;
                    }
                }
                if (smallNumbers[smallestIndex] == "null")
                {
                    return oks;
                }
                oks += int.Parse(smallNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
            }
            return oks;
        }
        public int GetBads(Bitmap bmp, Players players)
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
            Bitmap[] badBitmaps = new Bitmap[4];

            int sizeX = GetWidth(bmp, 0.015625);
            int sizeY = GetHeight(bmp, 0.04173106);
            int height = GetHeight(bmp, 0.602782071 + offset);

            badBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.730034722222), height);
            badBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.714409722222), height);
            badBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.698784722222), height);
            badBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.683159722222), height);

            for (int i = 0; i < badBitmaps.Length; i++)
            {
                badBitmaps[i] = ScaleDown(badBitmaps[i], 18, 27);
                badBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\badBitmaps." + i.ToString() + ".png");
            }

            //badBitmaps[0] = new Bitmap(859 - 841, 417 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(841, 390 + offset, 859 - 841, 417 - 390), ref badBitmaps[0], new Rectangle(0, 0, 859 - 841, 417 - 390));
            //badBitmaps[1] = new Bitmap(841 - 823, 417 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(823, 390 + offset, 841 - 823, 417 - 390), ref badBitmaps[1], new Rectangle(0, 0, 841 - 823, 417 - 390));
            //badBitmaps[2] = new Bitmap(823 - 805, 417 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(805, 390 + offset, 823 - 805, 417 - 390), ref badBitmaps[2], new Rectangle(0, 0, 823 - 805, 417 - 390));
            //badBitmaps[3] = new Bitmap(805 - 787, 417 - 390);
            //CopyRegionIntoImage(bmp, new Rectangle(787, 390 + offset, 805 - 787, 417 - 390), ref badBitmaps[3], new Rectangle(0, 0, 805 - 787, 417 - 390));

            int bads = 0;
            for (int i = 0; i < badBitmaps.Length; i++)
            {
                int pixelDifferences = -1;
                int smallestIndex = 0;
                for (int j = 0; j < smallNumberBitmaps.Count; j++)
                {
                    var tmpInt = CompareBitmaps(badBitmaps[i], smallNumberBitmaps[j]);
                    if (tmpInt < pixelDifferences || pixelDifferences == -1)
                    {
                        pixelDifferences = tmpInt;
                        smallestIndex = j;
                    }
                }
                if (smallNumbers[smallestIndex] == "null")
                {
                    return bads;
                }
                bads += int.Parse(smallNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
            }
            return bads;
        }
        public int GetCombo(Bitmap bmp, Players players)
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
            Bitmap[] comboBitmaps = new Bitmap[4];

            int sizeX = GetWidth(bmp, 0.015625);
            int sizeY = GetHeight(bmp, 0.0417310664);
            int height = GetHeight(bmp, 0.4868624420 + offset);

            comboBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.9210069444), height);
            comboBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.9053819444), height);
            comboBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.8897569444), height);
            comboBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.8741319444), height);

            for (int i = 0; i < comboBitmaps.Length; i++)
            {
                comboBitmaps[i] = ScaleDown(comboBitmaps[i], 18, 27);
                comboBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\comboBitmaps." + i.ToString() + ".png");
            }

            //comboBitmaps[0] = new Bitmap(1079 - 1061, 342 - 315);
            //CopyRegionIntoImage(bmp, new Rectangle(1061, 315 + offset, 1079 - 1061, 342 - 315), ref comboBitmaps[0], new Rectangle(0, 0, 1079 - 1061, 342 - 315));
            //comboBitmaps[1] = new Bitmap(1061 - 1043, 342 - 315);
            //CopyRegionIntoImage(bmp, new Rectangle(1043, 315 + offset, 1061 - 1043, 342 - 315), ref comboBitmaps[1], new Rectangle(0, 0, 1061 - 1043, 342 - 315));
            //comboBitmaps[2] = new Bitmap(1043 - 1025, 342 - 315);
            //CopyRegionIntoImage(bmp, new Rectangle(1025, 315 + offset, 1043 - 1025, 342 - 315), ref comboBitmaps[2], new Rectangle(0, 0, 1043 - 1025, 342 - 315));
            //comboBitmaps[3] = new Bitmap(1025 - 1007, 342 - 315);
            //CopyRegionIntoImage(bmp, new Rectangle(1007, 315 + offset, 1025 - 1007, 342 - 315), ref comboBitmaps[3], new Rectangle(0, 0, 1025 - 1007, 342 - 315));

            int combo = 0;
            for (int i = 0; i < comboBitmaps.Length; i++)
            {
                int pixelDifferences = -1;
                int smallestIndex = 0;
                for (int j = 0; j < smallNumberBitmaps.Count; j++)
                {
                    var tmpInt = CompareBitmaps(comboBitmaps[i], smallNumberBitmaps[j]);
                    if (tmpInt < pixelDifferences || pixelDifferences == -1)
                    {
                        pixelDifferences = tmpInt;
                        smallestIndex = j;
                    }
                }
                if (smallNumbers[smallestIndex] == "null")
                {
                    return combo;
                }
                combo += int.Parse(smallNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
            }
            return combo;
        }
        public int GetDrumroll(Bitmap bmp, Players players)
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
            Bitmap[] drumrollBitmaps = new Bitmap[4];

            int sizeX = GetWidth(bmp, 0.015625);
            int sizeY = GetHeight(bmp, 0.04173106646);
            int height = GetHeight(bmp, 0.545595054095 + offset);

            drumrollBitmaps[0] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.92013888), height);
            drumrollBitmaps[1] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.90451388), height);
            drumrollBitmaps[2] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.88888888), height);
            drumrollBitmaps[3] = GetBitmapArea(bmp, sizeX, sizeY, GetWidth(bmp, 0.87326388), height);

            for (int i = 0; i < drumrollBitmaps.Length; i++)
            {
                drumrollBitmaps[i] = ScaleDown(drumrollBitmaps[i], 18, 27);
                drumrollBitmaps[i].Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\drumrollBitmaps." + i.ToString() + ".png");
            }

            //drumrollBitmaps[0] = new Bitmap(1079 - 1061, 380 - 353);
            //CopyRegionIntoImage(bmp, new Rectangle(1060, 353 + offset, 1079 - 1061, 380 - 353), ref drumrollBitmaps[0], new Rectangle(0, 0, 1079 - 1061, 380 - 353));
            //drumrollBitmaps[1] = new Bitmap(1061 - 1043, 380 - 353);
            //CopyRegionIntoImage(bmp, new Rectangle(1042, 353 + offset, 1061 - 1043, 380 - 353), ref drumrollBitmaps[1], new Rectangle(0, 0, 1061 - 1043, 380 - 353));
            //drumrollBitmaps[2] = new Bitmap(1043 - 1025, 380 - 353);
            //CopyRegionIntoImage(bmp, new Rectangle(1024, 353 + offset, 1043 - 1025, 380 - 353), ref drumrollBitmaps[2], new Rectangle(0, 0, 1043 - 1025, 380 - 353));
            //drumrollBitmaps[3] = new Bitmap(1025 - 1007, 380 - 353);
            //CopyRegionIntoImage(bmp, new Rectangle(1006, 353 + offset, 1025 - 1007, 380 - 353), ref drumrollBitmaps[3], new Rectangle(0, 0, 1025 - 1007, 380 - 353));

            int drumroll = 0;
            for (int i = 0; i < drumrollBitmaps.Length; i++)
            {
                int pixelDifferences = -1;
                int smallestIndex = 0;
                for (int j = 0; j < smallNumberBitmaps.Count; j++)
                {
                    var tmpInt = CompareBitmaps(drumrollBitmaps[i], smallNumberBitmaps[j]);
                    if (tmpInt < pixelDifferences || pixelDifferences == -1)
                    {
                        pixelDifferences = tmpInt;
                        smallestIndex = j;
                    }
                }
                if (smallNumbers[smallestIndex] == "null")
                {
                    return drumroll;
                }
                drumroll += int.Parse(smallNumbers[smallestIndex]) * ((int)Math.Pow(10, i));
            }
            return drumroll;
        }

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




        // These were for getting the bitmaps, just here for future reference and in case they'd be needed in the future
        private void GetSmallDigits(Bitmap bmp)
        {
            //Bitmap bmp = screen.CaptureApplication();

            Bitmap good0Bmp = new Bitmap(859 - 841, 342 - 315);
            CopyRegionIntoImage(bmp, new Rectangle(841, 315, 859 - 841, 342 - 315), ref good0Bmp, new Rectangle(0, 0, 859 - 841, 342 - 315));
            Bitmap good1Bmp = new Bitmap(841 - 823, 342 - 315);
            CopyRegionIntoImage(bmp, new Rectangle(823, 315, 841 - 823, 342 - 315), ref good1Bmp, new Rectangle(0, 0, 841 - 823, 342 - 315));
            Bitmap good2Bmp = new Bitmap(823 - 805, 342 - 315);
            CopyRegionIntoImage(bmp, new Rectangle(805, 315, 823 - 805, 342 - 315), ref good2Bmp, new Rectangle(0, 0, 823 - 805, 342 - 315));
            Bitmap good3Bmp = new Bitmap(805 - 787, 342 - 315);
            CopyRegionIntoImage(bmp, new Rectangle(787, 315, 805 - 787, 342 - 315), ref good3Bmp, new Rectangle(0, 0, 805 - 787, 342 - 315));
            good0Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.good0Bmp.png");
            good1Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.good1Bmp.png");
            good2Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.good2Bmp.png");
            good3Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.good3Bmp.png");

            Bitmap ok0Bmp = new Bitmap(859 - 841, 380 - 353);
            CopyRegionIntoImage(bmp, new Rectangle(841, 353, 859 - 841, 380 - 353), ref ok0Bmp, new Rectangle(0, 0, 859 - 841, 380 - 353));
            Bitmap ok1Bmp = new Bitmap(841 - 823, 380 - 353);
            CopyRegionIntoImage(bmp, new Rectangle(823, 353, 841 - 823, 380 - 353), ref ok1Bmp, new Rectangle(0, 0, 841 - 823, 380 - 353));
            Bitmap ok2Bmp = new Bitmap(823 - 805, 380 - 353);
            CopyRegionIntoImage(bmp, new Rectangle(805, 353, 823 - 805, 380 - 353), ref ok2Bmp, new Rectangle(0, 0, 823 - 805, 380 - 353));
            Bitmap ok3Bmp = new Bitmap(805 - 787, 380 - 353);
            CopyRegionIntoImage(bmp, new Rectangle(787, 353, 805 - 787, 380 - 353), ref ok3Bmp, new Rectangle(0, 0, 805 - 787, 380 - 353));
            ok0Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.ok0Bmp.png");
            ok1Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.ok1Bmp.png");
            ok2Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.ok2Bmp.png");
            ok3Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.ok3Bmp.png");

            Bitmap bad0Bmp = new Bitmap(859 - 841, 417 - 390);
            CopyRegionIntoImage(bmp, new Rectangle(841, 390, 859 - 841, 417 - 390), ref bad0Bmp, new Rectangle(0, 0, 859 - 841, 417 - 390));
            Bitmap bad1Bmp = new Bitmap(841 - 823, 417 - 390);
            CopyRegionIntoImage(bmp, new Rectangle(823, 390, 841 - 823, 417 - 390), ref bad1Bmp, new Rectangle(0, 0, 841 - 823, 417 - 390));
            Bitmap bad2Bmp = new Bitmap(823 - 805, 417 - 390);
            CopyRegionIntoImage(bmp, new Rectangle(805, 390, 823 - 805, 417 - 390), ref bad2Bmp, new Rectangle(0, 0, 823 - 805, 417 - 390));
            Bitmap bad3Bmp = new Bitmap(805 - 787, 417 - 390);
            CopyRegionIntoImage(bmp, new Rectangle(787, 390, 805 - 787, 417 - 390), ref bad3Bmp, new Rectangle(0, 0, 805 - 787, 417 - 390));
            bad0Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.bad0Bmp.png");
            bad1Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.bad1Bmp.png");
            bad2Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.bad2Bmp.png");
            bad3Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.bad3Bmp.png");

            Bitmap combo0Bmp = new Bitmap(1079 - 1061, 342 - 315);
            CopyRegionIntoImage(bmp, new Rectangle(1061, 315, 1079 - 1061, 342 - 315), ref combo0Bmp, new Rectangle(0, 0, 1079 - 1061, 342 - 315));
            Bitmap combo1Bmp = new Bitmap(1061 - 1043, 342 - 315);
            CopyRegionIntoImage(bmp, new Rectangle(1043, 315, 1061 - 1043, 342 - 315), ref combo1Bmp, new Rectangle(0, 0, 1061 - 1043, 342 - 315));
            Bitmap combo2Bmp = new Bitmap(1043 - 1025, 342 - 315);
            CopyRegionIntoImage(bmp, new Rectangle(1025, 315, 1043 - 1025, 342 - 315), ref combo2Bmp, new Rectangle(0, 0, 1043 - 1025, 342 - 315));
            Bitmap combo3Bmp = new Bitmap(1025 - 1007, 342 - 315);
            CopyRegionIntoImage(bmp, new Rectangle(1007, 315, 1025 - 1007, 342 - 315), ref combo3Bmp, new Rectangle(0, 0, 1025 - 1007, 342 - 315));
            combo0Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.combo0Bmp.png");
            combo1Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.combo1Bmp.png");
            combo2Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.combo2Bmp.png");
            combo3Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.combo3Bmp.png");

            Bitmap drumroll0Bmp = new Bitmap(1079 - 1061, 378 - 351);
            CopyRegionIntoImage(bmp, new Rectangle(1061, 353, 1079 - 1061, 378 - 351), ref drumroll0Bmp, new Rectangle(0, 0, 1079 - 1061, 378 - 351));
            Bitmap drumroll1Bmp = new Bitmap(1061 - 1043, 378 - 351);
            CopyRegionIntoImage(bmp, new Rectangle(1043, 353, 1061 - 1043, 378 - 351), ref drumroll1Bmp, new Rectangle(0, 0, 1061 - 1043, 378 - 351));
            Bitmap drumroll2Bmp = new Bitmap(1043 - 1025, 378 - 351);
            CopyRegionIntoImage(bmp, new Rectangle(1025, 353, 1043 - 1025, 378 - 351), ref drumroll2Bmp, new Rectangle(0, 0, 1043 - 1025, 378 - 351));
            Bitmap drumroll3Bmp = new Bitmap(1025 - 1007, 378 - 351);
            CopyRegionIntoImage(bmp, new Rectangle(1007, 353, 1025 - 1007, 378 - 351), ref drumroll3Bmp, new Rectangle(0, 0, 1025 - 1007, 378 - 351));
            drumroll0Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.drumroll0Bmp.png");
            drumroll1Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.drumroll1Bmp.png");
            drumroll2Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.drumroll2Bmp.png");
            drumroll3Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.drumroll3Bmp.png");
        }
        private void GetBigDigits(Bitmap bmp)
        {
            //Bitmap bmp = screen.CaptureApplication();

            Bitmap score0Bmp = new Bitmap(660 - 634, 382 - 346);
            CopyRegionIntoImage(bmp, new Rectangle(634, 346, 660 - 634, 382 - 346), ref score0Bmp, new Rectangle(0, 0, 660 - 634, 382 - 346));
            Bitmap score1Bmp = new Bitmap(634 - 608, 382 - 346);
            CopyRegionIntoImage(bmp, new Rectangle(608, 346, 634 - 608, 382 - 346), ref score1Bmp, new Rectangle(0, 0, 634 - 608, 382 - 346));
            Bitmap score2Bmp = new Bitmap(608 - 582, 382 - 346);
            CopyRegionIntoImage(bmp, new Rectangle(582, 346, 608 - 582, 382 - 346), ref score2Bmp, new Rectangle(0, 0, 608 - 582, 382 - 346));
            Bitmap score3Bmp = new Bitmap(582 - 556, 382 - 346);
            CopyRegionIntoImage(bmp, new Rectangle(556, 346, 582 - 556, 382 - 346), ref score3Bmp, new Rectangle(0, 0, 582 - 556, 382 - 346));
            Bitmap score4Bmp = new Bitmap(556 - 530, 382 - 346);
            CopyRegionIntoImage(bmp, new Rectangle(530, 346, 556 - 530, 382 - 346), ref score4Bmp, new Rectangle(0, 0, 556 - 530, 382 - 346));
            Bitmap score5Bmp = new Bitmap(530 - 504, 382 - 346);
            CopyRegionIntoImage(bmp, new Rectangle(504, 346, 530 - 504, 382 - 346), ref score5Bmp, new Rectangle(0, 0, 530 - 504, 382 - 346));
            Bitmap score6Bmp = new Bitmap(504 - 478, 382 - 346);
            CopyRegionIntoImage(bmp, new Rectangle(478, 346, 504 - 478, 382 - 346), ref score6Bmp, new Rectangle(0, 0, 504 - 478, 382 - 346));

            score0Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.score0Bmp.png");
            score1Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.score1Bmp.png");
            score2Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.score2Bmp.png");
            score3Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.score3Bmp.png");
            score4Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.score4Bmp.png");
            score5Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.score5Bmp.png");
            score6Bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\SingleResults.score6Bmp.png");


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
