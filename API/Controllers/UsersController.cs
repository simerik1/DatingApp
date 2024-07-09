using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IUserrRepository userrRepository, IMapper mapper, IPhotoService photoService) : BaseApiController
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
            var user = await userrRepository.GetUserByUsernameAsync(User.GetUserName());
            if (user == null) return BadRequest("No user found ");
            mapper.Map(memberUpdateDto, user);
            if (await userrRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update the user");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await userrRepository.GetUserByUsernameAsync(User.GetUserName());
            if (user == null) return BadRequest("Cannot find User");
            var result = await photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            user.Photos.Add(photo);
            if (await userrRepository.SaveAllAsync())
                return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, mapper.Map<PhotoDto>(photo));

            return BadRequest("Problem adding photo");
        }
        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await userrRepository.GetUserByUsernameAsync(User.GetUserName());
            if (user == null) return BadRequest("Cannot find User");
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null || photo.IsMain) return BadRequest("Cannot Use this as main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await userrRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Problem setting main photo");
        }
        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await userrRepository.GetUserByUsernameAsync(User.GetUserName());
            if (user == null) return BadRequest("user Not Found");
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null || photo.IsMain) return BadRequest("Photo cannot be deleated");
            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await userrRepository.SaveAllAsync()) return Ok();
            return BadRequest("Problem deleting Photo");
        }
    }
}