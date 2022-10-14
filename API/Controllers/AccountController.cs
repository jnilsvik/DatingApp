using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }
        
        [HttpPost(template: "register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(username: registerDto.Username)) 
                return BadRequest(error: "Username is taken");

            using HMACSHA512 hmac = new HMACSHA512();

            AppUser user = new AppUser{
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(buffer: Encoding.UTF8.GetBytes(s: registerDto.Password)),
                PasswordSalt = hmac.Key,
            };
            
            _context.Users.Add(entity: user);
            await _context.SaveChangesAsync();

            return new UserDto{
                Username = user.UserName,
                Token = _tokenService.CreateToken(user: user),
            };
        }
        
        private async Task<bool> UserExists(string username) => 
            await _context.Users.AnyAsync(x =>
                x.UserName == username.ToLower());

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto){
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            
            if (user == null) return Unauthorized("Invalid Username");

            using HMACSHA512 hmac = new (user.PasswordSalt);

            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
                if (computedHash[i] != user.PasswordHash[i]) 
                    return Unauthorized("Invalid password");

            return user;
        }
    }
}