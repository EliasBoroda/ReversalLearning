using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    interface IPerfLog
    {
        void Init();
        void LogReversalSummary(string SummeryMetrics);
        void LogRaw(string rawLine);
		void LogBaselineSummary(string BaselineSummeryMetrics);
        void LogScan(string scanLine);
        void Term();
    }

    public class DebugLogger : IPerfLog
    {
        public void LogReversalSummary(string SummeryMetrics)
        {
            Debug.Log(SummeryMetrics);
        }

        public void LogRaw(string rawLine)
        {

        }

        public void LogScan(string scanLine)
        {

        }

        public void LogBaselineSummary(string BaselineSummeryMetrics) 
		{
		
		}

        public void Init()
        {

        }

        public void Term()
        {

        }
    }

    public class GridLogger : IPerfLog
    {
        public void LogReversalSummary(string SummeryMetrics)
        {

        }

        public void LogRaw(string rawLine)
        {

        }

        public void LogScan(string scanLine)
        {

        }

        public void LogBaselineSummary(string BaselineSummeryMetrics)
		{

		}

        public void Init()
        {

        }

        public void Term()
        {

        }
    }

    public class FileSystemLogger : IPerfLog
    {
        private StringBuilder _sbSummary;
        private StringBuilder _sbRaw;
		private StringBuilder _sbBaselineRaw;
		private StringBuilder _sbScan;
		private string _folderPath;

        public FileSystemLogger() //constructor
        {
            _sbRaw = new StringBuilder();
            _sbScan = new StringBuilder();
        }

		public void LogBaselineSummary(string BaselineSummaryMetrics) 
		{
			string _subId = TaskSettingsManager.TaskSettings.SubjectId;
            string _eventId = TaskSettingsManager.TaskSettings.EventId;

            DateTime d = DateTime.Now;
			string dateTime = d.ToString("yyyyMMdd_HHmmss");

			string fileName = Path.Combine(_folderPath, dateTime + "_" + _subId + "_" + _eventId + "_" + "BaselineSummaryLog" + ".txt");
			File.WriteAllText(fileName, BaselineSummaryMetrics);
		}

        public void LogReversalSummary(string SummaryMetrics)
        {
            string _subId = TaskSettingsManager.TaskSettings.SubjectId;
            string _eventId = TaskSettingsManager.TaskSettings.EventId;

            DateTime d = DateTime.Now;
            string dateTime = d.ToString("yyyyMMdd_HHmmss");

			string fileName = Path.Combine(_folderPath, dateTime + "_" + _subId + "_" + _eventId + "_" + "ReversalSummaryLog" + ".txt");
			File.WriteAllText(fileName, SummaryMetrics);
        }

        public void LogRaw(string rawLine)
        {
            _sbRaw.AppendLine(rawLine);
        }

        public void LogScan(string scanLine)
        {
            _sbScan.AppendLine(scanLine);
        }

        public void Init()
        {
            _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\ReversalLearningData\";

            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }
        }

        public void Term()
        {
            string _subId = TaskSettingsManager.TaskSettings.SubjectId;
			string _eventId = TaskSettingsManager.TaskSettings.EventId;

            DateTime d = DateTime.Now;
            string dateTime = d.ToString("yyyyMMdd_HHmmss");

            string fileName = Path.Combine(_folderPath, dateTime + "_" + _subId + "_" + _eventId + "_" + "RawLog" + ".txt");
            File.WriteAllText(fileName, "ReversalIterationCount,attemptCount,correctAttemptCount,incorrectAttemptCount,incorrectAttemptOnReversalCount,responseTime,reversalCount,reversalResponseTime, " +
                "baselineiterationCount,baselineResponseTime,baselineAttemptCount,baselineCorrectAttemptCount,baselineIncorrectAttemptCount" + Environment.NewLine + _sbRaw.ToString());

            string scanFileName = Path.Combine(_folderPath, dateTime + "_" + _subId + "_" + _eventId + "_" + "ScanLog" + ".txt");
            File.WriteAllText(scanFileName, "scanCount , scanTime" + Environment.NewLine + _sbScan.ToString());
        }
    }
}





