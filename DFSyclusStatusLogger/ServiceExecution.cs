using DFSyclusStatusLogger.Helper;
using DFSyclusStatusLogger.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace DFSyclusStatusLogger
{
    /// <summary>
    /// Used for the state of the ServiceExecution
    /// </summary>
    public enum State
	{
		Running,
		Shutting_Down,
		Stopped
	}

	/// <summary>
	/// I have far more different services, so I need to identicate them clearly
	/// One of the threads in my service is far more complex, than the others
	/// </summary>
	public enum ThreadType
	{
		DMSFolderThread,
		SyclusFolderThread,
	}

	/// <summary>
	/// This is the main thread which handles all the other worker threads
	/// </summary>
	class ServiceExecution : IServiceExecution
	{
		private ServiceExecution() { }
		private static ServiceExecution instance;
		private static readonly object myInstanceLock = new object();

		public IDFSyclusStatusLoggerService myService { get; set; }

		private State currentState = State.Stopped;

		private readonly Dictionary<Guid, ThreadHolder> runningThreads = new Dictionary<Guid, ThreadHolder>();

		private DateTime nextRunDMSFolder = DateTime.Now;
		private DateTime nextRunSyclusFolder = DateTime.Now;

		private bool isProcessingDMSFolder;
		private bool isProcessingSyclusFolder;

		/// <summary>
		/// Create a threadsave singleton instance
		/// </summary>
		/// <returns></returns>
		public static ServiceExecution GetInstance()
		{
			// DoubleLock for thread safety
			if (instance == null)
			{
				lock (myInstanceLock)
				{
					if (instance == null)
					{
						instance = new ServiceExecution();
					}
				}
			}
			return instance;
		}

		/// <summary>
		/// Starts working on the threads
		/// </summary>
		public void StartServiceExecution()
		{
			try
			{
				currentState = State.Running;

				while (currentState == State.Running)
				{
					EventLogger.DebugEntry("Main-Loop", EventLogEntryType.Information);

					CheckForDMSFolderRun();
					
					CheckForSyclusFolderRun();

					// It's not necessary to use timers and/or bools
					// I also have a type which looks at a table in the database and starts all open tasks as seperate thread in one bunch

					// 10 calls per minute should be enough for everyone ;-)
					EventLogger.DebugEntry("Main-Loop - Sleep", EventLogEntryType.Information);
					Thread.Sleep(10000);
				}

				// Here all open threads are closed, this takes as long as the last thread has been broken or has finished
				while (currentState == State.Shutting_Down)
				{
					using (LockHolder<Dictionary<Guid, ThreadHolder>> lockObj =
						new LockHolder<Dictionary<Guid, ThreadHolder>>(runningThreads, 1000))
					{
						if (lockObj.LockSuccessful)
						{
							foreach (ThreadHolder currentThread in runningThreads.Values)
							{
								//// Now break the processing of the complex thread
								//if (currentThread.thisThread.GetThreadType() == ThreadType.ComplexThread)
								//{
								//	currentThread.thisThread.BreakOperation();
								//}
							}

							// If no more threads are left, set the state to stopped
							if (runningThreads.Count == 0)
							{
								currentState = State.Stopped;
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				EventLogger.Entry("Service Error: " + e, EventLogEntryType.Error);
			}
		}

		private void CheckForDMSFolderRun()
		{
			if (isProcessingDMSFolder == false && nextRunDMSFolder <= DateTime.Now)
			{
				EventLogger.DebugEntry("Creating DMS Folder thread", EventLogEntryType.Information);

				const ThreadType threadType = ThreadType.DMSFolderThread;
				ThreadHolder exportThread = new ThreadHolder();
				Guid guid = Guid.NewGuid();
				exportThread.thisThread = new ThreadFolderDMS(this, guid, threadType);

				if (CreateWorkerThread(exportThread, threadType, guid))
				{
					isProcessingDMSFolder = true;
					EventLogger.DebugEntry("Creating DMS Folder thread - successful", EventLogEntryType.Information);
				}
				else
				{
					EventLogger.Entry("Creating DMS Folder thread - failed", EventLogEntryType.Error);
				}
			}
		}

		private void CheckForSyclusFolderRun()
		{
			if (isProcessingSyclusFolder == false && nextRunSyclusFolder <= DateTime.Now)
			{
				EventLogger.DebugEntry("Creating Syclus Folder thread", EventLogEntryType.Information);

				const ThreadType threadType = ThreadType.SyclusFolderThread;
				ThreadHolder exportThread = new ThreadHolder();
				Guid guid = Guid.NewGuid();
				exportThread.thisThread = new ThreadFolderSyclus(this, guid, threadType);

				if (CreateWorkerThread(exportThread, threadType, guid))
				{
					isProcessingSyclusFolder = true;
					EventLogger.DebugEntry("Creating Syclus Folder thread - successful", EventLogEntryType.Information);
				}
				else
				{
					EventLogger.Entry("Creating Syclus Folder thread - failed", EventLogEntryType.Error);
				}
			}
		}

		//private void CheckForSimpleRun()
		//{
		//	if (isProcessingSimple == false && nextRunSimple <= DateTime.Now)
		//	{
		//		EventLogger.DebugEntry("Creating simple thread", EventLogEntryType.Information);

		//		const ThreadType threadType = ThreadType.SimpleThread;
		//		ThreadHolder exportThread = new ThreadHolder();
		//		Guid guid = Guid.NewGuid();
		//		exportThread.thisThread = new ThreadSimple(this, guid, threadType);

		//		if (CreateWorkerThread(exportThread, threadType, guid))
		//		{
		//			isProcessingSimple = true;
		//			EventLogger.DebugEntry("Creating simple thread - successful", EventLogEntryType.Information);
		//		}
		//		else
		//		{
		//			EventLogger.Entry("Creating simple thread - failed", EventLogEntryType.Error);
		//		}
		//	}
		//}

		//private void CheckForComplexRun()
		//{
		//	if (isProcessingComplex == false && nextRunComplex <= DateTime.Now)
		//	{
		//		EventLogger.DebugEntry("Creating complex thread", EventLogEntryType.Information);

		//		const ThreadType threadType = ThreadType.ComplexThread;
		//		ThreadHolder exportThread = new ThreadHolder();
		//		Guid guid = Guid.NewGuid();

		//		exportThread.thisThread = new ThreadComplex(this, guid, threadType);
		//		// Here you can add the resuming, on certain conditions
		//		// In my case I insert data to thisThread which contains all the necessary objects to continue processing from the last halt
		//		// exportThread.thisThread = new MyThreadComplex(this, guid, threadType, 3);

		//		if (CreateWorkerThread(exportThread, threadType, guid))
		//		{
		//			isProcessingComplex = true;
		//			EventLogger.DebugEntry("Creating complex thread - successful", EventLogEntryType.Information);
		//		}
		//		else
		//		{
		//			EventLogger.Entry("Creating complex thread - failed", EventLogEntryType.Error);
		//		}
		//	}
		//}

		//private void CheckForMultipeRun()
		//{
		//	// This thread starts everytime, it is ticked, the other two have to wait until their corresponding thread has finished
		//	if (nextRunMulti <= DateTime.Now)
		//	{
		//		EventLogger.DebugEntry("Creating multi thread", EventLogEntryType.Information);

		//		const ThreadType threadType = ThreadType.MultiThread;
		//		ThreadHolder exportThread = new ThreadHolder();
		//		Guid guid = Guid.NewGuid();
		//		exportThread.thisThread = new ThreadMulti(this, guid, threadType);

		//		if (CreateWorkerThread(exportThread, threadType, guid))
		//		{
		//			EventLogger.DebugEntry("Creating multi thread - successful", EventLogEntryType.Information);
		//		}
		//		else
		//		{
		//			EventLogger.Entry("Creating multi thread - failed", EventLogEntryType.Error);
		//		}

		//		nextRunMulti = DateTime.Now.AddSeconds(int.Parse(ConfigurationManager.AppSettings["TriggerMultiThread"].ToString()));
		//	}
		//}

		/// <summary>
		/// This function creates and starts all kind of threads
		/// </summary>
		/// <param name="exportThread"></param>
		/// <param name="threadType"></param>
		/// <param name="guid"></param>
		/// <returns>True if the thread has been created</returns>
		private bool CreateWorkerThread(ThreadHolder exportThread, ThreadType threadType, Guid guid)
		{
			using (LockHolder<Dictionary<Guid, ThreadHolder>> lockObj =
				new LockHolder<Dictionary<Guid, ThreadHolder>>(runningThreads, 1000))
			{
				if (lockObj.LockSuccessful)
				{
					Thread thread = new Thread(exportThread.Process) { Name = threadType.ToString() };
					exportThread.thread = thread;

					runningThreads.Add(guid, exportThread);

					thread.Start();

					return true;
				}
			}

			return false;
		}

		// Processing finished Threads
		public void ThreadFinished(Guid threadId)
		{
			EventLogger.DebugEntry("Thread closing start", EventLogEntryType.Information);

			using (LockHolder<Dictionary<Guid, ThreadHolder>> lockObj =
				new LockHolder<Dictionary<Guid, ThreadHolder>>(runningThreads, 1000))
			{
				if (lockObj.LockSuccessful)
				{
					if (runningThreads[threadId].thisThread.GetThreadType() == ThreadType.DMSFolderThread)
					{
						// You can add special handling here
						nextRunDMSFolder = DateTime.Now.AddSeconds(int.Parse(ConfigurationManager.AppSettings["TriggerDMSFolderThread"].ToString()));
						isProcessingDMSFolder = false;
					}
					else if (runningThreads[threadId].thisThread.GetThreadType() == ThreadType.SyclusFolderThread)
					{
						nextRunSyclusFolder = DateTime.Now.AddSeconds(int.Parse(ConfigurationManager.AppSettings["TriggerSyclusFolderThread"].ToString()));
						isProcessingSyclusFolder = false;
					}

					runningThreads.Remove(threadId);
				}
			}

			EventLogger.DebugEntry("Thread closing end", EventLogEntryType.Information);
		}

		/// <summary>
		/// Sends the current state to the GUI
		/// </summary>
		/// <returns></returns>
		public State CheckIfActive()
		{
			EventLogger.DebugEntry("wcf check activity", EventLogEntryType.Information);
			return currentState;
		}

		/// <summary>
		/// Sends all active threads to the GUI
		/// I use a concatenated string for a minimum of traffic, depends on your likes
		/// </summary>
		/// <returns></returns>
		public string GetCurrentThreadInfo()
		{
			EventLogger.DebugEntry("wcf ask for threads start", EventLogEntryType.Information);

			using (LockHolder<Dictionary<Guid, ThreadHolder>> lockObj =
				new LockHolder<Dictionary<Guid, ThreadHolder>>(runningThreads, 1000))
			{
				if (lockObj.LockSuccessful)
				{
					int counter = 0;
					string threadInfo = "";

					foreach (KeyValuePair<Guid, ThreadHolder> currentThread in runningThreads)
					{
						DateTime tmpTime = currentThread.Value.thisThread.GetStartTime();

						if (counter == 0)
						{
							threadInfo = String.Format("{0:dd.MM.yyyy HH:mm:ss}", tmpTime) + "§" +
								currentThread.Value.thisThread.GetThreadType().ToString();
						}
						else
						{
							threadInfo = threadInfo + "#" + String.Format("{0:dd.MM.yyyy HH:mm:ss}", tmpTime) + "§" +
								currentThread.Value.thisThread.GetThreadType().ToString();
						}

						counter++;
					}

					EventLogger.DebugEntry("wcf ask for threads: " + threadInfo, EventLogEntryType.Information);

					return threadInfo;
				}
			}

			return "";
		}

		/// <summary>
		/// Advise the service to stop all running threads
		/// </summary>
		public void StopServiceExecution()
		{
			currentState = State.Shutting_Down;
		}
	}
}
