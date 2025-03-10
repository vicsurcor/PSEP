using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static string authToken;

    private static async Task Main(string[] args)
    {
        string apiUrlGame = "https://localhost:5001/api/Game";
        string apiUrlUser = "https://localhost:5001/api/User";
        await GetToken(apiUrlUser, "vicsurcor", "12345");
        await RunTwoClientsAsync(apiUrlGame, apiUrlUser, authToken);
        
    }
    private static async Task RunTwoClientsAsync(string gameUrl, string userUrl, string token)
    {
        // Start both tasks simultaneously
        Task task1 = TestGames(gameUrl, authToken);
        Task task2 = TestUsers(userUrl, authToken);

        // Wait for both to complete
        await Task.WhenAll(task1, task2);

        Console.WriteLine("Both clients have finished their tasks.");
    }

    private static async Task GetToken(string apiUrlUser, string username, string password) {
        var loginData = new
        {
            UserName = username,
            Email = "",
            Password = password,
            Role = ""
        };

        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{apiUrlUser}", content);

        if (response.IsSuccessStatusCode)
        {
            // Read the response content
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Deserialize the response to extract the token
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

            // Use the token
            var token = loginResponse?.Token;

            // If the token is null or empty, handle the error
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Error: Token not found in the response.");
            }
            else
            {
                authToken = token;
            }
        }
        else
        {
            Console.WriteLine("Login failed: " + response.ReasonPhrase);
        }
    }
    // Testing game Methods
    private static async Task TestGames(string apiUrlGame, string token) {
        Console.WriteLine("Client 1 started.");
        await GetAllGames(apiUrlGame);
        // Test Add Single Game (POST)
        await AddGame(apiUrlGame, token);
        // Test Get All Games (GET)
        await GetAllGames(apiUrlGame);
        // Test Update Game (PUT)
        await UpdateGame(apiUrlGame, 1, token); // assuming game with id=1 exists
        await GetAllGames(apiUrlGame);
        // Test Delete Game (DELETE)
        await DeleteGame(apiUrlGame, 2, token); // assuming game with id=2 exists
        await GetAllGames(apiUrlGame);
        Console.WriteLine("Client 1 completed.");
    }
    // Testing user Methods
    private static async Task TestUsers(string apiUrlUser, string token) {
        Console.WriteLine("Client 2 started.");
        await RegisterUser(apiUrlUser);
        await GetAllUsers(apiUrlUser, token);
        await UpdateUser(apiUrlUser, "TestUser");
        await GetAllUsers(apiUrlUser, token);
        await DeleteUserAdmin(apiUrlUser, "UpdatedTestUser", token);
        await GetAllUsers(apiUrlUser, token);
        Console.WriteLine("Client 2 completed.");
        
    }
    // Method to POST a single game
    private static async Task AddGame(string apiUrlGame, string token)
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
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
        

        var response = await client.PostAsync($"{apiUrlGame}/add", content);
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
    private static async Task GetAllGames(string apiUrlGame)
    {
        Console.WriteLine(apiUrlGame);
        var response = await client.GetAsync(apiUrlGame);
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
    private static async Task UpdateGame(string apiUrlGame, int gameId, string token)
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
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

        var response = await client.PutAsync($"{apiUrlGame}/{gameId}", content);
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
    private static async Task DeleteGame(string apiUrlGame, int gameId, string token)
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await client.DeleteAsync($"{apiUrlGame}/{gameId}");
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
    // Method to POST a single user
    private static async Task RegisterUser(string apiUrlUser) {
        var newUser = new {
            UserName = "TestUser",
            Email = "TestEmail",
            Password = "TestPassword",
            Role = "Client"
        };

        // Serialize the game object to JSON using Newtonsoft.Json
        string jsonContent = JsonConvert.SerializeObject(newUser);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        

        var response = await client.PostAsync($"{apiUrlUser}/add", content);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("\nUser added successfully.\n");
        }
        else
        {
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("\nError adding user.\n");
        }
    }
    // Method to PUT (Update) a users username and pass
    private static async Task UpdateUser(string apiUrlUser, string username) {
        var updatedUser = new {
            UserName = "UpdatedTestUser",
            Email = "",
            Password = "UpdatedTestPassword",
            Role = ""
        };

        // Serialize the game object to JSON using Newtonsoft.Json
        string jsonContent = JsonConvert.SerializeObject(updatedUser);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        

        var response = await client.PutAsync($"{apiUrlUser}/{username}", content);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("\nUser updated successfully.\n");
        }
        else
        {
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("\nError updating user.\n");
        }
    }

    private static async Task GetAllUsers(string apiUrlUser, string token)
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine(apiUrlUser);
        var response = await client.GetAsync(apiUrlUser + "/get");
        if (response.IsSuccessStatusCode)
        {
            // Deserialize the JSON response into a list of dynamic objects
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<dynamic>>(jsonResponse);
            
            Console.WriteLine("Users:\n");
            foreach (var user in users)
            {
                Console.WriteLine($"- {user.userName}, {user.email}, ${user.password}, Role: {user.role}");
            }
        }
        else
        {
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("\nError retrieving users.\n");
        }
    }
    // Method for an Admin to DELETE a user
    private static async Task DeleteUserAdmin(string apiUrlUser, string username, string token) {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await client.DeleteAsync($"{apiUrlUser}/delete/admin/{username}");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("\nUser deleted successfully.\n");
        }
        else
        {
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("\nError deleting user.\n");
        }
    }
    // Method for a Client to DELETE its user
    private static async Task DeleteUserClient(string apiUrlUser, string username, string password, string token) {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var userPassword = new {
            Password = password,
        };

        string jsonContent = JsonConvert.SerializeObject(userPassword);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{apiUrlUser}/delete/client/{username}", content);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("\nUser deleted successfully.\n");
        }
        else
        {
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("\nError deleting user.\n");
        }
    }
}
public class LoginResponse
{
    public string Message { get; set; }
    public User user{ get; set; }
    public string Token { get; set; }
    
}