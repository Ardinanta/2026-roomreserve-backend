using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using _2026_roomreserve_backend.Data;
using _2026_roomreserve_backend.DTOs.Borrowing;
using _2026_roomreserve_backend.Models;

namespace _2026_roomreserve_backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BorrowingsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public BorrowingsController(AppDbContext context)
		{
			_context = context;
		}

		// POST: api/borrowings
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] CreateBorrowingRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
				return Unauthorized(new { message = "Token tidak valid" });

			// Cek ruangan
			var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId && r.IsAvailable);
			if (room == null)
				return BadRequest(new { message = "Ruangan tidak ditemukan atau tidak tersedia" });

			// Validasi waktu bentrok (Approved only)
			var isConflict = await _context.Borrowings.AnyAsync(b =>
				b.RoomId == request.RoomId &&
				b.BorrowDate == request.BorrowDate &&
				b.Status == "Approved" &&
				((request.StartTime < b.EndTime && request.EndTime > b.StartTime))
			);
			if (isConflict)
				return Conflict(new { message = "Ruangan sudah dipinjam pada waktu tersebut" });

			var borrowing = new Borrowing
			{
				UserId = userId,
				RoomId = request.RoomId,
				Purpose = request.Purpose,
				BorrowDate = DateTime.SpecifyKind(request.BorrowDate, DateTimeKind.Utc),
				StartTime = request.StartTime,
				EndTime = request.EndTime,
				Status = "Pending",
				CreatedAt = DateTime.UtcNow
			};

			_context.Borrowings.Add(borrowing);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetById), new { id = borrowing.Id }, new { message = "Pengajuan peminjaman berhasil!", borrowing.Id });
		}

		// GET: api/borrowings/me
		[HttpGet("me")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<BorrowingResponse>>> GetMyBorrowings()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
				return Unauthorized(new { message = "Token tidak valid" });

			var borrowings = await _context.Borrowings
				.Include(b => b.Room)
				.Where(b => b.UserId == userId)
				.OrderByDescending(b => b.BorrowDate)
				.ThenByDescending(b => b.StartTime)
				.Select(b => new BorrowingResponse
				{
					Id = b.Id,
					RoomId = b.RoomId,
					RoomName = b.Room.Name,
					RoomLocation = b.Room.Location,
					UserId = b.UserId,
					Purpose = b.Purpose,
					BorrowDate = b.BorrowDate,
					StartTime = b.StartTime,
					EndTime = b.EndTime,
					Status = b.Status,
					RejectReason = b.RejectReason,
					CreatedAt = b.CreatedAt
				})
				.ToListAsync();

			return Ok(borrowings);
		}

		// GET: api/borrowings/{id}
		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<BorrowingResponse>> GetById(int id)
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
				return Unauthorized(new { message = "Token tidak valid" });

			var b = await _context.Borrowings.Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == id);
			if (b == null)
				return NotFound(new { message = "Peminjaman tidak ditemukan" });

			// Hanya pemilik atau admin yang boleh lihat detail
			if (b.UserId != userId && !User.IsInRole("Admin"))
				return Forbid();

			var resp = new BorrowingResponse
			{
				Id = b.Id,
				RoomId = b.RoomId,
				RoomName = b.Room.Name,
				RoomLocation = b.Room.Location,
				UserId = b.UserId,
				Purpose = b.Purpose,
				BorrowDate = b.BorrowDate,
				StartTime = b.StartTime,
				EndTime = b.EndTime,
				Status = b.Status,
				RejectReason = b.RejectReason,
				CreatedAt = b.CreatedAt
			};
			return Ok(resp);
		}
	}
}
