using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MailRoomMessager.Helper
{
    public class OpenIdHelper
    {
        public static async Task<string> GetOpenIdAsync(string code)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", "wxc13ba2d6438ac40a", "e755c0ada29c81741891506e4d93edd7", code));

            string s = await response.Content.ReadAsStringAsync();

            JObject obj = JObject.Parse(s);

            if (obj["access_token"] == null)
            {
                return null;
            }
            return obj["openid"].ToString();
        }
    }
}