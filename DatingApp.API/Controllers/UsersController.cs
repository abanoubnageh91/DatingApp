using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Collections.Generic;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository datingRepository;
        private readonly IMapper mapper;

        public UsersController(IDatingRepository authRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.datingRepository = authRepository;

        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await datingRepository.GetUsers();
            var usersToReturn = mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await datingRepository.GetUser(id);
            var userToReturn = mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto){
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var UserFromDb = await datingRepository.GetUser(id);
            mapper.Map(userForUpdateDto, UserFromDb);

            if(await datingRepository.SaveAll())
            return NoContent();

            throw new Exception($"Updating user {id} failed on save."); 
        }
    }
}