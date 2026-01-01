#:property PublishAot=false
#:property PublishTrimmed=false
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

(string publicKeyPem, string privateKeyPem) GenerateKeyPair(int keySize = 2048)
{
    using var rsa = RSA.Create(keySize);
    var privateKeyPem = ExportPrivateKeyToPem(rsa);
    var publicKeyPem = ExportPublicKeyToPem(rsa);
    return (publicKeyPem, privateKeyPem);
}

string ExportPrivateKeyToPem(RSA rsa)
{
    var bytes = rsa.ExportPkcs8PrivateKey();
    var base64 = Convert.ToBase64String(bytes);
    return "-----BEGIN PRIVATE KEY-----\n" + InsertLineBreaks(base64) + "-----END PRIVATE KEY-----\n";
}

string ExportPublicKeyToPem(RSA rsa)
{
    var bytes = rsa.ExportSubjectPublicKeyInfo();
    var base64 = Convert.ToBase64String(bytes);
    return "-----BEGIN PUBLIC KEY-----\n" + InsertLineBreaks(base64) + "-----END PUBLIC KEY-----\n";
}

string InsertLineBreaks(string base64)
{
    var sb = new StringBuilder();
    for (int i = 0; i < base64.Length; i += 64)
    {
        sb.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
    }
    return sb.ToString();
}

Console.WriteLine("Creating keypair...");
(var pubKey, var priKey) = GenerateKeyPair();
Console.WriteLine($"Public Key:\r\n{pubKey}");
Console.WriteLine($"Private Key:\r\n{priKey}");
KeyValuePair<string, string> kp = new(pubKey, priKey);

File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "key.json"), JsonSerializer.Serialize(kp));

Console.WriteLine($"Keypair is in {Path.Combine(Environment.CurrentDirectory, "key.json")}. \r\nPlease protect your keypair file carefully. \r\nPress any key to continue...");
Console.ReadLine();

