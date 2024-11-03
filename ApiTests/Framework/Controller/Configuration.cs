namespace Framework.Controller;

// Configuration class is a wrapper of all essential configuration-related objects
public class Configuration
{
    public Logger.LoggerId LoggerId {get;}
    public Logger.LogLevel LogLevel {get;}
    public Environment.EnvironmentId EnvironmentId {get;}

    public Configuration (
        Environment.EnvironmentId environmentId,
        Logger.LoggerId loggerId,
        Logger.LogLevel logLevel)
        {
            EnvironmentId = environmentId;
            LoggerId = loggerId;
            LogLevel = logLevel;
        }
}
