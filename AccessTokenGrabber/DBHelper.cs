using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessTokenGrabber
{
    public class DBHelper
    {
        private static string ConnectionString;

        public static string GetConnectionString()
        {
            if (ConnectionString == null)
            {
                ConnectionString = ConfigurationManager.AppSettings["DBConnectionString"].ToString();
            }
            return ConnectionString;
        }
    }
}
