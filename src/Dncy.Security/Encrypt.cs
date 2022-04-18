using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#if !NET40
using System.Threading.Tasks;
#endif


namespace Dncy.Tools.Security
{
    public static class Encrypt
    {
        static Encoding defaultEncoding = Encoding.UTF8;

        #region Hash
        /// <summary>
        /// 对字符串进行MD5加密
        /// </summary>
        /// <param name="message">需要加密的字符串</param>
        /// <returns>加密后的结果,大写</returns>
        public static string MDString(this string message, Encoding? encoding=null, MD5ResultType d5ResultType = MD5ResultType.MD5_32)
        {
            var md5 = MD5.Create();
            if (md5 == null)
            {
                throw new ArgumentNullException(nameof(md5));
            }
            encoding ??= defaultEncoding;
            byte[] buffer = encoding.GetBytes(message);
            byte[] bytes = md5.ComputeHash(buffer);
            if (d5ResultType==MD5ResultType.MD5_16)
                return BitConverter.ToString(bytes, 4, 8).Replace("-", string.Empty);
            if (d5ResultType == MD5ResultType.MD5_32)
                return BitConverter.ToString(bytes).Replace("-", string.Empty);
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

#if NET5_0 || NET6_0
        /// <summary>
        /// 对字符串进行MD5加密
        /// </summary>
        /// <param name="message">需要加密的字符串</param>
        /// <returns>加密后的结果,大写</returns>
        public static async Task<string> MDStringAsync(this string message, Encoding? encoding = null, MD5ResultType d5ResultType = MD5ResultType.MD5_32)
        {
            var md5 = MD5.Create();
            if (md5 == null)
            {
                throw new ArgumentNullException(nameof(md5));
            }
            encoding ??= defaultEncoding;
            byte[] buffer = encoding.GetBytes(message);
            using var ms = new MemoryStream(buffer);
            byte[] bytes = await md5.ComputeHashAsync(ms);
            if (d5ResultType == MD5ResultType.MD5_16)
                return BitConverter.ToString(bytes, 4, 8).Replace("-", string.Empty);
            if (d5ResultType == MD5ResultType.MD5_32)
                return BitConverter.ToString(bytes).Replace("-", string.Empty);
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }
#endif



        /// <summary>
        /// MD5 三次加密算法
        /// </summary>
        /// <param name="s">需要加密的字符串</param>
        /// <returns>MD5字符串</returns>
        public static string MDString3(this string s, Encoding? encoding = null, MD5ResultType d5ResultType = MD5ResultType.MD5_32)
        {
            using MD5 md5 = MD5.Create();
            if (md5 == null)
            {
                throw new ArgumentNullException(nameof(md5));
            }
            encoding ??= defaultEncoding;
            byte[] bytes = encoding.GetBytes(s);
            byte[] bytes1 = md5.ComputeHash(bytes);
            byte[] bytes2 = md5.ComputeHash(bytes1);
            byte[] bytes3 = md5.ComputeHash(bytes2);
            if (d5ResultType == MD5ResultType.MD5_16)
                return BitConverter.ToString(bytes, 4, 8).Replace("-", string.Empty);
            if (d5ResultType == MD5ResultType.MD5_32)
                return BitConverter.ToString(bytes).Replace("-", string.Empty);
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        /// <summary>
        /// 获取本地文件的MD5值
        /// </summary>
        /// <param name="localFileName">需要求MD5值的文件的文件名及路径</param>
        /// <returns>MD5字符串</returns>
        public static string MDFile(this string localFileName, MD5ResultType d5ResultType = MD5ResultType.MD5_32)
        {
            using var fs = new BufferedStream(File.Open(localFileName, FileMode.Open, FileAccess.Read), 1048576);
            using MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(fs);
            if (d5ResultType == MD5ResultType.MD5_16)
                return BitConverter.ToString(bytes, 4, 8).Replace("-", string.Empty);
            if (d5ResultType == MD5ResultType.MD5_32)
                return BitConverter.ToString(bytes).Replace("-", string.Empty);
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }


#if NET5_0 || NET6_0
        /// <summary>
        /// 获取本地文件的MD5值
        /// </summary>
        /// <param name="localFileName">需要求MD5值的文件的文件名及路径</param>
        /// <returns>MD5字符串</returns>
        public static async Task<string> MDFileAsync(this string localFileName, MD5ResultType d5ResultType = MD5ResultType.MD5_32)
        {
            using var fs = new BufferedStream(File.Open(localFileName, FileMode.Open, FileAccess.Read), 1048576);
            using MD5 md5 = MD5.Create();
            byte[] bytes = await md5.ComputeHashAsync(fs);
            if (d5ResultType == MD5ResultType.MD5_16)
                return BitConverter.ToString(bytes, 4, 8).Replace("-", string.Empty);
            if (d5ResultType == MD5ResultType.MD5_32)
                return BitConverter.ToString(bytes).Replace("-", string.Empty);
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }
#endif


        /// <summary>
        /// 获取数据流的MD5值
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>MD5字符串</returns>
        public static string MDString(this Stream stream, MD5ResultType d5ResultType = MD5ResultType.MD5_32)
        {
            using MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(stream);
            stream.Position = 0;
            if (d5ResultType == MD5ResultType.MD5_16)
                return BitConverter.ToString(bytes, 4, 8).Replace("-", string.Empty);
            if (d5ResultType == MD5ResultType.MD5_32)
                return BitConverter.ToString(bytes).Replace("-", string.Empty);
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

#if NET5_0 || NET6_0
        /// <summary>
        /// 获取数据流的MD5值
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>MD5字符串</returns>
        public static async Task<string> MDStringAsync(this Stream stream, MD5ResultType d5ResultType = MD5ResultType.MD5_32)
        {
            using MD5 md5 = MD5.Create();
            byte[] bytes = await md5.ComputeHashAsync(stream);
            stream.Position = 0;
            if (d5ResultType == MD5ResultType.MD5_16)
                return BitConverter.ToString(bytes, 4, 8).Replace("-", string.Empty);
            if (d5ResultType == MD5ResultType.MD5_32)
                return BitConverter.ToString(bytes).Replace("-", string.Empty);
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }
#endif


        /// <summary>
        /// 获取数据流的sha256
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string Sha256(this Stream stream)
        {
            using var sha = SHA256.Create();
            byte[] checksum = sha.ComputeHash(stream);
            var shaStr = BitConverter.ToString(checksum).Replace("-", string.Empty);
            stream.Position = 0;
            return shaStr;
        }

#if NET5_0 || NET6_0
        /// <summary>
        /// 获取数据流的sha256
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<string> Sha256Async(this Stream stream)
        {
            using var sha = SHA256.Create();
            byte[] checksum = await sha.ComputeHashAsync(stream);
            var shaStr = BitConverter.ToString(checksum).Replace("-", string.Empty);
            stream.Position = 0;
            return shaStr;
        }
#endif



        /// <summary>
        /// SHA256函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果(大写)</returns>
        public static string Sha256(this string str,Encoding? encoding=null)
        {
            encoding ??= defaultEncoding;
            byte[] sha256Data = encoding.GetBytes(str);
            using var sha256 = SHA256.Create();
            byte[] result = sha256.ComputeHash(sha256Data);
            return BitConverter.ToString(result).Replace("-", string.Empty);
        }


#if NET5_0 || NET6_0
        /// <summary>
        /// SHA256函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果(大写)</returns>
        public static async Task<string> Sha256Async(this string str, Encoding? encoding = null)
        {
            encoding ??= defaultEncoding;
            byte[] sha256Data = encoding.GetBytes(str);
            using var sha256 = SHA256.Create();
            using var ms = new MemoryStream(sha256Data);
            byte[] result = await sha256.ComputeHashAsync(ms);
            return BitConverter.ToString(result).Replace("-", string.Empty);
        }
#endif



        /// <summary>
        /// SHA256函
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>SHA256 Base64String结果</returns>
        public static string SHA256Base64String(this string str, Encoding? encoding = null)
        {
            encoding ??= defaultEncoding;
            byte[] sha256Data = encoding.GetBytes(str);
            using var sha256 = SHA256.Create();
            byte[] result = sha256.ComputeHash(sha256Data);
            return Convert.ToBase64String(result);
        }

        #endregion


        #region Base64
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的数据</returns>
        public static string Base64Encrypt(this string str, Encoding? encoding = null)
        {
            encoding ??= defaultEncoding;
            byte[] encbuff = encoding.GetBytes(str);
            return Convert.ToBase64String(encbuff);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <returns>解密后的数据</returns>
        public static string Base64Decrypt(this string str, Encoding? encoding = null)
        {
            try
            {
                encoding ??= defaultEncoding;
                byte[] decbuff = Convert.FromBase64String(str);
                return encoding.GetString(decbuff);
            }
            catch
            {
                return str;
            }
        }
        #endregion


        #region Des
        /// <summary> 
        /// 加密字符串
        /// </summary> 
        /// <param name="strText">被加密的字符串</param> 
        /// <param name="strEncrKey">密钥</param> 
        /// <returns>加密后的数据</returns> 
        public static string DesEncrypt(this string strText, string strEncrKey,string iv,Encoding? encoding=null)
        {
            if (strEncrKey.Length != 8)
            {
                throw new InvalidOperationException("密钥长度无效，密钥必须是8位！");
            }
            encoding ??= defaultEncoding;
            var ret = new StringBuilder();
            using var des = DES.Create();
            byte[] inputByteArray = encoding.GetBytes(strText);
            des.Key = encoding.GetBytes(strEncrKey);
            des.IV = encoding.GetBytes(iv);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat($"{b:X2}");
            }

            return ret.ToString();
        }

        /// <summary> 
        /// 加密字符串
        /// </summary> 
        /// <param name="strText">被加密的字符串</param>
        /// <param name="desKey"></param>
        /// <param name="desIV"></param>
        /// <returns>加密后的数据</returns> 
        public static string DesEncrypt(this string strText, byte[] desKey, byte[] desIV, Encoding? encoding = null)
        {
            encoding ??= defaultEncoding;
            var ret = new StringBuilder();
            using var des = DES.Create();
            byte[] inputByteArray = encoding.GetBytes(strText);
            des.Key = desKey;
            des.IV = desIV;
            using MemoryStream ms = new MemoryStream();
            using var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat($"{b:X2}");
            }

            return ret.ToString();
        }

        /// <summary>
        /// DES解密算法
        /// </summary>
        /// <param name="pToDecrypt">需要解密的字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns>解密后的数据</returns>
        public static string DesDecrypt(this string pToDecrypt, string sKey, string iv, Encoding? encoding = null)
        {
            if (sKey.Length != 8)
            {
                throw new InvalidOperationException("密钥长度无效，密钥必须为8位！");
            }
            encoding ??= defaultEncoding;
            using var ms = new MemoryStream();
            using var des = DES.Create();
            var inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }

            des.Key = encoding.GetBytes(sKey);
            des.IV = encoding.GetBytes(iv);
            using var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return encoding.GetString(ms.ToArray());
        }

        /// <summary>
        /// DES解密算法
        /// </summary>
        /// <param name="pToDecrypt">需要解密的字符串</param>
        /// <param name="desKey"></param>
        /// <param name="desIV"></param>
        /// <returns>解密后的数据</returns>
        public static string DesDecrypt(this string pToDecrypt, byte[] desKey, byte[] desIV, Encoding? encoding = null)
        {
            encoding ??= defaultEncoding;
            using var ms = new MemoryStream();
            using var des = DES.Create();
            var inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }

            des.Key = desKey;
            des.IV = desIV;
            using var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return encoding.GetString(ms.ToArray());
        }

        #endregion


        #region 对称加密算法AES RijndaelManaged加密解密

        /// <summary>
        /// 生成符合AES加密规则的密钥
        /// </summary>
        /// <param name="length">常用的有 128  256等</param>
        /// <returns></returns>
        public static string GenerateAesKey(int length)
        {
            using var crypto = Aes.Create();
            crypto.KeySize = length;
            crypto.BlockSize = 128;
            crypto.GenerateKey();
            return Convert.ToBase64String(crypto.Key);
        }

       
        /// <summary>
        /// 对称加密算法AES 加密
        /// </summary>
        /// <param name="text">待加密文本, UTF8编码</param>
        /// <param name="key">加密密钥，须半角字符 可调用GenerateAesKey获取</param>
        /// <param name="iv">向量</param>
        /// <param name="mode">加密模式</param>
        /// <param name="encoding">编码格式。默认utf8</param>
        /// <returns>加密结果字符串</returns>
        public static string AESEncryptWithHEX(this string text, string key, string iv, CipherMode mode = CipherMode.CBC, Encoding? encoding = null, PaddingMode padding=PaddingMode.None)
        {
            encoding ??= defaultEncoding;
            using var aes = Aes.Create();
            aes.Key = encoding.GetBytes(key);
            aes.IV = encoding.GetBytes(iv);
            aes.Mode = mode;
            if (padding != PaddingMode.None)
            {
                aes.Padding = padding;
            }
            using var rijndaelEncrypt = aes.CreateEncryptor();
            byte[] inputData = ToHexByte(text);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return ToHexString(encryptedData);
        }

        /// <summary>
        /// 对称加密算法AES 加密
        /// </summary>
        /// <param name="text">待加密文本, UTF8编码</param>
        /// <param name="key">加密密钥，须半角字符 可调用GenerateAesKey获取</param>
        /// <param name="iv">向量</param>
        /// <param name="mode">加密模式</param>
        /// <param name="encoding">编码格式。默认utf8</param>
        /// <returns>加密结果字符串 HEX</returns>
        public static string AESEncryptWithHEX(this string text, byte[] key, byte[] iv, CipherMode mode = CipherMode.CBC, Encoding? encoding = null, PaddingMode padding = PaddingMode.None)
        {
            encoding ??= defaultEncoding;
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = mode;
            if (padding != PaddingMode.None)
            {
                aes.Padding = padding;
            }
            using var rijndaelEncrypt = aes.CreateEncryptor();
            byte[] inputData = ToHexByte(text);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return ToHexString(encryptedData);
        }

        /// <summary>
        /// 对称加密算法AES 解密
        /// </summary>
        /// <param name="text">待加密文本, UTF8编码</param>
        /// <param name="key">加密密钥，须半角字符 可调用GenerateAesKey获取</param>
        /// <param name="iv">向量</param>
        /// <param name="mode">加密模式</param>
        /// <param name="encoding">编码格式。默认utf8</param>
        /// <returns>加密结果字符串 Base64String</returns>
        public static string AESDecryptHEX(this string text, string key, string iv, CipherMode mode = CipherMode.CBC, Encoding? encoding = null, PaddingMode padding = PaddingMode.None)
        {
            try
            {
                encoding ??= defaultEncoding;
                using var aes = Aes.Create();
                aes.Key = encoding.GetBytes(key);
                aes.IV = encoding.GetBytes(iv);
                aes.Mode = mode;
                if (padding != PaddingMode.None)
                {
                    aes.Padding = padding;
                }
                using var rijndaelDecrypt = aes.CreateDecryptor();
                byte[] inputData = ToHexByte(text);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
                return ToHexString(decryptedData);
            }
            catch
            {
                return string.Empty;
            }
        }



        /// <summary>
        /// 对称加密算法AES 解密
        /// </summary>
        /// <param name="text">待加密文本, UTF8编码</param>
        /// <param name="key">加密密钥，须半角字符 可调用GenerateAesKey获取</param>
        /// <param name="iv">向量</param>
        /// <param name="mode">加密模式</param>
        /// <param name="encoding">编码格式。默认utf8</param>
        /// <returns>加密结果字符串 Base64String</returns>
        public static string AESDecryptHEX(this string text, byte[] key, byte[] iv, CipherMode mode = CipherMode.CBC, Encoding? encoding = null, PaddingMode padding = PaddingMode.None)
        {
            try
            {
                encoding ??= defaultEncoding;
                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = mode;
                if (padding != PaddingMode.None)
                {
                    aes.Padding = padding;
                }
                using var rijndaelDecrypt = aes.CreateDecryptor();
                byte[] inputData = ToHexByte(text);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
                return encoding.GetString(decryptedData);
            }
            catch
            {
                return string.Empty;
            }
        }



        /// <summary>
        /// 对称加密算法AES 加密
        /// </summary>
        /// <param name="text">待加密文本</param>
        /// <param name="key">加密密钥，须半角字符 可调用GenerateAesKey获取</param>
        /// <param name="iv">向量</param>
        /// <param name="mode">加密模式</param>
        /// <param name="encoding">编码格式。默认utf8</param>
        /// <returns>加密结果字符串 Base64String</returns>
        public static string AESEncryptWithBase64String(this string text, string key, string iv, CipherMode mode = CipherMode.CBC, Encoding? encoding = null, PaddingMode padding = PaddingMode.None)
        {
            encoding ??= defaultEncoding;
            using var aes = Aes.Create();
            aes.Key = encoding.GetBytes(key);
            aes.IV = encoding.GetBytes(iv);
            aes.Mode = mode;
            if (padding!=PaddingMode.None)
            {
                aes.Padding = padding;
            }
            using var rijndaelEncrypt = aes.CreateEncryptor();
            byte[] inputData = encoding.GetBytes(text);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return Convert.ToBase64String(encryptedData);
        }


        /// <summary>
        /// 对称加密算法AES 加密
        /// </summary>
        /// <param name="text">待加密文本</param>
        /// <param name="key">加密密钥，须半角字符 可调用GenerateAesKey获取</param>
        /// <param name="iv">向量</param>
        /// <param name="mode">加密模式</param>
        /// <param name="encoding">编码格式。默认utf8</param>
        /// <returns>加密结果字符串</returns>
        public static string AESEncryptWithBase64String(this string text, byte[] key,byte[] iv,CipherMode mode = CipherMode.CBC, Encoding? encoding = null, PaddingMode padding = PaddingMode.None)
        {
            encoding ??= defaultEncoding;            
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = mode;
            if (padding != PaddingMode.None)
            {
                aes.Padding = padding;
            }
            using var rijndaelEncrypt = aes.CreateEncryptor();
            byte[] inputData = encoding.GetBytes(text);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// 对称加密算法AES解密
        /// </summary>
        /// <param name="text">待解密文本</param>
        /// <param name="key">加密密钥，须半角字符 </param>
        /// <param name="iv">向量</param>
        /// <param name="mode">加密模式</param>
        /// <param name="encoding">编码格式。默认utf8</param>
        /// <returns>解密成功返回解密后的字符串,失败返回空</returns>
        public static string AESDecryptBase64String(this string text, string key,string iv, CipherMode mode = CipherMode.CBC, Encoding? encoding = null, PaddingMode padding = PaddingMode.None)
        {
            try
            {
                encoding ??= defaultEncoding;
                using var aes = Aes.Create();
                aes.Key = encoding.GetBytes(key);
                aes.IV = encoding.GetBytes(iv);
                aes.Mode = mode;
                if (padding != PaddingMode.None)
                {
                    aes.Padding = padding;
                }
                using var rijndaelDecrypt = aes.CreateDecryptor();
                byte[] inputData = Convert.FromBase64String(text);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
                return encoding.GetString(decryptedData);
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 对称加密算法AES 解密
        /// </summary>
        /// <param name="text">待解密文本</param>
        /// <param name="key">加密密钥，须半角字符  </param>
        /// <param name="iv">向量</param>
        /// <param name="mode">加密模式</param>
        /// <param name="encoding">编码格式。默认utf8</param>
        /// <returns>解密成功返回解密后的字符串,失败返回空</returns>
        public static string AESDecryptBase64String(this string text, byte[] key, byte[] iv, CipherMode mode = CipherMode.CBC, Encoding? encoding = null, PaddingMode padding = PaddingMode.None)
        {
            try
            {
                encoding ??= defaultEncoding;
                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = mode;
                if (padding != PaddingMode.None)
                {
                    aes.Padding = padding;
                }
                using var rijndaelDecrypt = aes.CreateDecryptor();
                byte[] inputData = Convert.FromBase64String(text);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
                return encoding.GetString(decryptedData);
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// byte数组Hex编码
        /// </summary>
        /// <param name="bytes">需要进行编码的byte[]</param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ( (byte)( bytes[bx] >> 4 ) );
                c[cx] = (char)( b > 9 ? b + 0x37 + 0x20 : b + 0x30 );

                b = ( (byte)( bytes[bx] & 0x0F ) );
                c[++cx] = (char)( b > 9 ? b + 0x37 + 0x20 : b + 0x30 );
            }

            return new string(c);
        }
        /// <summary> 
        /// 字符串进行Hex解码(Hex.decodeHex())
        /// </summary> 
        /// <param name="hexString">需要进行解码的字符串</param> 
        /// <returns></returns> 
        public static byte[] ToHexByte(string str)
        {
            if (str.Length == 0 || str.Length % 2 != 0)
                return new byte[0];

            byte[] buffer = new byte[str.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                // Convert first half of byte
                c = str[sx];
                buffer[bx] = (byte)( ( c > '9' ? ( c > 'Z' ? ( c - 'a' + 10 ) : ( c - 'A' + 10 ) ) : ( c - '0' ) ) << 4 );

                // Convert second half of byte
                c = str[++sx];
                buffer[bx] |= (byte)( c > '9' ? ( c > 'Z' ? ( c - 'a' + 10 ) : ( c - 'A' + 10 ) ) : ( c - '0' ) );
            }

            return buffer;
        }
       
        #endregion
    }
}
