using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFSyclusStatusLogger.StatusLoggers
{
    class StatusLogger : IStatusLogger
    {
        IStatusLogger objStatusLogger;

        public StatusLogger()
        {
            string dbEngine = ConfigurationManager.AppSettings["DatabaseEngine"].ToString();

            switch (dbEngine)
            {
                case "MySql":
                    objStatusLogger = new StatusLoggerMySql();
                    break;

                case "SqlServerExpress":
                    objStatusLogger = new StatusLoggerSqlServerExpresss();
                    break;
            }
        }

        public string GetStatus(string filename)
        {
            string status = objStatusLogger.GetStatus(filename);

            return status;
        }

        public DataTable GetNewSyclusIdList(DateTime datetime)
        {
            DataTable dt = objStatusLogger.GetNewSyclusIdList(datetime);

            return dt;
        }

        public void SetCreateStatus(string filename)
        {
            objStatusLogger.SetCreateStatus(filename);
        }

        public void SetFailedStatus(string filename)
        {
            objStatusLogger.SetFailedStatus(filename);
        }

        public void SetSuccessStatus(string filename)
        {
            objStatusLogger.SetSuccessStatus(filename);
        }
    }
}
