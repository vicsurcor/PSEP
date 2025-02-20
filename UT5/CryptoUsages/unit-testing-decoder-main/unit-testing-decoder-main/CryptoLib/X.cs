using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CryptoLib
{
    public class X
    {

        public static string RsaGetPubParsXml(RSACryptoServiceProvider rsa)
        {
            bool isPriv = false;
            RSAParameters pars = new RSAParameters();
            pars.Exponent = rsa.ExportParameters(isPriv).Exponent;
            pars.Modulus = rsa.ExportParameters(isPriv).Modulus;
            return RsaParsToXml(pars);
        }
        private static string RsaParsToXml(RSAParameters pars)
        {
            var serializer = new XmlSerializer(typeof(RSAParameters));
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(true),
                Indent = false,
                NewLineHandling = NewLineHandling.None
            };
            using (var stringWriter = new Utf8StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, pars);
                }
                return stringWriter.ToString();
            }
        }
        private static RSAParameters RsaParsFromXml(string data)
        {
            XmlSerializer xml = new XmlSerializer(typeof(RSAParameters));
            object result;
            using (TextReader reader = new StringReader(data))
            {
                result = xml.Deserialize(reader);
            }
            return (RSAParameters)result;
        }

        public static string RsaEncrypt(string text, string pubParsXml)
        {
            byte[] data = Encoding.Default.GetBytes(text);
            using (RSACryptoServiceProvider tester = new RSACryptoServiceProvider())
            {
                tester.ImportParameters(RsaParsFromXml(pubParsXml));
                byte[] encrypted = tester.Encrypt(data, false);
                string base64 = Convert.ToBase64String(encrypted, 0, encrypted.Length);
                return base64;
            }
        }

        public static string RsaDecrypt(string code, RSACryptoServiceProvider rsa)
        {
            byte[] encrypted = System.Convert.FromBase64String(code);
            byte[] decrypted = rsa.Decrypt(encrypted, false);
            string text = Encoding.UTF8.GetString(decrypted);
            return text;
        }
        public static string SignedData(string text, RSACryptoServiceProvider rsa)
        {
            byte[] data = Encoding.Default.GetBytes(text);
            byte[] xdata = rsa.SignData(data, new SHA1CryptoServiceProvider());
            string base64 = Convert.ToBase64String(xdata, 0, xdata.Length);
            return base64;
        }
        public static bool VerifyData(string text, string signedText, string pubParsXml)
        {
            byte[] data = Encoding.Default.GetBytes(text);
            byte[] signedData = Convert.FromBase64String(signedText);
            RSACryptoServiceProvider tester = new RSACryptoServiceProvider();
            tester.ImportParameters(RsaParsFromXml(pubParsXml));
            return tester.VerifyData(data, new SHA1CryptoServiceProvider(), signedData);
        }


        public static string AesEncrypt(string msg, string pwd, out string iv)
        {
            iv = "";
            return null;
        }
        public static string AesDecrypt(string enc, string pwd, string sal)
        {
            return null;
        }

        public static string ShaHash(Object input)
        {
            using (SHA512 SHA512 = SHA512.Create())
            {
                string hash = GetHash(SHA512, input);

                return hash;
            }
            
        }

        public static string RandomString(int length)
        {
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }
            return res.ToString();
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

    }
    

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

    }

    
}
