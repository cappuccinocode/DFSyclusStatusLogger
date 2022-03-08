using System;
using System.Configuration;
using System.Diagnostics;

namespace DFSyclusStatusLogger.Helper
{
    public static class EventLogger
    {
		public static EventLog eventLog { get; set; }
		public static log4net.ILog activityTextLog { get; set; }
		public static log4net.ILog debugTextLog { get; set; }

		public static void InitLogger()
		{
			log4net.Config.XmlConfigurator.Configure();
		}

		static EventLogger()
		{
			InitLogger();
		}

		/// <summary>
		/// Only writes to event-log if parameter [enable_debug_log] is set true in settings
		/// </summary>
		/// <param name="entry">String to be written to log</param>
		/// <param name="type">Event type</param>
		public static void DebugEntry(string entry, EventLogEntryType type, Exception ex = null)
		{
			if (bool.Parse(ConfigurationManager.AppSettings["EnableDebugLog"].ToString()))
			{
				if (bool.Parse(ConfigurationManager.AppSettings["LogToEventViewer"].ToString()))
				{
					eventLog.WriteEntry(DateTime.Now + " - " + entry, type);
				}

				debugTextLog.Debug(entry, ex);
			}
		}

		/// <summary>
		/// Always writes entry to event-log
		/// </summary>
		/// <param name="entry">String to be written to log</param>
		/// <param name="type">Event type</param>
		public static void Entry(string entry, EventLogEntryType type, Exception ex = null)
		{
			if (bool.Parse(ConfigurationManager.AppSettings["LogToEventViewer"].ToString()))
			{ 
				eventLog.WriteEntry(DateTime.Now + " - " + entry, type);
			}

			switch (type)
			{
				case EventLogEntryType.Error:
					activityTextLog.Error(entry);
					if (bool.Parse(ConfigurationManager.AppSettings["EnableDebugLog"].ToString()))
					{
						debugTextLog.Error(entry, ex);
					}
					break;
				case EventLogEntryType.Warning:
					activityTextLog.Warn(entry);
					break;
				default:
					activityTextLog.Info(entry);
					break;
			}
		}
	}
}
