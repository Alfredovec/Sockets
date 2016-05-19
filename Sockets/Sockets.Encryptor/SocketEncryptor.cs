using System;
using System.Security.Cryptography;
using System.Text;

namespace Sockets.Encryptor
{
    public static class SocketEncryptor
    {
        public static string Encrypt(this string text)
        {
            return Convert.ToBase64String(
                ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(text), null, DataProtectionScope.LocalMachine));
        }

        public static string Decrypt(this string text)
        {
            return Encoding.Unicode.GetString(
                ProtectedData.Unprotect(
                     Convert.FromBase64String(text), null, DataProtectionScope.LocalMachine));
        }
    }
}
