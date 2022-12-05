using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SmartEco.API.Helpers;
using SmartEco.API.Options;
using SmartEco.Common.Data.Repositories.Abstractions;
using SmartEco.Common.Enums;
using SmartEco.Common.Models.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System;
using SmartEco.Common.Data.Entities;
using SmartEco.Common.Models.Responses;

namespace SmartEco.API.Services
{
    public class AuthService
    {
        private readonly ISmartEcoRepository _repository;
        private readonly IEmailService _emailService;

        public AuthService(ISmartEcoRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public async Task<(AuthResponse, HttpStatusCode)> GetToken(PersonRequest personReq)
        {
            var email = personReq.Email;
            var password = personReq.Password;

            var identity = await GetIdentity(email, password);
            if (identity == null)
            {
                var authResponse = new AuthResponse
                {
                    Message = "Invalid username or password."
                };
                return (authResponse, HttpStatusCode.Unauthorized);
            }
            else
            {
                var encodedJwt = CreateToken(email, password, identity);
                var person = await _repository.GetFirstOrDefault<Person>(p => p.Email == identity.Name);
                var authResponse = new AuthResponse
                {
                    AccessToken = encodedJwt,
                    Email = identity.Name,
                    RoleId = person.RoleId,
                    Role = person?.Role.ToString()
                };
                return (authResponse, HttpStatusCode.OK);
            }
        }

        public async Task<(AuthResponse, HttpStatusCode)> Register(IUrlHelper urlHelper, string scheme, PersonRequest person)
        {

            var authResponse = new AuthResponse();
            if (string.IsNullOrEmpty(person.Password) || person.Password.Length < 6)
            {
                authResponse.Message = $"The Password must be at least 6 characters long.";
                return (authResponse, HttpStatusCode.Unauthorized);
            }
            if (!IsValidEmail(person.Email))
            {
                authResponse.Message = $"{person.Email} is not a valid email.";
                return (authResponse, HttpStatusCode.Unauthorized);
            }
            if (await PersonExist(person.Email))
            {
                authResponse.Message = $"User with {person.Email} email already exist.";
                return (authResponse, HttpStatusCode.Unauthorized);
            }

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
                host: "localhost:3573")
                .Replace("/api", "");

            var send = await _emailService.SendAsync(new[] { person.Email }, "Confirm your account",
                       $"Confirm your registration by clicking on the link: <a href='{callbackUrl}'>link to confirm</a>");

            authResponse.Message = send
                ? $"To complete the registration, check your email {person.Email} and follow the link provided in the email"
                : $"Error when sending email to {person.Email}";

            return (authResponse, HttpStatusCode.OK);
        }

        public async Task<(AuthResponse, HttpStatusCode)> ConfirmEmail(ConfirmRequest confirm)
        {
            var authResponse = new AuthResponse();

            string token = StringCipher.Decrypt(confirm.Code);
            if (confirm.Code is null || !DateTime.TryParse(token, out DateTime dateTime) || dateTime.AddDays(1) < DateTime.UtcNow)
            {
                authResponse.Message = $"The link is invalid. Please try again.";
                return (authResponse, HttpStatusCode.Unauthorized);
            }
            else
            {
                string email = StringCipher.Decrypt(confirm.EmailСiphered);
                string password = StringCipher.Decrypt(confirm.PasswordСiphered);

                var person = new Person
                {
                    Email = email,
                    Password = password,
                    Role = Role.User,
                    PasswordHash = GetHash(password)
                };

                await _repository.Create(person);

                var identity = await GetIdentity(person.Email, person.Password);
                var encodedJwt = CreateToken(person.Email, person.Password, identity);
                authResponse = new AuthResponse
                {
                    AccessToken = encodedJwt,
                    Email = identity.Name,
                    RoleId = person.RoleId,
                    Role = person.Role.ToString(),
                    Message = $"User {person.Email} successfully registered!"
                };

                return (authResponse, HttpStatusCode.OK);
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

        private async Task<ClaimsIdentity> GetIdentity(string Email, string Password)
        {
            string passwordHash = GetHash(Password);
            Person person = await _repository.GetFirstOrDefault<Person>(x => x.Email == Email && x.PasswordHash == passwordHash);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role.ToString())
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            // если пользователя не найдено
            return null;
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

        private async Task<bool> PersonExist(string email)
        {
            var person = await _repository.GetFirstOrDefault<Person>(p => p.Email == email);
            return person != null;
        }

        private string CreateToken(string email, string password, ClaimsIdentity identity)
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
            return encodedJwt;
        }
    }
}
