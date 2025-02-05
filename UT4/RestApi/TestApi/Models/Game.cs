public class Game
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Genre { get; set; }
    public decimal Price { get; set; } // Game price
    public List<DLC> Dlcs { get; set; } = new List<DLC>(); // List of DLCs
}

public class DLC
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
