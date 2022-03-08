using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFSyclusStatusLogger.Threading
{
	class ThreadMulti : ThisThread
	{
		/// <summary>
		/// Uses the ProcessingData function of the base class = Sleep for 5 seconds
		/// </summary>
		/// <param name="serviceExecution">The Interface for the service execution</param>
		/// <param name="threadId">The guid of the thread</param>
		/// <param name="threadType">The thread type</param>
		public ThreadMulti(IServiceExecution serviceExecution, Guid threadId, ThreadType threadType)
			: base(serviceExecution, threadId, threadType)
		{

		}
	}
}
