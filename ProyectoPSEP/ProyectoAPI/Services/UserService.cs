using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

public class UserService
{
    public string path = "Datasets/";
    private byte[] key;
    private byte[] iv;
    public List<User> Users { get; private set; } = new List<User>();
    private int _nextUserId;

    public UserService()
    {
        InitAes();
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
            
            foreach (var user in Users)
            {
                
                // Hash password if not already hashed
                if (user.Password != HashPassword(user.Password)){
                    user.Password = HashPassword(user.Password);
                }
                // Encrypt email if not already encrypted
                if (Encoding.ASCII.GetBytes(user.Email) != EncryptEmail(user.Email)){
                    user.Email = Convert.ToBase64String(EncryptEmail(user.Email), 0, EncryptEmail(user.Email).Length);
                }
            }
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
    private void InitAes() {
        using (Aes aes = Aes.Create()) {
            key = aes.Key;
            iv = aes.IV;
        }
    }

    public byte[] EncryptEmail(string email) {
        return EncryptStringToBytes_Aes(email, key, iv);
    }

    public string DecryptEmail(byte[] encryptedEmail) {
        return DecryptStringFromBytes_Aes(encryptedEmail, key, iv);
    }

    static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
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