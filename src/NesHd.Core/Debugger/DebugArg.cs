using System;

namespace NesHd.Core.Debugger
{
    public class DebugArg : EventArgs
    {
        private readonly string _debugLine;
        private readonly DebugStatus _status;

        public DebugArg(string line, DebugStatus status)
        {
            _debugLine = line;
            _status = status;
        }

        public DebugStatus Status
        {
            get { return _status; }
        }

        public string DebugLine
        {
            get { return _debugLine; }
        }
    }
}