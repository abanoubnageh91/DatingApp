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
using Microsoft.Extensions.Options;
using DatingApp.API.Helpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Linq;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("users/{userId}/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository datingRepository;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinarConfig;
        private Cloudinary cloudinary;
        public PhotosController(IDatingRepository authRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinarConfig)
        {
            this.cloudinarConfig = cloudinarConfig;
            this.mapper = mapper;
            this.datingRepository = authRepository;

            Account account = new Account(
                cloudinarConfig.Value.CloudName,
                cloudinarConfig.Value.ApiKey,
                cloudinarConfig.Value.ApiSecret
            );

            cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromDb = await datingRepository.GetPhoto(id);

            var photo = mapper.Map<PhotoForReturnDto>(photoFromDb);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var UserFromDb = await datingRepository.GetUser(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Url.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = mapper.Map<Photo>(photoForCreationDto);

            if (!UserFromDb.Photos.Any(p => p.IsMain))
                photo.IsMain = true;

            UserFromDb.Photos.Add(photo);
            if (await datingRepository.SaveAll())
            {
                var photoToReturn = mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id, userId = photo.UserId }, photoToReturn);
            }


            return BadRequest("Failed to add Photo for user.");
        }

        [HttpPost("{id}/SetMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await datingRepository.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromDb = await datingRepository.GetPhoto(id);
            if (photoFromDb.IsMain)
                return BadRequest("This is already Main Photo.");

            var currentMainPhoto = await datingRepository.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromDb.IsMain = true;

            if (await datingRepository.SaveAll())
                return NoContent();

            return BadRequest("Couldn't set photo to Main.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await datingRepository.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromDb = await datingRepository.GetPhoto(id);
            if (photoFromDb.IsMain)
                return BadRequest("You cannot delete your main photo.");

            if (photoFromDb.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromDb.PublicId);
                var result = cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                    datingRepository.Delete(photoFromDb);
            }

            if (photoFromDb.PublicId == null)
            {
                datingRepository.Delete(photoFromDb);
            }



            if (await datingRepository.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photo.");
        }


    }
}