using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging
{
    class HighScore
    {
        // This class will be for whatever gets done when a highscore is made
        Bitmap resultBitmap;
        string account;
        ImageAnalysis.Difficulty difficulty;

        string title;
        int score;
        int goods;
        int oks;
        int bads;
        int combo;
        int drumroll;

        int level;

        public HighScore(Bitmap bmp, List<object> info, List<string> headers)
        {
            resultBitmap = bmp;

            account = (string)info[headers.IndexOf("Account")];
            difficulty = (ImageAnalysis.Difficulty)info[headers.IndexOf("Difficulty")];
            title = (string)info[headers.IndexOf("Title")];
            score = (int)info[headers.IndexOf("Score")];
            goods = (int)info[headers.IndexOf("GOOD")];
            oks = (int)info[headers.IndexOf("OK")];
            bads = (int)info[headers.IndexOf("BAD")];
            combo = (int)info[headers.IndexOf("MAX Combo")];
            drumroll = (int)info[headers.IndexOf("Drumroll")];

            level = GetSongLevel(title);
        }

        private int GetSongLevel(string title)
        {
            string range = difficulty.ToString() + "!B2:E";
            var values = Program.sheet.GetValues(range);

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i][0].ToString() == title)
                {
                    return (int)values[i][3];
                }
            }
            return 0;
        }

        // Later on I can set this class up to send highscores to discord or something, move some of the code from sheets/imageanalysis here

    }
}
