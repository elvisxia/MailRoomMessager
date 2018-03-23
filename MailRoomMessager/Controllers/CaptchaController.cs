using Dapper;
using MailRoomMessager.Helper;
using MailRoomMessager.Messages;
using MailRoomMessager.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MailRoomMessager.Controllers
{
    [RoutePrefix("api/captchas")]
    public class CaptchaController : ApiController
    {
        [Route("wx")]
        [HttpPost]
        public HttpResponseMessage Bind([FromBody]EmployeeMap emp)
        {
            if (emp == null || string.IsNullOrEmpty(emp.Alias) || string.IsNullOrEmpty(emp.Captcha))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Error.LackInfo);
            }

            emp.CaptchaExpiresTime = DateTime.UtcNow;
            string sql = @"select * from EmployeeMap where Alias = @Alias and Captcha=@Captcha and CaptchaExpiresTime>@CaptchaExpiresTime";

            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    var employeeMaps = connection.Query<EmployeeMap>(sql, emp);

                    if (employeeMaps == null || employeeMaps.Count() == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, Error.LackInfo);
                    }
                    else
                    {
                        emp.OpenId = employeeMaps.ToList()[0].OpenId;
                        sql = @"insert Employee(Alias, OpenId)
                                               values(@Alias, @OpenId)";
                        connection.Execute(sql, emp);
                        return Request.CreateResponse(HttpStatusCode.OK, emp.OpenId);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }


        [Route("wx")]
        [HttpGet]
        public HttpResponseMessage GetCaptcha([FromUri]Employee emp)
        {
            if (emp == null || string.IsNullOrEmpty(emp.Code) || string.IsNullOrEmpty(emp.Alias))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Error.LackInfo);
            }

            EmployeeMap employeeMap = new EmployeeMap();
            employeeMap.Alias = emp.Alias;
            employeeMap.Code = emp.Code;
            employeeMap.CaptchaExpiresTime = DateTime.UtcNow.AddMinutes(30);
            string sql = @"select * from EmployeeMap where Code = @Code";

            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    var employeeMaps = connection.Query<EmployeeMap>(sql, employeeMap);

                    if (employeeMaps == null || employeeMaps.Count() == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
                    }
                    else
                    {
                        Random random = new Random();
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < 6; i++)
                        {
                            stringBuilder.Append(random.Next() % 10);
                        }

                        employeeMap.Captcha = stringBuilder.ToString();

                        //database
                        sql = @"update EmployeeMap set Alias = @Alias, Captcha = @Captcha, CaptchaExpiresTime = @CaptchaExpiresTime where Code = @Code";
                        connection.Execute(sql, employeeMap);

                        //email
                        CaptchaMessage message = new CaptchaMessage() { Alias = employeeMap.Alias, Captcha = employeeMap.Captcha };

                        MessageHelper.SendMessage(message);

                        //SendEmail(employeeMap.Alias, employeeMap.Captcha);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }
    }
}
