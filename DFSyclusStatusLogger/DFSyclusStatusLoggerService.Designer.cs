
namespace DFSyclusStatusLogger
{
    partial class DFSyclusStatusLoggerService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DFSyclusStatusLoggerServiceEventLog = new System.Diagnostics.EventLog();
            //DFSyclusStatusLoggerServiceActivityTextLog = log4net.LogManager.GetLogger(typeof(log4net.Repository.Hierarchy.Logger));
            //DFSyclusStatusLoggerServiceDebugTextLog = log4net.LogManager.GetLogger(typeof(log4net.Repository.Hierarchy.Logger));

            ((System.ComponentModel.ISupportInitialize)(this.DFSyclusStatusLoggerServiceEventLog)).BeginInit();

            components = new System.ComponentModel.Container();
            this.ServiceName = "DFSyclusStatusLoggerService";
            ((System.ComponentModel.ISupportInitialize)(this.DFSyclusStatusLoggerServiceEventLog)).EndInit();
        }

        #endregion

        private System.Diagnostics.EventLog DFSyclusStatusLoggerServiceEventLog;
        //private log4net.ILog DFSyclusStatusLoggerServiceActivityTextLog;
        //private log4net.ILog DFSyclusStatusLoggerServiceDebugTextLog;
    }
}
