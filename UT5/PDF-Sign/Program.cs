using System;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace signpdf
{
    // PDF DIGITAL SIGNATURES WITH ITEXT7, BOUNCY CASTLE AND .NET CORE
    //
    // Posted on December 24, 2019 by AZMAT ULLAH KHAN
    //
    // https://viewbag.wordpress.com/2019/12/24/pdf-digital-signatures-itext7-bouncy-castle-net-core/
    //
    // https://certificatetools.com/
    //
    // dotnet add package itext7
    //
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Sign pdf");

            // By having bouncy castle references in our code we can call the following methods 
            // to read the Keystore (PFX/P12 file) and extract the private key and chain of the certificate. 
            // Every P12 file is protected with a password hence password is needed to open the store access the keys. 
            // This password is usually known the end users only. Which means in real application, we will need to ask 
            // for this password from the end user.            
            string KEYSTORE = "cert.pfx";
            char[] PASSWORD = "".ToCharArray();
            Pkcs12Store pk12 = new Pkcs12Store(new FileStream(KEYSTORE, FileMode.Open, FileAccess.Read), PASSWORD);
            string alias = null;
            foreach (object a in pk12.Aliases)
            {
                alias = ((string)a);
                if (pk12.IsKeyEntry(alias))
                {
                    break;
                }
            }
            // pk variable now have a private key extracted from the certificate that was found in the store.
            ICipherParameters pk = pk12.GetKey(alias).Key;

            // Certificates are usually divided (but not limited to) in to three types i.e., 
            // root certificates, intermediate certificates and end entity certificates. 
            // Human identities and certificates are usually the end entity certificates 
            // which are issued by either an intermediate or a root CA. Therefore we need 
            // to maintain the chain of certificates while adding this identity into the PDF 
            // so that later while verifying the verification server can verify 
            // the certificates of end entity along with all the intermediate and root CA certificates.
            X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
            X509Certificate[] chain = new X509Certificate[ce.Length];
            for (int k = 0; k < ce.Length; ++k)
            {
                chain[k] = ce[k].Certificate;
            }

            // Now that we have the chain and the private key to sign the PDF file with, lets read the PDF file
            string DEST = "signed-contract.pdf";
            string SRC = "sample-contract.pdf";
            PdfReader reader = new PdfReader(SRC);
            PdfSigner signer = new PdfSigner(reader, new FileStream(DEST, FileMode.Create), new StampingProperties());

            // To create a signature field we need to create a signature appearance object from PdfSigner object.
            PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            appearance.SetReason("I accept some terms of this contract.")
                .SetLocation("Pamplona")
                .SetPageRect(new Rectangle(70, 575, 200, 80))
                .SetPageNumber(1);
            signer.SetFieldName("Sign");
            // Above code creates a new field with the name “MyFieldName” and set the reason 
            // and location for signature in the PDF before signing the field. 
            // It also specifies the coordinates where the signature box will be created in the document 
            // and the page of PDF on which the new field will be created.

            // If you want to use an existing field to sign the document use the following code snippet. 
            // It removes the page rectangle and page number from the above code. 
            // Then iText looks for an existing field with the name “MyFieldName”.
            // PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            // appearance.SetReason("My reason to sign...")
            //     .SetLocation("Lahore");
            // signer.SetFieldName("MyFieldName"); 

            // iText comes with only one implementation of IExternalSignature which is 
            // the basic implementation of PDF signatures with a private key (PrivateKeySignature). 
            // We will pass the private key that was extracted from the P12/PFX file previously.
            IExternalSignature pks = new PrivateKeySignature(pk, DigestAlgorithms.SHA256);

            // We can add our own IExternalSignature implementations wherever we need.
            signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);

            // iText provides the SignDetached method to sign the PDF. 
            // It requires the private key access along with the hashing algorithm that we set in the last step. 
            // It also requires the cryto standard which we have selected CMS for now. 

            // iText also provide another method to sign that is SignExternalContainer and 
            // there is another static method called SignDeferred. 

            Console.WriteLine("Done.");
        }
    }
}