using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(DataContext context) : BaseApiController
    {
        [HttpPost("register")] //account/register
        public async Task<ActionResult<AppUser>> Register (RegisterDto  registerDto){
            if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
        {
            var user = context.Users.FirstOrDefaultAsync(x => x.UserName.
            ToLower() == loginDto.Username.ToLower());
            if (user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        }
         private async Task<bool>UserExists(string UserName){
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == UserName.ToLower());
    }
    }
   
}