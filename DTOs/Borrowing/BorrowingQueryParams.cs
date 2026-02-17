namespace RoomReserve.Api.DTOs.Borrowing
{
	public class BorrowingQueryParams
	{
		public DateTime? Date { get; set; }
		public string? Status { get; set; }
		public int? RoomId { get; set; }
	}
}
