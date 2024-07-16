﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Interfaces;

namespace Talabat.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> _userManager)
        {
            //1. HEADER


            //2. Payload
            //2.1. Private Claims (name , id , email , ...)

            var AuthClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName, user.DisplayName),
                new Claim(ClaimTypes.Email, user.Email),

            };
           var UserRoles = await _userManager.GetRolesAsync(user); 

            foreach(var Role in UserRoles)
            {
                AuthClaims.Add(new Claim(ClaimTypes.Role, Role));
            }

            //3. Signature

            //3.1. Key:

            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var Token = new JwtSecurityToken(
                issuer: _configuration[key: "JWT:ValidIssure"],
                audience: _configuration[key: "JWT:ValidAudience"],
                claims: AuthClaims,
                expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationInDays"])),
                signingCredentials: new SigningCredentials(AuthKey,SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(Token);

        }
    }
}
