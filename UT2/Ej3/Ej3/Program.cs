//Modify the next program in order to print the word "Hello". Do Not repeat the letters many times!

using System;
using System.Threading;

namespace ConsoleApp5
{
    class Program
    {
        private static string word = "";
        private static readonly string hello = "HELLO";
        private static readonly object lockObject = new object();

        public static void Main()
        {
            Thread[] threads = new Thread[5];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(new ThreadStart(Write));
                threads[i].Start();
            }
            
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }

            Console.WriteLine("Final result: " + word);
        }

        public static void Write()
        {
            while (true)
            {
                bool lockAcquired = false;
                try
                {
                    lockAcquired = Monitor.TryEnter(lockObject);
                    if (lockAcquired)
                    {
                        if (word.Length >= hello.Length)
                        {
                            break;
                        }
                        word += hello[word.Length];
                        Console.WriteLine("Thread {0} added a letter. Word: {1}", Thread.CurrentThread.ManagedThreadId, word);
                    }
                }
                finally
                {
                    if (lockAcquired)
                    {
                        Monitor.Exit(lockObject);
                    }
                }
            }
        }
    }
}
