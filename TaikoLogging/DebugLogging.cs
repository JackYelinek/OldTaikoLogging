using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TaikoLogging
{
    class DebugLogging
    {
        const string logFolderLocation = @"D:\My Stuff\My Programs\Taiko\TaikoLogging Logs\";



        public DebugLogging()
        {
            // Currently just used for testing

        }




        // So the basics of this class would be to write down certain information to files that I may need
        // If something fails, log it
        // If something succeeds but I think it can be done better, log it
        // Should I be logging these things in different files? Should I only separate files by date?


        // Let's start with a super basic function that just takes in a message and appends it to a file
        // This function will be used by the other functions I make here
        private void AppendLogFile(string message)
        {
            // This is the file location for the logs
            // (@"D:\My Stuff\My Programs\Taiko\TaikoLogging Logs") 
            string date = DateTime.Now.ToString("MM-dd-yyyy");

            File.AppendAllText(logFolderLocation + date + ".log", message + "\n");
        }

        // I may have gone a bit too generic with this one
        public void LogVariable(string category, string variableName, object variableValue)
        {
            string message = "[" + category + "] " + variableName + ": " + variableValue.ToString();
            AppendLogFile(message);
        }

        // I may be going a bit too specific with this next one
        // although I think I'll get more use out of specific than generic right now until I get the hang of logging
        public void LogPixelDifference(string songTitle, int pixelDifferences)
        {
            string message = "[Pixel Difference] " + songTitle + ": " + pixelDifferences;
            AppendLogFile(message);
        }

        // This might be useful, or maybe not, I have no clue
        public void LogManyVariables(string category, List<string> headers, List<object> info)
        {
            string message = "[" + category + "] ";
            for (int i = 0; i < Math.Max(headers.Count, info.Count); i++)
            {
                if (headers.Count > i)
                {
                    message += headers[i];
                }
                message += ":\t";
                if (info.Count > i)
                {
                    message += info[i];
                }
                message += "\n";
            }
        }
    }
}
