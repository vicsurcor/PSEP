﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace resumen
{


    public class Program
    {
        public static void Main()
        {
            string source = "Hello World!";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string hash = GetHash(sha256Hash, source);

                Console.WriteLine($"The SHA256 hash of {source} is: {hash}.");

                Console.WriteLine("Verifying the hash...");

                if (VerifyHash(sha256Hash, source, hash))
                {
                    Console.WriteLine("The hashes are the same.");
                }
                else
                {
                    Console.WriteLine("The hashes are not same.");
                }
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
}