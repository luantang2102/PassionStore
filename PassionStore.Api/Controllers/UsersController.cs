using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassionStore.Api.Controllers.Base;
using PassionStore.Api.Extensions;
using PassionStore.Application.DTOs.UserProfiles;
using PassionStore.Application.DTOs.Users;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;

namespace PassionStore.Api.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var users = await _userService.GetUsersAsync(userParams);
            Response.AddPaginationHeader(users.Metadata);
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var currentUserId = User.GetUserId();
            var user = await _userService.GetUserByIdAsync(id, currentUserId);
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UserRequest userProfileRequest)
        {
            var currentUserId = User.GetUserId();
            var user = await _userService.UpdateUserAsync(userProfileRequest, id, currentUserId);
            return Ok(user);
        }


            [HttpGet("me/profiles")]
            [Authorize]
        public async Task<IActionResult> GetMyProfile([FromQuery] UserProfileParams userProfileParams)
        {
            var userId = User.GetUserId();
            var profiles = await _userService.GetUserProfilesByUserIdAsync(userProfileParams, userId);
            Response.AddPaginationHeader(profiles.Metadata);
            return Ok(profiles);
        }

        [HttpGet("me/profiles/{id}")]
        [Authorize]
        public async Task<IActionResult> GetMyProfileById(Guid id)
        {
            var userId = User.GetUserId();
            var profile = await _userService.GetUserProfileByIdAsync(userId, id);
            return Ok(profile);
        }

        [HttpPost("me/profiles")]
        [Authorize]
        public async Task<IActionResult> CreateMyProfile([FromForm] UserProfileRequest userProfileRequest)
        {
            var userId = User.GetUserId();
            var profile = await _userService.CreateUserProfileAsync(userProfileRequest, userId);
            return CreatedAtAction(nameof(GetMyProfileById), new { id = profile.Id }, profile);
        }

        [HttpPut("me/profiles/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateMyProfile(Guid id, [FromForm] UserProfileRequest userProfileRequest)
        {
            var userId = User.GetUserId();
            var profile = await _userService.UpdateUserProfileAsync(userProfileRequest, userId, id);
            return Ok(profile);
        }

        [HttpDelete("me/profiles/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMyProfile(Guid id)
        {
            var userId = User.GetUserId();
            await _userService.DeleteUserProfileAsync(userId, id);
            return NoContent();
        }
    }
}
