using Microsoft.AspNetCore.Mvc;
using SocialConnectionsAPI.DTOs;
using SocialConnectionsAPI.Services;

namespace SocialConnectionsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/Users
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // POST /api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.CreateUserAsync(request);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(CreateUser), result.Data); // 201 Created
            }
            else if (result.ErrorCode == "user_exists")
            {
                return Conflict(new { message = result.ErrorMessage }); // 409 Conflict
            }
            return StatusCode(500, new { message = result.ErrorMessage }); // 500 Internal Server Error
        }

        // GET /api/users/{user_str_id}/friends
        [HttpGet("{userStrId}/friends")]
        public async Task<IActionResult> GetDirectFriends(string userStrId)
        {
            var result = await _userService.GetDirectFriendsAsync(userStrId);

            if (result.IsSuccess)
            {
                return Ok(result.Data); // 200 OK
            }
            else if (result.ErrorCode == "user_not_found")
            {
                return NotFound(new { message = result.ErrorMessage }); // 404 Not Found
            }
            return StatusCode(500, new { message = result.ErrorMessage }); // 500 Internal Server Error
        }

        // GET /api/users/{user_str_id}/friends-of-friends
        [HttpGet("{userStrId}/friends-of-friends")]
        public async Task<IActionResult> GetFriendsOfFriends(string userStrId)
        {
            var result = await _userService.GetFriendsOfFriendsAsync(userStrId);

            if (result.IsSuccess)
            {
                return Ok(result.Data); // 200 OK
            }
            else if (result.ErrorCode == "user_not_found")
            {
                return NotFound(new { message = result.ErrorMessage }); // 404 Not Found
            }
            return StatusCode(500, new { message = result.ErrorMessage }); // 500 Internal Server Error
        }
    }
}
