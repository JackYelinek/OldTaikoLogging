using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoLogging;

namespace TaikoLoggingTests
{
    [TestClass]
    public class TestGoogleSheetInterface
    {
        // Quick link to test spreadsheet: https://docs.google.com/spreadsheets/d/169qLgB5Pxxsoy501wp12c9nNXY8-tTmFnQPu9Nktvzk/edit#gid=1678696668
        string spreadsheetId = "169qLgB5Pxxsoy501wp12c9nNXY8-tTmFnQPu9Nktvzk";
        GoogleSheetInterface sheet = new GoogleSheetInterface();
        
        // This isn't going to work for some reason
        // It's unable to initialize the GoogleSheetInterface

        // I have some plans to implement this, but I will wait to do that until later
        [TestMethod]
        public void TestAddRankedMatch()
        {
            // These 2 lines need to be at the start of every test
            sheet.spreadsheetId = spreadsheetId;
            sheet.test = true;

            // Putting all the info and headers down kinda takes awhile, so maybe just one test is plenty for this
            List<object> info = new List<object>
            {
                "Day by Day!",
                ImageAnalysis.Difficulty.Oni,
                true,
                989900,
                589,
                18,
                1,
                474,
                109,
                915200,
                509,
                98,
                1,
                603,
                94,
            };

            List<string> headers = new List<string>
            {
                "Song",
                "Difficulty",
                "Win/Loss",
                "My Score",
                "My Goods",
                "My OKs",
                "My Bads",
                "My Combo",
                "My Drumroll",
                "Opp Score",
                "Opp Goods",
                "Opp OKs",
                "Opp Bads",
                "Opp Combo",
                "Opp Drumroll",
            };

            // This doesn't actually get used, I just kinda need a bitmap
            Bitmap bmp = new Bitmap(@"D:\My Stuff\My Programs\Taiko\Image Data\Ranked Logs\1441.png");

            sheet.AddRankedEntry(info, headers, bmp);

            var Headers = sheet.GetHeaders("'Ranked Logs'");
            var values = sheet.GetValues("Ranked Logs!A2:A");
            values = sheet.GetValues("Ranked Logs!" + sheet.GetColumnName(Headers.IndexOf("Match")) + (values.Count + 1) + ":" +sheet.GetColumnName(Headers.IndexOf("DateTime")) + (values.Count + 1));

            // I didn't do this correctly
            // Should make it dynamic with the Headers, which is why I grabbed the Headers up above
            List<object> expectedInfo = new List<object>
            {
                "1793",
                "Day by Day!",
                "Oni",
                "Win",
                "74700",
                "989900",
                "589",
                "18",
                "1",
                "474",
                "109",
                "915200",
                "509",
                "98",
                "1",
                "603",
                "94",
                DateTime.Today.ToString(),
            };
            List<bool> results = new List<bool>();
            

            for (int i = 0; i < values[0].Count; i++)
            {
                results.Add(false);
                if (values[0][i] == expectedInfo[i])
                {
                    results[i] = true;
                    continue;
                }
            }

            for (int i = 0; i < results.Count; i++)
            {
                Assert.IsTrue(results[i], Headers[i] + " resulted in " + values[0][i] + ", should've been " + expectedInfo[i]);
            }
        }

    }
}
