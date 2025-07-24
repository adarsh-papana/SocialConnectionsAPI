using Microsoft.AspNetCore.Mvc;
using SocialConnectionsAPI.DTOs;
using SocialConnectionsAPI.Services;

namespace SocialConnectionsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/Connections
    public class ConnectionsController : ControllerBase
    {
        private readonly IConnectionService _connectionService;

        public ConnectionsController(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        // POST /api/connections
        [HttpPost]
        public async Task<IActionResult> CreateConnection([FromBody] ConnectionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _connectionService.CreateConnectionAsync(request);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(CreateConnection), result.Data); // 201 Created
            }
            else if (result.ErrorCode == "users_not_found")
            {
                return NotFound(new { message = result.ErrorMessage }); // 404 Not Found
            }
            else if (result.ErrorCode == "connection_exists" || result.ErrorCode == "self_connection_invalid")
            {
                return Conflict(new { message = result.ErrorMessage }); // 409 Conflict
            }
            return StatusCode(500, new { message = result.ErrorMessage }); // 500 Internal Server Error
        }

        // DELETE /api/connections
        [HttpDelete]
        public async Task<IActionResult> RemoveConnection([FromBody] ConnectionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _connectionService.RemoveConnectionAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.Data); // 200 OK
            }
            else if (result.ErrorCode == "users_not_found" || result.ErrorCode == "not_connected")
            {
                return NotFound(new { message = result.ErrorMessage }); // 404 Not Found
            }
            return StatusCode(500, new { message = result.ErrorMessage }); // 500 Internal Server Error
        }

        // GET /api/connections/degree?from_user_str_id=alice&to_user_str_id=dave
        [HttpGet("degree")]
        public async Task<IActionResult> GetDegreeOfSeparation(
            [FromQuery] string from_user_str_id,
            [FromQuery] string to_user_str_id)
        {
            if (string.IsNullOrWhiteSpace(from_user_str_id) || string.IsNullOrWhiteSpace(to_user_str_id))
            {
                return BadRequest(new { message = "Both from_user_str_id and to_user_str_id are required." });
            }

            var result = await _connectionService.GetDegreeOfSeparationAsync(from_user_str_id, to_user_str_id);

            if (result.IsSuccess)
            {
                return Ok(result.Data); // 200 OK
            }
            else if (result.ErrorCode == "users_not_found")
            {
                return NotFound(new { message = result.ErrorMessage }); // 404 Not Found
            }
            // If degree is -1 and message is "not_connected", it's still a 200 OK with specific data
            return StatusCode(500, new { message = result.ErrorMessage }); // 500 Internal Server Error
        }
    }
}
