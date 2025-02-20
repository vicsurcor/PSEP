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
        private const int KeyLength = 32; // AES-256 requires a 32-byte key
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
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKey(pwd);
                aes.GenerateIV();
                iv = Convert.ToBase64String(aes.IV);

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(msg);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string AesDecrypt(string enc, string pwd, string sal)
        {
            var cipherBytes = Convert.FromBase64String(enc);
            var iv = Convert.FromBase64String(sal);

            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKey(pwd);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
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

        private static byte[] GetKey(string pwd)
        {
            if (pwd.Length < KeyLength)
            {
                // Pad the password with zeroes
                pwd = pwd.PadRight(KeyLength, '0');
            }
            else if (pwd.Length > KeyLength)
            {
                // Truncate the password
                pwd = pwd.Substring(0, KeyLength);
            }

            return Encoding.UTF8.GetBytes(pwd);
        }   
        

    }
    
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

    }

    
}
