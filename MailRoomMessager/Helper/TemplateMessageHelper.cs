using Dapper;
using MailRoomMessager.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MailRoomMessager.Helper
{
    public class TemplateMessageHelper
    {
        public static HttpClient client = new HttpClient();
        public static void SendAsync(FetchMessage message)
        {
            JObject msg = new JObject();
            JObject data = new JObject();
            data["first"] = JObject.Parse("{\"value\":\"" + message.First + "\",\"color\":\"#173177\"}");
            data["operator"] = JObject.Parse("{\"value\":\"" + message.Operator + "\",\"color\":\"#173177\"}");
            data["orderId"] = JObject.Parse("{\"value\":\"" + message.OrderId + "\",\"color\":\"#173177\"}");
            msg["touser"] = message.OpenId;
            msg["template_id"] = message.TemplateId;
            msg["data"] = data;
            HttpContent content = new StringContent(msg.ToString());
            string token = "";
            string sql = "select top 1 Token from AccessToken order by ExpireTime desc";
            using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
            {
                token = connection.QueryFirst<string>(sql);
            }
            client.PostAsync($"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={token}", content).Wait();
        }
    }
}