using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }
        
        [HttpPost(template: "register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) 
                return BadRequest("Username is taken");

            using HMACSHA512 hmac = new HMACSHA512();

            AppUser user = new AppUser{
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(buffer: Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
            };
            
            _context.Users.Add(entity: user);
            await _context.SaveChangesAsync();

            return user;
        }
        
        private async Task<bool> UserExists(string username) => 
            await _context.Users.AnyAsync(x =>
                x.UserName == username.ToLower());
    }
}