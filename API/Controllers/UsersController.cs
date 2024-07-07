using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IUserrRepository userrRepository, IMapper mapper) : BaseApiController
    {


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userrRepository.GetMembersAsync();

            return Ok(users);

        }
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await userrRepository.GetMemberAsync(username);
            if (user == null) return NotFound();
            return user;

        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null) return BadRequest("No username found in token");
            var user = await userrRepository.GetUserByUsernameAsync(username);
            if (user == null) return BadRequest("No user found ");
            mapper.Map(memberUpdateDto, user);
            if (await userrRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update the user");
        }

    }
}