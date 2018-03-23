using Microsoft.VisualStudio.TestTools.UnitTesting;
using MailRoomMessager.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailRoomMessager.Messages;
using MailRoomMessager.Controllers;

namespace MailRoomMessager.Helper.Tests
{
    [TestClass()]
    public class Sha1HelperTests
    {
        [TestMethod()]
        public void SHA1EncryptTest()
        {
            //FetchMessage message = new FetchMessage
            //{
            //    OpenId = "o5dhF1EKpj54qs6-GNoYtLPUSUOA",
            //    TemplateId = "GicaQGB7iOQWHBVs_0riiEYPSo-LyOMVa1jLIXPWRBA",
            //    First = "您的包裹已到",
            //    Operator = "EMS",
            //    OrderId = "111"
            //};

            //TemplateMessageHelper.SendAsync(message).Wait();
            //string s = "jsapi_ticket=sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg&noncestr=Wm3WZYTPz0wzccnW&timestamp=1414587457&url=http://mp.weixin.qq.com?params=value";

            //string result = Sha1Helper.SHA1Encrypt(s);
            Assert.Fail();
        }
    }
}