using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;

namespace GigachatServer.Services
{
    public static class HashService
    {
        public static byte[] GetSalt()
        {
            byte[] buf = new byte[32];

            using (var pr = new RNGCryptoServiceProvider())
            {
                pr.GetBytes(buf);
            }
            return buf;
        }

        public static byte[] GetHash(string password)
        {
            return DigestUtilities.CalculateDigest("SHA256", Encoding.Default.GetBytes(password));
        }
    }
}
