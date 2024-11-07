using System.Threading;

namespace CountNumber{

   class Program{
        static int count=0;
        static void Main(string[] args)
        {
            CountANumber();
            CountANumber();
            CountANumber();
            CountANumber();
            CountANumber();
            Console.WriteLine($"Count is {count}");
        }

        public static void CountANumber(){
            for(int i=1;i<=10000000;i++) count++;              
        }
    }
}