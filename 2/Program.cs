using System;
using System.Diagnostics;

namespace pbrowser
{
    class Program
    {
        


        static void Main(string[] args)
        {
            
        }

        void ProcessStart (int num) {
            

            if (num == 1) {
                var prog = "notepad++.exe";
                ProcessStartInfo pInfo = new ProcessStartInfo(prog);
                
            } else if (num == 2) {
                var prog = "calc.exe";
                ProcessStartInfo pInfo = new ProcessStartInfo(prog);
                
            } else if (num == 3) {
                var prog = "calc.exe";
                ProcessStartInfo pInfo = new ProcessStartInfo(prog);
                
            } else if (num == 4) {
                var prog = "calc.exe";
                ProcessStartInfo pInfo = new ProcessStartInfo(prog);
                
            }

        }

        void ProcessKill (int num) {
            if (num == 1) {
                
            } else if (num == 2) {
                
            } else if (num == 3) {
                
            } else if (num == 4) {
                
            }
        }
    }
}
