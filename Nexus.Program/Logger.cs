using System;
using DSharpPlus;
using Nexus.Utilities;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Nexus
{
    public class Logger
    {
        private readonly string _application;
        
        private readonly Serilog.Core.Logger _logger;
        
        public Logger(string application)
        {
            _application = application;

            var consoleOutputTemplate = $"[{application}" + " {Timestamp:HH:mm:ss.ff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: consoleOutputTemplate)
                .WriteTo.File($"./Logs/{application}/{application}.log", rollingInterval: RollingInterval.Day);

            _logger = loggerConfiguration.CreateLogger();
        }

        public void LogMessage(string message)
        {
            LogMessage(LogLevel.Info, message);
        }
        
        public void LogDebugMessage(string message)
        {
            LogMessage(LogLevel.Debug, message);
        }

        public void LogMessage(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    _logger.Fatal(message);
                    LogReceived?.Invoke(LogEventLevel.Fatal, _logger, message, _application);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(message);
                    LogReceived?.Invoke(LogEventLevel.Debug, _logger, message, _application);
                    break;
                case LogLevel.Info:
                    _logger.Information(message);
                    LogReceived?.Invoke(LogEventLevel.Information, _logger, message, _application);
                    break;
                case LogLevel.Warning:
                    _logger.Warning(message);
                    LogReceived?.Invoke(LogEventLevel.Warning, _logger, message, _application);
                    break;
                case LogLevel.Error:
                    _logger.Error(message);
                    LogReceived?.Invoke(LogEventLevel.Error, _logger, message, _application);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
        
        public void LogMessage(LogEventLevel logLevel, string message)
        {
            _logger.Write(logLevel, message);
            LogReceived?.Invoke(logLevel, _logger, message, _application);
        }
        
        public void LogException(Exception exception)
        {
            _logger.Write(LogEventLevel.Error, exception, exception.Message);
            LogExceptionReceived?.Invoke(LogEventLevel.Error, _logger, exception, _application);
        }
        
        public static event LogRaised LogReceived;

        public static event LogExceptionRaised LogExceptionReceived;
        
        public delegate void LogRaised(LogEventLevel logLevel, Serilog.Core.Logger logger, string message, string application);
        
        public delegate void LogExceptionRaised(LogEventLevel logLevel, Serilog.Core.Logger logger, Exception exception, string application);
    }
}