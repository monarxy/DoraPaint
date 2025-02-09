using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MyApp
{
    public class Hashing
    {
        public string Hash(string input)
        {
            using (var sha256 = SHA1.Create())
            {
                byte[] passBytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha256.ComputeHash(passBytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
