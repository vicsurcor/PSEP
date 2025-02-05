using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class GameService
{
    public List<Game> Games { get; private set; } = new List<Game>();
    private int _nextGameId;

    public GameService()
    {
        LoadData();
        InitializeIds();
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
     // Initialize the next available GameId
    private void InitializeIds()
    {
        if (Games.Any())
        {
            _nextGameId = Games.Max(g => g.Id) + 1; // Start after the highest Game ID
        }
        else
        {
            _nextGameId = 1;
        }
    }
    // Method to get the next Game ID
    public int GetNextGameId()
    {
        return _nextGameId++;
    }
}
