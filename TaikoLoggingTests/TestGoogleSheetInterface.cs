using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoLogging;

namespace TaikoLoggingTests
{
    [TestClass]
    public class TestGoogleSheetInterface
    {
        string spreadsheetId = "169qLgB5Pxxsoy501wp12c9nNXY8-tTmFnQPu9Nktvzk";
        GoogleSheetInterface sheet = new GoogleSheetInterface();
        
        // This isn't going to work for some reason
        // It's unable to initialize the GoogleSheetInterface

        // I have some plans to implement this, but I will wait to do that until later
        //[TestMethod]
        public void TestAddRankedMatch()
        {
            sheet.spreadsheetId = spreadsheetId;

            IList<object> baseValues = new List<object>();
            baseValues.Add("Test");

            List<IList<object>> sendValues = new List<IList<object>>();
            sendValues.Add(baseValues);

            sheet.SendData("Ranked Logs!S4", sendValues);

        }

    }
}
