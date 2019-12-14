using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoLogging
{
    class PlayRecordingSheetsInterface
    {
        // Alright here's the plan
        // I'm going to create a shit load of sheets
        // And then, I'm going to link all those sheets in Oni/Ura
        // Each title will turn into a hyperlink looking like this 
        // =HYPERLINK("#gid=1305883953","あなたとトゥラッタッタ♪")
        // =HYPERLINK("#gid=sheetID","songTitle")
        // @"=HYPERLINK("#gid=" + sheetID + "\","あなたとトゥラッタッタ♪")"

        // This is how I want each song's sheet to look like
        // https://docs.google.com/spreadsheets/d/169qLgB5Pxxsoy501wp12c9nNXY8-tTmFnQPu9Nktvzk/edit#gid=1944605931
        // I'm not sure how to properly size each chart image, some chart images are longer than others/bigger/whatever
        // I'm also not sure how to put images on via the sheets api

        public void Test()
        {
            int sheetID = GetSheetID("Ura");
        }

        private void AddSheet(string sheetName)
        {
            Program.sheet.AddSheet(sheetName);
        }

        private int GetSheetID(string sheetName)
        {
            int sheetID = Program.sheet.GetSheetID(sheetName);
            if (sheetID == -1)
            {
                // failed to get sheetID
                return -1;
            }
            return sheetID;
        }
    }
}
