using SocialConnectionsAPI.DTOs;
using SocialConnectionsAPI.Models;

namespace SocialConnectionsAPI.Services
{
    public interface IUserService
    {
        Task<ServiceResult<CreateUserResponse>> CreateUserAsync(CreateUserRequest request);
        Task<ServiceResult<IEnumerable<FriendResponse>>> GetDirectFriendsAsync(string userStrId);
        Task<ServiceResult<IEnumerable<FriendResponse>>> GetFriendsOfFriendsAsync(string userStrId);
        Task<bool> UserExistsAsync(string userStrId); // Helper for other services
    }
}
