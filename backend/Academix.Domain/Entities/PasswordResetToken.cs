using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Domain.Entities
{
    [Table("PasswordResetToken")]
    [Index("UserId", Name = "IX_PasswordResetToken_User")]
    [Index("Token", Name = "UQ__Password__1EB4F8179BC9A231", IsUnique = true)]
    public partial class PasswordResetToken
    {
        [Key]
        public int TokenId { get; set; }

        public int UserId { get; set; }

        [StringLength(500)]
        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UsedAt { get; set; }

        [StringLength(50)]
        public string? CreatedByIp { get; set; }

        public int IsExpired { get; set; }

        public int IsUsed { get; set; }
        public bool IsValid => IsExpired == 0 && IsUsed == 0;

        public virtual User User { get; set; } = null!;
    }
}
