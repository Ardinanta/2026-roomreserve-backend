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

        // PATCH: api/borrowings/{id}/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var borrowing = await _context.Borrowings.FirstOrDefaultAsync(b => b.Id == id);
            if (borrowing == null)
                return NotFound(new { message = "Peminjaman tidak ditemukan" });

            if (borrowing.Status != "Pending")
                return BadRequest(new { message = "Status hanya bisa diubah jika masih Pending" });

            if (request.Status == "Approved")
            {
                borrowing.Status = "Approved";
                borrowing.RejectReason = null;
            }
            else if (request.Status == "Rejected")
            {
                borrowing.Status = "Rejected";
                borrowing.RejectReason = request.RejectReason;
            }
            else
            {
                return BadRequest(new { message = "Status tidak valid" });
            }

            borrowing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Status peminjaman berhasil diupdate" });
        }
        [HttpPatch("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token tidak valid" });

            var borrowing = await _context.Borrowings.FirstOrDefaultAsync(b => b.Id == id);
            if (borrowing == null)
                return NotFound(new { message = "Peminjaman tidak ditemukan" });

            if (borrowing.UserId != userId)
                return Forbid();

            if (borrowing.Status != "Pending")
                return BadRequest(new { message = "Peminjaman hanya bisa dibatalkan jika status masih Pending" });

            borrowing.Status = "Cancelled";
            borrowing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Peminjaman berhasil dibatalkan" });
        }

        // PUT: api/borrowings/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBorrowingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token tidak valid" });

            var borrowing = await _context.Borrowings.FirstOrDefaultAsync(b => b.Id == id);
            if (borrowing == null)
                return NotFound(new { message = "Peminjaman tidak ditemukan" });

            if (borrowing.UserId != userId)
                return Forbid();

            if (borrowing.Status != "Pending")
                return BadRequest(new { message = "Peminjaman hanya bisa diubah jika status masih Pending" });

            // Validasi ruangan
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId && r.IsAvailable);
            if (room == null)
                return BadRequest(new { message = "Ruangan tidak ditemukan atau tidak tersedia" });

            // Validasi waktu bentrok (Approved only)
            var isConflict = await _context.Borrowings.AnyAsync(b =>
                b.RoomId == request.RoomId &&
                b.BorrowDate == request.BorrowDate &&
                b.Status == "Approved" &&
                b.Id != borrowing.Id &&
                (request.StartTime < b.EndTime && request.EndTime > b.StartTime)
            );
            if (isConflict)
                return Conflict(new { message = "Ruangan sudah dipinjam pada waktu tersebut" });

            borrowing.RoomId = request.RoomId;
            borrowing.BorrowDate = DateTime.SpecifyKind(request.BorrowDate, DateTimeKind.Utc);
            borrowing.StartTime = request.StartTime;
            borrowing.EndTime = request.EndTime;
            borrowing.Purpose = request.Purpose;
            borrowing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Peminjaman berhasil diupdate" });
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

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId && r.IsAvailable);
            if (room == null)
                return BadRequest(new { message = "Ruangan tidak ditemukan atau tidak tersedia" });

            var isConflict = await _context.Borrowings.AnyAsync(b =>
                b.RoomId == request.RoomId &&
                b.BorrowDate == request.BorrowDate &&
                b.Status == "Approved" &&
                (request.StartTime < b.EndTime && request.EndTime > b.StartTime)
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

            return CreatedAtAction(nameof(GetById), new { id = borrowing.Id },
                new { message = "Pengajuan peminjaman berhasil!", borrowing.Id });
        }

        // GET: api/borrowings
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowingResponse>>> GetAllBorrowings()
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Room)
                .Include(b => b.User)
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
                    CreatedAt = b.CreatedAt,
                    BorrowerName = b.User.FullName,
                    BorrowerEmail = b.User.Email
                })
                .ToListAsync();

            return Ok(borrowings);
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
                .Include(b => b.User)
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
                    CreatedAt = b.CreatedAt,
                    BorrowerName = b.User.FullName,
                    BorrowerEmail = b.User.Email
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

            var b = await _context.Borrowings.Include(x => x.Room).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
            if (b == null)
                return NotFound(new { message = "Peminjaman tidak ditemukan" });

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
                CreatedAt = b.CreatedAt,
                BorrowerName = b.User.FullName,
                BorrowerEmail = b.User.Email
            };

            return Ok(resp);
        }
    }
}
