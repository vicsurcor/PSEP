using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace serialization
{
    [Serializable]
    public class Message : ISerializable
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
        public Message(SerializationInfo info, StreamingContext context)
        {
            Txt = info.GetString("txt");
            Hash = (string)info.GetValue("hash", typeof(string));
            Stamp = (DateTime)info.GetValue("stamp", typeof(DateTime));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("txt", Txt);
            info.AddValue("hash", Hash);
            info.AddValue("stamp", Stamp);
        }
    }

    public static class Program
    {
        static void Main()
        {
            string fileName = "message.data";

            // Use a BinaryFormatter or SoapFormatter.
            IFormatter formatter = new BinaryFormatter();
            //IFormatter formatter = new SoapFormatter();

            Program.SerializeItem(fileName, formatter); // Serialize an instance of the class.
            Program.DeserializeItem(fileName, formatter); // Deserialize the instance.
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        public static void SerializeItem(string fileName, IFormatter formatter)
        {
            // Create an instance of the type and serialize it.
            Message m = new Message("Hola mundo!", "DAAEF2");

            FileStream s = new FileStream(fileName, FileMode.Create);
            formatter.Serialize(s, m);
            s.Close();
        }

        public static void DeserializeItem(string fileName, IFormatter formatter)
        {
            FileStream s = new FileStream(fileName, FileMode.Open);
            Message m = (Message)formatter.Deserialize(s);
            Console.WriteLine(m.Txt);
        }
    }
}