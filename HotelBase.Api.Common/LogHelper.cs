using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using System.ComponentModel;

namespace HotelBase.Api.Common
{
    /// <summary>
    /// NLog日志框架辅助类。
    /// </summary>
    public static class LogHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 注册日志配置文件。
        /// </summary>
        public static void RegisterConfig()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + @"\Configs\NLog.config";
            LogManager.Configuration = new XmlLoggingConfiguration(configPath);
        }

        /// <summary>
        /// 记录日志。
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="operation">动作</param>
        /// <param name="message">消息</param>
        public static void Write(Level level, string operation, string message)
        {
            LogEventInfo logEvent = new LogEventInfo();
            logEvent.Message = message;
            switch (level)
            {
                case Level.Trace:
                    logEvent.Level = LogLevel.Trace;
                    break;
                case Level.Debug:
                    logEvent.Level = LogLevel.Debug;
                    break;
                case Level.Info:
                    logEvent.Level = LogLevel.Info;
                    break;
                case Level.Warn:
                    logEvent.Level = LogLevel.Warn;
                    break;
                case Level.Error:
                    logEvent.Level = LogLevel.Error;
                    break;
                case Level.Fatal:
                    logEvent.Level = LogLevel.Fatal;
                    break;
            }
            //logEvent.Properties["Account"] = 0;
            //logEvent.Properties["RealName"] = "";
            logEvent.Properties["Operation"] = operation;
            //logEvent.Properties["IP"] = IPHelper.Ip;
            //logEvent.Properties["Browser"] = IPHelper.Browser;
            logger.Log(logEvent);
        }

        /// <summary>
        /// 最常见的记录信息，一般用于普通输出。
        /// </summary>
        /// <param name="message"></param>
        public static void Trace(string message)
        {
            logger.Trace(message);
        }

        /// <summary>
        /// 同样是记录信息，不过出现的频率要比Trace少一些，一般用来调试程序。
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            logger.Debug(message);
        }

        /// <summary>
        /// 信息类型的消息。
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message, string operation = "日常记录")
        {
            Write(Level.Info, operation, message);
        }

        /// <summary>
        /// 警告信息，一般用于比较重要的场合。
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(string message, string operation = "警告信息")
        {
            Write(Level.Warn, operation, message);
        }

        /// <summary>
        /// 错误信息。
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            Write(Level.Error, "程序异常", message);
        }
        /// <summary>
        /// 错误信息。
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message, Exception ex = null, string operation = "程序异常")
        {
            var log = $"{message},异常堆栈：{ex?.ToString()}";
            Write(Level.Error, operation, log);
        }

        /// <summary>
        /// 致命异常信息。一般来讲，发生致命异常之后程序将无法继续执行。
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(string message)
        {
            logger.Fatal(message);
        }
    }

    public enum Level
    {
        [Description("普通输出")]
        Trace,
        [Description("一般调试")]
        Debug,
        [Description("普通消息")]
        Info,
        [Description("警告信息")]
        Warn,
        [Description("一般错误")]
        Error,
        [Description("致命错误")]
        Fatal
    }

}
