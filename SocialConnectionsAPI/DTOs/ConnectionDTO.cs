using System.ComponentModel.DataAnnotations;

namespace SocialConnectionsAPI.DTOs
{
    // Request DTO for creating/removing a connection
    public class ConnectionRequest
    {
        [Required]
        public string User1StrId { get; set; }
        [Required]
        public string User2StrId { get; set; }
    }

    // Response DTO for connection operations
    public class ConnectionStatusResponse
    {
        public string Status { get; set; }
    }
}
