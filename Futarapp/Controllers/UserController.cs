using Futarapp.Context;
using Futarapp.Helpers;
using Futarapp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Futarapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public UserController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userobj)
        {
            if(userobj == null)
            {
                return BadRequest();
            }
            var user = await appDbContext.users.FirstOrDefaultAsync(x => x.UserName == userobj.UserName && x.password == userobj.password);
            if(user == null)
            {
                return NotFound(new { Message = "user not found" });
            }

            return Ok(new { Message = " login succes"});
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
            string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";

            return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        }
    }
}
