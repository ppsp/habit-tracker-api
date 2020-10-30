using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace HyperTaskTools
{
    public static class Logger
    {
        private static ILog Log = LogManager.GetLogger(Assembly.GetEntryAssembly(), "Logger");

        /// <summary>
        /// Configure the logger and it will depend of the config file in : Application.StartupPath + @"\log4net.config"
        /// </summary>
        static public void ConfigLogger() 
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var logRepository = LogManager.GetRepository(entryAssembly);
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            if (!logRepository.Configured)
            {
                // log4net not configured
                foreach (log4net.Util.LogLog message in logRepository.ConfigurationMessages)
                {
                    // evaluate configuration message
                }
            }
        }

        static public void Debug(object message)
        {
            if (Log != null)
                Log.Debug(message);
        }

        static public void Debug(object message, Exception exception)
        {
            if (Log != null)
                Log.Debug(message, exception);
        }

        static public void Warn(object message)
        {
            if (Log != null)
                Log.Warn(message);
        }

        static public void Warn(object message, Exception exception)
        {
            if (Log != null)
                Log.Debug(message, exception);
        }

        static public void Info(object message)
        {
            if (Log != null)
                Log.Info(message);
        }

        static public void Info(object message, Exception exception)
        {
            if (Log != null)
                Log.Info(message, exception);
        }

        static public void Error(object message)
        {
            if (Log != null)
                Log.Error(message);
        }

        static public void Error(object message, Exception exception)
        {
            if (Log != null)
                Log.Error(message, exception);
        }
    }
}
