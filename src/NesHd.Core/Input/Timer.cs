using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NesHd.Core.Input
{
    public class Timer
    {
        private readonly long _frequency;

        public Timer()
        {
            if (!QueryPerformanceFrequency(out _frequency))
            {
                throw new Win32Exception();
            }
        }

        public double GetCurrentTime()
        {
            long num;
            QueryPerformanceCounter(out num);
            return ((num)/((double) _frequency));
        }

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
    }
}