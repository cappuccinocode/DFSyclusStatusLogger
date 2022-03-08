using DFSyclusStatusLogger.Helper;
using System;
using System.Diagnostics;
using System.Threading;

namespace DFSyclusStatusLogger.Threading
{
    class ThisThread
    {
		protected IServiceExecution serviceExecution;
		protected Guid threadId;
		protected ThreadType threadType;
		protected DateTime startTime;

		public ThisThread(IServiceExecution serviceExecution, Guid threadId, ThreadType threadType)
		{
			this.serviceExecution = serviceExecution;
			this.threadId = threadId;
			this.threadType = threadType;
		}

		/// <summary>
		/// The main function for processing in the thread
		/// </summary>
		/// <param name="resumeProcessing"></param>
		public void Process(bool resumeProcessing = false)
		{
			startTime = DateTime.Now;

			try
			{
				if (resumeProcessing)
				{
					ResumeProcessingData();
				}
				else
				{
					ProcessingData();
				}
			}
			catch (Exception e)
			{
				EventLogger.Entry("Service Error: Thread [" + threadType + "] crashed - " + e, EventLogEntryType.Error);
			}
			finally
			{
				// Make sure, the thread gets closed
				// Add exeption-handling before, if needed!!!
				serviceExecution.ThreadFinished(threadId);
			}
		}

		/// <summary>
		/// Returns the type of the thread
		/// </summary>
		/// <returns></returns>
		public ThreadType GetThreadType()
		{
			return threadType;
		}

		/// <summary>
		/// Returns the start time
		/// </summary>
		/// <returns></returns>
		public DateTime GetStartTime()
		{
			return startTime;
		}

		/// <summary>
		/// Standard implementation for processing data
		/// </summary>
		protected virtual void ProcessingData()
		{
			Thread.Sleep(15000);
		}

		/// <summary>
		/// Only needed in the complex thread
		/// </summary>
		protected virtual void ResumeProcessingData() { }

		/// <summary>
		/// Only needed in the complex thread
		/// </summary>
		public virtual void BreakOperation() { }
	}
}
