using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoomReserve.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Borrowings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RejectReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrowings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Borrowings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Borrowings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BorrowingStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BorrowingId = table.Column<int>(type: "integer", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "integer", nullable: false),
                    PreviousStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NewStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowingStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowingStatusHistories_Borrowings_BorrowingId",
                        column: x => x.BorrowingId,
                        principalTable: "Borrowings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowingStatusHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Capacity", "CreatedAt", "DeletedAt", "Description", "ImageUrl", "IsAvailable", "Location", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 40, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lab komputer dengan 40 PC, proyektor, dan AC", null, true, "Gedung A, Lantai 1", "Lab Komputer 1", null },
                    { 2, 35, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lab komputer dengan 35 PC dan proyektor", null, true, "Gedung A, Lantai 2", "Lab Komputer 2", null },
                    { 3, 200, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Aula besar untuk seminar dan acara kampus", null, true, "Gedung B, Lantai 1", "Aula Utama", null },
                    { 4, 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ruang rapat dengan meja oval dan proyektor", null, true, "Gedung C, Lantai 2", "Ruang Rapat Lt. 2", null },
                    { 5, 500, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Auditorium besar untuk wisuda dan acara formal", null, true, "Gedung D, Lantai 1", "Auditorium", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Email", "FullName", "Password", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@roomreserve.com", "Admin RoomReserve", "$2a$11$TI./O40qyuOdoxmMW/KJveOTvWABZZS7XSAupKhR5EtdN6QIElcOa", "Admin", null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "ahmad@mail.com", "Ahmad Rizky", "$2a$11$TI./O40qyuOdoxmMW/KJveOTvWABZZS7XSAupKhR5EtdN6QIElcOa", "User", null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "siti@mail.com", "Siti Nurhaliza", "$2a$11$TI./O40qyuOdoxmMW/KJveOTvWABZZS7XSAupKhR5EtdN6QIElcOa", "User", null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "budi@mail.com", "Budi Santoso", "$2a$11$TI./O40qyuOdoxmMW/KJveOTvWABZZS7XSAupKhR5EtdN6QIElcOa", "User", null },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "dewi@mail.com", "Dewi Lestari", "$2a$11$TI./O40qyuOdoxmMW/KJveOTvWABZZS7XSAupKhR5EtdN6QIElcOa", "User", null }
                });

            migrationBuilder.InsertData(
                table: "Borrowings",
                columns: new[] { "Id", "BorrowDate", "CreatedAt", "DeletedAt", "EndTime", "Purpose", "RejectReason", "RoomId", "StartTime", "Status", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, new TimeSpan(0, 10, 0, 0, 0), "Praktikum Pemrograman Web", null, 1, new TimeSpan(0, 8, 0, 0, 0), "Approved", null, 2 },
                    { 2, new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, new TimeSpan(0, 16, 0, 0, 0), "Seminar Nasional", null, 3, new TimeSpan(0, 13, 0, 0, 0), "Pending", null, 3 },
                    { 3, new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, new TimeSpan(0, 11, 0, 0, 0), "Rapat BEM", null, 4, new TimeSpan(0, 9, 0, 0, 0), "Rejected", null, 4 },
                    { 4, new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, new TimeSpan(0, 12, 0, 0, 0), "Workshop Data Science", null, 2, new TimeSpan(0, 10, 0, 0, 0), "Pending", null, 5 },
                    { 5, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, new TimeSpan(0, 12, 0, 0, 0), "Wisuda Fakultas Teknik", null, 5, new TimeSpan(0, 7, 0, 0, 0), "Approved", null, 2 }
                });

            migrationBuilder.InsertData(
                table: "BorrowingStatusHistories",
                columns: new[] { "Id", "BorrowingId", "ChangedAt", "ChangedByUserId", "NewStatus", "Note", "PreviousStatus" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 2, 9, 8, 30, 0, 0, DateTimeKind.Utc), 1, "Approved", "Ruangan tersedia", "Pending" },
                    { 2, 3, new DateTime(2026, 2, 9, 9, 0, 0, 0, DateTimeKind.Utc), 1, "Rejected", "Bentrok jadwal kuliah", "Pending" },
                    { 3, 5, new DateTime(2026, 2, 9, 10, 0, 0, 0, DateTimeKind.Utc), 1, "Approved", "Disetujui oleh Dekan", "Pending" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_BorrowDate",
                table: "Borrowings",
                column: "BorrowDate");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_RoomId",
                table: "Borrowings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_Status",
                table: "Borrowings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_UserId",
                table: "Borrowings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowingStatusHistories_BorrowingId",
                table: "BorrowingStatusHistories",
                column: "BorrowingId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowingStatusHistories_ChangedByUserId",
                table: "BorrowingStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Name",
                table: "Rooms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowingStatusHistories");

            migrationBuilder.DropTable(
                name: "Borrowings");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
