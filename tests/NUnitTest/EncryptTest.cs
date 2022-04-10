using System;
using System.IO;
using System.Text;

using Dncy.Tools;
using Dncy.Tools.Security;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnitTest
{
    public class EncryptTest
    {

        [SetUp]
        public void Setup()
        {

        }


        [Test]
        public void MD5_Test()
        {
            var s = "123456";
            var md5 = s.MDString(d5ResultType:MD5ResultType.MD5_32);
            Console.WriteLine(md5);
            Assert.IsTrue("e10adc3949ba59abbe56e057f20f883e".ToUpper() == md5);


            md5 = s.MDString(d5ResultType: MD5ResultType.MD5_16);
            Console.WriteLine(md5);
            Assert.IsTrue("49BA59ABBE56E057" == md5);



            var file = $"{AppDomain.CurrentDomain.BaseDirectory}\\test.txt";
            var md5_f = file.MDFile();
            Console.WriteLine(md5_f);


            using (var fs=File.OpenRead(file))
            {
                var md5_fs = fs.MDString();
                Console.WriteLine(md5_f);
                
                byte[] b= fs.ToByteArray();
                var file2 = $"{AppDomain.CurrentDomain.BaseDirectory}\\test5.txt";
                File.WriteAllBytes(file2, b);

                using (BinaryReader br = new BinaryReader(fs))
                {
                    b = br.ReadBytes((int)fs.Length);
                }
                var file3 = $"{AppDomain.CurrentDomain.BaseDirectory}\\test6.txt";
                File.WriteAllBytes(file3, b);
            }


            var sha = "123123".Sha256();
            Console.WriteLine(sha);
            Assert.IsTrue("96cae35ce8a9b0244178bf28e4966c2ce1b8385723a96a6b838858cdd6ca0a1e" == sha.ToLower());
        }

        [Test]
        public void Base64_Test()
        {
            var s = "123456";
            var res = s.Base64Encrypt();
            Console.WriteLine(res);


            var orginal = res.Base64Decrypt();
            Console.WriteLine(orginal);

            Assert.IsTrue(s == orginal);


        }




        [Test]
        public void Des_Test()
        {
            var s = "123456";
            var res = s.DesEncrypt("123456ab", "123456ab");
            Console.WriteLine(res);


            var orginal = res.DesDecrypt("123456ab", "123456ab");
            Console.WriteLine(orginal);

            Assert.IsTrue(s == orginal);

        }




        [Test]
        public void Aes_Test()
        {
            byte[] iv =
            {
                0x41,
                0x72,
                0x65,
                0x79,
                0x6F,
                0x75,
                0x6D,
                0x79,
                0x53,
                0x6E,
                0x6F,
                0x77,
                0x6D,
                0x61,
                0x6E,
                0x3F
            };
            var key = Encrypt.GenerateAesKey(128);

            var s = "123456";
            var res = s.AESEncryptWithBase64String(key, Encoding.UTF8.GetString(iv),padding:System.Security.Cryptography.PaddingMode.PKCS7);
            Console.WriteLine(res);


            var orginal = res.AESDecryptBase64String(Encoding.UTF8.GetBytes(key), iv);
            Console.WriteLine(orginal);
            Assert.IsTrue(s == orginal);



            var a2 = s.AESEncryptWithHEX(key, Encoding.UTF8.GetString(iv));

            var a22 = a2.AESDecryptHEX(key, Encoding.UTF8.GetString(iv));

            Assert.IsTrue(s == a22);


        }


    }
}
