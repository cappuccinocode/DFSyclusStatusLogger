using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFSyclusStatusLogger.Helper
{
    public static class DBHelper
    {
        public static string GetConnectionString()
        {
            string sqlConnString = "";
            string dbEngine = ConfigurationManager.AppSettings["DatabaseEngine"].ToString();
            
            switch (dbEngine)
            {
                case "MySql":
                    sqlConnString = string.Format("server={0};database={1};port={2};uid={3};password={4}",
                        ConfigurationManager.AppSettings["DbHostname"].ToString(),
                        ConfigurationManager.AppSettings["DbName"].ToString(),
                        ConfigurationManager.AppSettings["DbPort"].ToString(),
                        ConfigurationManager.AppSettings["DbUser"].ToString(),
                        ConfigurationManager.AppSettings["DbPassword"].ToString()
                        );
                    break;

                case "SqlServerExpress":
                    sqlConnString = string.Format("server={0};database={1};user id={2};password={3}",
                        ConfigurationManager.AppSettings["DbHostname"].ToString(),
                        ConfigurationManager.AppSettings["DbName"].ToString(),
                        ConfigurationManager.AppSettings["DbUser"].ToString(),
                        ConfigurationManager.AppSettings["DbPassword"].ToString()
                        );
                    break;
            }

            return sqlConnString;
        }
    }
}
