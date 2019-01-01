using Microsoft.Extensions.Logging;

namespace Unity
{
    public static class LoggingUnityContainerExtensions
    {
        public static ILoggerFactory AddLogging(this IUnityContainer container)
        {
            var factory = new LoggerFactory();
            container.RegisterInstance<ILoggerFactory>(factory);
            return factory;
        }
    }
}
