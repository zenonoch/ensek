
using Framework.Logger;
using Framework.Environment;
using Framework.Controller;
using EnsekClient;


namespace EnsekTests
{
    public class EnsekTestController : TestController
    {
        // This controller can be extended to read external parameters
        // e.g. from CI pipeline and substitute the hardcoded values

        //These parametes control the test execution
        public static readonly EnvironmentId EnvironmentId = EnvironmentId.QA;
        public static readonly LoggerId LoggerId = LoggerId.Log4Net;

        public static readonly LogLevel LogLevel = LogLevel.Debug;

        public ApiClient ApiClient;
        public EnsekTestController():base(
            new Configuration(
                EnvironmentId,
                LoggerId,
                LogLevel))
        {
            ApiClient = new ApiClient(Environment, Logger, Configuration);
        }
    }
}