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
            Thread t1 = new Thread(new ThreadStart(WriteL));
            Thread t2 = new Thread(new ThreadStart(WriteE));
            //t1.Priority = ThreadPriority.BelowNormal;
            t0.Start();
            t.Start();
            t1.Start();

        }

        public static void WriteO()
        {
            
        }
        public static void WriteH()
        {
            
        }
        public static void WriteL()
        {
            
        }
        public static void WriteE()
        {
            
        }
      
    }
}