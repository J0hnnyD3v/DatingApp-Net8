using System.Security.Claims;
using API.Data;
using API.DTOs.User;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper) : ApiBaseController
{
    private readonly IUserRepository _userRepository = userRepository;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        // var users = await _userRepository.GetUsersAsync();
        // var members = mapper.Map<IEnumerable<MemberDto>>(users);

        // return Ok(members);
        var users = await _userRepository.GetMembersAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")] // /api/users/1
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUser(int id)
    {
        // var user = await _userRepository.GetUserByIdAsync(id);

        // if (user == null)
        // {
        //     return NotFound();
        // }

        // var member = mapper.Map<MemberDto>(user);

        // return Ok(member);
        var user = await _userRepository.GetMemberByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("{email}")] // /api/users/davidsonlong@intrawear.com
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUserByEmail(string email)
    {
        // var user = await _userRepository.GetUserByEmailAsync(email);

        // if (user == null)
        // {
        //     return NotFound();
        // }

        // var member = mapper.Map<MemberDto>(user);

        // return Ok(member);
        var user = await _userRepository.GetMemberByEmailAsync(email);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null)
        {
            return BadRequest("No email found in token");
        }

        var user = await _userRepository.GetMemberByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound("Could not find user");
        }

        mapper.Map(memberUpdateDto, user);

        if (await _userRepository.SaveAllAsync())
        {
            return NoContent();
        }
        return BadRequest("Failed to update user");
    }
}
