using System.ComponentModel.DataAnnotations;

namespace RoomReserve.Api.DTOs.Borrowing
{
	public class CreateBorrowingRequest
	{
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
	}
}
