using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

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
    // AllCaps Required
    public string HashPassword(string password){
        using (SHA512 SHA512 = SHA512.Create()){
            return GetHash(SHA512, password);
        }
        
    }
    // AllCaps Required
    public bool VerifyPassword(string password, string password2){
        using (SHA512 SHA512 = SHA512.Create()){
            return VerifyHash(SHA512, password, password2);
        }
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, Object input)
        {

            // Convert the input string to a byte array and compute the hash.
            // Se puede convertir un objeto que haya sido serializado de la misma forma a partir 
            // de los bytes que se van a enviar por el socket
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes((String)input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
    private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
    {
        // Hash the input.
        var hashOfInput = GetHash(hashAlgorithm, input);

        // Create a StringComparer an compare the hashes.
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        return comparer.Compare(hashOfInput, hash) == 0;
    }
}