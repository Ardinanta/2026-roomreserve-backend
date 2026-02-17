# RoomReserve Backend (RoomReserve.Api)

Backend API untuk RoomReserve, sistem manajemen peminjaman ruangan.

---

## Deskripsi
Backend ini menyediakan REST API untuk:
- Register & login user
- Pengajuan, edit, pembatalan, dan approval peminjaman ruangan
- Manajemen user, ruangan, dan status peminjaman
- Otentikasi JWT

---

## Fitur
- Registrasi & login user
- CRUD peminjaman ruangan
- Approval & tracking status peminjaman
- Manajemen data ruangan
- Manajemen user (admin)
- Riwayat status peminjaman
- Otentikasi JWT

---

## Teknologi
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL

---

## Instalasi

1. Masuk ke folder backend:
   ```bash
   cd RoomReserve.Api
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```

---

## Konfigurasi Environment

Edit file `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=roomreserve;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your_jwt_secret_key",
    "Issuer": "RoomReserve",
    "Audience": "RoomReserveUsers"
  }
}
```

---

## Menjalankan Backend

1. Migrasi database (sekali saja):
   ```bash
   dotnet ef database update
   ```
2. Jalankan API:
   ```bash
   dotnet run
   ```
Backend akan berjalan di `http://localhost:5000`

---

## Catatan
- Pastikan PostgreSQL sudah berjalan dan database sudah dibuat.
- Untuk pengaturan lebih lanjut, cek file `appsettings.Development.json`.
