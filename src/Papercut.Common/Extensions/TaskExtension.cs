namespace Papercut.Common.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskExtension
    {
        /// <summary>
        /// Creates a I/O-bound delay without using of .NET threads.
        /// This is a replacement of <see cref="Thread.Sleep(int)"/>.
        /// </summary>
        /// <param name="delaySpan"></param>
        /// <returns></returns>
        public static Task Delay(TimeSpan delaySpan)
        {
            // From Albahari's book "C# 7.0 in a Nutshell".


            var completion = new TaskCompletionSource<object>();
            var timer = new System.Timers.Timer
            {
                Interval = delaySpan.TotalMilliseconds,
                AutoReset = false
            };

            timer.Elapsed += delegate
            {
                timer.Dispose();
                completion.SetResult(null);
            };

            timer.Start();

            return completion.Task;
        }
    }
}
