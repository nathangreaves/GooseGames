using GooseGames.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogTraceJson(this ILogger logger, string message, object obj)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                var logId = LogIdGlobalFilter.LogId;

                logger.LogTrace($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public static void LogTraceJson(this ILogger logger, string message, Exception e, object obj)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogTrace(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public static void LogDebugJson(this ILogger logger, string message, object obj)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogDebug($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public static void LogDebugJson(this ILogger logger, string message, Exception e, object obj)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogDebug(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }



        public static void LogInformationJson(this ILogger logger, string message, object obj)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogInformation($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public static void LogInformationJson(this ILogger logger, string message, Exception e, object obj)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogInformation(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public static void LogWarningJson(this ILogger logger, string message, object obj)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogWarning($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public static void LogWarningJson(this ILogger logger, string message, Exception e, object obj)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogWarning(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public static void LogErrorJson(this ILogger logger, string message, object obj)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogError($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public static void LogErrorJson(this ILogger logger, string message, Exception e, object obj)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogError(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public static void LogCriticalJson(this ILogger logger, string message, object obj)
        {
            if (logger.IsEnabled(LogLevel.Critical))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogCritical($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public static void LogCriticalJson(this ILogger logger, string message, Exception e, object obj)
        {
            if (logger.IsEnabled(LogLevel.Critical))
            {
                var logId = LogIdGlobalFilter.LogId;
                logger.LogCritical(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }
    }
}
