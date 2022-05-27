using ApiTest20220523.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiTest20220523.Services
{
    public class JwtAuth : IJwtAuth
    {
        private readonly string username = "chitsuhein";
        private readonly string password = "sa@123";
        private readonly string key;
        public JwtAuth(string key)
        {
            this.key = key;
        }
        public string Authentication(string username, string password)
        {
            if (!(username.Equals(username) || password.Equals(password)))
            {
                return null;
            }

            //TokenSettingModel tokenSetting = StaticConfigService.Configuration.GetValue<TokenSettingModel>("TokenSetting");
            TokenSettingModel tokenSetting = StaticConfigService.Configuration.GetSection("TokenSetting").Get<TokenSettingModel>();

            // 1. Create Security Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // 2. Create Private Key to Encrypted
            var tokenKey = Encoding.ASCII.GetBytes(key);

            //3. Create JETdescriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, username)
                    }),
                //Expires = DateTime.UtcNow.AddSeconds(5),
                Expires = DateTime.Now.AddMinutes(tokenSetting.ExpireTimeMinute),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            //4. Create Token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 5. Return Token from method
            return tokenHandler.WriteToken(token);
        }
    }
}
