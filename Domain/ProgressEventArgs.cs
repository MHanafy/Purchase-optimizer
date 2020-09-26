using System;
using System.Diagnostics;

namespace Gluh.TechnicalTest.Domain
{
    [DebuggerStepThrough()]
    public class ProgressEventArgs : EventArgs
    {
        public readonly long Current;
        public readonly long Total;
        public readonly bool Done;
        public readonly string Activity;
        public float Percentage => (float)Current / Total;
        public ProgressEventArgs(bool done, long current=1, long total=1, string activity = null)
        {
            Current = current;
            Total = total;
            Done = done;
            Activity = activity;
        }
    }
}
