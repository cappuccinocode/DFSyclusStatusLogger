using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFSyclusStatusLogger
{
    interface IServiceExecution
    {
        void StartServiceExecution();

        void StopServiceExecution();

        void ThreadFinished(Guid threadId);
    }
}
