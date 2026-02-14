namespace _2026_roomreserve_backend.DTOs.Borrowing
{
	public class BorrowingResponse
	{
		public int Id { get; set; }
		public int RoomId { get; set; }
		public string RoomName { get; set; } = string.Empty;
		public string RoomLocation { get; set; } = string.Empty;
		public int UserId { get; set; }
		public string BorrowerName { get; set; } = string.Empty;
		public string BorrowerEmail { get; set; } = string.Empty;
		public string Purpose { get; set; } = string.Empty;
		public DateTime BorrowDate { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
		public string Status { get; set; } = string.Empty;
		public string? RejectReason { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
