using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using DFSyclusStatusLogger.Helper;

namespace DFSyclusStatusLogger
{
    public partial class DFSyclusStatusLoggerService : ServiceBase, IDFSyclusStatusLoggerService
    {
        private Thread thread { get; set; }

        private List<CustomFolderSettings> listFolders;
        private List<FileSystemWatcher> listFileSystemWatcher;

        /// <summary>
		/// Hence OnStop() is derived from ServiceBase, this helper is needed to know when it's time also to stop the WCFProvider 
		/// </summary>
		private bool finalExit { get; set; }

        public DFSyclusStatusLoggerService()
        {
            try
            {
                InitializeComponent();

                finalExit = true;

                if (!EventLog.SourceExists("DFSyclusStatusLoggerServiceEvent"))
                {
                    EventLog.CreateEventSource("DFSyclusStatusLoggerServiceEvent", "DFSyclusStatusLoggerServiceEventLog");
                }

                DFSyclusStatusLoggerServiceEventLog.Source = "DFSyclusStatusLoggerServiceEvent";
                DFSyclusStatusLoggerServiceEventLog.Log = "DFSyclusStatusLoggerServiceEventLog";

                ServiceExecution.GetInstance().myService = this;

                EventLogger.InitLogger();
                EventLogger.eventLog = DFSyclusStatusLoggerServiceEventLog;
                EventLogger.activityTextLog = log4net.LogManager.GetLogger("ActivityLogger");
                EventLogger.debugTextLog = log4net.LogManager.GetLogger("DebugLogger");

                ////////wcfProvider = new WCFProvider();
            }
            catch (Exception e)
            {
                EventLogger.Entry("Service Error: Init - " + e, EventLogEntryType.Error);
            }
        }

        ///// <summary>Event automatically fired when the service is started by Windows</summary>
        ///// <param name="args">array of arguments</param>
        //protected override void OnStart(string[] args)
        //{
        //    // Initialize the list of FileSystemWatchers based on the XML configuration file
        //    PopulateListFileSystemWatchers();
        //    // Start the file system watcher for each of the file specification
        //    // and folders found on the List<>
        //    StartFileSystemWatcher();
        //}

        ///// <summary>Event automatically fired when the service is stopped by Windows</summary>
        //protected override void OnStop()
        //{
        //    if (listFileSystemWatcher != null)
        //    {
        //        foreach (FileSystemWatcher fsw in listFileSystemWatcher)
        //        {
        //            // Stop listening
        //            fsw.EnableRaisingEvents = false;
        //            // Dispose the Object
        //            fsw.Dispose();
        //        }
        //        // Clean the list
        //        listFileSystemWatcher.Clear();
        //    }
        //}

        /// <summary>
		/// Starts the service execution in a new thread
		/// </summary>
		/// <param name="args"></param>
		protected override void OnStart(string[] args)
        {
            try
            {
                thread = new Thread(ServiceExecution.GetInstance().StartServiceExecution)
                {
                    Name = "Service Executer"
                };
                thread.Start();
                EventLogger.Entry("DFSyclusStatusLogger service started", EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                EventLogger.Entry("DFSyclusStatusLogger service Error: Starting - " + e, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Stops the service execution, but the service itself keeps running
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                ServiceExecution.GetInstance().StopServiceExecution();
                thread.Join();
                EventLogger.Entry("Service stopped", EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                EventLogger.Entry("Service Error: Stopping - " + e, EventLogEntryType.Error);
            }

            //if (finalExit)
            //{
            //    wcfProvider.StopProvidingService();
            //}
            //else
            //{
            //    finalExit = true;
            //}

            finalExit = true;
        }

        public void StartService()
        {
            OnStart(null);
        }

        public void StopService()
        {
            finalExit = false;
            OnStop();
        }




        ///// <summary>Reads an XML file and populates a list of <CustomFolderSettings> </summary>
        //private void PopulateListFileSystemWatchers()
        //{
        //    // Get the XML file name from the App.config file
        //    string fileNameXML = ConfigurationManager.AppSettings["XMLFileFolderSettings"];
        //    // Create an instance of XMLSerializer
        //    XmlSerializer deserializer = new XmlSerializer(typeof(List<CustomFolderSettings>));
        //    TextReader reader = new StreamReader(fileNameXML);
        //    object obj = deserializer.Deserialize(reader);
        //    // Close the TextReader object
        //    reader.Close();
        //    // Obtain a list of CustomFolderSettings from XML Input data
        //    listFolders = obj as List<CustomFolderSettings>;
        //}

        ///// <summary>Start the file system watcher for each of the file
        ///// specification and folders found on the List<>/// </summary>
        //private void StartFileSystemWatcher()
        //{
        //    // Creates a new instance of the list
        //    this.listFileSystemWatcher = new List<FileSystemWatcher>();
        //    // Loop the list to process each of the folder specifications found
        //    foreach (CustomFolderSettings customFolder in listFolders)
        //    {
        //        DirectoryInfo dir = new DirectoryInfo(customFolder.FolderPath);
        //        // Checks whether the folder is enabled and
        //        // also the directory is a valid location
        //        if (customFolder.FolderEnabled && dir.Exists)
        //        {
        //            // Creates a new instance of FileSystemWatcher
        //            FileSystemWatcher fileSWatch = new FileSystemWatcher();
        //            // Sets the filter
        //            fileSWatch.Filter = customFolder.FolderFilter;
        //            // Sets the folder location
        //            fileSWatch.Path = customFolder.FolderPath;
        //            // Sets the action to be executed
        //            StringBuilder actionToExecute = new StringBuilder(
        //              customFolder.ExecutableFile);
        //            // List of arguments
        //            StringBuilder actionArguments = new StringBuilder(
        //              customFolder.ExecutableArguments);
        //            // Subscribe to notify filters
        //            fileSWatch.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName |
        //              NotifyFilters.DirectoryName;
        //            // Associate the event that will be triggered when a new file
        //            // is added to the monitored folder, using a lambda expression                   
        //            fileSWatch.Created += (senderObj, fileSysArgs) =>
        //              fileSWatch_Created(senderObj, fileSysArgs,
        //               actionToExecute.ToString(), actionArguments.ToString());
        //            // Begin watching
        //            fileSWatch.EnableRaisingEvents = true;
        //            // Add the systemWatcher to the list
        //            listFileSystemWatcher.Add(fileSWatch);
        //            // Record a log entry into Windows Event Log
        //            CustomLogEvent(String.Format(
        //              "Starting to monitor files with extension ({0}) in the folder ({1})",
        //              fileSWatch.Filter, fileSWatch.Path));
        //        }
        //    }
        //}

        ///// <summary>This event is triggered when a file with the specified
        ///// extension is created on the monitored folder</summary>
        ///// <param name="sender">Object raising the event</param>
        ///// <param name="e">List of arguments - FileSystemEventArgs</param>
        ///// <param name="action_Exec">The action to be executed upon detecting a change in the File system</param>
        ///// <param name="action_Args">arguments to be passed to the executable (action)</param>
        //void fileSWatch_Created(object sender, FileSystemEventArgs e,
        //  string action_Exec, string action_Args)
        //{
        //    string fileName = e.FullPath;
        //    // Adds the file name to the arguments. The filename will be placed in lieu of {0}
        //    string newStr = string.Format(action_Args, fileName);
        //    // Executes the command from the DOS window
        //    ExecuteCommandLineProcess(action_Exec, newStr);
        //}

        ///// <summary>Executes a set of instructions through the command window</summary>
        ///// <param name="executableFile">Name of the executable file or program</param>
        ///// <param name="argumentList">List of arguments</param>
        //private void ExecuteCommandLineProcess(string executableFile, string argumentList)
        //{
        //    // Use ProcessStartInfo class
        //    ProcessStartInfo startInfo = new ProcessStartInfo();
        //    startInfo.CreateNoWindow = true;
        //    startInfo.UseShellExecute = false;
        //    startInfo.FileName = executableFile;
        //    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //    startInfo.Arguments = argumentList;
        //    try
        //    {
        //        // Start the process with the info specified
        //        // Call WaitForExit and then the using-statement will close
        //        using (Process exeProcess = Process.Start(startInfo))
        //        {
        //            exeProcess.WaitForExit();
        //            // Register a log of the successful operation
        //            CustomLogEvent(string.Format(
        //              "Succesful operation --> Executable: {0} --> Arguments: {1}",
        //              executableFile, argumentList));
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        // Register a Log of the Exception
        //    }
        //}

        //private void CustomLogEvent(string value)
        //{ 
        
        //}
    }
}
