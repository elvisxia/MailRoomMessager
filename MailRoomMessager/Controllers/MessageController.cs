using MailRoomMessager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Configuration;
using MailRoomMessager.Helper;

namespace MailRoomMessager.Controllers
{
    [RoutePrefix("message")]
    public class MessageController : ApiController
    {
        [Route("")]
        [HttpGet,HttpPost]
        public void HandleMessageAsync()
        {
            Validation validation = new Validation();
            validation.Signature = HttpContext.Current.Request.QueryString["signature"];
            validation.Timestamp = HttpContext.Current.Request.QueryString["timestamp"];
            validation.Nonce = HttpContext.Current.Request.QueryString["nonce"];
            validation.Echostr = HttpContext.Current.Request.QueryString["echostr"];
            if (validation == null || string.IsNullOrEmpty(validation.Echostr))
            {
                return;
            }
            if (!CheckSignature(validation))
            {
                return;
            }
            HttpContext.Current.Response.Write(validation.Echostr);
            HttpContext.Current.Response.End();
        }

        private bool CheckSignature(Validation validation)
        {
            string token = ConfigurationManager.AppSettings["token"].ToString();

            string[] arr = { token, validation.Timestamp, validation.Nonce };
            Array.Sort(arr);
            string result = Sha1Helper.SHA1Encrypt(string.Join("", arr));

            if (result == validation.Signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
