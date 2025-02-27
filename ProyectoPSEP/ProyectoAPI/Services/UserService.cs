using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class UserService
{
    public string path = "Datasets/";
    public List<User> Users { get; private set; } = new List<User>();
    private int _nextUserId;

    public UserService()
    {
        LoadData();
        InitializeIds();
    }

    private void LoadData()
    {
        string filePath = path + "test-data-user.json";
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            Users = JsonConvert.DeserializeObject<List<User>>(jsonData) ?? new List<User>();
        }
    }
     // Initialize the next available UserId
    private void InitializeIds()
    {
        if (Users.Any())
        {
            _nextUserId = Users.Max(u => u.Id) + 1; // Start after the highest User ID
        }
        else
        {
            _nextUserId = 1;
        }
    }
    // Method to get the next User ID
    public int GetNextUserId()
    {
        return _nextUserId++;
    }

    // TODO: Encryption and Hashing Methods
}