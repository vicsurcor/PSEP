
using System;
using System.Diagnostics;

namespace pbrowser
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = "calc.exe";
            var arg = "";
            ProcessStartInfo p = new ProcessStartInfo(prog, arg);
            Process process = new Process();
            process.StartInfo.FileName = p.FileName;
            process.Start();
            Console.WriteLine(process.Id);
            Console.WriteLine(process.StartInfo.FileName);
            Console.WriteLine("Desea Cerrar");
            if (Console.ReadLine() == "Si"){
                Console.WriteLine(process.CloseMainWindow());
                process.Kill();
            }
        }
    }
}