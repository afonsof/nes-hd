using System;

namespace NesHd.Core.Debugger
{
    public class Debug
    {
        public static void WriteLine(object sender, string line, DebugStatus status)
        {
            var handler = DebugRised;
            if (handler != null)
            {
                handler(sender, new DebugArg(line, status));
            }
        }

        public static void WriteSeparateLine(object sender, DebugStatus status)
        {
            var handler = DebugRised;
            if (handler != null)
            {
                handler(sender, new DebugArg("==========================", status));
            }
        }

        /// <summary>
        /// Rised when the system write a debug
        /// </summary>
        public static event EventHandler<DebugArg> DebugRised;
    }
}