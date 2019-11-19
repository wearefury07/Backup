using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System;

public class RSAPro : MonoBehaviour
{
    string publickey;
    string privatekey;
    string RSAed;
    public  string mingwen;
    RSACryptoServiceProvider rsa;
    void Start()
    {
        rsa = new RSACryptoServiceProvider();

        privatekey = "<RSAKeyValue><Modulus>oGn58xDXUQptw6KymOJsO5BqwzzinU5qpgZPqY8fUm1Nv1UdERjNEjo8pxQZs4huVoLX1lSYJaqj1MnWa87ioCgn0yy52NWm+IV2GW5T3SiZc0AmfnIeAmWKejkW1el5DvU/qHFRpe7rblNvnI+H65KbxfBd/xJtwXp1mfqriQk=</Modulus><Exponent>EQ==</Exponent><P>vnZOrEwbtXgIa1ZjOM3RQUrT9mmLtLVMeNNazoBOWDatF3gmS96OcuCjVVVhk96X9Mzzi03nr+qS16Ollbcfsw==</P><Q>15y+kf1mkN9gxDCS+a9UyMB52uPA67gjeyC24Q7MGV+Tydxo5PTAHqgxWLNro5KLgQGiLIAZZH37sDTK17G2Uw==</Q><DP>WaEV9rpnZHS4qvt5/J0XLcjcN7kyr2RgONvuf0twKYMkR0eZjR1wNg9b6+vxrv9WkVFjjNld+G5jOE0CoM6lgQ==</DP><DQ>ciXOTUnrAWczOrBNz3rwpoQERrTPi9n0qpjZSfjGZ8kwH5LOHtvtPWgaH+aER4nRYmpGzEPRNTOjXUkgF9aNsw==</DQ><InverseQ>aEgnthSFxAhGskOb7RYBQcW8dhv/UwnV5FyAlI0u4aJnWU5UEH9WrMaReImn/GyGuHYG1r3HgDZ5rLO/CV6gcQ==</InverseQ><D>eqtkyO7CxYByDhMBC3/4acjKHNQ00qVgnRPikLi9mWKk3ZtwlJp+s5XyJWm5TQ36BeuV/kCwlUZBDCHgNFLpiFGXai7OYE95E0G23rVJ11OZ4GPTRDbwb/yCE7R22qb4u7Tg9TFRjho8V2BdrZ23EX9vu3aMOrMxJvXDeiC6iOU=</D></RSAKeyValue>";
        publickey = "<RSAKeyValue><Modulus>oGn58xDXUQptw6KymOJsO5BqwzzinU5qpgZPqY8fUm1Nv1UdERjNEjo8pxQZs4huVoLX1lSYJaqj1MnWa87ioCgn0yy52NWm+IV2GW5T3SiZc0AmfnIeAmWKejkW1el5DvU/qHFRpe7rblNvnI+H65KbxfBd/xJtwXp1mfqriQk=</Modulus><Exponent>EQ==</Exponent></RSAKeyValue>";
        //publickey = rsa.ToXmlString(false);
        //privatekey = rsa.ToXmlString(true);
        Debug.Log("公钥：" + publickey);
        Debug.Log("私钥：" + privatekey);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //RSAed = RSAEncrypt(publickey, mingwen);
            RSAed = "HwfsSP0XO6qRSd4HEzdfvgCTZLHuhEjIgHA6PX8QqQ84FcXvHnzHVoS4hz7rhszj3RcFJ0l5lGfeW96hyEuaCIykqog1IwQpcisxplViDt0Sw1BH4FIoAxa9RIP7b/nNYFdkOSrR+jaI7bq0WAeyQZEJYHmQVIqffSHYp0q8z9s=";
            //Debug.Log("加密后：" + RSAed);
            Debug.Log("解密后：" + RSADecrypt(privatekey, RSAed));

            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = ByteConverter.GetBytes("ABC");

            //RAS数字签名  
            RSAParameters Key = rsa.ExportParameters(true);
            byte[] signedData = HashAndSignBytes(dataToEncrypt, Key);

            if (VerifySignedHash(dataToEncrypt, signedData, Key))
            {
                Debug.Log("数据验证通过");
            }
            else
            {
                Debug.Log("没有通过");
            }
        }
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="publickey">公钥</param>
    /// <param name="content">所加密的内容</param>
    /// <returns>加密后的内容</returns>
    string RSAEncrypt(string publickey, string content)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        byte[] cipherbytes;
        rsa.FromXmlString(publickey);
        cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
        return Convert.ToBase64String(cipherbytes); ;
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="privatekey">私钥</param>
    /// <param name="content">加密后的内容</param>
    /// <returns>解密后的内容</returns>
    string RSADecrypt(string privatekey, string content)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        byte[] cipherbytes;
        rsa.FromXmlString(privatekey);
        cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);
        return Encoding.UTF8.GetString(cipherbytes);
    }

    /// <summary>
    /// 签名
    /// </summary>
    /// <param name="DataToSign"></param>
    /// <param name="Key"></param>
    /// <returns></returns>
    static byte[] HashAndSignBytes(byte[] DataToSign, RSAParameters Key)
    {
        try
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.ImportParameters(Key);
            return RSA.SignData(DataToSign, new SHA1CryptoServiceProvider());
        }
        catch
        {
            return null;
        }
    }


    /// <summary>
    /// 验证签名
    /// </summary>
    /// <param name="DataToVerify"></param>
    /// <param name="SignedData"></param>
    /// <param name="Key"></param>
    /// <returns></returns>
    static bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData, RSAParameters Key)
    {
        try
        {
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
            RSAalg.ImportParameters(Key);
            return RSAalg.VerifyData(DataToVerify, new SHA1CryptoServiceProvider(), SignedData);
        }
        catch
        {
            return false;
        }
    }
}