using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace MyRsaTools
{
    public class MRT
    {
        public static string ExportPublicParametersToXml(RSACryptoServiceProvider rsa)
        {
            RSAParameters publicParameters = new RSAParameters();
            publicParameters.Exponent = rsa.ExportParameters(false).Exponent;
            publicParameters.Modulus = rsa.ExportParameters(false).Modulus;
            using (StringWriter writer = new Utf8StringWriter())
            {
                XmlSerializer xml = new XmlSerializer(typeof(RSAParameters));
                xml.Serialize(writer, publicParameters);
                return writer.ToString();
            }
        }
        public static RSAParameters PublicParametersFromXml(string data)
        {
            XmlSerializer xml = new XmlSerializer(typeof(RSAParameters));
            object result;
            using (TextReader reader = new StringReader(data))
            {
                result = xml.Deserialize(reader);
            }
            return (RSAParameters)result;
        }

        public static string Encrypt(string text, RSAParameters publicParameters)
        {
            byte[] data = Encoding.Default.GetBytes(text);
            using (RSACryptoServiceProvider tester = new RSACryptoServiceProvider())
            {
                tester.ImportParameters(publicParameters);
                byte[] encrypted = tester.Encrypt(data, false);
                string base64 = Convert.ToBase64String(encrypted, 0, encrypted.Length);
                return base64;
            }
        }

        public static string Decrypt(string code, RSACryptoServiceProvider rsa)
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

        public static bool VerifyData(string text, string signedText, RSAParameters publicParameters)
        {
            byte[] data = Encoding.Default.GetBytes(text);
            byte[] signedData = Convert.FromBase64String(signedText);
            RSACryptoServiceProvider tester = new RSACryptoServiceProvider();
            tester.ImportParameters(publicParameters);
            return tester.VerifyData(data, new SHA1CryptoServiceProvider(), signedData);
        }

        public static void Main()
        {
            string PATH1 = "pub1.xml";

            // the maximum size of data which can be encrypted with RSA is 245 bytes !!!
            string TEXTO = "Mi clave AES super secreta";

            // El usuario 1 genera sus llaves y escribe la clave pública (módulo+exponente) a fichero            
            RSACryptoServiceProvider rsa1 = new RSACryptoServiceProvider(3072);
            string pubXml = MRT.ExportPublicParametersToXml(rsa1);
            System.IO.File.WriteAllText(PATH1, pubXml);
            Console.WriteLine(pubXml);

            //El usuario 2 lee la clave pública de 1 para utilizarla   
            string pubXml1 = System.IO.File.ReadAllText(PATH1);
            RSAParameters pubPars1 = MRT.PublicParametersFromXml(pubXml1);
            //El usuario 2 encripta el TEXTO con la clave pública de 1
            var datosEncriptadosConPub1 = MRT.Encrypt(TEXTO, pubPars1);
            Console.WriteLine(datosEncriptadosConPub1);

            // //El usuario 1 desencripta los DATOS con su clave privada
            var datosDesencriptados = MRT.Decrypt(datosEncriptadosConPub1, rsa1);
            Console.WriteLine(datosDesencriptados);



            //Si el usuario 1 quisiese firmar algo lo haría con su clave privada (sólo él tiene esa clave)
            var signedData = MRT.SignedData(TEXTO, rsa1);
            Console.WriteLine(signedData);

            //Cualquier usuario, como el 2, que dispusiese de la clave pública de 1 podría validar el origen
            var b = MRT.VerifyData(TEXTO, signedData, pubPars1);
            Console.WriteLine(b);

        }

    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}