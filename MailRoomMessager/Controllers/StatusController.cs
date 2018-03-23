using MailRoomMessager.Helper;
using MailRoomMessager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MailRoomMessager.Controllers
{
    [RoutePrefix("api/status")]
    public class StatusController : ApiController
    {
        public HttpResponseMessage AddStatus([FromBody]Status status)
        {
            int result=0;
            if (status != null && (!string.IsNullOrEmpty(status.Name)) && status.Value != 0)
            {
                string sql = $"insert into dbo.Status(Name,Value) values({status.Name},{status.Value})";
                result=DBHelper.Execute(sql);
            }
            if (result == 1)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
            } else
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
