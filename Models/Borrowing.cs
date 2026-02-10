using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_roomreserve_backend.Models
{
    [Table("Borrowings")]
    public class Borrowing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required, MaxLength(200)]
        public string Purpose { get; set; } = string.Empty;

        [Required]
        public DateTime BorrowDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"

        [MaxLength(500)]
        public string? RejectReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================
        // 1 Borrowing dimiliki oleh 1 User
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // 1 Borrowing untuk 1 Room
        [ForeignKey("RoomId")]
        public Room Room { get; set; } = null!;

        // 1 Borrowing bisa punya banyak StatusHistory
        public ICollection<BorrowingStatusHistory> StatusHistories { get; set; } = new List<BorrowingStatusHistory>();
    }
}
