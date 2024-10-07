using System;
using log4net;

namespace BhagirathFincareUtil
{
    public static class LogUtil
    {

        /// <summary>
        /// Method Log error using Log4net
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="callingType">calling type instance</param>
        public static void LogError(this Exception ex, object callingType)
        {
            var log = LogManager.GetLogger(callingType.GetType());
            if (log.IsErrorEnabled)
            {
                log.Error(ex.Message, ex);
            }
            // throw ex;
        }

        /// <summary>
        /// Method Log error using Log4net
        /// </summary>
        /// <param name="ex">Exception</param>
        /// /// <param name="exmessage">For custome message</param>
        /// <param name="callingType">calling type instance</param>
        public static void LogErrorCustomessage(this Exception ex, string exmessage, object callingType)
        {
            var log = LogManager.GetLogger(callingType.GetType());
            if (log.IsErrorEnabled)
            {
                log.Error(ex.Message + exmessage, ex);
            }
            // throw ex;
        }

        /// <summary>
        /// Method Log error using Log4net
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="callingType">calling type</param>
        public static void LogError(this Exception ex, Type callingType)
        {
            var log = LogManager.GetLogger(callingType);
            if (log.IsErrorEnabled)
            {
                log.Error(ex.Message, ex);
            }
            // throw ex;
        }

        /// <summary>
        /// Method Log error using Log4net
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="callingType">calling type instance</param>
        public static void LogErrorWithoutMessageBox(this Exception ex, object callingType)
        {
            var log = LogManager.GetLogger(callingType.GetType());
            if (log.IsErrorEnabled)
            {
                log.Error(ex.Message, ex);

            }
        }

        /// <summary>
        /// Method Log error using Log4net
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="callingType">calling type</param>
        public static void LogErrorWithoutMessageBox(this Exception ex, Type callingType)
        {
            var log = LogManager.GetLogger(callingType);
            if (log.IsErrorEnabled)
            {
                log.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Method log info into DB using Log4net
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="callingType">calling type instance</param>
        public static void LogMessage(this string message, object callingType)
        {
            var log = LogManager.GetLogger(callingType.GetType());
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
        }

        /// <summary>
        /// Method log info into DB using Log4net
        /// </summary>
        /// <param name="message">messahe</param>
        /// <param name="type">calling type instance</param>
        public static void LogMessage(this string message, Type type)
        {
            var log = LogManager.GetLogger(type);
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
        }
    }
}
