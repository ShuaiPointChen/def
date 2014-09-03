namespace Photon.LoadBalancing.LoadShedding
{
    using System.Diagnostics;
    using System.Threading;

    public sealed class ThreadSafeStopwatch 
    {
        private readonly Thread worker;
        private readonly EventWaitHandle getWaitHandle = new AutoResetEvent(false);
        private readonly EventWaitHandle workThreadWaitHandle = new AutoResetEvent(false);

        long ticks;
        bool isStopped;
        
        public ThreadSafeStopwatch()
        {
            worker = new Thread(this.WorkThreadFunction);
            worker.Start();
            this.getWaitHandle.WaitOne();
        }

        private void WorkThreadFunction()
        {
            Thread.BeginThreadAffinity();
            while (!isStopped)
            {
                this.workThreadWaitHandle.WaitOne();
                if (!isStopped)
                {
                    ticks = Stopwatch.GetTimestamp();
                }
                this.getWaitHandle.Set();
            }
        }

        public long GetTimestamp()
        {
            WaitHandle.SignalAndWait(this.workThreadWaitHandle, this.getWaitHandle);
            return ticks;
        }

        public void Stop()
        {
            isStopped = true;
            this.workThreadWaitHandle.Set();
        }
    }
}