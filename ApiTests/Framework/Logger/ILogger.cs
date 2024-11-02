
namespace Framework.Logger
{
    public enum LoggerId {
        Log4Net
    }
    public enum LogLevel
    {
        Debug,
        Info,        
        Warning,
        Error
    }
    public interface ILogger
    {        
        public LoggerId Id { get; }
        public LogLevel Level { get; }
        public void Info(string? message);
        public void Warning(string? message);
        public void Error(string? message);
        public void Debug(string? message);

        public void Initialize();
    }
}
