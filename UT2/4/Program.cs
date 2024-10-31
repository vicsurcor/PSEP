using System;
using System.Collections.Generic;
using System.Threading;

namespace Filosofos
{
    public static class C
    {
        public const int NUMERO_FILOSOFOS = 5;
        public const int NUMERO_BOCADOS = 5;
        public const char PENSANDO = '_';
        public const char HAMBRIENTO = 'Q';
        public const char COMIENDO = 'X';
        public const char DEJANDO = 'v';
        public const char TERMINADO = 'T';
        public const char PROBLEMA = '!';
        public const int T_MIN = 1000;
        public const int T_MAX = 2000;
    }

    class Program
    {
        static Mesa mesa = new Mesa();
        static List<Thread> hilosCenaFilosofos;

        static void Main(string[] args)
        {
            Console.WriteLine("-> Cena de filósofos.");
            PrepararFilosofosParaCenar();// Crea hilos
            Console.WriteLine("-> Filósofos listos.");
            EmpezarCena();// Inicia hilos
            Console.WriteLine("-> Empieza la cena.");
            EsperarQueTerminen();// Espera hilos
            Console.WriteLine("-> Termina la cena.");
        }

        private static void PrepararFilosofosParaCenar()
        {
            // Crea la lista de filosofos
            var listaFilosofos = new List<Filosofo>(C.NUMERO_FILOSOFOS);
            for (int i = 0; i < C.NUMERO_FILOSOFOS; i++)
            {
                listaFilosofos.Add(new Filosofo(mesa, listaFilosofos, i, C.NUMERO_BOCADOS));
            }
            // Asigna palillos a los filosofos. 
            foreach (var filosofo in listaFilosofos)
            {
                // Si ya hay un filosofo a su lado con palillo, le asigna ese palillo (lados contrarios)
                // Si no hay palillo hay que darle uno nuevo.
                filosofo.PalilloIzquierdo = filosofo.FilosofoIzquierda.PalilloDerecho ?? new Palillo();
                filosofo.PalilloDerecho = filosofo.FilosofoDerecha.PalilloIzquierdo ?? new Palillo();
            }
            // Crea una lista de hilos asociados a los filósofos
            hilosCenaFilosofos = new List<Thread>();
            foreach (var filosofo in listaFilosofos)
            {
                var hilo = new Thread(filosofo.Cenar);
                hilosCenaFilosofos.Add(hilo);
            }
        }

        private static void EmpezarCena()
        {
            foreach (var hilo in hilosCenaFilosofos)
            {
                hilo.Start();
            }
        }

        private static void EsperarQueTerminen()
        {
            foreach (var hilo in hilosCenaFilosofos)
            {
                hilo.Join();
            }
        }
    }

    public class Filosofo
    {
        private Mesa _mesa;
        private readonly List<Filosofo> _listaFilosofos;
        private readonly int _idFilosofo;
        private int _bocados = 0;
        private Random _rnd = new Random();

        public Filosofo(Mesa mesa, List<Filosofo> listaFilosofos, int idFilosofo, int bocados)
        {
            _mesa = mesa;
            _listaFilosofos = listaFilosofos;
            _idFilosofo = idFilosofo;
        }
        public Palillo PalilloIzquierdo { get; set; }
        public Palillo PalilloDerecho { get; set; }

        public Filosofo FilosofoIzquierda
        {
            get
            {
                return _listaFilosofos[(C.NUMERO_FILOSOFOS + _idFilosofo - 1) % C.NUMERO_FILOSOFOS];
            }
        }

        public Filosofo FilosofoDerecha
        {
            get
            {
                return _listaFilosofos[(_idFilosofo + 1) % C.NUMERO_FILOSOFOS];
            }
        }

        public void Cenar()
        {
            Console.WriteLine("Filosofo {0} sentado a la mesa, utiliza palillos {1} y {2}.",
            this._idFilosofo, this.PalilloIzquierdo.Id, this.PalilloDerecho.Id);
            while (_bocados < C.NUMERO_BOCADOS)//(true)
            {
                Thread.Sleep(_rnd.Next(C.T_MIN, C.T_MAX));
                Console.WriteLine("{0} Filosofo {1} tiene hambre.",
                _mesa.Desc(this._idFilosofo, C.HAMBRIENTO), this._idFilosofo);
                // Intenta comer
                while (true)
                {
                    if (Monitor.TryEnter(this.PalilloIzquierdo))
                    {
                        if (Monitor.TryEnter(this.PalilloDerecho))
                        {
                            break;// Adquiere los dos palillos, puede comer.
                        }
                        else
                        {
                            // Console.WriteLine("{0} Filosofo {1} tiene problemas con el palillo derecho.",
                            // _mesa.Desc(this._idFilosofo, C.PROBLEMA), this._idFilosofo);//Muchos logs
                            // Le falta un palillo, libera el otro.
                            Monitor.Exit(this.PalilloIzquierdo);
                            continue;
                        }
                    }
                    else
                    {
                        // Console.WriteLine("{0} Filosofo {1} tiene problemas con el palillo izquierdo.",
                        // _mesa.Desc(this._idFilosofo, C.PROBLEMA), this._idFilosofo);//Muchos logs
                    }
                }
                // Come
                _bocados++;
                Console.WriteLine("{0} Filosofo {1} está comiendo.",
                _mesa.Desc(this._idFilosofo, C.COMIENDO), this._idFilosofo);
                Thread.Sleep(_rnd.Next(C.T_MIN, C.T_MAX));
                // Deja los palillos
                Console.WriteLine("{0} Filosofo {1} deja los palillos.",
                _mesa.Desc(this._idFilosofo, C.DEJANDO), this._idFilosofo);
                Monitor.Exit(this.PalilloIzquierdo);
                Monitor.Exit(this.PalilloDerecho);
                // Piensa
                Console.WriteLine("{0} Filosofo {1} está pensando.",
                _mesa.Desc(this._idFilosofo, C.PENSANDO), this._idFilosofo);
            }
            // Piensa
            Console.WriteLine("{0} Filosofo {1} ha terminado de cenar.",
            _mesa.Desc(this._idFilosofo, C.TERMINADO), this._idFilosofo);
        }

    }

    public class Palillo
    {
        private static int _contador = 0;
        public int Id { get; private set; }

        public Palillo()
        {
            this.Id = _contador++;
        }
    }

    public class Mesa
    {
        private int _tam = C.NUMERO_FILOSOFOS;
        private char[] _a;
        public Mesa()
        {
            _a = new char[_tam * 2];
            for (int i = 0; i < _tam; i++)
            {
                _a[i * 2] = '|';
                _a[i * 2 + 1] = '_';
            }
        }
        public string Desc(int idFilosofo, char queHace)
        {
            _a[idFilosofo * 2 + 1] = queHace;
            return new string(_a);
        }
    }

}