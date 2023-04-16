﻿using Futarapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Futarapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthControler : ControllerBase
    {
        public static User2 user2 = new User2();
        private readonly IConfiguration _configuration;


        public AuthControler(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            CreatePassworldHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user2.Username = request.Username;
            user2.PasswordHash = passwordHash;
            user2.PasswordSalt = passwordSalt;

            return Ok(user2);

        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            if(user2.Username != request.Username)
            {
                return BadRequest("User not found.");
            }

            if (!VerifyPassWordHash(request.Password, user2.PasswordHash, user2.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(user2);

            return Ok(token);
        }

        private string CreateToken(User2 user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private bool VerifyPassWordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }

        }


        private void CreatePassworldHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512()) 
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));                                       
             }
        }

    }
}
