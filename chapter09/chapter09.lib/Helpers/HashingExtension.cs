using System;

namespace chapter09.lib.Helpers
{
    public static class HashingExtension
    {
        public static string ToSHA1(this byte[] data)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();

            var hash = sha1.ComputeHash(data);

            return Convert.ToBase64String(hash);
        }
    }
}