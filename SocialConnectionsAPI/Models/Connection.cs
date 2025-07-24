using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialConnectionsAPI.Models
{
    public class Connection
    {
        // Internal database ID (Primary Key)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // UserStrId of the first user in the connection (lexicographically smaller)
        [Required]
        [StringLength(50)]
        public string User1StrId { get; set; }

        // UserStrId of the second user in the connection (lexicographically larger)
        [Required]
        [StringLength(50)]
        public string User2StrId { get; set; }

        // Navigation properties (optional, for easier EF Core relationships, but not strictly needed for this problem's scope)
        // [ForeignKey("User1StrId")]
        // public User User1 { get; set; }
        // [ForeignKey("User2StrId")]
        // public User User2 { get; set; }
    }
}
