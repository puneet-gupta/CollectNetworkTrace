using System;
using System.Diagnostics;
using System.Text;

namespace CollectNetworkTrace
{
    public class Utility
    {
        private static readonly StringBuilder _trace = new StringBuilder();
        public static void Trace(string message)
        {
            message = $"{ DateTime.UtcNow } {message}";
            Console.WriteLine(message);
            _trace.AppendLine(message);
        }

        public static void FlushTrace()
        {
            EventLog.WriteEntry("Application", $"CollectNetworkTrace Output \r\n{_trace}");
        }

    }
}
