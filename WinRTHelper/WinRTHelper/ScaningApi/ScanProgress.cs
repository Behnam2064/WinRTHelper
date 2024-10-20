using System;

namespace WinRTHelper.ScaningApi
{
    public class ScanProgress<TP> : IProgress<TP> where TP : struct
    {
        public EventHandler OnReport;
        public TP Progress { get; protected set; }

        public void Report(TP value)
        {
            this.Progress = value;
            OnReport?.Invoke(this, EventArgs.Empty);
        }
    }
}
