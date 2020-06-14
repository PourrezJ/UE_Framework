using System;
using System.Linq;

namespace UE_Shared
{
    public enum LogLevel
    {
        Trace,
        Debug,  
        Info,
        Warn,
        Error
    }

    public static class Logger
    {
        public static LogLevel Level { get; } = LogLevel.Trace;

        public static string Prefix { get; } = "UE";

        //public Log(LogLevel minLevel, string prefix = "")
        //{
        //    Level = minLevel;
        //    Prefix = prefix;
        //}

        public static void Trace(string message)
        {
            Log(message, LogLevel.Trace);
        }

        public static void Debug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        public static void Info(string message)
        {
            Log(message, LogLevel.Info);
        }

        public static void Warn(string message)
        {
            Log(message, LogLevel.Warn);
        }

        public static void Error(Exception exception)
        {
            Error(exception, "ERROR");
        }

        public static void Error(Exception exception, string message)
        {
            var output = $"{message}:{Environment.NewLine}{exception.Message}{Environment.NewLine}";
            var ex = exception;

            while (ex.InnerException != null)
            {
                output += $"{ex.InnerException.Message}{Environment.NewLine}";
                ex = ex.InnerException;
            }

            Log($"{output} {exception.TargetSite} in {exception.Source}{Environment.NewLine}{exception.StackTrace}", LogLevel.Error);
        }

        public static void Log(string message, LogLevel level)
        {
            if (Level > level) return;

            var output = $"{DateTime.Now:s} [{level}]";

            if (!string.IsNullOrEmpty(Prefix)) output += $" [{Prefix}]";

            var lines = message?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) ?? new[] { "" };
            message = string.Join(Environment.NewLine, lines.Select(l => $"{output} {l}"));
      
            CitizenFX.Core.Debug.Write($"{message}{Environment.NewLine}");
        }
    }
}