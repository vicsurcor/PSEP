using System.Threading;

namespace CountNumber{

   class Program{
        private static Thread[] threads;
        static int count = 0;
        private static readonly object lockObject = new object();
        static void Main(string[] args)
        {
            threads = new Thread[5];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(new ThreadStart(CountANumber));
                threads[i].Start();
            }
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }
        }

        public static void CountANumber(){
            while (count < 50000000) {
                bool lockAcquired = false;
                try
                {
                    lockAcquired = Monitor.TryEnter(lockObject);
                    if (lockAcquired)
                    {
                        for(int i=0;i<=9999999;i++) count++;
                    }
                }
                finally
                {
                    if (lockAcquired)
                    {
                        Console.WriteLine("Thread {0} added new total = {1}",Thread.CurrentThread.ManagedThreadId, count);
                        Monitor.Exit(lockObject);
                    }
                }
            }
                          
        }
    }
}