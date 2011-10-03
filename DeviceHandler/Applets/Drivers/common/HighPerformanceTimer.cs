using System;
using System.Runtime.InteropServices;

namespace DeviceHandler
{
    public class HiPerformanceTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(
            out long lpFrequency);

        private long startTime;
        private long stopTime;
        private long freq;

        public HiPerformanceTimer()
        {
            startTime = 0;
            stopTime = 0;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                throw new SystemException("High-performance counter not supported");
            }
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            QueryPerformanceCounter(out startTime);
        }

        /// <summary>
        /// Stop the timer
        /// </summary> 
        public void Stop()
        {
            QueryPerformanceCounter(out stopTime);
        }

        /// <summary>
        /// Returns the duration of the timer (in seconds) (ti)
        /// </summary>
        public double Duration
        {
            get
            {
                return (double)(stopTime - startTime) / (double)freq;
            }
        }
    }
}
