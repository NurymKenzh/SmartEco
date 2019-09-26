using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получение токена авторизованного пользователя. Работает только для авторизованных пользователей.
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetToken")]
        public async Task GetToken()
        {
            var email = Request.Form["email"];
            var password = Request.Form["password"];

            var identity = GetIdentity(email, password);
            if (identity == null)
            {
                Response.StatusCode = 400;
                //await Response.WriteAsync("Invalid username or password.");
                var responseerror = new
                {
                    error = "Invalid username or password."
                };
                Response.ContentType = "application/json";
                await Response.WriteAsync(JsonConvert.SerializeObject(responseerror, new JsonSerializerSettings { Formatting = Formatting.Indented }));
                //return;
            }
            else
            {
                var now = DateTime.UtcNow;
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: identity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    email = identity.Name,
                    role = _context.Person.FirstOrDefault(p => p.Email == identity.Name)?.Role
                };

                // сериализация ответа
                Response.ContentType = "application/json";
                await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
            }
        }

        private ClaimsIdentity GetIdentity(string Email, string Password)
        {
            string passwordHash = GetHash(Password);
            Person person = _context.Person.FirstOrDefault(x => x.Email == Email && x.PasswordHash == passwordHash);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            // если пользователя не найдено
            return null;
        }

        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        /// <param name="person">
        /// Пользователь.
        /// </param>
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<string> Register(Person person)
        {
            if (string.IsNullOrEmpty(person.Password))
            {
                return $"The Password must be at least 6 characters long.";
            }
            if (person.Password.Length < 6)
            {
                return $"The Password must be at least 6 characters long.";
            }
            if (!IsValidEmail(person.Email))
            {
                return $"{person.Email} is not a valid email.";
            }
            if (_context.Person.FirstOrDefault(p => p.Email == person.Email) != null)
            {
                return $"User with {person.Email} email already exist.";
            }

            person.Role = "";

            person.PasswordHash = GetHash(person.Password);
            _context.Person.Add(person);
            await _context.SaveChangesAsync();

            return $"User {person.Email} successfully registered!";
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GetHash(string Password)
        {
            byte[] salt = new byte[128 / 8];
            salt[0] = 1;
            salt[1] = 6;
            salt[2] = 1;
            salt[3] = 4;
            salt[4] = 23;
            salt[5] = 123;
            salt[6] = 56;
            salt[7] = 6;
            salt[8] = 65;
            salt[9] = 89;
            salt[10] = 3;
            salt[11] = 12;
            salt[12] = 1;
            salt[13] = 76;
            salt[14] = 122;
            salt[15] = 54;
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        //[Authorize]
        [Route("GetEmail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetEmail()
        {
            return Ok(User.Identity.Name);
        }

        //[Authorize(Roles = "admin")]
        [Authorize]
        [Route("GetRole")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetRole()
        {
            //return Ok("Ваша роль: администратор");
            Person person = _context.Person.FirstOrDefault(p => p.Email == User.Identity.Name);
            string result = "";
            if (person != null)
            {
                result = person.Role;
            }
            return Ok(result);
        }

        [Route("GetAuthenticated")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool Authenticated()
        {
            return User.Identity.IsAuthenticated;
        }
    }
}