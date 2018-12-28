using System;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Logging
{
    [NonInjectable]
    public interface ILoggable
    {
        ILogger Logger { get; }
    }

    public static class LoggableExtensions
    {
        private static readonly IMemoryCache LoggerCache = new MemoryCache(new MemoryCacheOptions());

        public static string GetLogCategory(this object loggable)
        {
            Ensure.That(loggable, nameof(loggable)).IsNotNull();

            return loggable is IIdentifiable identifiable
                ? string.Join(":", loggable.GetType().FullName, identifiable.Key)
                : loggable.GetType().FullName;
        }

        public static void LogTrace(this ILoggable loggable, string message, params object[] args) =>
            Log(loggable, LogLevel.Trace, message, args);

        public static void LogDebug(this ILoggable loggable, string message, params object[] args) =>
            Log(loggable, LogLevel.Debug, message, args);

        public static void LogInfo(this ILoggable loggable, string message, params object[] args) =>
            Log(loggable, LogLevel.Information, message, args);

        public static void LogWarn(this ILoggable loggable, string message, params object[] args) =>
            Log(loggable, LogLevel.Warning, message, args);

        public static void LogWarn(
            this ILoggable loggable,
            Exception exception,
            string message,
            params object[] args) => Log(loggable, LogLevel.Warning, message, args, exception);

        public static void LogError(this ILoggable loggable, string message, params object[] args) =>
            Log(loggable, LogLevel.Error, message, args);

        public static void LogError(
            this ILoggable loggable,
            Exception exception,
            string message,
            params object[] args) => Log(loggable, LogLevel.Error, message, args, exception);

        public static void LogFatal(this ILoggable loggable, string message, params object[] args) =>
            Log(loggable, LogLevel.Critical, message, args);

        public static void LogFatal(
            this ILoggable loggable,
            Exception exception,
            string message,
            params object[] args) => Log(loggable, LogLevel.Critical, message, args, exception);

        private static void Log(
            ILoggable loggable,
            LogLevel level,
            string message,
            object[] args,
            [CanBeNull] Exception exception = null)
        {
            Ensure.That(loggable, nameof(loggable)).IsNotNull();

            var logger = Optional(loggable.Logger).IfNone(() => GetOrCreateDefaultLogger(loggable));

            if (!logger.IsEnabled(level)) return;

            object Format(object arg)
            {
                if (arg is Func<object> func)
                {
                    arg = func.Invoke();
                }
                else if (arg is IOptional opt)
                {
                    arg = opt.MatchUntyped(identity, () => "(None)");
                }

                switch (arg)
                {
                    case INamed named:
                        return string.Join(":", arg.GetType().Name, named.DisplayName);
                    case IIdentifiable identifiable:
                        return string.Join(":", arg.GetType().Name, identifiable.Key);
                    case Node node:
                        return node.GetPath();
                    case Resource resource:
                        return string.Join(":", arg.GetType().Name, resource.GetKey());
                    case Vector3 v3:
                        return v3.ToFormatString();
                    case Vector2 v2:
                        return v2.ToFormatString();
                    default:
                        return arg;
                }
            }

            var evalArgs = args.Map(Format).ToArray();

            switch (level)
            {
                case LogLevel.Trace:
                    logger.LogTrace(message, evalArgs);
                    break;
                case LogLevel.Debug:
                    logger.LogDebug(message, evalArgs);
                    break;
                case LogLevel.Information:
                    logger.LogInformation(message, evalArgs);
                    break;
                case LogLevel.Warning:
                    logger.LogWarning(exception, message, evalArgs);
                    break;
                case LogLevel.Error:
                    logger.LogError(exception, message, evalArgs);
                    break;
                case LogLevel.Critical:
                    logger.LogCritical(exception, message, evalArgs);
                    break;
            }
        }

        private static ILogger GetOrCreateDefaultLogger(ILoggable loggable)
        {
            var category = loggable.GetLogCategory();

            return LoggerCache.GetOrCreate(category, _ => new PrintLogger(category));
        }
    }
}
