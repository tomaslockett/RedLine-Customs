using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RedLine.Servicios 
{
    public static class Hashing
    {
        // Clave secreta de 32 bytes (256 bits) para AES-256
        private static readonly string ClaveSecreta = "a2f5g8h1j4k7l3n6p9s2v5y8z1b4e7d9";
        private static readonly string VectorInicial = "c8f3h6k9m2p5s8v1";

        // Encriptar un texto plano utilizando AES-256
        public static string EncriptarAES(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano))
                return textoPlano;

            byte[] iv = Encoding.UTF8.GetBytes(VectorInicial);
            byte[] key = Encoding.UTF8.GetBytes(ClaveSecreta);
            byte[] textoBytes = Encoding.UTF8.GetBytes(textoPlano);
            byte[] textoCifrado;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(textoBytes, 0, textoBytes.Length);
                        cs.FlushFinalBlock();
                    }
                    textoCifrado = ms.ToArray();
                }
            }
            return Convert.ToBase64String(textoCifrado);
        }

        // Desencriptar un texto cifrado utilizando AES-256
        public static string DesencriptarAES(string textoCifrado)
        {
            if (string.IsNullOrEmpty(textoCifrado))
                return textoCifrado;

            byte[] iv = Encoding.UTF8.GetBytes(VectorInicial);
            byte[] key = Encoding.UTF8.GetBytes(ClaveSecreta);
            byte[] textoBytes = Convert.FromBase64String(textoCifrado);
            byte[] textoPlanoBytes;
            int GCM_TAG_LENGTH = 16;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(textoBytes, 0, textoBytes.Length);
                        cs.FlushFinalBlock();
                    }
                    textoPlanoBytes = ms.ToArray();
                }
            }
            return Encoding.UTF8.GetString(textoPlanoBytes);
        }

        // Hashing de contraseña utilizando SHA-256
        public static string Sha256(string contraseña)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(contraseña);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
