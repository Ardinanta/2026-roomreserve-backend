using System.ComponentModel.DataAnnotations;

namespace _2026_roomreserve_backend.DTOs.Room
{
    public class CreateRoomRequest
    {
        [Required(ErrorMessage = "Nama ruangan wajib diisi")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lokasi wajib diisi")]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kapasitas wajib diisi")]
        [Range(1, 10000, ErrorMessage = "Kapasitas harus antara 1 - 10000")]
        public int Capacity { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
