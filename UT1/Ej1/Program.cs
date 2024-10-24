/* Using our old programs:

Create a program. You need to use 4 different keys to open four different processes and four to kill them.
Depending on the key the user presses, it will open or kill a specific process.
There user can write arguments. If you do not write anything, it is understood that there are no inputs. 
Programming must be modular: calls to procedures of the main program.
The result is valued, but also the procedure, modularization, code reuse and its optimization. */

using System;
using System.Diagnostics;

namespace Ejercicio1
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Presiona una tecla para abrir (1-4) o cerrar (5-8) un proceso:\n");
                var key = Console.ReadKey().KeyChar;

                if (char.IsDigit(key))
                {
                    int num = int.Parse(key.ToString());
                    if (num >= 1 && num <= 4){

                        ProcessStart(num);

                    }
                    else if (num >= 5 && num <= 8){

                        ProcessKill(num - 4);

                    }
                }
                else {
                    break;
                }
            }
        }

        static void ProcessStart(int num)
        {
            string prog = num switch
            {
                1 => @"C:\Program Files\Notepad++\notepad++.exe",
                2 => @"C:\Program Files\PowerToys\PowerToys.Settings.exe",
                3 => @"C:\Program Files\Microsoft VS Code\Code.exe",
                4 => @"C:\Program Files\Unity Hub\Unity Hub.exe",
                _ => throw new ArgumentException("Número de proceso no válido")
            };


            ProcessStartInfo pInfo = new ProcessStartInfo(prog);
            Process.Start(pInfo);
        }

        static void ProcessKill(int num)
        {
            string procName = num switch
            {
                1 => "notepad++",
                2 => "PowerToys.Settings",
                3 => "Code",
                4 => "Unity Hub",
                _ => throw new ArgumentException("Número de proceso no válido")
            };

            foreach (var process in Process.GetProcessesByName(procName))
            {
                process.Kill();
            }
        }
    }
}
