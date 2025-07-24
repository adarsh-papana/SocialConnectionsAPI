namespace SocialConnectionsAPI.DTOs
{
    // Response DTO for Degree of Separation
    public class DegreeSeparationResponse
    {
        public int? Degree { get; set; } // Nullable for 'not_connected' case
        public string Message { get; set; } // For 'not_connected'
    }
}
