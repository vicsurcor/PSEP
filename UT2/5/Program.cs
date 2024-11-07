using System;
using System.Threading;
using System.Threading.Tasks;

namespace E5 {
    class Program {
        private static Thread[] threads;
        public static int contador;
        private static readonly object lockObject = new object();
        private static string[] names = ["Uno","Dos","Tres","Cuatro","Cinco"];
        static void Main(string[] args) {
            threads = new Thread[5];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(new ThreadStart(Write));
                threads[i].Name = names[i];
                threads[i].Start();
            }
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Join();
            }
        }
        static void Write() {
            while (true) {
                bool lockAcquired = false;
                try
                {
                    lockAcquired = Monitor.TryEnter(lockObject);
                    if (lockAcquired)
                    {
                        contador++;
                        Thread.Sleep(3000);
                        Console.WriteLine("Thread {0} Name: {1} added to the total by 1, new total = {2}", Array.IndexOf(names, Thread.CurrentThread.Name), Thread.CurrentThread.Name, contador);
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