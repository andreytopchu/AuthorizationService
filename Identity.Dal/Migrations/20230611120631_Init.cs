using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cap");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "outbox",
                schema: "cap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageType = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ActivityId = table.Column<string>(type: "text", nullable: true),
                    Retries = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockTimeout = table.Column<TimeSpan>(type: "interval", nullable: false, defaultValue: new TimeSpan(0, 0, 0, 30, 0), comment: "Maximum allowable blocking time"),
                    LockId = table.Column<Guid>(type: "uuid", nullable: true, comment: "Idempotency key (unique key of the thread that captured the lock)"),
                    LockExpirationTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Preventive timeout (maximum lifetime of actuality 'LockId')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Policy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientPolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PolicyName = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientPolicy_Policy_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyRole",
                columns: table => new
                {
                    PoliciesId = table.Column<Guid>(type: "uuid", nullable: false),
                    RolesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyRole", x => new { x.PoliciesId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_PolicyRole_Policy_PoliciesId",
                        column: x => x.PoliciesId,
                        principalTable: "Policy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PolicyRole_Role_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientPolicy_PolicyId",
                table: "ClientPolicy",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_CorrelationId",
                schema: "cap",
                table: "outbox",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_CreatedUtc",
                schema: "cap",
                table: "outbox",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_Retries",
                schema: "cap",
                table: "outbox",
                column: "Retries");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_Status",
                schema: "cap",
                table: "outbox",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Policy_CreatedUtc",
                table: "Policy",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Policy_DeletedUtc",
                table: "Policy",
                column: "DeletedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Policy_Name",
                table: "Policy",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policy_UpdatedUtc",
                table: "Policy",
                column: "UpdatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRole_RolesId",
                table: "PolicyRole",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_CreatedUtc",
                table: "Role",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Role_DeletedUtc",
                table: "Role",
                column: "DeletedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_UpdatedUtc",
                table: "Role",
                column: "UpdatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedUtc",
                table: "User",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_User_DeletedUtc",
                table: "User",
                column: "DeletedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Phone",
                table: "User",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UpdatedUtc",
                table: "User",
                column: "UpdatedUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientPolicy");

            migrationBuilder.DropTable(
                name: "outbox",
                schema: "cap");

            migrationBuilder.DropTable(
                name: "PolicyRole");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Policy");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
