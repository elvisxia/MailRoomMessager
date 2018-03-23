using Dapper;
using MailRoomMessager.Helper;
using MailRoomMessager.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MailRoomMessager.Controllers
{
    [RoutePrefix("api/sites")]
    public class SiteController : ApiController
    {
        [Route("")]
        [HttpGet]
        public HttpResponseMessage GetSite()
        {
            string sql = @"select * from Site order by Name,Building,Floor";

            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    var sites = connection.Query<Site>(sql);
                    if (sites == null || sites.Count() <= 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, Error.NotFound);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, sites);

                    //result["action"] = "show";
                    //JArray data = JArray.Parse(JsonConvert.SerializeObject(sites.ToList()));
                    //result["data"] = data;
                    //HttpContext.Current.Response.Write(result);
                    //HttpContext.Current.Response.End();
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }
    }
}
