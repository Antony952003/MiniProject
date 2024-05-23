using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniProjectAPI.Models
{
    public class UserDetail
    {
        [Key]
        public int UserId { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordHashKey { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
