using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Api.V5;
using TwitchLib.Client.Models;

namespace TaikoLogging
{
    class RinClient
    {
        readonly ConnectionCredentials credentials = new ConnectionCredentials(TwitchInfo.BotUsername, TwitchInfo.BotToken);
        TwitchClient client;
        GoogleSheetInterface sheet;

        public RinClient()
        {
            sheet = new GoogleSheetInterface();
            Connect();
        }


        ~RinClient()
        {
            Disconnect();
        }

        internal void Connect()
        {
            client = new TwitchClient();
            client.Initialize(credentials, TwitchInfo.ChannelName);

            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnConnectionError += Client_OnConnectionError;
            client.Connect();
        }

        private void Client_OnConnectionError(object sender, TwitchLib.Client.Events.OnConnectionErrorArgs e)
        {
            //Console.WriteLine($"Error!! {e.Error}");
        }

        private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            if(e.ChatMessage.Message.StartsWith("!notranked", StringComparison.InvariantCultureIgnoreCase) && string.Compare(e.ChatMessage.Username, "Deathblood", true) == 0)
            {
                sheet.RemoveLastRanked();
                SendTwitchMessage("Last ranked match removed");
            }
            else if (e.ChatMessage.Message.StartsWith("!removeranked", StringComparison.InvariantCultureIgnoreCase) && string.Compare(e.ChatMessage.Username, "Deathblood", true) == 0)
            {
                sheet.RemoveLastRanked();
                SendTwitchMessage("Last ranked match removed");
            }
            else if (e.ChatMessage.Message.StartsWith("!undoranked", StringComparison.InvariantCultureIgnoreCase) && string.Compare(e.ChatMessage.Username, "Deathblood", true) == 0)
            {
                sheet.RemoveLastRanked();
                SendTwitchMessage("Last ranked match removed");
            }
        }

        internal void Disconnect()
        {

        }

        public void SendTwitchMessage(string message)
        {
            if (Program.twitchOn == true)
            {
                client.SendMessage(TwitchInfo.ChannelName, message);
            }
        }

    }
}
