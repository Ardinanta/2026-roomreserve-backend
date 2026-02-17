using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReserve.Api.Data;
using RoomReserve.Api.Models;
using RoomReserve.Api.DTOs.Room;

namespace RoomReserve.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        // ==================== GET ALL ROOMS ====================
        // GET: api/rooms?search=lab&available=true
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] bool? available)
        {
            var query = _context.Rooms.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r =>
                    r.Name.Contains(search) ||
                    r.Location.Contains(search));
            }

            if (available.HasValue)
            {
                query = query.Where(r => r.IsAvailable == available.Value);
            }

            var rooms = await query
                .OrderBy(r => r.Name)
                .Select(r => MapToResponse(r))
                .ToListAsync();

            return Ok(rooms);
        }

        // ==================== GET ROOM BY ID ====================
        // GET: api/rooms/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound(new { message = "Ruangan tidak ditemukan" });

            return Ok(MapToResponse(room));
        }

        // ==================== CREATE ROOM ====================
        // POST: api/rooms
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Cek apakah nama ruangan sudah ada
            var existingRoom = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Name == request.Name);

            if (existingRoom != null)
                return Conflict(new { message = "Nama ruangan sudah digunakan" });

            var room = new Room
            {
                Name = request.Name,
                Location = request.Location,
                Capacity = request.Capacity,
                Description = request.Description,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, MapToResponse(room));
        }

        // ==================== UPDATE ROOM ====================
        // PUT: api/rooms/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound(new { message = "Ruangan tidak ditemukan" });

            // Cek duplikat nama (kecuali diri sendiri)
            var duplicate = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Name == request.Name && r.Id != id);

            if (duplicate != null)
                return Conflict(new { message = "Nama ruangan sudah digunakan" });

            room.Name = request.Name;
            room.Location = request.Location;
            room.Capacity = request.Capacity;
            room.Description = request.Description;
            room.IsAvailable = request.IsAvailable;
            room.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(MapToResponse(room));
        }

        // ==================== DELETE ROOM (SOFT DELETE) ====================
        // DELETE: api/rooms/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound(new { message = "Ruangan tidak ditemukan" });

            // Cek apakah ada borrowing aktif (Pending/Approved) untuk room ini
            var activeBorrowing = await _context.Borrowings
                .AnyAsync(b => b.RoomId == id && (b.Status == "Pending" || b.Status == "Approved"));

            if (activeBorrowing)
                return BadRequest(new { message = "Tidak dapat menghapus ruangan yang masih memiliki peminjaman aktif" });

            room.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ruangan berhasil dihapus" });
        }

        // ==================== HELPER ====================
        private static RoomResponse MapToResponse(Room room)
        {
            return new RoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                Location = room.Location,
                Capacity = room.Capacity,
                Description = room.Description,
                IsAvailable = room.IsAvailable,
                CreatedAt = room.CreatedAt,
                UpdatedAt = room.UpdatedAt
            };
        }
    }
}
