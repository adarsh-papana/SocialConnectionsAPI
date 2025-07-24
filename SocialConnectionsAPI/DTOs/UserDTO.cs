using System.ComponentModel.DataAnnotations;

namespace SocialConnectionsAPI.DTOs
{
    public class CreateUserRequest
    {
        [Required]
        [StringLength(50)]
        public string UserStrId { get; set; }
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }
    }

    // Response DTO for a created user
    public class CreateUserResponse
    {
        public int InternalDbId { get; set; }
        public string UserStrId { get; set; }
        public string Status { get; set; } = "created";
    }

    // Response DTO for a friend (used in GetFriends, FriendsOfFriends)
    public class FriendResponse
    {
        public string UserStrId { get; set; }
        public string DisplayName { get; set; }
    }
}
