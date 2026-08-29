using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoSmart.Models
{
    [Table("USERS")]
    public class User
    {
        [Key]
        [Column("U_USERID")]
        public int UserId { get; set; }

        [Required]
        [Column("U_USERNAME")]
        public string Username { get; set; }

        [Required]
        [Column("U_PASSWORD")]
        public string Password { get; set; }

        [Required]
        [Column("U_ROLE")]
        public string Role { get; set; }
    }
}