using System;
using NUnit.Framework;

namespace Framework.Environment;

public class Environment : IEnvironment
    {
        // Example of an environment that reads the data from test.runsettings file
        // Run test using: dotnet test --settings test.runsettings
        public EnvironmentId Id => EnvironmentId.QA;


        //public Uri BaseUri => new Uri(@"https://qacandidatetest.ensek.io/");
        public Uri BaseUri => new Uri(TestContext.Parameters["BaseUri"]?? throw new Exception("Could not evaluate BaseUri"));
        
 }

