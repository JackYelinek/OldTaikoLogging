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
using System.Windows.Forms;
using IronOcr;
using System.Diagnostics;

namespace TaikoLogging
{
    class ImageAnalysis
    {
        public enum State
        {
            CustomizeRoom, DifficultySelect, EventPage, MainMenu, MainMenuSettings, MenuLoading, PracticePause, PracticeSelect, PracticeSong, PS4MainMenu, PS4CaptureGallery,
            RankedEndSong, RankedLeaderboards, RankedMidSong, RankedPause, RankedPointsGain, RankedResults, RankedSelect, RankedSongFound, RankedStats,
            SingleResults, SingleSong, SingleSessionResults, SingleSongPause, SongLoading, SongSelect, SongSelectSettings, SongSettings, TreasureBoxes, SongBingoCard
        };
        State previousState;
        State currentState;

        public enum Players { Single, RankedTop, RankedBottom };

        List<Bitmap> stateBitmaps = new List<Bitmap>();
        List<string> states = new List<string>();

        List<Bitmap> titleBitmaps = new List<Bitmap>();
        List<string> titles = new List<string>();

        List<Bitmap> rankedTitleBitmaps = new List<Bitmap>();
        List<string> rankedTitles = new List<string>();

        List<Bitmap> baseTitleBitmaps = new List<Bitmap>();
        List<string> baseTitles = new List<string>();

        List<Bitmap> titleBackgroundBitmaps = new List<Bitmap>();
        List<string> titleBackgrounds = new List<string>();

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
        List<string> winLoss = new List<string>();

        List<Bitmap> accountBitmaps = new List<Bitmap>();
        List<string> accounts = new List<string>();

        public bool newSongMode = false;


        private int j = 0;
        private int numStatesSaved = 0;
        public ImageAnalysis()
        {
            InitializeAll();
        }

        bool inCaptureGallery = false;
        public void StandardLoop()
        {
            using (Bitmap bmp = Program.screen.CaptureApplication())
            {
                CheckState(bmp);
                if (currentState == State.PS4CaptureGallery)
                {
                    inCaptureGallery = true;
                }
                if (currentState == State.PS4MainMenu)
                {
                    inCaptureGallery = false;
                }
                //if (previousState != currentState)
                //{
                //    //Console.WriteLine(currentState);
                //    bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\TestingStates\" + j++ + "." + currentState + ".png");
                //}
                if (previousState != currentState && inCaptureGallery == false)
                {
                    if (currentState == State.SingleResults || currentState == State.SingleSessionResults || currentState == State.RankedResults)
                    {
                        Thread.Sleep(3500);
                        AnalyzeResults();
                    }
                }
            }
            System.GC.Collect();
        }

        public void NotStandardLoop()
        {
            // For something that'd be looped, but isn't the real deal, just testing stuff usually

            // Time to start working in this function again
        }
        public void NotStandardNotLoop()
        {
            // For something that just has to be done once, usually testing stuff or adding files or something
            TestRecordingFunction();

        }

        DateTime prevTime = DateTime.Now;


        int recordingNumber = 0;
        List<Thread> threads = new List<Thread>();
        const int numThreads = 4;
        DateTime startTime;
        Stopwatch stopwatch = new Stopwatch();
        private void RecordingFunction()
        {
            startTime = DateTime.Now;
            stopwatch.Start();
            for (int i = 0; i < numThreads; i++)
            {
                Thread thread = new Thread(CaptureGood);
                thread.Start(i);
            }
        }

        private void TestRecordingFunction()
        {
            int i = 0;
            while (true)
            {
                //Thread.Sleep(Math.Max((int)(DateTime.Now - startTime - nextTime).TotalMilliseconds, 0));

                // x = 311
                // y = 214
                // width = 383 - 311 = 72
                // height = 248 - 214 = 34
                // 360 216
                var bmp = Program.screen.CapturePixelColor(360, 216);
                //var tmp = Program.screen.CaptureAreaQuickly(311, 212, 72, 15);
                //tmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\tmpRecording\" + i + ".png");
                //tmp = Program.screen.CaptureApplication();
                //tmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\tmpFull\" + i++ + ".png");

                //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\TestingRecording\" + i++ + ".png");

                // This is probably the best way to make a thread with multiple parameters
                Thread analysisThread = new Thread(
                    unused => AnalyzeGood(bmp, i)
                    );
                analysisThread.Start();

                //Console.WriteLine(DateTime.Now - prevTime);
                //prevTime = DateTime.Now;
            }
        }

        private void CaptureGood(object threadNumber)
        {
            int i = 0;
            long nextTime = (long)(((1.0f / 60.0f) * (int)threadNumber) + (((1.0f / 60.0f) * numThreads) * i))*1000;
            while (true)
            {
                //Thread.Sleep(Math.Max((int)(DateTime.Now - startTime - nextTime).TotalMilliseconds, 0));
                if (stopwatch.ElapsedMilliseconds <= nextTime)
                {
                    continue;
                }
                i = (int)((stopwatch.ElapsedMilliseconds - (long)((1.0f / 60.0f) * (int)threadNumber)) / (long)(((double)((1.0f / 60.0f) * numThreads)) * 1000));
                // x = 311
                // y = 214
                // width = 383 - 311 = 72
                // height = 248 - 214 = 34
                var bmp = Program.screen.CaptureAreaQuickly(311, 212, 72, 15);
                bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\TestingRecording\" + ((i++ * numThreads) + (int)threadNumber) + ".png");

                // This is probably the best way to make a thread with multiple parameters
                Thread analysisThread = new Thread(
                    unused => AnalyzeGood(bmp, ((i++ * numThreads) + (int)threadNumber))
                    );
                analysisThread.Start();

                //Console.WriteLine(DateTime.Now - prevTime);
                //prevTime = DateTime.Now;

                nextTime = (long)(((1.0f / 60.0f) * (int)threadNumber) + (((1.0f / 60.0f) * numThreads) * i)) * 1000;
            }
        }
        public enum Note { Good, OK, Bad };
        Dictionary<int, Note> NoteResults = new Dictionary<int, Note>();
        private void AnalyzeGood(Bitmap bmp, int i)
        {
            const int tolerance = 50;
            Color bmpColor = bmp.GetPixel(0, 0);
            //Color bmpColor = bmp.GetPixel(49, 4);
            Color goodColor = Color.FromArgb(138, 151, 25);
            Color okColor = Color.FromArgb(234,236,233);
            Color badColor = Color.FromArgb(128,97,228);

            if(CompareColors(bmpColor, goodColor) < tolerance)
            {
                NoteResults.Add(i, Note.Good);
                Console.WriteLine("Good");
            }
            if (CompareColors(bmpColor, okColor) < tolerance)
            {
                NoteResults.Add(i, Note.OK);
                Console.WriteLine("OK");
            }
            if (CompareColors(bmpColor, badColor) < tolerance)
            {
                NoteResults.Add(i, Note.Bad);
                Console.WriteLine("Bad");
            }
            bmp.Dispose();
        }

        public void AnalyzeResults()
        {
            CheckState(Program.screen.CaptureApplication());

            if (currentState == State.SingleResults)
            {
                GetSingleResults(false);
            }
            else if (currentState == State.SingleSessionResults)
            {
                GetSingleResults(true);
            }
            else if (currentState == State.RankedResults)
            {
                GetRankedResults();
            }
        }

        const int BitmapLeniency = 15;
        const int StrikesBeforeFailed = 500;

        


        private void TestingScreenshot()
        {
            Program.screen.CaptureApplication().Save(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\" + numStatesSaved++.ToString() + "." + currentState.ToString() + ".png", ImageFormat.Png);
        }

        #region Initialization
        private void InitializeAll()
        {
            InitializeStateBitmaps();
            InitializeTitleBitmaps();
            InitializeRankedTitleBitmaps();
            //InitializeBaseTitleBitmaps();
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
                Bitmap tmp = new Bitmap(result[i].FullName);
                Bitmap bitmap = new Bitmap(tmp);
                tmp.Dispose();
                stateBitmaps.Add(bitmap);
                states.Add(result[i].Name.Remove(result[i].Name.IndexOf('.')));
            }
        }
        public void CreateNewState(string state)
        {
            var tmp = Program.screen.CaptureApplication();

            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\State Bitmaps Original");
            var result = dirInfo.GetFiles();

            int numStates = 0;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].Name.Remove(result[i].Name.IndexOf('.')) == state)
                {
                    numStates++;
                }
            }

            tmp.Save(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\State Bitmaps Original\" + state + "." + numStates + ".png");
            for (int i = 0; i < stateBitmaps.Count; i++)
            {
                stateBitmaps[i].Dispose();
            }
            stateBitmaps.Clear();
            states.Clear();
            InitializeStateBitmaps();
            Console.WriteLine(state + " has been added as a state.");
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
                Bitmap tmp = new Bitmap(result[i].FullName);
                Bitmap bitmap = new Bitmap(tmp);
                tmp.Dispose();

                titleBitmaps.Add(bitmap);
                string songTitle = result[i].Name.Remove(result[i].Name.LastIndexOf('.'));
                titles.Add(songTitle);

            }
        }
        private void InitializeRankedTitleBitmaps()
        {
            rankedTitleBitmaps = new List<Bitmap>();
            rankedTitles = new List<string>();
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Ranked Title Bitmaps");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                Bitmap tmp = new Bitmap(result[i].FullName);
                Bitmap bitmap = new Bitmap(tmp);
                tmp.Dispose();

                rankedTitleBitmaps.Add(bitmap);
                string songTitle = result[i].Name.Remove(result[i].Name.LastIndexOf('.'));
                rankedTitles.Add(songTitle);
            }
        }
        private void ClearTitleBitmaps()
        {
            for (int i = 0; i < titleBitmaps.Count; i++)
            {
                titleBitmaps.Clear();
            }
            titles.Clear();
        }
        private void ClearRankedTitleBitmaps()
        {
            for (int i = 0; i < rankedTitleBitmaps.Count; i++)
            {
                rankedTitleBitmaps.Clear();
            }
            rankedTitles.Clear();
        }
        private void InitializeBaseTitleBitmaps()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Title Bitmaps\BaseTitles");
            var result = dirInfo.GetFiles();
            for (int i = 0; i < result.Length; i++)
            {
                var bitmap = new Bitmap(result[i].FullName);
                baseTitleBitmaps.Add(bitmap);
                baseTitles.Add(result[i].Name.Remove(result[i].Name.LastIndexOf('.')));
            }

            // More hardcodey than I'd like, but I'm lazy and it's just two, and mostly for testing right now
            Bitmap rankedBackground = new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Title Bitmaps\Background\Ranked.png");
            Bitmap singleBackground = new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Title Bitmaps\Background\Single.png");
            titleBackgroundBitmaps.Add(rankedBackground);
            titleBackgroundBitmaps.Add(singleBackground);
            titleBackgrounds.Add("Ranked");
            titleBackgrounds.Add("Single");
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
                winLoss.Add(result[i].Name.Remove(result[i].Name.LastIndexOf('.')));
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
                accounts.Add(result[i].Name.Remove(result[i].Name.LastIndexOf('.')));
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
            if (stateBitmaps.Count == 0 || states.Count == 0 || stateBitmaps.Count != states.Count)
            {
                return previousState;
            }
            previousState = currentState;
            Enum.TryParse(states[smallestIndex], out State state);
            currentState = state;
            return state;
        }

        public void GetSingleResults(bool isSession)
        {
            Bitmap bmp = Program.screen.CaptureApplication();
            Console.WriteLine("Screen Captured");

            Thread thread = new Thread(() => Clipboard.SetImage(bmp));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();

            Players players;

            if (isSession == true)
            {
                players = Players.RankedTop;
            }
            else
            {
                players = Players.Single;
            }

            List<object> info = new List<object>();
            List<string> headers = new List<string>();

            bool isShinUchi = false;

            string account = CheckAccount(bmp, players);

            var mods = CheckMods(bmp, players);
            info.Add(CheckDifficulty(bmp, players));
            headers.Add("Difficulty");
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i] == "Shin-uchi")
                {
                    // I don't want to save any shin-uchi scores (except BestGoods which I forgot to update this to take into account)
                    isShinUchi = true;
                }
            }

            if ((Difficulty)info[headers.IndexOf("Difficulty")] == Difficulty.Easy || (Difficulty)info[headers.IndexOf("Difficulty")] == Difficulty.Normal)
            {
                // I don't care about easy or normal for my sheet
                // I don't really care about hard either, but I have the sheet anyway, so might as well save it if I get it
                return;
            }


            if (newSongMode == true)
            {
                var titleBitmap = GetTitleBitmap(bmp);
                Console.WriteLine("Please input title, or 'n' if this isn't a new song:");
                var title = Console.ReadLine();
                if (title != "n")
                {
                    AddNewSongTitleBitmap(titleBitmap, title);
                }
            }


            info.Add(GetTitle(bmp));
            headers.Add("Title");
            if ((string)info[headers.IndexOf("Title")] == "")
            {
                return;
            }

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

            info.Add(account);
            headers.Add("Account");

            Program.sheet.UpdatePS4BestGoods(info, headers);
            if ( /*highScore == true && */ isShinUchi == false)
            {
                Program.sheet.UpdatePS4HighScore(info, headers, bmp);

                //DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores");
                //var result = dirInfo.GetFiles();
                // NOT USED, NOT TESTING
                //bmp.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\HighScores\" + result.Length + ".png", ImageFormat.Png);
            }
            Console.WriteLine(info[headers.IndexOf("Title")].ToString());
            Console.WriteLine("Analysis Complete\n");
            if (randomMode == true)
            {
                Program.sheet.GetRandomSong();
            }
        }


        public void GetRankedResults()
        {
            Bitmap bmp = Program.screen.CaptureApplication();
            Console.WriteLine("Screen Captured");

            Thread thread = new Thread(() => Clipboard.SetImage(bmp));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();

            List<object> info = new List<object>();
            List<string> headers = new List<string>();

            string account = CheckAccount(bmp, Players.RankedTop);
            if (account != "Deathblood")
            {
                // I don't care if it's ranked on my alt
                return;
            }

            // Song Data
            info.Add(GetTitle(bmp));
            headers.Add("Title");
            if (info[headers.IndexOf("Title")].ToString() == "")
            {
                return;
            }
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
            info.Add(GetRankedWinLoss(bmp));
            headers.Add("Win/Loss");

            info.Add(account);
            headers.Add("Account");

            // Check to see if the scores are possible, they must always end with a 0
            if ((int)info[headers.IndexOf("My Score")] % 10 != 0 || (int)info[headers.IndexOf("Opp Score")] % 10 != 0)
            {
                return;
            }
            Program.sheet.AddRankedEntry(info, headers, bmp);
            Program.sheet.UpdatePS4BestGoods(info, headers);
            Console.WriteLine(info[headers.IndexOf("Title")].ToString());
            Console.WriteLine("Analysis Complete\n");

        }

        public void FixRankedLogs()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"D:\My Stuff\My Programs\Taiko\Image Data\Ranked Logs");
            var results = dirInfo.GetFiles();

            for (int i = 0; i < results.Length; i++)
            {
                if (int.Parse(results[i].Name.Remove(results[i].Name.IndexOf('.'))) < 1640)
                {
                    continue;
                }
                using (Bitmap bmp = new Bitmap(results[i].FullName))
                {
                    int matchNumber = int.Parse(results[i].Name.Remove(results[i].Name.IndexOf('.')));

                    List<object> info = new List<object>();
                    List<string> headers = new List<string>();

                    info.Add(GetTitle(bmp));
                    headers.Add("Title");
                    if (info[headers.IndexOf("Title")].ToString() == "")
                    {
                        return;
                    }
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
                    info.Add(GetRankedWinLoss(bmp));
                    headers.Add("Win/Loss");

                    Program.sheet.FixRankedLogsData(info, headers, matchNumber);

                }
            }


        }



        #region Data gathering




        public bool IsHighScore(Bitmap bmp)
        {
            var highScoreBmp = GetBitmapArea(bmp, GetWidth(bmp, 0.13454861111), GetHeight(bmp, 0.0247295208), GetWidth(bmp, 0.42447916666), GetHeight(bmp, 0.59969088098));
            highScoreBmp = ScaleDown(highScoreBmp, 155, 16);
            //Bitmap highScoreBmp = new Bitmap(644 - 489, 404 - 388);
            //CopyRegionIntoImage(bmp, new Rectangle(489, 388, 644 - 489, 404 - 388), ref highScoreBmp, new Rectangle(0, 0, 644 - 489, 404 - 388));

            return CompareBitmaps(highScoreBmp, highScoreBitmaps[0]) < CompareBitmaps(highScoreBmp, highScoreBitmaps[1]);
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
        public string OldGetTitle(Bitmap bmp)
        {
            // bmp in this case would be the full game screen

            // First: Get a bitmap of just the title area that will be compared
            using (Bitmap titleBmp = GetTitleBitmap(bmp))
            {
                // Second: Compare that bitmap to the List of bitmaps to find the closest match
                int index = CompareBitmapToList(titleBmp, titleBitmaps);
                // Third: Return the string in the index of the closest match
                // If index == -1, then it couldn't find a song that was a close enough match
                if (index == -1)
                {
                    return "";
                }
                return titles[index];
            }
        }

        //private bool CompareTitleBitmaps(Bitmap checkingBitmap, Bitmap titleBitmap)
        //{
        //    // This function should only work for titles
        //    // I'm only focusing on it working for titles

        //    // checkingBitmap = the bitmap on screen being checked
        //    // titleBitmap = the initialized bitmaps used to see if that's the song
        //    // Ranked is so I know which background to compare to

        //    int wid = checkingBitmap.Width;
        //    int hgt = checkingBitmap.Height;

        //    Bitmap singleBackgroundBitmap = new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Title Bitmaps\Background\Single.png");
        //    Bitmap rankedBackgroundBitmap = new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Title Bitmaps\Background\Ranked.png");

        //    Bitmap failedPixels = new Bitmap(titleBitmap);
        //    Bitmap failedCheckPixels = new Bitmap(checkingBitmap);

        //    int strikes = StrikesBeforeFailed;
        //    // Get the differences.
        //    for (int y = hgt - 1; y >= 0; y--)
        //    {
        //        for (int x = wid - 1; x >= 0; x--)
        //        {
        //            // Calculate the pixels' difference.
        //            Color checkingColor = checkingBitmap.GetPixel(x, y);
        //            Color titleColor = titleBitmap.GetPixel(x, y);
        //            Color singleBackgroundColor = singleBackgroundBitmap.GetPixel(x, y);
        //            Color rankedBackgroundColor = rankedBackgroundBitmap.GetPixel(x, y);

        //            // First check to see if the Base bitmap expects it to be a background pixel
        //            // (If the titleColor is (0, 0, 0, 0), it's background

        //            // Then check to see if the Checking bitmap's pixel should be a background pixel
        //            // (If the checkingColor can get through the "Background check", then it should still be a candidate)

        //            // If base should be a background, but checking isn't, strike--
        //            // if titleColor isn't background (not (0, 0, 0, 0)), then check to see if the checking bitmap's pixel should be a background pixel

        //            bool isBackground = false;
        //            if (checkingColor.R - checkingColor.G > BitmapLeniency + 5 || checkingColor.G - checkingColor.B > BitmapLeniency + 5 || checkingColor.B - checkingColor.R > BitmapLeniency + 5 ||
        //                checkingColor == singleBackgroundColor || checkingColor == rankedBackgroundColor)
        //            {
        //                isBackground = true;
        //            }

        //            if (titleColor == Color.FromArgb(0, 0, 0, 0))
        //            {
        //                if (isBackground == false)
        //                {
        //                    failedPixels.SetPixel(x, y, Color.HotPink);
        //                    failedCheckPixels.SetPixel(x, y, Color.HotPink);
        //                    if (strikes-- <= 0)
        //                    {
        //                        checkingBitmap.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j + ".RealChecking.png");
        //                        titleBitmap.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j + ".BaseTitle.png");
        //                        failedPixels.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j + ".testing.png");
        //                        failedCheckPixels.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j++ + ".checking.png");
        //                        return false;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (isBackground == true)
        //                {
        //                    failedPixels.SetPixel(x, y, Color.HotPink);
        //                    failedCheckPixels.SetPixel(x, y, Color.HotPink);
        //                    if (strikes-- <= 0)
        //                    {
        //                        checkingBitmap.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j + ".RealChecking.png");
        //                        titleBitmap.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j + ".BaseTitle.png");
        //                        failedPixels.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j + ".testing.png");
        //                        failedCheckPixels.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j++ + ".checking.png");
        //                        return false;
        //                    }
        //                }
        //                else
        //                {
        //                    if (ComparePixels(checkingColor, titleColor) > 300)
        //                    {
        //                        failedPixels.SetPixel(x, y, Color.HotPink);
        //                        failedCheckPixels.SetPixel(x, y, Color.HotPink);
        //                        if (strikes-- <= 0)
        //                        {
        //                            checkingBitmap.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j + ".RealChecking.png");
        //                            titleBitmap.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j + ".BaseTitle.png");
        //                            failedPixels.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j + ".testing.png");
        //                            failedCheckPixels.Save(@"D:\My Stuff\My Programs\Taiko\Image Data\Test Data\" + j++ + ".checking.png");
        //                            return false;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return true;
        //}
        public string GetTitle(Bitmap bmp)
        {
            // bmp in this case would be the full game screen

            // First: Get a bitmap of just the title area that will be compared
            using (Bitmap titleBmp = GetTitleBitmap(bmp))
            {
                //int index = CompareBitmapToList(CreateTitleBitmap(titleBmp, titleBackgroundBitmaps[titleBackgrounds.IndexOf("Single")]), baseTitleBitmaps);
                string songTitle = string.Empty;
                if (currentState == State.RankedResults)
                {
                    songTitle = CompareRankedTitleBitmaps(titleBmp);
                }
                else
                {
                    songTitle = CompareSingleTitleBitmaps(titleBmp);
                }
                return songTitle;
            }
        }
        private string CompareSingleTitleBitmaps(Bitmap bmp)
        {
            int pixelDifferences = -1;
            int smallestIndex = 0;

            List<Bitmap> bitmaps = titleBitmaps;

            for (int i = 0; i < bitmaps.Count; i++)
            {
                var tmpInt = CompareBitmaps(bmp, bitmaps[i]);

                if ((tmpInt < pixelDifferences || pixelDifferences == -1))
                {
                    pixelDifferences = tmpInt;
                    smallestIndex = i;
                }
            }

            Program.logger.LogPixelDifference(titles[smallestIndex], pixelDifferences);
            Console.WriteLine("Title pixelDifference = " + pixelDifferences);

            if (currentState == State.RankedResults)
            {
                return titles[smallestIndex];
            }
            else if (pixelDifferences > 50000)
            {
                AddNewSongTitleBitmap(bmp, titles[smallestIndex]);
            }

            return titles[smallestIndex];
        }
        private string CompareRankedTitleBitmaps(Bitmap bmp)
        {
            int pixelDifferences = -1;
            int smallestIndex = 0;

            string songTitle = string.Empty;

            List<Bitmap> bitmaps = rankedTitleBitmaps;

            for (int i = 0; i < bitmaps.Count; i++)
            {
                var tmpInt = CompareBitmaps(bmp, bitmaps[i]);

                if ((tmpInt < pixelDifferences || pixelDifferences == -1))
                {
                    pixelDifferences = tmpInt;
                    smallestIndex = i;
                }
            }

            if (pixelDifferences >= 30000 || pixelDifferences  == -1)
            {
                songTitle = CompareSingleTitleBitmaps(bmp);
                AddNewSongTitleBitmap(bmp, songTitle);
            }
            else
            {
                songTitle = rankedTitles[smallestIndex];
                AddNewSongTitleBitmap(bmp, songTitle);
            }
            return songTitle;
        }

        public Bitmap GetTitleBitmap(Bitmap bmp)
        {
            // I have a sheet on my test taiko spreadsheet that shows how I got these values, although it's a bit of a mess
            // These are Relative...     Width         Height            X              Y
            float[] relativeValues = { 0.390625f, 0.04327666151f, 0.5590277778f, 0.05100463679f };

            var titleBmp = GetBitmapArea(bmp, GetWidth(bmp, relativeValues[0]), GetHeight(bmp, relativeValues[1]), GetWidth(bmp, relativeValues[2]), GetHeight(bmp, relativeValues[3]));
            return ScaleDown(titleBmp, 450, 28);
        }
        public string CheckAccount(Bitmap bmp, Players players)
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

            return accounts[CompareBitmapToList(accountBmp, accountBitmaps)];
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
        public string GetRankedWinLoss(Bitmap bmp)
        {
            using (Bitmap winLossBmp = GetWinLossBitmap(bmp))
            {
                // Second: Compare that bitmap to the List of bitmaps to find the closest match
                int index = CompareBitmapToList(winLossBmp, winLossBitmaps);
                // Third: Return the string in the index of the closest match
                // If index == -1, then it couldn't find a song that was a close enough match
                if (index == -1)
                {
                    return "";
                }
                return winLoss[index];
            }
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
        public string GetOpponentName(Bitmap bmp)
        {
            //AutoOcr Ocr = new AutoOcr() { ReadBarCodes = false };

            var Ocr = new AdvancedOcr()
            {
                InputImageType = AdvancedOcr.InputTypes.Snippet,
                ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                ReadBarCodes = false,
                Strategy = AdvancedOcr.OcrStrategy.Advanced,
            };


            var Results = Ocr.Read(GetOpponentNameBitmap(bmp));
            return Results.Text;
        }
        public Bitmap GetOpponentNameBitmap(Bitmap bmp)
        {
            int width = GetWidth(bmp, 0.13932399f);
            int height = GetHeight(bmp, 0.02052785f);
            
            int x = GetWidth(bmp, 0.08903544f);
            int y = GetHeight(bmp, 0.57478005f);

            Bitmap nameBitmap = GetBitmapArea(bmp, width, height, x, y);

            return nameBitmap;
        }
        public Bitmap GetWinLossBitmap(Bitmap bmp)
        {
            int width = GetWidth(bmp, 0.146701388f);
            int height = GetHeight(bmp, 0.0401854f);

            int x = GetWidth(bmp, 0.2465277f);
            int y = GetHeight(bmp, 0.26120556f);

            Bitmap winlossBmp = GetBitmapArea(bmp, width, height, x, y);

            winlossBmp = ScaleDown(winlossBmp, 169, 26);

            return winlossBmp;
        }

        public int CompareBitmapToList(Bitmap bmp, List<Bitmap> bitmaps)
        {
            int pixelDifferences = -1;
            int smallestIndex = 0;

            //List<int> testingDifferences = new List<int>();

            for (int i = 0; i < bitmaps.Count; i++)
            {
                var tmpInt = CompareBitmaps(bmp, bitmaps[i]);
                //testingDifferences.Add(tmpInt);

                if ((tmpInt < pixelDifferences || pixelDifferences == -1))
                {
                    pixelDifferences = tmpInt;
                    smallestIndex = i;
                }
            }

            return smallestIndex;
        }

        #endregion

        //public void NewSongAdded()
        //{
        //    // Go through the GetSingleResults() so it will put the score on the spreadsheet
        //    using (Bitmap resultsBmp = Program.screen.CaptureApplication())
        //    {
        //        currentState = CheckState(resultsBmp);
        //        if (currentState == State.SingleResults)
        //        {
        //            GetSingleResults(false);
        //        }
        //        else if (currentState == State.SingleSessionResults)
        //        {
        //            GetSingleResults(true);
        //        }
        //        else if (currentState == State.RankedResults)
        //        {
        //            GetRankedResults();
        //        }
        //    }
        //}
        public void AddNewSongTitleBitmap(Bitmap bmp, string songTitle)
        {
            ClearTitleBitmaps();
            ClearRankedTitleBitmaps();
            string folder = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\";
            if (currentState == State.RankedResults)
            {
                folder += @"Ranked Title Bitmaps\";
            }
            else
            {
                folder += @"Title Bitmaps\";
            }
            if (File.Exists(folder + songTitle + ".png") == true)
            {
                File.Delete(folder + songTitle + ".png");
            }
            bmp.Save(folder + songTitle + ".png");

            InitializeTitleBitmaps();
            InitializeRankedTitleBitmaps();
        }

        bool randomMode = false;
        public void RandomModeToggle()
        {
            randomMode = !randomMode;
            if (randomMode == true)
            {
                Program.rin.SendTwitchMessage("Random Mode on");
                Program.sheet.GetRandomSong();
            }
            else
            {
                Program.rin.SendTwitchMessage("Random Mode off");
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

            for (int x = 0; x < wid; x += 2)
            {
                for (int y = 0; y < hgt; y += 2)
                {
                    // Calculate the pixels' difference.
                    Color color1 = bmp1.GetPixel(x, y);
                    Color color2 = bmp2.GetPixel(x, y);

                    diffs[x, y] = CompareColors(color1, color2);
                    max_diff += diffs[x, y];
                }
            }
            return max_diff;
        }
        private int CompareColors(Color color1, Color color2)
        {
            return (int)(
                    Math.Abs(color1.R - color2.R) +
                    Math.Abs(color1.G - color2.G) +
                    Math.Abs(color1.B - color2.B));
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
        // This creates a title bitmap without the background
        private Bitmap CreateTitleBitmap(Bitmap bmp, Bitmap background)
        {
            // Get height and width to iterate through each pixel
            int wid = bmp.Width;
            int hgt = bmp.Height;

            Bitmap newBmp = new Bitmap(wid, hgt);

            // iterate through each pixel
            for (int x = 0; x < wid; x++)
            {
                for (int y = 0; y < hgt; y++)
                {
                    // get the colors
                    Color color1 = bmp.GetPixel(x, y);
                    Color backgroundColor = background.GetPixel(x, y);
                    // if color1 is the same as backgroundColor
                    // set pixel(x, y) fully transparent (0, 0, 0, 0)
                    // else, keep the pixel

                    // (226,245,246) is probably the closest I'm willing to get to a different color
                    if (color1.R - color1.G > BitmapLeniency || color1.G - color1.B > BitmapLeniency || color1.B - color1.R > BitmapLeniency || color1 == backgroundColor)
                    {
                        newBmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                    }
                    else
                    {
                        newBmp.SetPixel(x, y, color1);
                    }
                }
            }
            return newBmp;
            //bmp1.Save(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Title Bitmaps\BaseTitles\" + songTitle + ".png");
        }
    }
}
