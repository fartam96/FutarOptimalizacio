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
using System.Security.Cryptography;
using Futarapp.Services;

namespace Futarapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration _configuration;
        private IEmailSender _emailSender;

        public UserController(AppDbContext appDbContext, IConfiguration configuration, IEmailSender emailSender)
        {
            this.appDbContext = appDbContext;
            _configuration = configuration;
            _emailSender = emailSender;
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

            user.Token = CreateJwt(user);
            var newAccessToken = user.Token;
            await appDbContext.SaveChangesAsync();

            return Ok(new
            {
                Token = user.Token,
                Message = "login succes"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }

            //check email format
            if (!IsValid(userObj.Email))
            {
                return BadRequest(new { Message = "wrong email format" });
            }

            // check email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Already Exist" });

            //check username
            if (await CheckUsernameExistAsync(userObj.UserName))
                return BadRequest(new { Message = "Username Already Exist" });

            var passMessage = CheckPasswordStrength(userObj.password);
            if (!string.IsNullOrEmpty(passMessage))
                return BadRequest(new { Message = passMessage.ToString() });

            userObj.password = PasswordHasher.HashPassword(userObj.password);
            userObj.Role = "User";
            userObj.Token = "";
            await appDbContext.AddAsync(userObj);
            await appDbContext.SaveChangesAsync();
            return Ok(new
            {
                Status = 200,
                Message = "User registered!"
            });
        }

        private Task<bool> CheckEmailExistAsync(string? email)
            => appDbContext.users.AnyAsync(x => x.Email == email);

        private Task<bool> CheckUsernameExistAsync(string? username)
            => appDbContext.users.AnyAsync(x => x.Email == username);

        private static string CheckPasswordStrength(string pass)
        {
            StringBuilder sb = new StringBuilder();
            if (pass.Length < 9)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(pass, "[a-z]") && Regex.IsMatch(pass, "[A-Z]") && Regex.IsMatch(pass, "[0-9]")))
                sb.Append("Password should be AlphaNumeric" + Environment.NewLine);
            if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special charcter" + Environment.NewLine);
            return sb.ToString();
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
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

        private bool IsValid(string email)
        {
            string regex = @"^[a-z0-9]+@[a-z]+\.[a-z]{2,3}$";

            return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await appDbContext.users.ToListAsync());
        }

        [HttpPost("resetemail/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {

            var user = await appDbContext.users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return NotFound("Email doesnt exist");
            }
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.ResetPasswordToken = emailToken;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(10);
            var message = new Message(new string[] { email }, "Reset Password", EmailBody.EmailStrinngBody(email, emailToken));
            _emailSender.SendEmails(message);
            appDbContext.Entry(user).State = EntityState.Modified;
            await appDbContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Email sent!"
            });
        }

        [HttpPost("resetpass")]
        public async Task<IActionResult> ResetPassword(ResetPassModel resetpass)
        {
            var newToken = resetpass.EmailToken.Replace(" ", "+");
            var user = await appDbContext.users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == resetpass.Email);

            if (user == null)
            {
                return NotFound("Email doesnt exist");
            }

            var tokenCode = user.ResetPasswordToken;
            DateTime emailTokenExp = user.ResetPasswordExpiry;

            if (tokenCode != resetpass.EmailToken || emailTokenExp < DateTime.Now)
            {
                return BadRequest("invalid reset link");
            }

            user.password = PasswordHasher.HashPassword(resetpass.NewPassword);
            appDbContext.Entry(user).State = EntityState.Modified;
            await appDbContext.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "Reset is succesfull"
            });
        }
    }
}