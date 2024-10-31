//Modify the next program in order to print the word "Hello". Do Not repeat the letters many times!
 
using System;
using System.Threading;
namespace ConsoleApp5
{
    class Program
    {
       
        public static void Main()
        {
            //Thread.CurrentThread.Priority = ThreadPriority.Highest;

            Thread t0 = new Thread(new ThreadStart(WriteO));
            //t0.Priority = ThreadPriority.AboveNormal
            Thread t = new Thread(new ThreadStart(WriteH));
            //t.Priority = ThreadPriority.Normal
            Thread t1 = new Thread(new ThreadStart(WriteA));
            //t1.Priority = ThreadPriority.BelowNormal;
            t0.Start();
            t.Start();
            t1.Start();

            t0.Join();
            t.Join();
            t1.Join();

        }

        public static void WriteO()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("O");
               
            }
        }
        public static void WriteH()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("H");
 
            }
        }
      
    }
}