using MailRoomMessager.Messages;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MailRoomMessager.Helper
{
    public class MessageHelper
    {
        private static QueueClient Client;
        private static QueueClient ToastClient;

        public static void SendMessage(CaptchaMessage message)
        {
            if (Client == null)
            {
                Client = QueueClient.CreateFromConnectionString(ConfigurationManager.AppSettings["QueueConnectionString"].ToString(), ConfigurationManager.AppSettings["QueueName"].ToString());
            }
            var brokeredMessage = new BrokeredMessage(JsonConvert.SerializeObject(message));
            Client.Send(brokeredMessage);
        }
    }
}