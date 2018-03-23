using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Configuration;
using System.Net.Http;
using MailRoomMessager.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace AccessTokenGrabber
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            CheckToken();
        }

        static void CheckToken()
        {
            AccessToken token = new AccessToken();
            token.ExpireTime = DateTime.UtcNow;
            string sql = @"select * from AccessToken where ExpireTime>=@ExpireTime order by ExpireTime desc";

            using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
            {
                var tokens = connection.Query<AccessToken>(sql, token);

                if (tokens == null || tokens.Count() == 0)
                {
                    Grab().Wait();
                    return;
                }
                TimeSpan span = tokens.ToList()[0].ExpireTime - DateTime.UtcNow;
                if (span.TotalSeconds <= 660)
                {
                    Grab().Wait();
                }
            }
        }

        static async Task Grab()
        {
            string appId = ConfigurationManager.AppSettings["AppId"].ToString();
            string appSecret = ConfigurationManager.AppSettings["AppSecret"].ToString();

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appId, appSecret));

            string content = await response.Content.ReadAsStringAsync();
            JObject obj = JObject.Parse(content);

            //get access token
            AccessToken token = new AccessToken();
            token.Token = obj["access_token"].ToString();
            token.RefreshTime = DateTime.UtcNow;
            token.ExpireTime = DateTime.UtcNow.AddSeconds(Convert.ToInt32(obj["expires_in"].ToString()));

            response = await client.GetAsync(string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", token.Token));
            content = await response.Content.ReadAsStringAsync();
            obj = JObject.Parse(content);

            //get ticket for js
            token.Ticket = obj["ticket"].ToString();

            string sql = @"insert AccessToken(Token, Ticket, RefreshTime, ExpireTime) 
                                               values(@Token, @Ticket, @RefreshTime, @ExpireTime)";

            using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
            {
                connection.Execute(sql, token);
            }
        }
    }
}
