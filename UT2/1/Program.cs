using System;
public class Program {

    public static void Main() {
        for(int i = 0; i<3; i++){
            string l = i.ToString();
            Thread th = new Thread(eat);
            th.Name = l;
            th.Start();
            if (i == 0) {
                th.Join();
            }
        }
        Console.WriteLine("Comido");
        var x = Thread.CurrentThread;
        Console.WriteLine(x.ManagedThreadId);
        Console.WriteLine(x.Name);
    }
    public static void eat() {
        Console.WriteLine("Comiendo... ");
        var x = Thread.CurrentThread;
        Console.WriteLine(x.ManagedThreadId);
        Console.WriteLine(x.Name);
        Thread.Sleep(2000);
    }
}
