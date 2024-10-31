using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
public class Program {

    public static int contador = 0;
    private static readonly object _lockNumber = new object();
    
    public static void Main() {
        for (int rep = 0; rep <= 10000; rep++) {
            Thread t1 = new Thread(add);
            t1.Start();
            Console.WriteLine(contador);
        }
    }
    public static void add() {
        for (int rep = 0; rep <= 100; rep++) {
            lock(_lockNumber) {
                contador++;
            }
            
        }
    }
}