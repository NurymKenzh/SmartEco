using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartEco.API.Helpers;
using SmartEco.API.Options;
using SmartEco.Common.Data.Repositories.Abstractions;
using SmartEco.Common.Enums;
using SmartEco.Common.Models.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SmartEco.Common.Data.Entities;
using SmartEco.Common.Models.Responses;

namespace SmartEco.API.Services
{
    public class AuthService
    {
        private readonly ISmartEcoRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(ISmartEcoRepository repository, IEmailService emailService, IConfiguration configuration)
        {
            _repository = repository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<(int, AuthResponse)> GetToken(PersonRequest personReq)
        {
            var email = personReq.Email;
            var password = personReq.Password;

            string passwordHash = GetHash(password);
            var person = await _repository.GetFirstOrDefault<Person>(p => p.Email.Equals(email) && p.PasswordHash == passwordHash);
            if (person is null)
                return (StatusCodes.Status400BadRequest, new() { Message = "Invalid username or password." });

            var identity = GetIdentity(person);
            var encodedJwt = CreateToken(identity);
            return (StatusCodes.Status200OK, new()
            {
                AccessToken = encodedJwt,
                Email = identity.Name,
                RoleId = person.RoleId,
                Role = person?.Role.ToString()
            });
        }

        public async Task<(int, AuthResponse)> Register(IUrlHelper urlHelper, string scheme, PersonRequest person)
        {
            if (string.IsNullOrEmpty(person.Password) || person.Password.Length < 6)
                return (StatusCodes.Status400BadRequest, new() { Message = $"The Password must be at least 6 characters long." });

            if (!IsValidEmail(person.Email))
                return (StatusCodes.Status400BadRequest, new() { Message = $"{person.Email} is not a valid email." });

            if (await PersonExist(person.Email))
                return (StatusCodes.Status400BadRequest, new() { Message = $"User with {person.Email} email already exist." });

            var dateTime = DateTime.UtcNow.ToString();
            string code = StringCipher.Encrypt(dateTime);
            string email = StringCipher.Encrypt(person.Email);
            string pass = StringCipher.Encrypt(person.Password);
            var callbackUrl = urlHelper.Action(
                action: "ConfirmEmail",
                controller: "Account",
                values: new 
                { 
                    Code = code, 
                    EmailСiphered = email, 
                    PasswordСiphered = pass 
                },
                protocol: scheme,
                host: _configuration.GetValue<string>("EmailConfirmHost"))?.Replace("/api", "");

            var isSended = await _emailService.SendAsync(new[] { person.Email }, "Confirm your account",
                $"Confirm your registration by clicking on the link: <a href='{callbackUrl}'>link to confirm</a>");

            return (StatusCodes.Status200OK, new()
            {
                Message = isSended
                    ? $"To complete the registration, check your email {person.Email} and follow the link provided in the email"
                    : $"Error when sending email to {person.Email}"
            });
        }

        public async Task<(int, AuthResponse)> ConfirmEmail(ConfirmRequest confirm)
        {
            string token, email, password;
            try
            {
                token = StringCipher.Decrypt(confirm.Code);
                email = StringCipher.Decrypt(confirm.EmailСiphered);
                password = StringCipher.Decrypt(confirm.PasswordСiphered);
            }
            catch
            {
                return (StatusCodes.Status400BadRequest, new() { Message = $"The link is invalid. Please try again." });
            }

            if (DateTime.TryParse(token, out DateTime dateTime) is false || dateTime.AddDays(1) < DateTime.UtcNow)
            {
                return (StatusCodes.Status400BadRequest, new() { Message = $"The link is invalid. Please try again." });
            }
            else
            {
                var person = new Person
                {
                    Email = email,
                    Password = password,
                    Role = Role.User,
                    PasswordHash = GetHash(password)
                };

                await _repository.Create(person);

                var isPersonCreated = await _repository.IsAnyEntity<Person>(p => p.Email.Equals(person.Email) && p.PasswordHash == person.PasswordHash);
                if (isPersonCreated is false)
                    return (StatusCodes.Status400BadRequest, new() { Message = $"Failed to register. Please try again." });

                var identity = GetIdentity(person);
                var encodedJwt = CreateToken(identity);
                return (StatusCodes.Status200OK, new()
                {
                    AccessToken = encodedJwt,
                    Email = identity.Name,
                    RoleId = person.RoleId,
                    Role = person.Role.ToString(),
                    Message = $"User {person.Email} successfully registered!"
                });
            }
        }

        public static string GetHash(string Password)
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

        private ClaimsIdentity GetIdentity(Person person)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role.ToString()!)
                };
            return new(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        private static bool IsValidEmail(string email)
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

        private static string CreateToken(ClaimsIdentity identity)
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
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<bool> PersonExist(string email)
            => await _repository.IsAnyEntity<Person>(p => p.Email.Equals(email));
    }
}
