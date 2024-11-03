using System;

namespace Framework.Environment;
// Creates instances of Environments
public static class EnvironmentFactory
    {
        public static IEnvironment Create(EnvironmentId envType)
        {
            switch (envType)
            {
                case EnvironmentId.QA:
                    return new Environment_QA();
                case EnvironmentId.DEV:
                    return new Environment_DEV();
                default:
                    throw new Exception($"Environment '{envType}' is not supported.");
            }
        }
    }
