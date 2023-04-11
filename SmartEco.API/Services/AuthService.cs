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
using System.ComponentModel.DataAnnotations;
using SmartEco.Common.Services;

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

            var person = await _repository.GetFirstOrDefault<Person>(p => p.Email.Equals(email));
            if (person is null)
                return (StatusCodes.Status400BadRequest, new() { Message = "Invalid username." });
            if (PasswordHasher.IsNotVerify(password, person.PasswordHash!))
                return (StatusCodes.Status400BadRequest, new() { Message = "Invalid password." });

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
            if (person.Password.Length < 6)
                return (StatusCodes.Status400BadRequest, new() { Message = $"The Password must be at least 6 characters long." });

            if (!IsValidEmail(person.Email))
                return (StatusCodes.Status400BadRequest, new() { Message = $"{person.Email} is not a valid email." });

            if (await PersonExist(person.Email))
                return (StatusCodes.Status400BadRequest, new() { Message = $"User with {person.Email} email already exist." });

            var dateTime = DateTime.UtcNow.ToString();
            var callbackUrl = urlHelper.Action(
                action: "ConfirmEmail",
                controller: "Account",
                values: new 
                { 
                    Code = StringCipher.Encrypt(dateTime), 
                    EmailСiphered = StringCipher.Encrypt(person.Email), 
                    PasswordСiphered = StringCipher.Encrypt(person.Password)
                },
                protocol: scheme,
                host: _configuration.GetValue<string>("EmailConfirmHost"))?.Replace("/api", "");

            var isSended = await _emailService.SendAsync(new[] { person.Email }, "Confirm your SmartEco account",
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

            var person = new Person
            {
                Email = email,
                Password = password,
                Role = Role.User,
                PasswordHash = PasswordHasher.GetHash(password)
            };

            await _repository.Create(person);
            if (person.Id == default)
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

        private static ClaimsIdentity GetIdentity(Person person)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role.ToString()!)
                };
            return new(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        private static bool IsValidEmail(string email)
            => new EmailAddressAttribute().IsValid(email);

        private static string CreateToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
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
