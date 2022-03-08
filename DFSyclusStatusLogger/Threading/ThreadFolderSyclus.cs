using DFSyclusStatusLogger.Helper;
using DFSyclusStatusLogger.StatusLoggers;
using System;
using System.Configuration;
using System.Data;

namespace DFSyclusStatusLogger.Threading
{
    class ThreadFolderSyclus : ThisThread
	{
		/// <summary>
		/// Uses the ProcessingData function of the base class = Sleep for 5 seconds
		/// </summary>
		/// <param name="serviceExecution">The Interface for the service execution</param>
		/// <param name="threadId">The guid of the thread</param>
		/// <param name="threadType">The thread type</param>
		public ThreadFolderSyclus(IServiceExecution serviceExecution, Guid threadId, ThreadType threadType)
			: base(serviceExecution, threadId, threadType)
		{
			DateTime startTime = DateTime.Now;

			try
			{
				StatusLogger statusLogger = new StatusLogger();
				string syclusDir = startTime.ToString("yyyyMMdd");
				string syclusFullDirPath = string.Concat(ConfigurationManager.AppSettings["SyclusDirectory"].ToString(), syclusDir);
				string syclusTempFullDirPath = string.Concat(ConfigurationManager.AppSettings["SyclusDirectory"].ToString(), syclusDir, @"\", ConfigurationManager.AppSettings["SyclusTempFolderName"].ToString());
				DataTable dt = statusLogger.GetNewSyclusIdList(startTime);

				foreach (DataRow row in dt.Rows)
				{
					EventLogger.Entry(string.Format("Processing {0} in {1} folder", row["SyclusID"].ToString(), syclusFullDirPath), System.Diagnostics.EventLogEntryType.Information);

					bool fileExistInSyclusFolder = FileLocatorHelper.FileExists(syclusFullDirPath, row["SyclusID"].ToString());
					bool fileExistInSyclusTempFolder = FileLocatorHelper.FileExists(syclusTempFullDirPath, row["SyclusID"].ToString()); ;
					string currentStatus = statusLogger.GetStatus(row["SyclusID"].ToString());

					bool hasElapsed = false;
					DateTime fileStartTime = Convert.ToDateTime(row["CreateDate"].ToString());
					DateTime fileEndTime = DateTime.Now;
					double maxElapseConst = double.Parse(ConfigurationManager.AppSettings["SyclusFileExistElapseTimeLimit"].ToString());

					long elapsedTicks = fileEndTime.Ticks - fileStartTime.Ticks;
					TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
					if (elapsedSpan.TotalSeconds > maxElapseConst)
                    {
						hasElapsed = true;
					}

					if (!hasElapsed)
					{
						if (fileExistInSyclusTempFolder)
						{
							if (currentStatus.ToUpper() != StatusLoggerConstants.SUCCESS)
                            {
								statusLogger.SetSuccessStatus(row["SyclusID"].ToString());
								EventLogger.Entry(string.Format("File {0} status successfully updated to {1}", row["SyclusID"].ToString(), StatusLoggerConstants.SUCCESS), System.Diagnostics.EventLogEntryType.Information);
							}
						}
					}
					else
					{
						if (fileExistInSyclusFolder)
						{
							if (currentStatus.ToUpper() != StatusLoggerConstants.FAILED)
							{
								statusLogger.SetFailedStatus(row["SyclusID"].ToString());
								EventLogger.Entry(string.Format("File {0} status updated to {1}", row["SyclusID"].ToString(), StatusLoggerConstants.FAILED), System.Diagnostics.EventLogEntryType.Information);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				EventLogger.Entry("Failed processing ThreadFolderSyclus thread.", System.Diagnostics.EventLogEntryType.Error, ex);
			}
		}
	}
}
