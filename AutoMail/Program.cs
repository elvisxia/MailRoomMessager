using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AutoMail
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        private static QueueClient QClient;
        private static SendGridClient SClient;

        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);

            StartMessageListening();

            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }

        static void StartMessageListening()
        {
            if (QClient == null)
            {
                var connectionString = ConfigurationManager.AppSettings["QueueConnectionString"];
                var queueName = ConfigurationManager.AppSettings["QueueName"];

                QClient = QueueClient.CreateFromConnectionString(connectionString, queueName);
            }

            QClient.OnMessage(messageString =>
            {
                CaptchaMessage message = JsonConvert.DeserializeObject<CaptchaMessage>(messageString.GetBody<string>());

                SendEmail(message);
            });

        }

        static async void SendEmail(CaptchaMessage message)
        {
            var apiKey = "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";
            SendGridClient SClient = new SendGridClient(apiKey);
            try
            {
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("mailroomservice@microsoft.com", "Mailroom Service")
                };
                msg.SetTemplateId("9cba370f-5cf1-4a44-b73a-c7c2fe6e22c3");

                msg.AddTo(message.Alias + "@microsoft.com");

                msg.AddSubstitutions(new Dictionary<string, string>()
                {
                    {"-alias-",message.Alias },
                    {"-captcha-",message.Captcha }
                });

                await SClient.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
