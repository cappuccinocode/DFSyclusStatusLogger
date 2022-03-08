using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using DFSyclusStatusLogger.Helper;
using DFSyclusStatusLogger.StatusLoggers;

namespace DFSyclusStatusLogger.Threading
{
    class ThreadFolderDMS : ThisThread
	{
		/// <summary>
		/// Uses the ProcessingData function of the base class = Sleep for 5 seconds
		/// </summary>
		/// <param name="serviceExecution">The Interface for the service execution</param>
		/// <param name="threadId">The guid of the thread</param>
		/// <param name="threadType">The thread type</param>
		public ThreadFolderDMS(IServiceExecution serviceExecution, Guid threadId, ThreadType threadType)
			: base(serviceExecution, threadId, threadType)
		{
			DateTime startTime = DateTime.Now;

            try
            {
				StatusLogger statusLogger = new StatusLogger();
				string sourceDir = DateTime.Now.ToString("yyyyMMdd");
				string sourceFullDirPath = string.Concat(ConfigurationManager.AppSettings["SourceDirectory"].ToString(), sourceDir);
				List<FileInfo> files = FileLocatorHelper.GetFiles(sourceFullDirPath);

				foreach (FileInfo file in files)
				{
					string status = statusLogger.GetStatus(file.Name);
					if (status == "")
                    {
						FileLocatorHelper.MoveSourceFileToSyclusDirectory(file);
						statusLogger.SetCreateStatus(file.Name);
						EventLogger.Entry(string.Format("Added {0} status as {1} to database.", file.Name, StatusLoggerConstants.NEW), System.Diagnostics.EventLogEntryType.Information);
					}
				}
			}
            catch (Exception ex)
            {
				EventLogger.Entry("Error occured while running ThreadFolderDMS", System.Diagnostics.EventLogEntryType.Error, ex);
            }
		}
	}
}
