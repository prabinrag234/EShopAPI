using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShopAPI.Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string ?Username { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        [Column("email")]
        public string ?Email { get; set; }

        [Required]
        [Column("password_hash")]
        public string ?PasswordHash { get; set; }

        [MaxLength(100)]
        [Column("full_name")]
        public string ?FullName { get; set; }

        [MaxLength(20)]
        [Column("phone_number")]
        public string ?PhoneNumber { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
