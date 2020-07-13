using System;
using System.IO;

namespace MJBLogger
{
    partial class MJBLog
    {
        private void CheckDate()
        {
            if (Now > CurDate.Date)
            {
                CurDate = Now;

                if (StoreByDate)
                {
                    fileIndex = 1;
                    SetLogFilePath();
                }

                if (DaysToKeep > 0)
                {
                    CheckForOldDirectories();
                    CheckForOldFiles();
                }
            }
        }

        private void CheckSize()
        {
            if (disabled || !File.Exists(logPath))
            {
                return;
            }

            if (new FileInfo(logPath).Length >= maxFileSize)
            {
                fileIndex++;
                SetLogFilePath();
            }
        }

        private void CheckForOldDirectories()
        {
            string directoryToCheck;
            string[] parts;
            foreach (string directoryName in Directory.GetDirectories(LogDirectory))
            {
                parts = directoryName.Split('\\');
                directoryToCheck = parts[parts.Length - 1];
                CheckDirectory(directoryToCheck);
            }
        }

        private void CheckDirectory(string directoryToCheck)
        {
            try
            {
                if (Convert.ToDateTime(directoryToCheck) < OldestDate)
                {
                    Directory.Delete(Path.Combine(LogDirectory, directoryToCheck), true);
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    Warning($"Artifact found in logging directory: {Path.Combine(LogDirectory, directoryToCheck)}");
                }
                else
                {
                    Exception(ex, $"Could not delete old log folder: {Path.Combine(LogDirectory, directoryToCheck)}");
                }
            }
        }

        private void CheckForOldFiles()
        {
            FileInfo fInfo;
            DateTime LastWriteTime;

            foreach (string filePath in Directory.GetFiles(LogDirectory))
            {
                fInfo = new FileInfo(filePath);
                LastWriteTime = UseUtcTimestamps ? fInfo.LastWriteTimeUtc : fInfo.LastWriteTime;
                if (LastWriteTime < OldestDate)
                {
                    try
                    {
                        if (filePath.Contains(LogName) & string.Equals(Path.GetExtension(filePath), FileExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(filePath);
                            if (File.Exists(filePath))
                            {
                                Warning($"Unable to delete old log file: {filePath}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Exception(ex, $"Unable to delete old log file: {filePath}");
                    }
                }
            }
        }
    }
}
