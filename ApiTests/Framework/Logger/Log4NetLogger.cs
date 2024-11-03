using System;
using System.IO;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;


namespace Framework.Logger;

    // Logger implemented with Log4Net
    public class Log4NetLogger : ILogger
    {
        const string LogFileName = "automation.log";

        const string LogPattern = "%d{HH:mm:ss} %level: %message%newline";

        private static readonly ILog Log = LogManager.GetLogger("TestAutomationLogger");

        private LogLevel level;
        LogLevel ILogger.Level
        {
            get => level;
        }

        public LoggerId Id => LoggerId.Log4Net;
        public void Debug(string? message)
        {
            Log.Debug(message);
        }

        public void Error(string? message)
        {
            Log.Error(message);
        }

        public void Info(string? message)
        {
            Log.Info(message);
        }

        public void Warning(string? message)
        {
            Log.Warn(message);
        }

        public void Initialize()
        {
            Console.WriteLine(@"=================================================================================================");
            Console.WriteLine($"Initializing Logger with logging level: {level}");
            Console.WriteLine(@"-------------------------------------------------------------------------------------------------");

            Level log4netLevel = convertToLog4NetLevel(level);


            PatternLayout layout = new PatternLayout(LogPattern);
            layout.ActivateOptions();

            var consoleAppender = GetConsoleAppender(layout, log4netLevel);
            BasicConfigurator.Configure(consoleAppender);                

            var logFilePath = Path.Join(Globals.OutputDir, LogFileName);
            var fileAppender = GetFileAppender(logFilePath, layout, log4netLevel);
            BasicConfigurator.Configure(fileAppender);
            ((Hierarchy)LogManager.GetRepository()).Root.Level = log4netLevel;

            Console.WriteLine(@"-------------------------------------------------------------------------------------------------");

        }

        public Log4NetLogger(LogLevel level)
        {
            this.level = level;
        }

        private Level convertToLog4NetLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return Level.Debug;                       
                    
                case LogLevel.Info:
                    return Level.Info;

                case LogLevel.Warning:
                    return Level.Warn;
                     
                case LogLevel.Error:                    
                    return Level.Error;

                default:
                    throw new Exception($"Unsupported LogLevel: {level}");
            }
        }

        private IAppender GetConsoleAppender(PatternLayout layout, Level level)
        {
            var appender = new ConsoleAppender
            {
                Threshold = level,
                Layout = layout
            };
            appender.ActivateOptions();
            return appender;
        }

        private IAppender GetFileAppender(string logFile, PatternLayout layout, Level level)
        {
            
            var appender = new FileAppender
            {
                File = logFile,
                Encoding = Encoding.UTF8,
                Threshold = level,
                Layout = layout
            };

            appender.ActivateOptions();
            return appender;
        }
    }

