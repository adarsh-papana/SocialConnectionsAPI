using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConnectionsAPI.Models
{
    public class User
    {
        // Internal database ID (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Unique string identifier for the user (e.g., "alice")
        [Required]
        [StringLength(50)] // Limit length for user_str_id
        public string UserStrId { get; set; }

        // Display name for the user (e.g., "Alice Wonderland")
        [Required]
        [StringLength(100)] // Limit length for display_name
        public string DisplayName { get; set; }
    }
}
