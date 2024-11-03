using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Framework.Logger;
using Framework.Environment;
using System.Diagnostics;

namespace Framework.Controller;

// Test controller create instances of all objects requiered to execute automated tests
public abstract class TestController
{
    protected IEnvironment Environment {get;}
    protected ILogger Logger {get;}
    protected Configuration Configuration {get;}

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {

    }
    [SetUp]
    public void SetUp()
    {
        LogTestSetUp();
        
    }

    [TearDown]
    public void TearDown()
    {
        LogTestTearDown(TestContext.CurrentContext);
    }


    private void LogTestSetUp()
    {
        Logger.Info("--------------------------------------------------------------------------------");
        Logger.Info("Running test: " + TestContext.CurrentContext.Test.MethodName);
        Logger.Info($"Test started at: {DateTime.Now:U}");
        Logger.Info($"Test RunId is: {Globals.TestExecutionTimeStamp}");
        Logger.Info("--------------------------------------------------------------------------------");
    }

    private void LogTestTearDown(TestContext testContext)
    {
        if (testContext.Result.Outcome.Status == TestStatus.Failed)
            {
                var testName = testContext.Test.FullName;
                Logger.Error("===============================================================================");
                Logger.Error("This test failed: " + testName);
                Logger.Error("===============================================================================");
                Logger.Error($"Test result - {testContext.Result.Outcome}");
                Logger.Error("===============================================================================");
            }
            else
            {
                Logger.Info("===============================================================================");
                Logger.Info($"Test result: {testContext.Result.Outcome}");
                Logger.Info("===============================================================================");
            }
        Logger.Info("--------------------------------------------------------------------------------");
        Logger.Info($"Test finished at: {DateTime.Now:U}");
        Logger.Info("--------------------------------------------------------------------------------");
    }

    public TestController(Configuration configuration)
    {
        Logger = LoggerFactory.Create(configuration.LoggerId, configuration.LogLevel);
        Environment = EnvironmentFactory.Create(configuration.EnvironmentId);
        Configuration = configuration;
    }
}