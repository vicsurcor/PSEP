
    public class Game
    {
        public int Id { get; set; } 
        public string Name { get; set; } // Nombre
        public string Genre { get; set; } // Genero
        public decimal Price { get; set; } // Precio del Juego
        public int Stock { get; set; } // Cantidad disponible 
        public List<DLC> Dlcs { get; set; } = new List<DLC>(); // List of DLCs
    }

    public class DLC
    {
        public string Name { get; set; } // Nombre del DLC
        public double Price { get; set; } // Precio del DLC
    }
