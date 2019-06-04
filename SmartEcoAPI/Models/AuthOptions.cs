using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "SmartEco"; // издатель токена
        public const string AUDIENCE = "http://localhost:52207/"; // потребитель токена
        const string KEY = "supersecret_SECretkey!20541";   // ключ для шифрации
        public const int LIFETIME = 60; // время жизни токена - 60 минут
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
