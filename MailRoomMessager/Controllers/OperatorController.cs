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
    [RoutePrefix("api/operators")]
    public class OperatorController : ApiController
    {
        [Route("")]
        [HttpGet]
        public HttpResponseMessage GetOperators()
        {
            string sql = @"select Name from Operator where Name<>N'其他' order by len(Name), Name";
            using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
            {
                List<Operator> operators = connection.Query<Operator>(sql).ToList();
                operators.Add(new Operator() { Name = "其他" });
                return Request.CreateResponse(HttpStatusCode.OK, operators);
            }
        }
    }
}
