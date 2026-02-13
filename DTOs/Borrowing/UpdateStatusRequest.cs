
namespace _2026_roomreserve_backend.DTOs.Borrowing
{
	public class UpdateStatusRequest
	{
		public string Status { get; set; } // "Approved" atau "Rejected"
		public string? RejectReason { get; set; } // opsional, hanya untuk "Rejected"
	}
}
