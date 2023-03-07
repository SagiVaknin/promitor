using JustEat.StatsD;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Promitor.Core.Metrics.Interfaces;

namespace Promitor.Integrations.Sinks.Statsd.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Use prometheus for writing metrics
        /// </summary>
        public static IServiceCollection AddStatsDSystemMetrics(this IServiceCollection services)
        {
            services.AddTransient<ISystemMetricsSink, StatsdMetricSink>();
            services.AddStatsD(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var sinkLogger = loggerFactory.CreateLogger<StatsdMetricSink>();
                var host = "localhost";
                var port = 8125;
                var metricPrefix = "";

                return new StatsDConfiguration
                {
                    Host = host,
                    Port = port,
                    Prefix = metricPrefix,
                    OnError = ex =>
                    {
                        sinkLogger.LogCritical(ex, "Failed to emit metric to {StatsdHost} on {StatsdPort} with prefix {StatsdPrefix}", host, port, metricPrefix);
                        return true;
                    }
                };
            });

            return services;
        }
    }
}
