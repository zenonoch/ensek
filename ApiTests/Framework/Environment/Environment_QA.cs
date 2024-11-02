using System;
using NUnit.Framework;

namespace Framework.Environment
{
    public class Environment_QA : IEnvironment
    {
        // QA Environment in this demo project uses hard-coded values (never in production!)
        // Each environment (DEV, RC) in this solution is represented by different class
        // and different methods can be used to obtain environment settings, 
        // You may want to switch to DEV environment that reads data from a configurtion file, 
        // Potentially other enviromnets (or all of them) may read config data from Azure AppConfiguration (- an idea for a future extension)
        public EnvironmentId Id => EnvironmentId.QA;
        public Uri BaseUri => new Uri(@"https://qacandidatetest.ensek.io/");
        public string Token => "";
        
    }

}