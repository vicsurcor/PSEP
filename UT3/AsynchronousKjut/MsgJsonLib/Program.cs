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

    public static class JsonMsgSerializer
    {
        static void Main()
        {
            // string fileName = "messages.json";

            // Program.SerializeItem(fileName); // Serialize an instance of Message.
            // Program.DeserializeItem(fileName); // Deserialize the instance.
            // Console.WriteLine("Done");
            // Console.ReadLine();
        }

        //TODO: Create methods for singular messages

        public static void SerializeList(string fileName, string txt)
        //public static async Task SerializeItemAsync(string fileName)
        {
            // Read existing messages from the file
            List<Message> messages = new List<Message>();
            if (File.Exists(fileName))
            {
                string existingJson = File.ReadAllText(fileName);
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
            File.WriteAllText(fileName, newJson);

            Console.WriteLine("Message saved successfully!");
            // using (FileStream fs = File.Create(fileName))
            // {
            //     await JsonSerializer.SerializeAsync(fs, weatherForecast);
            // }
        }
        public static void SerializeItem(string fileName, string txt)
        //public static async Task SerializeItemAsync(string fileName)
        {
            // Create an instance of message and serialize it.
            Message m = new Message(txt);

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

        public static void DeserializeList(string fileName)
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