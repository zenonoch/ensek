using System;

namespace Framework.Logger
{
    public static class LoggerFactory
    {
        public static ILogger Create(LoggerId loggerType, LogLevel logLevel=LogLevel.Info)
        {
            switch (loggerType)
            {
                case LoggerId.Log4Net:
                    var logger = new Log4NetLogger(logLevel);
                    logger.Initialize();
                    return logger;
                default:
                    throw new Exception($"Logger '{loggerType}' is not supported.");
            }
        }
    }
}