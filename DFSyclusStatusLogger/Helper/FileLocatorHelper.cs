using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFSyclusStatusLogger.Helper
{
    public static class FileLocatorHelper
    {
        public static bool FileExists(string directory, string filename)
        {
            bool exist = false;
            
            try
            {
                if (directory.Length > 0)
                {
                    string backslash = directory.Substring(directory.Length - 1, 1);
                    if (!(backslash == @"\" || backslash == @"/"))
                    {
                        directory = string.Concat(directory, @"\");
                    }
                }

                string filepath = string.Concat(directory, filename);
                exist = File.Exists(filepath);
            }
            catch (Exception ex)
            {
                EventLogger.Entry(string.Format("Failed checking {0} existance in {1}.", filename, directory), System.Diagnostics.EventLogEntryType.Error, ex);
            }

            return exist;
        }

        public static List<FileInfo> GetFiles(string directory)
        {
            List<FileInfo> files = new List<FileInfo>();

            try
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                files = info.GetFiles().Where(f => f.Extension.Equals(ConfigurationManager.AppSettings["SourceFileExtension"].ToString())).OrderBy(p => p.CreationTime).ToList();
            }
            catch (Exception ex)
            {
                EventLogger.Entry(string.Format("Failed getting files from {0} directory.", directory), System.Diagnostics.EventLogEntryType.Error, ex);
            }

            return files;
        }

        public static string CreateSyclusDirectory(FileInfo file)
        {
            string dirName = "";

            try
            {
                string syclusDir = file.CreationTime.ToString("yyyyMMdd");
                dirName = syclusDir;
                string syclusFullDirPath = string.Concat(ConfigurationManager.AppSettings["SyclusDirectory"].ToString(), syclusDir);

                if (!Directory.Exists(syclusFullDirPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(syclusFullDirPath);
                    EventLogger.Entry(string.Format("Directory {0} was created successfully.", syclusFullDirPath), System.Diagnostics.EventLogEntryType.Information);
                }

                string syclusTempFullDirPath = string.Concat(syclusFullDirPath, @"\", ConfigurationManager.AppSettings["SyclusTempFolderName"].ToString());
                if (!Directory.Exists(syclusTempFullDirPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(syclusTempFullDirPath);
                    EventLogger.Entry(string.Format("Directory {0} was created successfully.", syclusTempFullDirPath), System.Diagnostics.EventLogEntryType.Information);
                }
            }
            catch (Exception ex)
            {
                EventLogger.Entry("Failed while creating syclus directory.", System.Diagnostics.EventLogEntryType.Error, ex);
            }

            return dirName;
        }

        public static void MoveSourceFileToSyclusDirectory(FileInfo file)
        {
            try
            {
                string syclusDir = CreateSyclusDirectory(file);
                string syclusFullDirPath = string.Concat(ConfigurationManager.AppSettings["SyclusDirectory"].ToString(), syclusDir);
                string newFilename = string.Concat(syclusFullDirPath, @"\", file.Name);
                File.Move(file.FullName, newFilename);
                Console.WriteLine("{0} was moved to {1}.", file.FullName, syclusFullDirPath);
                EventLogger.Entry(string.Format("{0} was moved to {1}.", file.FullName, syclusFullDirPath), System.Diagnostics.EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                EventLogger.Entry(string.Format("Failed to move {0}", file.FullName), System.Diagnostics.EventLogEntryType.Error, ex);
            }
        }
    }
}
