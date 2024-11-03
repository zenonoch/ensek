using System;
using NUnit.Framework;

namespace Framework.Environment;

public class Environment_DEV : IEnvironment
    {
        // Example of an environment that reads the data from test.runsettings file
        // Set EnvironmentId = EnvironmentId.DEV in SysTestController;
        // Run test using: dotnet test --settings test.runsettings
        public EnvironmentId Id => EnvironmentId.DEV;
        public Uri BaseUri => new Uri(TestContext.Parameters["BaseUri"]?? throw new Exception("Could not evaluate BaseUri"));
 }

