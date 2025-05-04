using VirtuoInventory.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace VirtuoInventory.Api.Helper
{
    public static class Common
    {
        #region ===[ Public Methods ]==============================================================

        /// <summary>
        /// Generate Jwt Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appSettingSecret"></param>
        /// <returns></returns>
        public static string GenerateJwtToken(User user, AppSettings appSettings)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim("id", user.Id.ToString()),
                    new Claim("role", user.Role) // Add role claim if needed
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        /// <summary>
        /// Hash a password using HMACSHA256.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <returns>The hashed password as a Base64 string.</returns>
        public static string HashPassword(string password)
        {
            string secret = "Don't touch my code plz"; // Add your secret key here
            using (var sha256 = SHA256.Create())
            {
                var combinedPassword = password + secret;
                var bytes = Encoding.UTF8.GetBytes(combinedPassword);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Verify if a plain text password matches a hashed password.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <param name="hashedPassword">The hashed password to compare against.</param>
        /// <returns>True if the password matches the hash, otherwise false.</returns>
        public static bool VerifyHashedPassword(string password, string hashedPassword)
        {
            var hashedInputPassword = HashPassword(password);
            return hashedInputPassword == hashedPassword;
        }

        #endregion
    }
}
