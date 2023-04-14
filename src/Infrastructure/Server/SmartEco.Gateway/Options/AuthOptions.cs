using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SmartEco.Gateway.Options
{
    public class AuthOptions
    {
        public const string ISSUER = "SmartEco"; // издатель токена
        public const string AUDIENCE = "http://localhost:52207"; // потребитель токена
        const string KEY = "supersecret_SECretkey!20541";   // ключ для шифрации
        public const int LIFETIME = 60 * 24 * 366; // время жизни токена - 60 минут * 24 часа * 366 дней = год
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
