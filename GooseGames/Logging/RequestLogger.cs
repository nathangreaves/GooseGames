using GooseGames.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GooseGames.Logging
{
    public class RequestLogger<T>
    {
        private ILogger<T> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestLogger(ILogger<T> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceIdentifier()
        {
            return _httpContextAccessor.HttpContext.TraceIdentifier;
        }

        public void LogTrace(string message, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace))
            {
                var logId = GetTraceIdentifier();

                _logger.LogTrace($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public void LogTrace(string message, Exception e, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Trace))
            {
                var logId = GetTraceIdentifier();
                _logger.LogTrace(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public void LogDebug(string message, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                var logId = GetTraceIdentifier();
                _logger.LogDebug($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public void LogDebug(string message, Exception e, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                var logId = GetTraceIdentifier();
                _logger.LogDebug(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }



        public void LogInformation(string message, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
            {
                var logId = GetTraceIdentifier();
                _logger.LogInformation($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public void LogInformation(string message, Exception e, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
            {
                var logId = GetTraceIdentifier();
                _logger.LogInformation(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public void LogWarning(string message, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning))
            {
                var logId = GetTraceIdentifier();
                _logger.LogWarning($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public void LogWarning(string message, Exception e, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning))
            {
                var logId = GetTraceIdentifier();
                _logger.LogWarning(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public void LogError(string message, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error))
            {
                var logId = GetTraceIdentifier();
                _logger.LogError($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public void LogError(string message, Exception e, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error))
            {
                var logId = GetTraceIdentifier();
                _logger.LogError(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public void LogCritical(string message, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical))
            {
                var logId = GetTraceIdentifier();
                _logger.LogCritical($"{logId} :: {message} :: {obj.ToJson()}");
            }
        }

        public void LogCritical(string message, Exception e, object obj = null)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Critical))
            {
                var logId = GetTraceIdentifier();
                _logger.LogCritical(e, $"{logId} :: {message} :: {obj.ToJson()}");
            }
        }
    }
}
