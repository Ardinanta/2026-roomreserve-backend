using System.ComponentModel.DataAnnotations;

namespace _2026_roomreserve_backend.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konfirmasi password wajib diisi")]
        [Compare("Password", ErrorMessage = "Password tidak cocok")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Role { get; set; } = "User";
    }
}
