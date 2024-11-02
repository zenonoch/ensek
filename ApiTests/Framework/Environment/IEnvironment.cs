using System;

namespace Framework.Environment
{
    // EnvironmentId represents a list of available environments. This can be extented with other environments e.g. DEV, RC etc
    public enum EnvironmentId 
        {
            QA,
            DEV
        }
    public interface IEnvironment
    {
        EnvironmentId Id {get;}
        Uri BaseUri { get; }

        string Token {get;}
    }
}