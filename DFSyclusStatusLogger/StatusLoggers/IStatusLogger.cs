using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFSyclusStatusLogger.StatusLoggers
{
    interface IStatusLogger
    {
        string GetStatus(string filename);
        DataTable GetNewSyclusIdList(DateTime datetime);
        void SetCreateStatus(string filename);
        void SetFailedStatus(string filename);
        void SetSuccessStatus(string filename);
    }
}
