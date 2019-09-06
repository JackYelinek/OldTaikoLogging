using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaikoLogging;

namespace TaikoLoggingTests
{
    [TestClass]
    public class TestImageAnalysis
    {
        string failedTestLocation = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\Failed Tests\";
        [TestMethod]
        public void TestFindGameWindow()
        {
            ScreenGrab screen = new ScreenGrab();


            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\FindGameWindow\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "初音ミクの消失‐劇場版‐.Ura.2.png")
            };


            for (int i = 0; i < bmps.Count; i++)
            {
                screen.FindGameWindow(bmps[i]);
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            // Top, Left, Bottom, Right
            List<List<int>> expectedResults = new List<List<int>>()
            {
                new List<int> {72, 20, 719, 1172 },
            };

            for (int i = 0; i < bmps.Count; i++)
            {
                if (screen.topOffset == expectedResults[i][0] && 
                    screen.leftOffset == expectedResults[i][1] && 
                    screen.bottomOffset == expectedResults[i][2] && 
                    screen.rightOffset == expectedResults[i][3])
                {
                    results[i] = true;
                }
                Assert.IsTrue(results[i]);//, "Expected " + expectedResults[i].ToString() + ", Result = '" + difficulties[i].ToString() + "'");
            }
            GC.Collect();
        }
        //[TestMethod]
        public void TestAnalyzeTwitch()
        {
            // I broke the test function by fixing the actual function, yay


            // I made this test because I thought the function was broken
            // Turns out I'm just a little stupid
            ScreenGrab screen = new ScreenGrab();


            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\FindGameWindow\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "NotStreaming.0.png"),
                new Bitmap(folderPath + "Streaming.0.png"),
            };

            List<bool> checkIsStreaming = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                checkIsStreaming.Add(true);
                //checkIsStreaming.Add(screen.AnalyzeTwitch(bmps[i]));
                bmps[i].Dispose();
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < checkIsStreaming.Count; i++)
            {
                results.Add(false);
            }

            // Top, Left, Bottom, Right
            List<bool> expectedResults = new List<bool>()
            {
                false,
                true,
            };

            for (int i = 0; i < expectedResults.Count; i++)
            {
                if (checkIsStreaming[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                //Assert.IsTrue(results[i]);//, "Expected " + expectedResults[i].ToString() + ", Result = '" + difficulties[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestCheckSingleDifficulty()
        {
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckDifficulty\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "Easy.png"),
                new Bitmap(folderPath + "Normal.png"),
                new Bitmap(folderPath + "Hard.png"),
                new Bitmap(folderPath + "Oni.png"),
                new Bitmap(folderPath + "Ura.png"),
                new Bitmap(folderPath + "ナイト・オブ・ナイツ.Oni.0.png")
            };

            List<ImageAnalysis.Difficulty> difficulties = new List<ImageAnalysis.Difficulty>();

            for (int i = 0; i < bmps.Count; i++)
            {
                difficulties.Add(imageAnalysis.CheckDifficulty(bmps[i], ImageAnalysis.Players.Single));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            List<ImageAnalysis.Difficulty> expectedResults = new List<ImageAnalysis.Difficulty>()
            {
                ImageAnalysis.Difficulty.Easy,
                ImageAnalysis.Difficulty.Normal,
                ImageAnalysis.Difficulty.Hard,
                ImageAnalysis.Difficulty.Oni,
                ImageAnalysis.Difficulty.Ura,
                ImageAnalysis.Difficulty.Oni,
            };

            for (int i = 0; i < results.Count; i++)
            {
                bmps[i].Dispose();
                if (difficulties[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + difficulties[i].ToString() + "'");
                bmps[i].Dispose();
            }
            GC.Collect();
        }

        //[TestMethod]
        public void TestCheckSingleMods()
        {
            //ImageAnalysis imageAnalysis = new ImageAnalysis();

            //List<Bitmap> bmps = new List<Bitmap>()
            //{
            //    new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckMods\!!!カオスタイム!!!.Oni.0.png")
            //};

            //List<List<string>> mods = new List<List<string>>()
            //{
                
            //};

            //for (int i = 0; i < bmps.Count; i++)
            //{
            //    mods[i] = imageAnalysis.CheckMods(bmps[i], ImageAnalysis.Players.Single);
            //}

            //List<bool> results = new List<bool>();
            //for (int i = 0; i < bmps.Count; i++)
            //{
            //    results.Add(false);
            //}

            //List<List<string>> expectedResults = new List<List<string>>()
            //{
            //    new List<string>()
            //    {
            //        null,
            //    }
            //};

            //for (int i = 0; i < bmps.Count; i++)
            //{
            //    if (mods[i] == expectedResults[i])
            //    {
            //        results[i] = true;
            //    }
            //    //Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + mods[i].ToString() + "'");
            //}
            Assert.IsTrue(true);
            GC.Collect();
        }

        //[TestMethod]
        public void TestCheckState()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();


            string songFolder = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckState\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                // I don't have any bitmaps to test with quite yet
                new Bitmap(songFolder + "!!!カオスタイム!!!.Oni.0.png")
            };

            List<ImageAnalysis.State> states = new List<ImageAnalysis.State>();

            for (int i = 0; i < bmps.Count; i++)
            {
                states.Add(imageAnalysis.CheckState(bmps[i]));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
                bmps[i].Dispose();
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetSingleBads()
        {
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "!!!カオスタイム!!!.Oni.0.png"),
                new Bitmap(folderPath + "HARDCOREノ心得.Oni.1.png"),
                new Bitmap(folderPath + "Infinite Rebellion.Oni.5.png"),
                new Bitmap(folderPath + "UNDEAD HEART(怒りのWarriors).Oni.3.png"),
                new Bitmap(folderPath + "Xa.Ura.4.png"),
                new Bitmap(folderPath + "初音ミクの消失‐劇場版‐.Ura.3.png"),
                new Bitmap(folderPath + "幽玄ノ乱.Oni.9.png"),
                new Bitmap(folderPath + "愛と浄罪の森.Oni.0.png"),
                new Bitmap(folderPath + "白鳥の湖.Ura.0.png"),
                new Bitmap(folderPath + "竜と黒炎の姫君.Ura.5.png"),
                new Bitmap(folderPath + "紫煌ノ乱.Oni.1.png"),
                new Bitmap(folderPath + "ナイト・オブ・ナイツ.Oni.0.png"),
            };

            List<int> bads = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                bads.Add(imageAnalysis.GetBads(bmps[i], ImageAnalysis.Players.Single));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                62,
                4,
                153,
                11,
                8,
                5,
                87,
                16,
                0,
                0,
                0,
                0
            };

            for (int i = 0; i < bmps.Count; i++)
            {
                if (bads[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetBadsBitmaps(bmps[i], ImageAnalysis.Players.Single);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetSingleBads[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < results.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + bads[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetSingleCombo()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();


            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "!!!カオスタイム!!!.Oni.0.png"),
                new Bitmap(folderPath + "HARDCOREノ心得.Oni.1.png"),
                new Bitmap(folderPath + "Infinite Rebellion.Oni.5.png"),
                new Bitmap(folderPath + "UNDEAD HEART(怒りのWarriors).Oni.3.png"),
                new Bitmap(folderPath + "Xa.Ura.4.png"),
                new Bitmap(folderPath + "初音ミクの消失‐劇場版‐.Ura.3.png"),
                new Bitmap(folderPath + "幽玄ノ乱.Oni.9.png"),
                new Bitmap(folderPath + "愛と浄罪の森.Oni.0.png"),
                new Bitmap(folderPath + "白鳥の湖.Ura.0.png"),
                new Bitmap(folderPath + "竜と黒炎の姫君.Ura.5.png"),
                new Bitmap(folderPath + "紫煌ノ乱.Oni.1.png"),
                new Bitmap(folderPath + "ナイト・オブ・ナイツ.Oni.0.png"),
            };

            List<int> combo = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                combo.Add(imageAnalysis.GetCombo(bmps[i], ImageAnalysis.Players.Single));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                167,
                476,
                184,
                751,
                275,
                777,
                164,
                391,
                861,
                932,
                823,
                651
            };
            for (int i = 0; i < bmps.Count; i++)
            {
                if (combo[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetComboBitmaps(bmps[i], ImageAnalysis.Players.Single);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetSingleCombo[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < bmps.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + combo[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetSingleDrumroll()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();


            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "!!!カオスタイム!!!.Oni.0.png"),
                new Bitmap(folderPath + "HARDCOREノ心得.Oni.1.png"),
                new Bitmap(folderPath + "Infinite Rebellion.Oni.5.png"),
                new Bitmap(folderPath + "UNDEAD HEART(怒りのWarriors).Oni.3.png"),
                new Bitmap(folderPath + "Xa.Ura.4.png"),
                new Bitmap(folderPath + "初音ミクの消失‐劇場版‐.Ura.3.png"),
                new Bitmap(folderPath + "幽玄ノ乱.Oni.9.png"),
                new Bitmap(folderPath + "愛と浄罪の森.Oni.0.png"),
                new Bitmap(folderPath + "白鳥の湖.Ura.0.png"),
                new Bitmap(folderPath + "竜と黒炎の姫君.Ura.5.png"),
                new Bitmap(folderPath + "紫煌ノ乱.Oni.1.png"),
                new Bitmap(folderPath + "ナイト・オブ・ナイツ.Oni.0.png"),
            };

            List<int> drumroll = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                drumroll.Add(imageAnalysis.GetDrumroll(bmps[i], ImageAnalysis.Players.Single));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                219,
                124,
                147,
                125,
                0,
                39,
                56,
                10,
                0,
                77,
                0,
                126
            };
            for (int i = 0; i < bmps.Count; i++)
            {
                if (drumroll[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetDrumrollBitmaps(bmps[i], ImageAnalysis.Players.Single);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetSingleDrumroll[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < bmps.Count; i++)
            {

                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + drumroll[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetSingleGoods()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "!!!カオスタイム!!!.Oni.0.png"),
                new Bitmap(folderPath + "HARDCOREノ心得.Oni.1.png"),
                new Bitmap(folderPath + "Infinite Rebellion.Oni.5.png"),
                new Bitmap(folderPath + "UNDEAD HEART(怒りのWarriors).Oni.3.png"),
                new Bitmap(folderPath + "Xa.Ura.4.png"),
                new Bitmap(folderPath + "初音ミクの消失‐劇場版‐.Ura.3.png"),
                new Bitmap(folderPath + "幽玄ノ乱.Oni.9.png"),
                new Bitmap(folderPath + "愛と浄罪の森.Oni.0.png"),
                new Bitmap(folderPath + "白鳥の湖.Ura.0.png"),
                new Bitmap(folderPath + "竜と黒炎の姫君.Ura.5.png"),
                new Bitmap(folderPath + "紫煌ノ乱.Oni.1.png"),
                new Bitmap(folderPath + "ナイト・オブ・ナイツ.Oni.0.png"),
            };

            List<int> goods = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                goods.Add(imageAnalysis.GetGoods(bmps[i], ImageAnalysis.Players.Single));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                650,
                763,
                663,
                1052,
                737,
                886,
                813,
                699,
                781,
                850,
                779,
                643
            };
            for (int i = 0; i < bmps.Count; i++)
            {
                if (goods[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetGoodsBitmaps(bmps[i], ImageAnalysis.Players.Single);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetSingleGoods[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < bmps.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + goods[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetSingleOKs()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();


            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "!!!カオスタイム!!!.Oni.0.png"),
                new Bitmap(folderPath + "HARDCOREノ心得.Oni.1.png"),
                new Bitmap(folderPath + "Infinite Rebellion.Oni.5.png"),
                new Bitmap(folderPath + "UNDEAD HEART(怒りのWarriors).Oni.3.png"),
                new Bitmap(folderPath + "Xa.Ura.4.png"),
                new Bitmap(folderPath + "初音ミクの消失‐劇場版‐.Ura.3.png"),
                new Bitmap(folderPath + "幽玄ノ乱.Oni.9.png"),
                new Bitmap(folderPath + "愛と浄罪の森.Oni.0.png"),
                new Bitmap(folderPath + "白鳥の湖.Ura.0.png"),
                new Bitmap(folderPath + "竜と黒炎の姫君.Ura.5.png"),
                new Bitmap(folderPath + "紫煌ノ乱.Oni.1.png"),
                new Bitmap(folderPath + "ナイト・オブ・ナイツ.Oni.0.png"),
            };

            List<int> oks = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                oks.Add(imageAnalysis.GetOKs(bmps[i], ImageAnalysis.Players.Single));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                275,
                118,
                488,
                71,
                159,
                108,
                362,
                117,
                80,
                82,
                44,
                8
            };

            for (int i = 0; i < bmps.Count; i++)
            {
                if (oks[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetOKsBitmaps(bmps[i], ImageAnalysis.Players.Single);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetSingleOKs[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < results.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + oks[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetSingleScore()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "!!!カオスタイム!!!.Oni.0.png"),
                new Bitmap(folderPath + "HARDCOREノ心得.Oni.1.png"),
                new Bitmap(folderPath + "Infinite Rebellion.Oni.5.png"),
                new Bitmap(folderPath + "UNDEAD HEART(怒りのWarriors).Oni.3.png"),
                new Bitmap(folderPath + "Xa.Ura.4.png"),
                new Bitmap(folderPath + "初音ミクの消失‐劇場版‐.Ura.3.png"),
                new Bitmap(folderPath + "幽玄ノ乱.Oni.9.png"),
                new Bitmap(folderPath + "愛と浄罪の森.Oni.0.png"),
                new Bitmap(folderPath + "白鳥の湖.Ura.0.png"),
                new Bitmap(folderPath + "竜と黒炎の姫君.Ura.5.png"),
                new Bitmap(folderPath + "紫煌ノ乱.Oni.1.png"),
                new Bitmap(folderPath + "ナイト・オブ・ナイツ.Oni.0.png"),
            };

            List<int> scores = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                scores.Add(imageAnalysis.GetScore(bmps[i], ImageAnalysis.Players.Single));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                565590,
                1077290,
                439780,
                997830,
                894010,
                1077470,
                548970,
                796520,
                1156460,
                1164020,
                1179040,
                1109030
            };
            for (int i = 0; i < bmps.Count; i++)
            {
                if (scores[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetScoreBitmaps(bmps[i], ImageAnalysis.Players.Single);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetSingleScores[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < results.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + scores[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestCheckRankedDifficulty()
        {
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\RankedTests\";

            // I really don't need to test this many for difficulty
            // I'm quite confident it works without fail, but I might as well test them I guess
            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "1441.png"),
                new Bitmap(folderPath + "1481.png"),
                new Bitmap(folderPath + "1548.png"),
                new Bitmap(folderPath + "1613.png"),
                new Bitmap(folderPath + "1670.png"),
                new Bitmap(folderPath + "1758.png"),
                new Bitmap(folderPath + "1768.png"),
                new Bitmap(folderPath + "1800.png"),
                new Bitmap(folderPath + "1805.png"),
                new Bitmap(folderPath + "1808.png")
            };

            List<ImageAnalysis.Difficulty> difficulties = new List<ImageAnalysis.Difficulty>();

            for (int i = 0; i < bmps.Count; i++)
            {
                difficulties.Add(imageAnalysis.CheckDifficulty(bmps[i], ImageAnalysis.Players.RankedTop));
                bmps[i].Dispose();
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }


            List<ImageAnalysis.Difficulty> expectedResults = new List<ImageAnalysis.Difficulty>()
            {
                ImageAnalysis.Difficulty.Oni,
                ImageAnalysis.Difficulty.Ura,
                ImageAnalysis.Difficulty.Oni,
                ImageAnalysis.Difficulty.Oni,
                ImageAnalysis.Difficulty.Oni,
                ImageAnalysis.Difficulty.Oni,
                ImageAnalysis.Difficulty.Oni,
                ImageAnalysis.Difficulty.Oni,
                ImageAnalysis.Difficulty.Oni,
                ImageAnalysis.Difficulty.Oni,
            };

            for (int i = 0; i < results.Count; i++)
            {
                if (difficulties[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + difficulties[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestCheckRankedMods()
        {
            //ImageAnalysis imageAnalysis = new ImageAnalysis();

            //List<Bitmap> bmps = new List<Bitmap>()
            //{
            //    new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckMods\!!!カオスタイム!!!.Oni.0.png")
            //};

            //List<List<string>> mods = new List<List<string>>()
            //{

            //};

            //for (int i = 0; i < bmps.Count; i++)
            //{
            //    mods[i] = imageAnalysis.CheckMods(bmps[i], ImageAnalysis.Players.Single);
            //}

            //List<bool> results = new List<bool>();
            //for (int i = 0; i < bmps.Count; i++)
            //{
            //    results.Add(false);
            //}

            //List<List<string>> expectedResults = new List<List<string>>()
            //{
            //    new List<string>()
            //    {
            //        null,
            //    }
            //};

            //for (int i = 0; i < bmps.Count; i++)
            //{
            //    if (mods[i] == expectedResults[i])
            //    {
            //        results[i] = true;
            //    }
            //    //Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + mods[i].ToString() + "'");
            //}
            GC.Collect();
        }

        [TestMethod]
        public void TestGetRankedBads()
        {
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\RankedTests\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "1441.png"),
                new Bitmap(folderPath + "1481.png"),
                new Bitmap(folderPath + "1548.png"),
                new Bitmap(folderPath + "1613.png"),
                new Bitmap(folderPath + "1670.png"),
                new Bitmap(folderPath + "1758.png"),
                new Bitmap(folderPath + "1768.png"),
                new Bitmap(folderPath + "1800.png"),
                new Bitmap(folderPath + "1805.png"),
                new Bitmap(folderPath + "1808.png")
            };

            List<int> bads = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                bads.Add(imageAnalysis.GetBads(bmps[i], ImageAnalysis.Players.RankedTop));
                bads.Add(imageAnalysis.GetBads(bmps[i], ImageAnalysis.Players.RankedBottom));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bads.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                13,13,
                0,0, // This one might break, I might have to get rid of this test
                2,19,
                0, 0,
                4, 6, 
                2,11,
                2,18, 
                13,8,
                6,10,
                75,92,
            };
            for (int i = 0; i < bmps.Count; i++)
            {
                if (bads[i * 2] == expectedResults[i * 2])
                {
                    results[i * 2] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetBadsBitmaps(bmps[i], ImageAnalysis.Players.RankedTop);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedBadsTop[" + i + "][" + j + "].png");
                    }
                }
                if (bads[(i * 2) + 1] == expectedResults[(i * 2) + 1])
                {
                    results[(i * 2) + 1] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetBadsBitmaps(bmps[i], ImageAnalysis.Players.RankedBottom);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedBadsBottom[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < results.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + bads[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetRankedCombo()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();


            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\RankedTests\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "1441.png"),
                new Bitmap(folderPath + "1481.png"),
                new Bitmap(folderPath + "1548.png"),
                new Bitmap(folderPath + "1613.png"),
                new Bitmap(folderPath + "1670.png"),
                new Bitmap(folderPath + "1758.png"),
                new Bitmap(folderPath + "1768.png"),
                new Bitmap(folderPath + "1800.png"),
                new Bitmap(folderPath + "1805.png"),
                new Bitmap(folderPath + "1808.png")
            };

            List<int> combo = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                combo.Add(imageAnalysis.GetCombo(bmps[i], ImageAnalysis.Players.RankedTop));
                combo.Add(imageAnalysis.GetCombo(bmps[i], ImageAnalysis.Players.RankedBottom));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < combo.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                461,278,
                523,523,
                605,337,
                303,303,
                571,406,
                572,250,
                459,322,
                340,378,
                455,263,
                278,123,
            };
            for (int i = 0; i < bmps.Count; i++)
            {
                if (combo[i * 2] == expectedResults[i * 2])
                {
                    results[i * 2] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetComboBitmaps(bmps[i], ImageAnalysis.Players.RankedTop);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedComboTop[" + i + "][" + j + "].png");
                    }
                }
                if (combo[(i * 2) + 1] == expectedResults[(i * 2) + 1])
                {
                    results[(i * 2) + 1] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetComboBitmaps(bmps[i], ImageAnalysis.Players.RankedBottom);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedComboBottom[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < results.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + combo[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetRankedDrumroll()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();


            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\RankedTests\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "1441.png"),
                new Bitmap(folderPath + "1481.png"),
                new Bitmap(folderPath + "1548.png"),
                new Bitmap(folderPath + "1613.png"),
                new Bitmap(folderPath + "1670.png"),
                new Bitmap(folderPath + "1758.png"),
                new Bitmap(folderPath + "1768.png"),
                new Bitmap(folderPath + "1800.png"),
                new Bitmap(folderPath + "1805.png"),
                new Bitmap(folderPath + "1808.png")

            };

            List<int> drumroll = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                drumroll.Add(imageAnalysis.GetDrumroll(bmps[i], ImageAnalysis.Players.RankedTop));
                drumroll.Add(imageAnalysis.GetDrumroll(bmps[i], ImageAnalysis.Players.RankedBottom));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < drumroll.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                0,0,
                22,23,
                263,219,
                23,20,
                7,7,
                62,62,
                204,227,
                278,270,
                220,97,
                0,0,
            };
            for (int i = 0; i < bmps.Count; i++)
            {
                if (drumroll[i * 2] == expectedResults[i * 2])
                {
                    results[i * 2] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetDrumrollBitmaps(bmps[i], ImageAnalysis.Players.RankedTop);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedDrumrollTop[" + i + "][" + j + "].png");
                    }
                }
                if (drumroll[(i * 2) + 1] == expectedResults[(i * 2) + 1])
                {
                    results[(i * 2) + 1] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetDrumrollBitmaps(bmps[i], ImageAnalysis.Players.RankedBottom);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedDrumrollBottom[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < results.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + drumroll[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetRankedGoods()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\RankedTests\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "1441.png"),
                new Bitmap(folderPath + "1481.png"),
                new Bitmap(folderPath + "1548.png"),
                new Bitmap(folderPath + "1613.png"),
                new Bitmap(folderPath + "1670.png"),
                new Bitmap(folderPath + "1758.png"),
                new Bitmap(folderPath + "1768.png"),
                new Bitmap(folderPath + "1800.png"),
                new Bitmap(folderPath + "1805.png"),
                new Bitmap(folderPath + "1808.png")
            };

            List<int> goods = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                goods.Add(imageAnalysis.GetGoods(bmps[i], ImageAnalysis.Players.RankedTop));
                goods.Add(imageAnalysis.GetGoods(bmps[i], ImageAnalysis.Players.RankedBottom));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < goods.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                805,657,
                515,434,
                812,605,
                303,250,
                643,511,
                751,559,
                449,433,
                587,616,
                913,806,
                1075,844,
            };
            for (int i = 0; i < bmps.Count; i++)
            {
                if (goods[i * 2] == expectedResults[i * 2])
                {
                    results[i * 2] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetGoodsBitmaps(bmps[i], ImageAnalysis.Players.RankedTop);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedGoodsTop[" + i + "][" + j + "].png");
                    }
                }
                if (goods[(i * 2) + 1] == expectedResults[(i * 2) + 1])
                {
                    results[(i * 2) + 1] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetGoodsBitmaps(bmps[i], ImageAnalysis.Players.RankedBottom);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedGoodsBottom[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < results.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + goods[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetRankedOKs()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();


            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\RankedTests\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "1441.png"),
                new Bitmap(folderPath + "1481.png"),
                new Bitmap(folderPath + "1548.png"),
                new Bitmap(folderPath + "1613.png"),
                new Bitmap(folderPath + "1670.png"),
                new Bitmap(folderPath + "1758.png"),
                new Bitmap(folderPath + "1768.png"),
                new Bitmap(folderPath + "1800.png"),
                new Bitmap(folderPath + "1805.png"),
                new Bitmap(folderPath + "1808.png")
            };

            List<int> oks = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                oks.Add(imageAnalysis.GetOKs(bmps[i], ImageAnalysis.Players.RankedTop));
                oks.Add(imageAnalysis.GetOKs(bmps[i], ImageAnalysis.Players.RankedBottom));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < oks.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                54,202,
                8,89,
                44,234,
                0,53,
                39,169,
                12,195,
                54,54,
                86,62,
                105,208,
                337,551,
            };
            for (int i = 0; i < bmps.Count; i++)
            {
                if (oks[i * 2] == expectedResults[i * 2])
                {
                    results[i * 2] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetOKsBitmaps(bmps[i], ImageAnalysis.Players.RankedTop);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedOKsTop[" + i + "][" + j + "].png");
                    }
                }
                if (oks[(i * 2) + 1] == expectedResults[(i * 2) + 1])
                {
                    results[(i * 2) + 1] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetOKsBitmaps(bmps[i], ImageAnalysis.Players.RankedBottom);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedOKsBottom[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < bmps.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + oks[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetRankedScore()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\RankedTests\";

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "1441.png"),
                new Bitmap(folderPath + "1481.png"),
                new Bitmap(folderPath + "1548.png"),
                new Bitmap(folderPath + "1613.png"),
                new Bitmap(folderPath + "1670.png"),
                new Bitmap(folderPath + "1758.png"),
                new Bitmap(folderPath + "1768.png"),
                new Bitmap(folderPath + "1800.png"),
                new Bitmap(folderPath + "1805.png"),
                new Bitmap(folderPath + "1808.png")
            };

            List<int> scores = new List<int>();

            for (int i = 0; i < bmps.Count; i++)
            {
                scores.Add(imageAnalysis.GetScore(bmps[i], ImageAnalysis.Players.RankedTop));
                scores.Add(imageAnalysis.GetScore(bmps[i], ImageAnalysis.Players.RankedBottom));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < scores.Count; i++)
            {
                results.Add(false);
            }

            List<int> expectedResults = new List<int>()
            {
                959100,873030,
                993320,917060,
                989000,856360,
                1003120,918120,
                965400,866810,
                990440,861380,
                954450,938000,
                944450,966350,
                962680,897100,
                844470,758670,

            };
            for (int i = 0; i < bmps.Count; i++)
            {
                if (scores[i * 2] == expectedResults[i * 2])
                {
                    results[i * 2] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetScoreBitmaps(bmps[i], ImageAnalysis.Players.RankedTop);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedScoreTop[" + i + "][" + j + "].png");
                    }
                }
                if (scores[(i * 2) + 1] == expectedResults[(i * 2) + 1])
                {
                    results[(i * 2) + 1] = true;
                }
                else
                {
                    var failedBitmaps = imageAnalysis.GetScoreBitmaps(bmps[i], ImageAnalysis.Players.RankedBottom);
                    for (int j = 0; j < failedBitmaps.Count; j++)
                    {
                        failedBitmaps[j].Save(failedTestLocation + "TestGetRankedScoreBottom[" + i + "][" + j + "].png");
                    }
                }
                bmps[i].Dispose();
            }
            for (int i = 0; i < results.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + scores[i].ToString() + "'");
            }
            GC.Collect();
        }

        [TestMethod]
        public void TestGetTitle()
        {
            // I won't test every single title, that'd take far too long
            // I can test like 10-15 and be pretty confident it works well
            // I suppose there could be some titles that end up not working, but it'd add too much time to this
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetTitle\";

            // If adding items to bmps here, add them to expectedResults below too
            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "!!!カオスタイム!!!.Oni.0.png"),
                new Bitmap(folderPath + "HARDCOREノ心得.Oni.1.png"),
                new Bitmap(folderPath + "Infinite Rebellion.Oni.5.png"),
                new Bitmap(folderPath + "UNDEAD HEART(怒りのWarriors).Oni.3.png"),
                new Bitmap(folderPath + "Xa.Ura.4.png"),
                new Bitmap(folderPath + "初音ミクの消失‐劇場版‐.Ura.3.png"),
                new Bitmap(folderPath + "幽玄ノ乱.Oni.9.png"),
                new Bitmap(folderPath + "愛と浄罪の森.Oni.0.png"),
                new Bitmap(folderPath + "白鳥の湖.Ura.0.png"),
                new Bitmap(folderPath + "竜と黒炎の姫君.Ura.5.png"),
                new Bitmap(folderPath + "紫煌ノ乱.Oni.1.png"),
                new Bitmap(folderPath + "Behemoth.Oni.0.png"),
                new Bitmap(folderPath + "ナイト・オブ・ナイツ.Oni.0.png"),
                new Bitmap(folderPath + "1573.png"),
                new Bitmap(folderPath + "1682.png"),
                new Bitmap(folderPath + "1719.png"),
                new Bitmap(folderPath + "1779.png"),
                new Bitmap(folderPath + "1823.png"),
                new Bitmap(folderPath + "1830.png"),
                new Bitmap(folderPath + "1847.png"),
                new Bitmap(folderPath + "1879.png"),
                new Bitmap(folderPath + "1884.png"),
            };

            List<string> titles = new List<string>();

            for (int i = 0; i < bmps.Count; i++)
            {
                titles.Add(imageAnalysis.GetTitle(bmps[i]));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            List<string> expectedResults = new List<string>()
            {
                "!!!カオスタイム!!!",
                "HARDCOREノ心得",
                "Infinite Rebellion",
                "UNDEAD HEART(怒りのWarriors)",
                "Xa",
                "初音ミクの消失‐劇場版‐",
                "幽玄ノ乱",
                "愛と浄罪の森",
                "白鳥の湖",
                "竜と黒炎の姫君",
                "紫煌ノ乱",
                "Behemoth",
                "ナイト・オブ・ナイツ",
                "流星",
                "Ignis Danse",
                "エイリアンエイリアン",
                "FREEDOM DiVE↓",
                "ギミチョコ！！",
                "VICTORIA",
                "残酷な天使のテーゼ",
                "Naked Glow",
                "限界突破×サバイバー",
            };

            for (int i = 0; i < results.Count; i++)
            {
                bmps[i].Dispose();
                if (titles[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                Assert.IsTrue(results[i], "Expected " + expectedResults[i] + ", Result = '" + titles[i] + "'");
            }
            GC.Collect();
        }

        //[TestMethod]
        public void TestOldGetTitle()
        {
            // This function isn't really needed anymore, but it's still here anyway

            // I won't test every single title, that'd take far too long
            // I can test like 10-15 and be pretty confident it works well
            // I suppose there could be some titles that end up not working, but it'd add too much time to this
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetTitle\";

            // If adding items to bmps here, add them to expectedResults below too
            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(folderPath + "!!!カオスタイム!!!.Oni.0.png"),
                new Bitmap(folderPath + "HARDCOREノ心得.Oni.1.png"),
                new Bitmap(folderPath + "Infinite Rebellion.Oni.5.png"),
                new Bitmap(folderPath + "UNDEAD HEART(怒りのWarriors).Oni.3.png"),
                new Bitmap(folderPath + "Xa.Ura.4.png"),
                new Bitmap(folderPath + "初音ミクの消失‐劇場版‐.Ura.3.png"),
                new Bitmap(folderPath + "幽玄ノ乱.Oni.9.png"),
                new Bitmap(folderPath + "愛と浄罪の森.Oni.0.png"),
                new Bitmap(folderPath + "白鳥の湖.Ura.0.png"),
                new Bitmap(folderPath + "竜と黒炎の姫君.Ura.5.png"),
                new Bitmap(folderPath + "紫煌ノ乱.Oni.1.png"),
                new Bitmap(folderPath + "Behemoth.Oni.0.png"),
                new Bitmap(folderPath + "ナイト・オブ・ナイツ.Oni.0.png"),
                new Bitmap(folderPath + "1573.png"),
                new Bitmap(folderPath + "1682.png"),
                new Bitmap(folderPath + "1719.png"),
                new Bitmap(folderPath + "1779.png"),
                new Bitmap(folderPath + "1823.png"),
                new Bitmap(folderPath + "1830.png"),
                new Bitmap(folderPath + "1847.png"),
                new Bitmap(folderPath + "1879.png"),
                new Bitmap(folderPath + "1884.png"),
            };

            List<string> titles = new List<string>();

            for (int i = 0; i < bmps.Count; i++)
            {
                titles.Add(imageAnalysis.OldGetTitle(bmps[i]));
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            List<string> expectedResults = new List<string>()
            {
                "!!!カオスタイム!!!",
                "HARDCOREノ心得",
                "Infinite Rebellion",
                "UNDEAD HEART(怒りのWarriors)",
                "Xa",
                "初音ミクの消失‐劇場版‐",
                "幽玄ノ乱",
                "愛と浄罪の森",
                "白鳥の湖",
                "竜と黒炎の姫君",
                "紫煌ノ乱",
                "Behemoth",
                "ナイト・オブ・ナイツ",
                "流星",
                "Ignis Danse",
                "エイリアンエイリアン",
                "FREEDOM DiVE↓",
                "ギミチョコ！！",
                "VICTORIA",
                "残酷な天使のテーゼ",
                "Naked Glow",
                "限界突破×サバイバー",
            };

            for (int i = 0; i < results.Count; i++)
            {
                bmps[i].Dispose();
                if (titles[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                Assert.IsTrue(results[i], "Expected " + expectedResults[i] + ", Result = '" + titles[i] + "'");
            }
            GC.Collect();
        }
    }
}
