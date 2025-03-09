using Google.Cloud.Firestore;

[FirestoreData]
public class Game
{
    [FirestoreProperty]
    public int Id { get; set; }

    [FirestoreProperty]
    public string Name { get; set; } // Nombre

    [FirestoreProperty]
    public string Genre { get; set; } // Genero

    [FirestoreProperty]
    public double Price { get; set; } // Precio del Juego

    [FirestoreProperty]
    public int Stock { get; set; } // Cantidad disponible 

    [FirestoreProperty]
    public List<DLC> Dlcs { get; set; } = new List<DLC>(); // List of DLCs

    // Ensure a parameterless constructor
    public Game() { }
}

[FirestoreData]
public class DLC
{
    [FirestoreProperty]
    public string Name { get; set; } // Nombre del DLC

    [FirestoreProperty]
    public double Price { get; set; } // Precio del DLC

    // Ensure a parameterless 
    public DLC() { }
}
