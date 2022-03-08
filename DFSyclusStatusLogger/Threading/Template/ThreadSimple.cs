using System;

namespace DFSyclusStatusLogger.Threading
{
    class ThreadSimple : ThisThread
	{
		/// <summary>
		/// Uses the ProcessingData function of the base class = Sleep for 5 seconds
		/// </summary>
		/// <param name="serviceExecution">The Interface for the service execution</param>
		/// <param name="threadId">The guid of the thread</param>
		/// <param name="threadType">The thread type</param>
		public ThreadSimple(IServiceExecution serviceExecution, Guid threadId, ThreadType threadType)
			: base(serviceExecution, threadId, threadType)
		{

		}
	}
}
