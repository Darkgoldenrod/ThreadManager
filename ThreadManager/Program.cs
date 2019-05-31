using Justsafe.ThreadManager;
using System;
using System.Threading;

namespace ThreadManager
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadUtil.RunTask(() => {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("JoJo：" + i.ToString());
                    Thread.Sleep(1000);
                }
            }, 5000);

            Console.WriteLine("............");
            Console.ReadKey();
        }
    }
}
