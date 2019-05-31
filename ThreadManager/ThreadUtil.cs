using System;
using System.Collections.Generic;
using System.Threading;

namespace Justsafe.ThreadManager
{
    public class ThreadUtil
    {
        private static Dictionary<int, Thread> _threads = new Dictionary<int, Thread>();
        private static Dictionary<int, Timer> _timers = new Dictionary<int, Timer>();
        private static Dictionary<int, DateTime> _timeout = new Dictionary<int, DateTime>();
        private static object _lock = new object();

        public static void RunTask(Action action, int timeout = 10 * 60 * 1000)
        {
            lock(_lock)
            {
                //创建线程
                Thread thread = new Thread(() => { action(); });
                _threads.Add(thread.ManagedThreadId, thread);
                thread.IsBackground = true;
                thread.Start();

                //设置线程超时时间
                DateTime time = DateTime.Now.AddMilliseconds(timeout);
                _timeout.Add(thread.ManagedThreadId, time);

                //定时器任务
                Timer timer = new Timer(TimerCallback, thread.ManagedThreadId, 500, 500);
                _timers.Add(thread.ManagedThreadId, timer);
            }
        }

        //计时器回调
        private static void TimerCallback(object threadIdArg)
        {
            lock (_lock)
            {
                int threadId = (int)threadIdArg;

                if (_threads[threadId] != null && !_threads[threadId].IsAlive)
                {
                    //线程执行完成
                    KillTimer(threadId);
                    return;
                }

                if (DateTime.Compare(DateTime.Now, _timeout[threadId]) > 0)
                {
                    //线程超时
                    _threads[threadId].Abort();
                    KillTimer(threadId);
                    return;
                }
            }        
        }

        //结束线程
        private static void KillTimer(int threadIdArg)
        {
            _threads.Remove(threadIdArg);
            _timers[threadIdArg].Dispose();
            _timers.Remove(threadIdArg);
            _timeout.Remove(threadIdArg);               
        }
    }
}
