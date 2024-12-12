using System;
using System.IO;
using System.Text.Json;

namespace jsonserializer
{

    //https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to
    //https://stackoverflow.com/questions/58139759/how-to-use-class-fields-with-system-text-json-jsonserializer

    public class Message
    {
        public string Txt { get; set; }
        public string Hash { get; set; }
        public DateTime Stamp { get; set; }
        public Message() { }
        public Message(string txt, string hash)
        {
            this.Txt = txt;
            this.Hash = hash;
            Stamp = DateTime.Now;
        }
    }

    public static class Program
    {
        static void Main()
        {
            string fileName = "message.json";

            Program.SerializeItem(fileName); // Serialize an instance of Message.
            Program.DeserializeItem(fileName); // Deserialize the instance.
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        public static void SerializeItem(string fileName)
        //public static async Task SerializeItemAsync(string fileName)
        {
            // Create an instance of message and serialize it.
            Message m = new Message("Hola mundo!", "DAAEF2");

            string jsonString = JsonSerializer.Serialize(m);

            //write string to file
            File.WriteAllText(fileName, jsonString);
            // using (FileStream fs = File.Create(fileName))
            // {
            //     await JsonSerializer.SerializeAsync(fs, weatherForecast);
            // }
        }

        public static void DeserializeItem(string fileName)
        // public static async Task DeserializeItemAsync(string fileName)
        {
            Message m;
            string jsonString = File.ReadAllText(fileName);
            m = JsonSerializer.Deserialize<Message>(jsonString);
            // using (FileStream fs = File.OpenRead(fileName))
            // {
            //     m = await JsonSerializer.DeserializeAsync<Message>(fs);
            // }
            Console.WriteLine(m.Txt);
        }
    }
}