using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;  // Add Newtonsoft.Json namespace

public class Program
{
    private static readonly HttpClient client = new HttpClient();

    private static async Task Main(string[] args)
    {
        string apiUrl = "http://localhost:5001/api/Game"; // Update with your API URL

        await GetAllGames(apiUrl);

        // Test Add Single Game (POST)
        await AddGame(apiUrl);

        // Test Get All Games (GET)
        await GetAllGames(apiUrl);

        // Test Update Game (PUT)
        await UpdateGame(apiUrl, 1); // assuming game with id=1 exists

        await GetAllGames(apiUrl);

        // Test Delete Game (DELETE)
        await DeleteGame(apiUrl, 2); // assuming game with id=1 exists

        await GetAllGames(apiUrl);
    }

    // Method to POST a single game
    private static async Task AddGame(string apiUrl)
    {
        var newGame = new
        {
            Name = "Test Game",
            Genre = "Action",
            Price = 59.99,
            Dlcs = new List<DLC> { 
                new DLC { Name = "Dlc 1", Price = 29.99 },
                new DLC { Name = "Dlc 2", Price = 2.99 }
            }
        };

        // Serialize the game object to JSON using Newtonsoft.Json
        string jsonContent = JsonConvert.SerializeObject(newGame);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        

        var response = await client.PostAsync($"{apiUrl}/add", content);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("\nGame added successfully.\n");
        }
        else
        {
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("\nError adding game.\n");
        }
    }

    // Method to GET all games
    private static async Task GetAllGames(string apiUrl)
    {
        var response = await client.GetAsync(apiUrl);
        if (response.IsSuccessStatusCode)
        {
            // Deserialize the JSON response into a list of dynamic objects
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var games = JsonConvert.DeserializeObject<List<dynamic>>(jsonResponse);
            
            Console.WriteLine("Games:\n");
            foreach (var game in games)
            {
                Console.WriteLine($"- {game.name}, {game.genre}, ${game.price}, DLCs: {game.dlcs.Count}");
            }
        }
        else
        {
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("\nError retrieving games.\n");
        }
    }

    // Method to PUT (Update) a game
    private static async Task UpdateGame(string apiUrl, int gameId)
    {
        var updatedGame = new
        {
            Name = "Updated Test Game",
            Genre = "Adventure",
            Price = 49.99,
            Dlcs = new List<DLC> { 
                new DLC { Name = "Dlc 2", Price = 2.99 }
            }
        };

        // Serialize the updated game object to JSON using Newtonsoft.Json
        string jsonContent = JsonConvert.SerializeObject(updatedGame);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"{apiUrl}/{gameId}", content);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("\nGame updated successfully.\n");
        }
        else
        {
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("\nError updating game.\n");
        }
    }

    // Method to DELETE a game
    private static async Task DeleteGame(string apiUrl, int gameId)
    {
        var response = await client.DeleteAsync($"{apiUrl}/{gameId}");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("\nGame deleted successfully.\n");
        }
        else
        {
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("\nError deleting game.\n");
        }
    }
}
public class DLC
{
    public string Name { get; set; }
    public double Price { get; set; }
}
