using System;
using System.Threading;

namespace DFSyclusStatusLogger.Threading
{
    class ThreadComplex : ThisThread
	{
		private readonly int resumeStep;
		private bool breakOperation;

		/// <summary>
		/// Uses the ProcessingData function of the base class = Sleep for 5 seconds
		/// </summary>
		/// <param name="serviceExecution">The Interface for the service execution</param>
		/// <param name="threadId">The guid of the thread</param>
		/// <param name="threadType">The thread type</param>
		/// <param name="resumeStep">The step to resume</param>
		public ThreadComplex(IServiceExecution serviceExecution, Guid threadId, ThreadType threadType, int resumeStep = 0)
			: base(serviceExecution, threadId, threadType)
		{
			this.resumeStep = resumeStep;
		}

		/// <summary>
		/// Overriding the 5s sleep of the base
		/// </summary>
		protected override void ProcessingData()
		{
			Worker();
		}

		/// <summary>
		/// I use this function to load additional data, which isn't available at a non-resumed run
		/// </summary>
		protected override void ResumeProcessingData()
		{
			Worker();
		}

		/// <summary>
		/// The service execution tells the thread to stop
		/// </summary>
		public override void BreakOperation()
		{
			breakOperation = true;
		}

		private void Worker()
		{
			for (int i = resumeStep; i < 5; i++)
			{
				Thread.Sleep(7500);

				// You can save your current state and resume at this point, when the service is restarted
				if (breakOperation == true)
				{
					return;
				}
			}
		}
	}
}
