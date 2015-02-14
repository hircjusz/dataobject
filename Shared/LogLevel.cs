using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareMind.Logger
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
        Empty
    }

    public static class LoggerHelper
    {
        public static string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }
            return stringBuilder.ToString();
        }
    }
}
