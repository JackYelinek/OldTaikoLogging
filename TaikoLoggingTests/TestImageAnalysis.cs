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
        [TestMethod]
        public void TestFindGameWindow()
        {
            ScreenGrab screen = new ScreenGrab();

            const int numBitmaps = 1;

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\FindGameWindow\";

            Bitmap[] bmps = new Bitmap[numBitmaps]
            {
                new Bitmap(folderPath + "初音ミクの消失‐劇場版‐.Ura.2.png")
            };


            for (int i = 0; i < bmps.Length; i++)
            {

                screen.FindGameWindow(bmps[i]);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }

            // Top, Left, Bottom, Right
            List<int>[] expectedResults = new List<int>[numBitmaps]
            {
                new List<int> {72, 20, 719, 1172 },
            };

            for (int i = 0; i < results.Length; i++)
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
            const int numBitmaps = 5;

            Bitmap[] bmps = new Bitmap[numBitmaps]
            {
                new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckDifficulty\Easy.png"),
                new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckDifficulty\Normal.png"),
                new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckDifficulty\Hard.png"),
                new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckDifficulty\Oni.png"),
                new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckDifficulty\Ura.png")
            };

            ImageAnalysis.Difficulty[] difficulties = new ImageAnalysis.Difficulty[numBitmaps];

            for (int i = 0; i < bmps.Length; i++)
            {
                difficulties[i] = imageAnalysis.CheckDifficulty(bmps[i], ImageAnalysis.Players.Single);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }

            ImageAnalysis.Difficulty[] expectedResults = new ImageAnalysis.Difficulty[numBitmaps]
            {
                ImageAnalysis.Difficulty.Easy,
                ImageAnalysis.Difficulty.Normal,
                ImageAnalysis.Difficulty.Hard,
                ImageAnalysis.Difficulty.Oni,
                ImageAnalysis.Difficulty.Ura
            };

            for (int i = 0; i < results.Length; i++)
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
            const int numBitmaps = 1; // or something like that

            Bitmap[] bmps = new Bitmap[numBitmaps]
            {
                new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckMods\!!!カオスタイム!!!.Oni.0.png")
            };

            List<List<string>> mods = new List<List<string>>();

            for (int i = 0; i < bmps.Length; i++)
            {
                mods[i] = imageAnalysis.CheckMods(bmps[i], ImageAnalysis.Players.Single);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }
        }

        [TestMethod]
        public void TestCheckState()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            const int numBitmaps = 1; // or something like that

            Bitmap[] bmps = new Bitmap[numBitmaps]
            {
                // I don't have any bitmaps to test with quite yet
                new Bitmap(@"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\CheckState\!!!カオスタイム!!!.Oni.0.png")
            };

            ImageAnalysis.State[] states = new ImageAnalysis.State[numBitmaps];

            for (int i = 0; i < bmps.Length; i++)
            {
                states[i] = imageAnalysis.CheckState(bmps[i]);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }
        }

        [TestMethod]
        public void TestGetSingleBads()
        {
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            const int numBitmaps = 11; // or something like that

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            Bitmap[] bmps = new Bitmap[numBitmaps]
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
            };

            int[] bads = new int[numBitmaps];

            for (int i = 0; i < bmps.Length; i++)
            {
                bads[i] = imageAnalysis.GetBads(bmps[i], ImageAnalysis.Players.Single);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }

            int[] expectedResults = new int[numBitmaps]
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
                0
            };

            for (int i = 0; i < results.Length; i++)
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

            const int numBitmaps = 11; // or something like that

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            Bitmap[] bmps = new Bitmap[numBitmaps]
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
            };

            int[] combo = new int[numBitmaps];

            for (int i = 0; i < bmps.Length; i++)
            {
                combo[i] = imageAnalysis.GetCombo(bmps[i], ImageAnalysis.Players.Single);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }

            int[] expectedResults = new int[numBitmaps]
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
                823
            };

            for (int i = 0; i < results.Length; i++)
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

            const int numBitmaps = 11; // or something like that

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            Bitmap[] bmps = new Bitmap[numBitmaps]
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
            };

            int[] drumroll = new int[numBitmaps];

            for (int i = 0; i < bmps.Length; i++)
            {
                drumroll[i] = imageAnalysis.GetDrumroll(bmps[i], ImageAnalysis.Players.Single);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }

            int[] expectedResults = new int[numBitmaps]
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
                0
            };

            for (int i = 0; i < results.Length; i++)
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

            Bitmap[] bmps = new Bitmap[numBitmaps]
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
            };

            int[] goods = new int[numBitmaps];

            for (int i = 0; i < bmps.Length; i++)
            {
                goods[i] = imageAnalysis.GetGoods(bmps[i], ImageAnalysis.Players.Single);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }

            int[] expectedResults = new int[numBitmaps]
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
                779
            };

            for (int i = 0; i < results.Length; i++)
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

            const int numBitmaps = 11; // or something like that

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            Bitmap[] bmps = new Bitmap[numBitmaps]
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
            };

            int[] oks = new int[numBitmaps];

            for (int i = 0; i < bmps.Length; i++)
            {
                oks[i] = imageAnalysis.GetOKs(bmps[i], ImageAnalysis.Players.Single);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }

            int[] expectedResults = new int[numBitmaps]
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
                44
            };

            for (int i = 0; i < results.Length; i++)
            {
                if (oks[i] == expectedResults[i])
                {
                    results[i] = true;
                }
                Assert.IsTrue(results[i], "Expected " + expectedResults[i].ToString() + ", Result = '" + oks[i].ToString() + "'");
            }
        }

        [TestMethod]
        public void TestGetSingleScore()
        {
            // This one's gonna be a massive one, both for testing and for setting up the tests
            ImageAnalysis imageAnalysis = new ImageAnalysis();

            const int numBitmaps = 11; // or something like that

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetNumbers\";

            Bitmap[] bmps = new Bitmap[numBitmaps]
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
            };

            int[] scores = new int[numBitmaps];

            for (int i = 0; i < bmps.Length; i++)
            {
                scores[i] = imageAnalysis.GetScore(bmps[i], ImageAnalysis.Players.Single);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }

            int[] expectedResults = new int[numBitmaps]
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
                1179040
            };

            for (int i = 0; i < results.Length; i++)
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

            const int numBitmaps = 11; // or something like that

            string folderPath = @"D:\My Stuff\My Programs\Taiko\TaikoLogging\TaikoLogging\Data\Test Data\GetTitle\";

            Bitmap[] bmps = new Bitmap[numBitmaps]
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
            };

            string[] titles = new string[numBitmaps];

            for (int i = 0; i < bmps.Length; i++)
            {
                titles[i] = imageAnalysis.GetTitle(bmps[i]);
            }

            bool[] results = new bool[numBitmaps];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = false;
            }

            string[] expectedResults = new string[numBitmaps]
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
                "紫煌ノ乱"
            };

            for (int i = 0; i < results.Length; i++)
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
