using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Exam2
{
    public partial class FileWatcherService : ServiceBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private FileSystemWatcher _fileWatcher;

        public FileWatcherService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Ensure the log directory exists
            string logDirectory = @"C:\logs";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Create monitored folders
            string folder1 = @"C:\Folder1";
            string folder2 = @"C:\Folder2";
            Directory.CreateDirectory(folder1);
            Directory.CreateDirectory(folder2);


            // Set up logging for Event Viewer
            if (!EventLog.SourceExists("FileWatcherService"))
            {
                EventLog.CreateEventSource("FileWatcherService", "Application");
            }

            // Start file watching
            StartFileWatcher(folder1, folder2);
            LogEvent("Service started and monitoring folder.");
        }

        protected override void OnStop()
        {
            _fileWatcher.EnableRaisingEvents = false;
            _fileWatcher.Dispose();
            LogEvent("Service stopped.");
        }

        private void StartFileWatcher(string folder1, string folder2)
        {
            _fileWatcher = new FileSystemWatcher
            {
                Path = folder1,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
                Filter = "*.*",
                EnableRaisingEvents = true
            };

            _fileWatcher.Created += (sender, e) => OnFileCreated(e.FullPath, folder2);
        }

        private void OnFileCreated(string sourceFile, string folder2)
        {
            try
            {
                string destinationFile = Path.Combine(folder2, Path.GetFileName(sourceFile));
                File.Move(sourceFile, destinationFile);
                LogEvent($"File {Path.GetFileName(sourceFile)} moved to {folder2}.");
            }
            catch (IOException ioEx)
            {
                LogError($"IOException moving file: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                LogError($"Error moving file: {ex.Message}");
            }
        }

        private void LogEvent(string message)
        {
            logger.Info(message); // Log general information
            EventLog.WriteEntry("FileWatcherService", message, EventLogEntryType.Information);
        }

        private void LogError(string message)
        {
            logger.Error(message); // For error logs
        }
    }
}

