using Microsoft.EntityFrameworkCore;
using RoomReserve.Api.Models;

namespace RoomReserve.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ==================== DBSET ====================
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }
        public DbSet<BorrowingStatusHistory> BorrowingStatusHistories { get; set; }

        // ==================== KONFIGURASI & SEEDING ====================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== USER CONFIG ====================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email)
                      .IsUnique()
                      .HasDatabaseName("IX_Users_Email");

                entity.HasQueryFilter(u => u.DeletedAt == null);
            });

            // ==================== ROOM CONFIG ====================
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasIndex(r => r.Name)
                      .IsUnique()
                      .HasDatabaseName("IX_Rooms_Name");

                entity.HasQueryFilter(r => r.DeletedAt == null);
            });

            // ==================== BORROWING CONFIG ====================
            modelBuilder.Entity<Borrowing>(entity =>
            {
                entity.HasIndex(b => b.UserId).HasDatabaseName("IX_Borrowings_UserId");
                entity.HasIndex(b => b.RoomId).HasDatabaseName("IX_Borrowings_RoomId");
                entity.HasIndex(b => b.Status).HasDatabaseName("IX_Borrowings_Status");
                entity.HasIndex(b => b.BorrowDate).HasDatabaseName("IX_Borrowings_BorrowDate");

                entity.HasQueryFilter(b => b.DeletedAt == null);

                entity.HasOne(b => b.User)
                      .WithMany(u => u.Borrowings)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Room)
                      .WithMany(r => r.Borrowings)
                      .HasForeignKey(b => b.RoomId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(b => b.StatusHistories)
                      .WithOne(sh => sh.Borrowing)
                      .HasForeignKey(sh => sh.BorrowingId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ==================== STATUS HISTORY CONFIG ====================
            modelBuilder.Entity<BorrowingStatusHistory>(entity =>
            {
                entity.HasOne(sh => sh.ChangedByUser)
                      .WithMany(u => u.StatusChanges)
                      .HasForeignKey(sh => sh.ChangedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ==================== DATA SEEDING ====================
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Pre-computed BCrypt hash of "password123" (static agar migration konsisten)
            const string passwordHash = "$2a$11$TI./O40qyuOdoxmMW/KJveOTvWABZZS7XSAupKhR5EtdN6QIElcOa";

            // ---------- SEED USERS ----------
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Admin RoomReserve",
                    Email = "admin@roomreserve.com",
                    Password = passwordHash,
                    Role = "Admin",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    FullName = "Ahmad Rizky",
                    Email = "ahmad@mail.com",
                    Password = passwordHash,
                    Role = "User",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 3,
                    FullName = "Siti Nurhaliza",
                    Email = "siti@mail.com",
                    Password = passwordHash,
                    Role = "User",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 4,
                    FullName = "Budi Santoso",
                    Email = "budi@mail.com",
                    Password = passwordHash,
                    Role = "User",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 5,
                    FullName = "Dewi Lestari",
                    Email = "dewi@mail.com",
                    Password = passwordHash,
                    Role = "User",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // ---------- SEED ROOMS ----------
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    Name = "Lab Komputer 1",
                    Location = "Gedung A, Lantai 1",
                    Capacity = 40,
                    Description = "Lab komputer dengan 40 PC, proyektor, dan AC",
                    IsAvailable = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Room
                {
                    Id = 2,
                    Name = "Lab Komputer 2",
                    Location = "Gedung A, Lantai 2",
                    Capacity = 35,
                    Description = "Lab komputer dengan 35 PC dan proyektor",
                    IsAvailable = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Room
                {
                    Id = 3,
                    Name = "Aula Utama",
                    Location = "Gedung B, Lantai 1",
                    Capacity = 200,
                    Description = "Aula besar untuk seminar dan acara kampus",
                    IsAvailable = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Room
                {
                    Id = 4,
                    Name = "Ruang Rapat Lt. 2",
                    Location = "Gedung C, Lantai 2",
                    Capacity = 20,
                    Description = "Ruang rapat dengan meja oval dan proyektor",
                    IsAvailable = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Room
                {
                    Id = 5,
                    Name = "Auditorium",
                    Location = "Gedung D, Lantai 1",
                    Capacity = 500,
                    Description = "Auditorium besar untuk wisuda dan acara formal",
                    IsAvailable = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // ---------- SEED BORROWINGS ----------
            modelBuilder.Entity<Borrowing>().HasData(
                new Borrowing
                {
                    Id = 1,
                    UserId = 2,
                    RoomId = 1,
                    BorrowDate = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Purpose = "Praktikum Pemrograman Web",
                    Status = "Approved",
                    CreatedAt = new DateTime(2026, 2, 9, 0, 0, 0, DateTimeKind.Utc)
                },
                new Borrowing
                {
                    Id = 2,
                    UserId = 3,
                    RoomId = 3,
                    BorrowDate = new DateTime(2026, 2, 12, 0, 0, 0, DateTimeKind.Utc),
                    StartTime = new TimeSpan(13, 0, 0),
                    EndTime = new TimeSpan(16, 0, 0),
                    Purpose = "Seminar Nasional",
                    Status = "Pending",
                    CreatedAt = new DateTime(2026, 2, 9, 0, 0, 0, DateTimeKind.Utc)
                },
                new Borrowing
                {
                    Id = 3,
                    UserId = 4,
                    RoomId = 4,
                    BorrowDate = new DateTime(2026, 2, 11, 0, 0, 0, DateTimeKind.Utc),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(11, 0, 0),
                    Purpose = "Rapat BEM",
                    Status = "Rejected",
                    CreatedAt = new DateTime(2026, 2, 9, 0, 0, 0, DateTimeKind.Utc)
                },
                new Borrowing
                {
                    Id = 4,
                    UserId = 5,
                    RoomId = 2,
                    BorrowDate = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Purpose = "Workshop Data Science",
                    Status = "Pending",
                    CreatedAt = new DateTime(2026, 2, 9, 0, 0, 0, DateTimeKind.Utc)
                },
                new Borrowing
                {
                    Id = 5,
                    UserId = 2,
                    RoomId = 5,
                    BorrowDate = new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc),
                    StartTime = new TimeSpan(7, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Purpose = "Wisuda Fakultas Teknik",
                    Status = "Approved",
                    CreatedAt = new DateTime(2026, 2, 9, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // ---------- SEED STATUS HISTORIES ----------
            modelBuilder.Entity<BorrowingStatusHistory>().HasData(
                new BorrowingStatusHistory
                {
                    Id = 1,
                    BorrowingId = 1,
                    ChangedByUserId = 1,
                    PreviousStatus = "Pending",
                    NewStatus = "Approved",
                    Note = "Ruangan tersedia",
                    ChangedAt = new DateTime(2026, 2, 9, 8, 30, 0, DateTimeKind.Utc)
                },
                new BorrowingStatusHistory
                {
                    Id = 2,
                    BorrowingId = 3,
                    ChangedByUserId = 1,
                    PreviousStatus = "Pending",
                    NewStatus = "Rejected",
                    Note = "Bentrok jadwal kuliah",
                    ChangedAt = new DateTime(2026, 2, 9, 9, 0, 0, DateTimeKind.Utc)
                },
                new BorrowingStatusHistory
                {
                    Id = 3,
                    BorrowingId = 5,
                    ChangedByUserId = 1,
                    PreviousStatus = "Pending",
                    NewStatus = "Approved",
                    Note = "Disetujui oleh Dekan",
                    ChangedAt = new DateTime(2026, 2, 9, 10, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}