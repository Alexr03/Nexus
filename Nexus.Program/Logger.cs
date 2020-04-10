using System;
using DSharpPlus;
using Nexus.Utilities;

namespace Nexus
{
    public class Logger
    {
        private readonly string _application;
        private readonly DebugLogger _debugLogger = NexusInformation.DiscordClient.DebugLogger;

        public Logger(string application)
        {
            _application = application;
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
            _debugLogger.LogMessage(logLevel, _application, message, DateTime.Now);
        }

        public void LogException(Exception e)
        {
            _debugLogger.LogMessage(LogLevel.Critical, _application, e.Message, DateTime.Now, e);
        }
    }
}