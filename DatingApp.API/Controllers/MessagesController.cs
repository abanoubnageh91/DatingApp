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
using DatingApp.API.Helpers;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [ApiController]
    [Route("users/{userId}/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository datingRepository;
        private readonly IMapper mapper;

        public MessagesController(IDatingRepository authRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.datingRepository = authRepository;

        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var messageFromDb = await datingRepository.GetMessage(id);

            if (messageFromDb == null)
                return NotFound();

            return Ok(messageFromDb);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;
            var messages = await datingRepository.GetMessagesForUser(messageParams);

            var messaagesToReturn = mapper.Map<IEnumerable<MessageToReturnDto>>(messages);

            Response.AddPagination(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages);

            return Ok(messaagesToReturn);

        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messages = await datingRepository.GetMessageThread(userId, recipientId);

            var messagesToReturn = mapper.Map<IEnumerable<MessageToReturnDto>>(messages);

            return Ok(messagesToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            var sender = await datingRepository.GetUser(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var recipient = await datingRepository.GetUser(messageForCreationDto.RecipientId);

            if (recipient == null)
                return BadRequest("Couldn't find user.");

            var message = mapper.Map<Message>(messageForCreationDto);

            datingRepository.Add(message);

            if (await datingRepository.SaveAll())
            {
                var messageToReturn = mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);
            }

            throw new Exception("Creating the message failed on save.");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromDb = await datingRepository.GetMessage(id);
            if (messageFromDb.SenderId == userId)
                messageFromDb.SenderDeleted = true;

            if (messageFromDb.RecipientId == userId)
                messageFromDb.RecipientDeleted = true;

            if (messageFromDb.RecipientDeleted && messageFromDb.SenderDeleted)
                datingRepository.Delete(messageFromDb);

            if (await datingRepository.SaveAll())
                return NoContent();

            throw new Exception("Error deleting the message.");
        }


        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await datingRepository.GetMessage(id);

            if (message.RecipientId != userId)
                return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await datingRepository.SaveAll();

            return NoContent();
        }


    }
}