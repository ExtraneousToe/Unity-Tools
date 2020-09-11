using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    /// <summary>
    /// Wrapper class to handle logging, allowing for extension without altering
    /// all of the calls throughout the codebase
    /// </summary>
    public class Logger : BaseSingleton<Logger>
    {
        #region Mono+Callbacks
        protected override void Awake()
        {
            base.Awake();

            Application.logMessageReceived += Application_logMessageReceived;
            Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            Log(type, $"Condition: {condition}\nStackTrace:\n{stackTrace}");
        }

        private void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type)
        {
            Log(type, $"-= Threaded =-\nCondition: {condition}\nStackTrace:\n{stackTrace}");
        }
        #endregion

        /// <summary>
        /// Filter LogTypes
        ///
        /// 0 :: Error
        /// 1 :: Assert
        /// 2 :: Warning
        /// 3 :: Log
        /// 4 :: Exception
        /// </summary>
        public static LogType filterLogType
        {
            get;
            set;
        } = LogType.Error;

        #region Internal Logging
        private static bool IsLogTypeAllowed(LogType logType)
        {
            return logType == LogType.Exception || logType >= filterLogType;
        }

        private static void Log(LogType logType, object message)
        {
            if (!IsLogTypeAllowed(logType)) return;

            Debug.Log(message);
        }

        private static void Log(LogType logType, object message, UnityEngine.Object context)
        {
            if (!IsLogTypeAllowed(logType)) return;

            Debug.Log(message, context);
        }

        private static void Log(LogType logType, string tag, object message)
        {
            if (!IsLogTypeAllowed(logType)) return;

            Debug.Log($"[{tag}] {message}");
        }

        private static void Log(LogType logType, string tag, object message, UnityEngine.Object context)
        {
            if (!IsLogTypeAllowed(logType)) return;

            Debug.Log($"[{tag}] {message}", context);
        }

        private static void LogFormat(LogType logType, string format, params object[] args)
        {
            if (!IsLogTypeAllowed(logType)) return;

            Log(logType, string.Format(format, args));
        }

        private static void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            if (!IsLogTypeAllowed(logType)) return;

            Log(logType, (object)string.Format(format, args), context);
        }
        #endregion

        #region LogLevel = Log
        public static void Log(object message)
        {
            Log(LogType.Log, message);
        }

        public static void Log(string tag, object message)
        {
            Log(LogType.Log, tag, message);
        }

        public static void Log(string tag, object message, UnityEngine.Object context)
        {
            Log(LogType.Log, tag, message, context);
        }

        public static void LogFormat(string format, params object[] args)
        {
            LogFormat(LogType.Log, format, args);
        }

        public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
        {
            LogFormat(LogType.Log, context, format, args);
        }

        public static void LogMethod(string tag, [System.Runtime.CompilerServices.CallerMemberName] string aCallerName = null)
        {
            Log(tag, aCallerName);
        }
        #endregion

        #region LogLevel = Warning
        public static void LogWarning(object message)
        {
            Log(LogType.Warning, message);
        }

        public static void LogWarning(string tag, object message)
        {
            Log(LogType.Warning, tag, message);
        }

        public static void LogWarning(string tag, object message, UnityEngine.Object context)
        {
            Log(LogType.Warning, tag, message, context);
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            LogFormat(LogType.Warning, format, args);
        }

        public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
        {
            LogFormat(LogType.Warning, context, format, args);
        }

        public static void LogWarningMethod(string tag, [System.Runtime.CompilerServices.CallerMemberName] string aCallerName = null)
        {
            LogWarning(tag, aCallerName);
        }
        #endregion

        #region LogLevel = Error
        public static void LogError(object message)
        {
            Log(LogType.Error, message);
        }

        public static void LogError(string tag, object message)
        {
            Log(LogType.Error, tag, message);
        }

        public static void LogError(string tag, object message, UnityEngine.Object context)
        {
            Log(LogType.Error, tag, message, context);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            LogFormat(LogType.Error, format, args);
        }

        public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
        {
            LogFormat(LogType.Error, context, format, args);
        }

        public static void LogErrorMethod(string tag, [System.Runtime.CompilerServices.CallerMemberName] string aCallerName = null)
        {
            LogError(tag, aCallerName);
        }
        #endregion

        #region LogLevel = Exception
        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }

        public static void LogException(Exception exception, UnityEngine.Object context)
        {
            Debug.LogException(exception, context);
        }
        #endregion
    }
}