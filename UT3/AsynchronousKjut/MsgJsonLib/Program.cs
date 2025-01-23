using System;
using System.IO;
using System.Text.Json;
using System.Runtime.Serialization;

namespace MsgJsonLib
{
    //https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to
    //https://stackoverflow.com/questions/58139759/how-to-use-class-fields-with-system-text-json-jsonserializer
    [Serializable]
    public class Message : ISerializable
    {
        public string Txt { get; set; }
        public string Hash { get; set; }
        public DateTime Stamp { get; set; }
        public Message() { }
        public Message(string txt)
        {
            this.Txt = txt;
            Stamp = DateTime.Now;
        }
        public Message(SerializationInfo info, StreamingContext context)
        {
            Txt = info.GetString("txt");
            Stamp = (DateTime)info.GetValue("stamp", typeof(DateTime));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("txt", Txt);
            info.AddValue("stamp", Stamp);
        }
    }

    public static class JsonSerializer
    {
        static void Main()
        {
            // string fileName = "messages.json";

            // Program.SerializeItem(fileName); // Serialize an instance of Message.
            // Program.DeserializeItem(fileName); // Deserialize the instance.
            // Console.WriteLine("Done");
            // Console.ReadLine();
        }

        public static void SerializeItem(string fileName, string txt)
        //public static async Task SerializeItemAsync(string fileName)
        {
            string filePath = "messages.json";

            // Read existing messages from the file
            List<Message> messages = new List<Message>();
            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                messages = JsonSerializer.Deserialize<List<Message>>(existingJson);
            }

            // Create a new message
            Message newMessage = new Message
            {
                Txt = txt,
                Stamp = DateTime.Now
            };

            // Add the new message to the list
            messages.Add(newMessage);

            // Serialize the list back to the JSON file
            string newJson = JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, newJson);

            Console.WriteLine("Message saved successfully!");
            // using (FileStream fs = File.Create(fileName))
            // {
            //     await JsonSerializer.SerializeAsync(fs, weatherForecast);
            // }
        }

        public static void DeserializeItem(string fileName, string txt)
        // public static async Task DeserializeItemAsync(string fileName)
        {
            List<Message> messages;
            string jsonString = File.ReadAllText(fileName);
            messages = JsonSerializer.Deserialize<List<Message>>(jsonString);
            // using (FileStream fs = File.OpenRead(fileName))
            // {
            //     m = await JsonSerializer.DeserializeAsync<Message>(fs);
            // }
            foreach (Message m in messages)
            {
                Console.WriteLine(m.Txt);
            }
            
        }
    }
}