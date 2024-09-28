using API.Data;
using API.DTOs.Account;
using API.DTOs.User;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : ApiBaseController
{
    [HttpPost("register")] // api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Email)) return BadRequest("User already exists");

        // create user
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var userDto = new UserDto
        {
            UserName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Token = tokenService.CreateToken(user)
        };
        return Ok(userDto);
    }

    [HttpPost("login")] // api/account/login
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email.ToLower());
        if (user == null) return Unauthorized();

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized();
        }

        var userDto = new UserDto
        {
            UserName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Token = tokenService.CreateToken(user)
        };
        return Ok(userDto);
    }

    private async Task<bool> UserExists(string email)
    {
        return await context.Users.AnyAsync(u => u.Email == email.ToLower());
    }
}
