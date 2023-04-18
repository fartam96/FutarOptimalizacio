using Futarapp.Helpers;
using Futarapp.Context;
using Futarapp.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace Futarapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.appDbContext = appDbContext;
            _configuration = configuration;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            var user = await appDbContext.users
                .FirstOrDefaultAsync(x => x.UserName == userObj.UserName);

            if (user == null)
                return NotFound(new { Message = "User not found!" });

            if (!PasswordHasher.VerifyPassword(userObj.password, user.password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }

            user.Token = CreateToken(user);

            return Ok(new { 
                Token = user.Token,
                Message = "login succes" 
            });
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userobj, string passwordHasher)
        {
            if (userobj == null)
            {
                return BadRequest();
            }

            if(await CheckUserNameExistAsync(userobj.UserName))
            {
                return BadRequest(new { Message = "username already exist" });
            }

            if (!IsValid(userobj.Email))
            {
                return BadRequest(new { Message = "wrong email format" });
            }

            if (await CheckEmailExistAsync(userobj.Email))
            {
                return BadRequest(new { Message = "email already exist" });
            }

            var pass = CheckPasswordStrength(userobj.password);
            if (!string.IsNullOrEmpty(pass))
            {
                return BadRequest(new { Message = pass.ToString() });
            }

            userobj.password = PasswordHasher.HashPassword(userobj.password);
            userobj.Role = "User";
            userobj.Token = "";
            await appDbContext.users.AddAsync(userobj);
            await appDbContext.SaveChangesAsync();
            return Ok(new { Message= "user registered"});    
        }

        private async Task<bool> CheckUserNameExistAsync(string userName) 
        {
            return await appDbContext.users.AnyAsync(x => x.UserName == userName);


        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await appDbContext.users.AnyAsync(x => x.Email == email);

        }

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if(password.Length < 8)
            {
                sb.Append("Minimum password lenght is 8"+ Environment.NewLine);
            }
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]")
                && Regex.IsMatch(password, "[0-9]")))
            {
                sb.Append("password should have capital letter small letter and number" + Environment.NewLine);
            }

            if(!(Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,*,(,),_,+,\\[,\\],{,},?,:,;,|;',\\,,.,/,~,`,-,=]")))
            {
                sb.Append("Password should contain special character" + Environment.NewLine);
            }

            return sb.ToString();
        }

        private bool IsValid(string email)
        {
            string regex = @"^[a-z0-9]+@[a-z]+\.[a-z]{2,3}$";

            return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysceret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name,$"{user.UserName}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name,$"{user.UserName}")
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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await appDbContext.users.ToListAsync());
        }
    }
}
