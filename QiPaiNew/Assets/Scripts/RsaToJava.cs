using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Text;
using UnityEngine;

public class RsaToJava : MonoBehaviour
{
    private static Encoding Encoding_UTF8 = Encoding.UTF8;

    public static RsaToJava rsa = new RsaToJava();

    public static string userName;
    public static string passWord;

    //私钥
    static string privateKey = "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDY+/5SlbbYuhbPbpwXV2tISJn1jV5VeDpoMssPLjqvzV5JItz3X1XaaUNIdpq0YRy/Xzu81UGc76A+BztAW70XDRLZuzQDkcda0LTyhG/dA3J6o+5y5KqoEGtsvX29sXm4knwPHVCck1jxQCl94IH1TO8/wqDACkb4Hrc2j4ww8diTox7yMklkIoY0P3xxOZQZoutgK8jS1D++8WaJ8iUqjfE22m+aMSYfnc4HJxpOjEGHw36+AT1QpKSA0uYzZpFI31q8Gs9+pjGzOTPKizAroCUtsid+ekePmq5mnyqHvSHA3VBBKZ/efoxuFHZGEGBFWBPtfDnNhUOA97eaf1w3AgMBAAECggEBAJtlRQsyC2QFSYa/CWyx/7QhwpkX6GI/m1y6W7CAhCkZZBfurt8+WIb18ei4gC0Ei4kGlAq1k6rn9hPWK/n4eWBDzac/KY4Q3fvNEnnInTBkYp0wP/nqh8mbEMQspRuwOaMUzWIptWydHgqQz+ZFXe0mbwVzxpoiAhd6L+4UOetmRlDH79bGC/6oy8VJ9Nrwn+S6fLiAiln7Lu467OkZRXnOkBXnV8p63dDCwfJpMdNzaba958jJfizNLQrHY6GB4BDSdaYN9ykCORHRdAOSgUjiEptVyEAOXRYm+udIdKS3yVnh1Y1dIr0PbhZEDF24mDGoI3gm/kKFvnaclRKRiZkCgYEA+3mA3/oSFRy4t3Z4bwV+GgQHB8Wxh+kXBMKbpY6C1tWxk0R6zpyB3I3VQPcrFRHMAJDYquDW1DfKttrFliXmF36ekIQ6z+/30pb0C2ohCjycJSW6facgjX5B6GKJGiiw0xd75FsFJzKdv2awVqJ1AssTNX1PmVR3JxX+Tvy8CgUCgYEA3OOZi65KddjqLGRfDbMWyJ0+5TfNK+I4XOYJ0rKhDU9KrSb+qyR49HkDFraG4EUHhl7MKQ06V8iKHDUNRbTWryE7E57AaplhZ50Q/N37XZlBQVmntz+3Vs6A2g3G0Afim76Np2ZBiBb7/A1wcm0u/6DwRW5dzRCZS5RnvxgclgsCgYEA5jeEuQc5WKSTojlN5e10VSLMwNvGO/MyxIPYdAb0VvMI/xYZC18SL7SUv0XH5iEFzprMEBuH8H4nEaFUc7G8ZFPhm9HB+azpBpx1TKHbH4D/Pa69fNzsmzwz4Cec9hq8TnPYUuTU4d6ftaKv2pAdaPI6YqSWo3tFHb4vs2YCS3UCgYAo5aL6L6ztoNnKmlLXEH95aVPJ0MkWBO52lMjgz93tvu6OnJeuTDRw3y7pN6ZFQ8Ke6FJYj3Br7F9V1aS9cTwGdNqhSVVK51cXfgc1KkxuCgAA/36D+TISym0k8kQLJEqal29WvSII9mh1RG+7X+fBwKR71nFblGUtjy6ePPKd/QKBgQDiJQwPu3e3bZHFnY1n5GBAfxm7qp+K09OmwNO4rdoHDTaDxoXRdD9vrHzxnhOZNCIKOEfIoGt5U1l9yHP7Mn1nY6wzHMeDwpwsCZUn3P/9nlpoPF/JmiQxNMfhW6znpKgqVkZGbjr/k1W8xNyEg5+tG/PLOyoF8x+jcz00hGddKQ==";
    //static string urlTest = "UffCJe9a/GjLMQEeUPvUGnVkOXaIWiUAsVrLKylVV++x2DA6+HLIkjQROLQUPVKPIW85cFAvf5YaGeTy6zpHizJGP3JyEcN57HqZPxNKqi8TaZ7hxErRRTg8Cp4axbZjLqEbKdqnyfX3hTj0hG5RQQdNKrl0DV/HPV0c77hjuctKBnyaPNzvfwUJhKJqtoplbgyLSd3urBAytG75HwbgjtTDJ56UvIU2BAtoPUrdHHyGnso0nPuHl5VImIMM7LMPZObmSmpj8SCCRtnJJe43FaheCW6CBPQYWMoB7feSHmt/RGE/CsJEmE1R/u8r0vH9XFxm3KnCYoX2ph0Kf9n5fQ==";
    public static string urlQuery;

    void Awake()
    {
#if UNITY_WEBGL
        //string WebUrl = Application.absoluteURL;
        ////string WebUrl = "https://webfrontqpyg.no6688.com/?param=jVA1bgt3UZab9WQK+4D5fdXkkJ//gcN6s+qnrAYZo0fnD15drJ/+DoSf4UjJZLuUp6puZ3b2fVbQkj/91ux7DhR71NMZuH5b/iujKbPAjjdt/qZgsuygAM7+ouZg9op2/S8OfjXvvuA9a5NEDIQQNqLnIk7DwEw22ibzHMoLuXbQQJpbT3dc90u9vxX9IYrJ0T4bVe4p5LPW5kiRcFPvu1hSLfc1uEUEMWpiVVgEpIz1muNp44ATL7LVFhHQnd1Qw2sum8PEOYW1eldtn5FiMJ+DCmDgG5V/O+tpxbvGtYuHXRG4QbgMW7fsVS/vf2/JPYqpO57sgxOIkrodTw6bGQ==";
        //if (WebUrl != null)
        //{
        //    Uri uri = new Uri(WebUrl);
        //    string param = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("param");
        //    var param1 = param.Replace(("\\+"), "+").Replace(" ","+");
        //   // Debug.LogError(param1);
        //    urlQuery = DecryptByPrivateKey(param1, privateKey);
        //    string[] sArray = urlQuery.Split('&');
        //    string sUserName = sArray[0];
        //    string sPassWord = sArray[1];
        //    string[] uArray = sUserName.Split('=');
        //    string[] pArray = sPassWord.Split('=');
        //    userName = uArray[1];
        //    passWord = pArray[1];
        //}

        //Debug.LogError("获取的用户名为：" +userName+"获取的密码为："+passWord );   
#endif
    }

    /// <summary>
    /// KEY 结构体
    /// </summary>
    public struct RSAKEY
    {
        /// <summary>
        /// 公钥an
        /// </summary>
        public string PublicKey
        {
            get;
            set;
        }
        /// <summary>
        /// 私钥
        /// </summary>
        public string PrivateKey
        {
            get;
            set;
        }
    }
    public RSAKEY GetKey()
    {
        //RSA密钥对的构造器  
        RsaKeyPairGenerator keyGenerator = new RsaKeyPairGenerator();

        //RSA密钥构造器的参数  
        RsaKeyGenerationParameters param = new RsaKeyGenerationParameters(Org.BouncyCastle.Math.BigInteger.ValueOf(3), new Org.BouncyCastle.Security.SecureRandom(),2048,25);//密钥长度 0
        //用参数初始化密钥构造器  
        keyGenerator.Init(param);
        //产生密钥对  
        AsymmetricCipherKeyPair keyPair = keyGenerator.GenerateKeyPair();
        //获取公钥和密钥  
        AsymmetricKeyParameter publicKey = keyPair.Public;
        AsymmetricKeyParameter privateKey = keyPair.Private;

        SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
        PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);


        Asn1Object asn1ObjectPublic = subjectPublicKeyInfo.ToAsn1Object();

        byte[] publicInfoByte = asn1ObjectPublic.GetEncoded("UTF-8");
        Asn1Object asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
        byte[] privateInfoByte = asn1ObjectPrivate.GetEncoded("UTF-8");

        RSAKEY item = new RSAKEY()
        {
            PublicKey = Convert.ToBase64String(publicInfoByte),
            PrivateKey = Convert.ToBase64String(privateInfoByte)
        };
        return item;
    }
    private AsymmetricKeyParameter GetPublicKeyParameter(string keyBase64)
    {
        keyBase64 = keyBase64.Replace("\r", "").Replace("\n", "").Replace(" ", "");
        byte[] publicInfoByte = Convert.FromBase64String(keyBase64);
        Asn1Object pubKeyObj = Asn1Object.FromByteArray(publicInfoByte);//这里也可以从流中读取，从本地导入   
        AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(publicInfoByte);
        return pubKey;
    }

    private AsymmetricKeyParameter GetPrivateKeyParameter(string keyBase64)
    {
        keyBase64 = keyBase64.Replace("\r", "").Replace("\n", "").Replace(" ", "");
        byte[] privateInfoByte = Convert.FromBase64String(keyBase64);
        // Asn1Object priKeyObj = Asn1Object.FromByteArray(privateInfoByte);//这里也可以从流中读取，从本地导入   
        // PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
        AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(privateInfoByte);
        return priKey;
    }

    /// <summary>
    /// 私钥加密
    /// </summary>
    /// <param name="data">加密内容</param>
    /// <param name="privateKey">私钥（Base64后的）</param>
    /// <returns>返回Base64内容</returns>
    public string EncryptByPrivateKey(string data, string privateKey)
    {
        //非对称加密算法，加解密用  
        IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());


        //加密  

        try
        {
            engine.Init(true, GetPrivateKeyParameter(privateKey));
            byte[] byteData = Encoding_UTF8.GetBytes(data);
            var ResultData = engine.ProcessBlock(byteData, 0, byteData.Length);
            return Convert.ToBase64String(ResultData);
            //Console.WriteLine("密文（base64编码）:" + Convert.ToBase64String(testData) + Environment.NewLine);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 私钥解密
    /// </summary>
    /// <param name="data">待解密的内容</param>
    /// <param name="privateKey">私钥（Base64编码后的）</param>
    /// <returns>返回明文</returns>
    public string DecryptByPrivateKey(string data, string privateKey)
    {
        data = data.Replace("\r", "").Replace("\n", "").Replace(" ", "");
        //非对称加密算法，加解密用  
        IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());

        //解密  
        try
        {
            engine.Init(false, GetPrivateKeyParameter(privateKey));
            byte[] byteData = Convert.FromBase64String(data);
            var ResultData = engine.ProcessBlock(byteData, 0, byteData.Length);

            return Encoding_UTF8.GetString(ResultData);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 公钥加密
    /// </summary>
    /// <param name="data">加密内容</param>
    /// <param name="publicKey">公钥（Base64编码后的）</param>
    /// <returns>返回Base64内容</returns>
    public string EncryptByPublicKey(string data, string publicKey)
    {
        //非对称加密算法，加解密用  
        IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());

        //加密  
        try
        {
            engine.Init(true, GetPublicKeyParameter(publicKey));
            byte[] byteData = Encoding_UTF8.GetBytes(data);
            var ResultData = engine.ProcessBlock(byteData, 0, byteData.Length);
            return Convert.ToBase64String(ResultData);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 公钥解密
    /// </summary>
    /// <param name="data">待解密的内容</param>
    /// <param name="publicKey">公钥（Base64编码后的）</param>
    /// <returns>返回明文</returns>
    public string DecryptByPublicKey(string data, string publicKey)
    {
        data = data.Replace("\r", "").Replace("\n", "").Replace(" ", "");
        //非对称加密算法，加解密用  
        IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());

        //解密  
        try
        {
            engine.Init(false, GetPublicKeyParameter(publicKey));
            byte[] byteData = Convert.FromBase64String(data);
            var ResultData = engine.ProcessBlock(byteData, 0, byteData.Length);
            return Encoding_UTF8.GetString(ResultData);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
