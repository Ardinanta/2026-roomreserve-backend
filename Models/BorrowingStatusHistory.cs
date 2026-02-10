using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_roomreserve_backend.Models
{
    [Table("BorrowingStatusHistories")]
    public class BorrowingStatusHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int BorrowingId { get; set; }

        [Required]
        public int ChangedByUserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string PreviousStatus { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string NewStatus { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // ==================== NAVIGATION PROPERTIES ====================
        // 1 StatusHistory milik 1 Borrowing
        [ForeignKey("BorrowingId")]
        public Borrowing Borrowing { get; set; } = null!;

        // 1 StatusHistory diubah oleh 1 User (Admin)
        [ForeignKey("ChangedByUserId")]
        public User ChangedByUser { get; set; } = null!;
    }
}
