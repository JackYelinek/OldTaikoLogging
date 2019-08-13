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

        }

        [TestMethod]
        public void TestCheckDifficulty()
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
                if (difficulties[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + difficulties[i].ToString() + "'");
            }
        }

        [TestMethod]
        public void TestCheckSingleMods()
        {
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            List<Bitmap> bmps = new List<Bitmap>()
            {
                new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckMods\!!!カオスタイム!!!.Oni.0.png")
            };

            List<List<string>> mods = new List<List<string>>()
            {
                
            };

            for (int i = 0; i < bmps.Count; i++)
            {
                mods[i] = imageAnalysis.CheckMods(bmps[i], ImageAnalysis.Players.Single);
            }

            List<bool> results = new List<bool>();
            for (int i = 0; i < bmps.Count; i++)
            {
                results.Add(false);
            }

            List<List<string>> expectedResults = new List<List<string>>()
            {
                new List<string>()
                {
                    null,
                }
            };

            for (int i = 0; i < bmps.Count; i++)
            {
                if (mods[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + mods[i].ToString() + "'");
            }
        }

        [TestMethod]
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
            }
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
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + bads[i].ToString() + "'");
            }
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
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + combo[i].ToString() + "'");
            }
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
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + drumroll[i].ToString() + "'");
            }
        }

        [TestMethod]
        public void TestGetSingleGoods()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            const int numBitmaps = 11; // or something like that

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
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + goods[i].ToString() + "'");
            }
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
                    imageAnalysis.GetSmallDigits(bmps[i], failedTestLocation, "TestGetSingleOKs[" + i + "]");
                }
            }
            for (int i = 0; i < bmps.Count; i++)
            {
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + oks[i].ToString() + "'");

            }
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

            for (int i = 0; i < results.Count; i++)
            {
                if (scores[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + scores[i].ToString() + "'");
            }
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
                new Bitmap(folderPath + "ナイト・オブ・ナイツ.Oni.0.png")
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
            };

            for (int i = 0; i < results.Count; i++)
            {
                if (titles[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                Assert.IsTrue(results[i], "Expected " + expectedResults[i] + ", Result = '" + titles[i] + "'");
            }
        }
    }
}
