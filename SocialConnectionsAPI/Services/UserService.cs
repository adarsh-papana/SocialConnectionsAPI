using Microsoft.EntityFrameworkCore;
using SocialConnectionsAPI.Data;
using SocialConnectionsAPI.DTOs;
using SocialConnectionsAPI.Models;

namespace SocialConnectionsAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<CreateUserResponse>> CreateUserAsync(CreateUserRequest request)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.UserStrId == request.UserStrId))
            {
                return ServiceResult<CreateUserResponse>.Failure("User with this ID already exists.", "user_exists");
            }

            var user = new User
            {
                UserStrId = request.UserStrId,
                DisplayName = request.DisplayName
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Save to get the generated Id

            var response = new CreateUserResponse
            {
                InternalDbId = user.Id,
                UserStrId = user.UserStrId,
                Status = "created"
            };
            return ServiceResult<CreateUserResponse>.Success(response);
        }

        public async Task<bool> UserExistsAsync(string userStrId)
        {
            return await _context.Users.AnyAsync(u => u.UserStrId == userStrId);
        }

        public async Task<ServiceResult<IEnumerable<FriendResponse>>> GetDirectFriendsAsync(string userStrId)
        {
            if (!await UserExistsAsync(userStrId))
            {
                return ServiceResult<IEnumerable<FriendResponse>>.Failure("User not found.", "user_not_found");
            }

            // Find connections where userStrId is either User1StrId or User2StrId
            var connections = await _context.Connections
                .Where(c => c.User1StrId == userStrId || c.User2StrId == userStrId)
                .ToListAsync();

            var friendIds = new HashSet<string>();
            foreach (var connection in connections)
            {
                if (connection.User1StrId == userStrId)
                {
                    friendIds.Add(connection.User2StrId);
                }
                else
                {
                    friendIds.Add(connection.User1StrId);
                }
            }

            // Fetch user details for all unique friend IDs
            var friends = await _context.Users
                .Where(u => friendIds.Contains(u.UserStrId))
                .Select(u => new FriendResponse { UserStrId = u.UserStrId, DisplayName = u.DisplayName })
                .ToListAsync();

            return ServiceResult<IEnumerable<FriendResponse>>.Success(friends);
        }

        public async Task<ServiceResult<IEnumerable<FriendResponse>>> GetFriendsOfFriendsAsync(string userStrId)
        {
            if (!await UserExistsAsync(userStrId))
            {
                return ServiceResult<IEnumerable<FriendResponse>>.Failure("User not found.", "user_not_found");
            }

            // 1. Get direct friends of the target user
            var directFriendsResult = await GetDirectFriendsAsync(userStrId);
            if (!directFriendsResult.IsSuccess)
            {
                return ServiceResult<IEnumerable<FriendResponse>>.Failure(directFriendsResult.ErrorMessage, directFriendsResult.ErrorCode);
            }
            var directFriendStrIds = directFriendsResult.Data.Select(f => f.UserStrId).ToHashSet();

            var friendsOfFriendsSet = new HashSet<string>();

            // 2. Iterate through each direct friend to find their friends
            foreach (var directFriendId in directFriendStrIds)
            {
                var friendsOfDirectFriendResult = await GetDirectFriendsAsync(directFriendId);
                if (friendsOfDirectFriendResult.IsSuccess)
                {
                    foreach (var fof in friendsOfDirectFriendResult.Data)
                    {
                        // 3. Exclude self and direct friends
                        if (fof.UserStrId != userStrId && !directFriendStrIds.Contains(fof.UserStrId))
                        {
                            friendsOfFriendsSet.Add(fof.UserStrId);
                        }
                    }
                }
            }

            // 4. Fetch user details for all unique friends-of-friends IDs
            var friendsOfFriends = await _context.Users
                .Where(u => friendsOfFriendsSet.Contains(u.UserStrId))
                .Select(u => new FriendResponse { UserStrId = u.UserStrId, DisplayName = u.DisplayName })
                .ToListAsync();

            return ServiceResult<IEnumerable<FriendResponse>>.Success(friendsOfFriends);
        }
    }
}
