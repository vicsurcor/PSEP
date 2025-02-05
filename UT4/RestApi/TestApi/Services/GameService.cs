using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class GameService
{
    public List<Game> Games { get; private set; } = new List<Game>();

    public GameService()
    {
        LoadData();
    }

    private void LoadData()
    {
        string filePath = "test-data.json";
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            Games = JsonConvert.DeserializeObject<List<Game>>(jsonData) ?? new List<Game>();
        }
    }
}
