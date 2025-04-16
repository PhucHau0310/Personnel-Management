using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PersonnelManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePersonnels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePersonnels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_RoleAccounts_RoleAccountId",
                        column: x => x.RoleAccountId,
                        principalTable: "RoleAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_RoleAccounts_RoleAccountId",
                        column: x => x.RoleAccountId,
                        principalTable: "RoleAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personnels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreatedCccd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RolePersonnel = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personnels_RolePersonnels_RolePersonnel",
                        column: x => x.RolePersonnel,
                        principalTable: "RolePersonnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tokens_Accounts_AccountId1",
                        column: x => x.AccountId1,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PersonnelHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonnelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelHistories_Personnels_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Code", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0197e334-3431-426e-ae7e-60374118707b"), "ADD_PERSONNEl", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6175), "Cho phép thêm nhân viên", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6178) },
                    { new Guid("0c24ccff-1cde-47a0-b58d-50ea0e8bc652"), "DELETE_PERSONNEl", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6184), "Cho phép xóa nhân viên", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6185) },
                    { new Guid("247ed2fa-2812-4d60-bb94-5131fe5ad65d"), "DELETE_ROLE_ACCOUNT", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6211), "Cho phép xóa nhóm phân quyền", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6212) },
                    { new Guid("2de135a5-3149-425c-a73e-02d372367380"), "EDIT_PERSONNEl_GROUP", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6189), "Cho phép sửa phân nhóm nhân viên", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6189) },
                    { new Guid("520bcdbd-8129-4dc6-8151-c16f238fda12"), "ADD_PERSONNEl_GROUP", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6187), "Cho phép thêm phân nhóm nhân viên", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6187) },
                    { new Guid("952dea7b-d16c-4548-a84d-e2cfbe810747"), "ADD_ACCOUNT", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6198), "Cho phép thêm tài khoản đăng nhập", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6198) },
                    { new Guid("95c66136-aca8-4b32-a852-c973b4cd2586"), "DELETE_PERSONNEl_GROUP", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6191), "Cho phép xóa phân nhóm nhân viên", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6191) },
                    { new Guid("96a52b57-258b-4cb4-bd21-47ebf9d04a70"), "EDIT_ROLE_ACCOUNT", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6209), "Cho phép sửa nhóm phân quyền", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6210) },
                    { new Guid("abc92f4e-980a-42fe-b62f-25c1ad27f8dd"), "DELETE_ACCOUNT", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6202), "Cho phép xóa tài khoản đăng nhập", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6202) },
                    { new Guid("b608ff06-a7f4-479f-8033-3813125870fc"), "EDIT_ACCOUNT", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6200), "Cho phép sửa tài khoản đăng nhập", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6200) },
                    { new Guid("c120becb-15d6-420b-aae7-b429709c391b"), "ADD_ROLE_ACCOUNT", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6204), "Cho phép thêm nhóm phân quyền", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6204) },
                    { new Guid("c6fc4c0b-5238-4698-8378-2b20500087aa"), "VIEW_HISTORY", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6196), "Cho phép xem lịch sử", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6196) },
                    { new Guid("f1831356-fe9d-4f74-a66d-50c42da4efd6"), "EDIT_PERSONNEl", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6181), "Cho phép sửa thông tin nhân viên", new DateTime(2025, 4, 15, 10, 15, 10, 195, DateTimeKind.Utc).AddTicks(6181) }
                });

            migrationBuilder.InsertData(
                table: "RoleAccounts",
                columns: new[] { "Id", "CreatedAt", "RoleName", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("7a0c9b26-55a0-4197-a8d9-513b983b39b2"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(5988), "Phó giám đốc", new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(5989) },
                    { new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(5979), "Quản trị viên", new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(5980) },
                    { new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(5985), "Giám đốc", new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(5985) }
                });

            migrationBuilder.InsertData(
                table: "RolePersonnels",
                columns: new[] { "Id", "CreatedAt", "RoleName", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("47137af3-dcb8-4d9c-81ad-430c274ad031"), new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5772), "Kế toán", new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5773) },
                    { new Guid("9e23ffa5-75c5-4dfc-b822-bc6057ce0c73"), new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5759), "Thực tập sinh", new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5760) },
                    { new Guid("b972b9d7-5bd3-4a24-aa1f-3a0d75799ce2"), new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5736), "Phó giám đốc", new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5737) },
                    { new Guid("bb879534-d386-41ad-b5a6-555361c189a6"), new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5768), "Nhân viên nhân sự", new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5769) },
                    { new Guid("c4d52df8-5860-4a42-9eb8-6937bd536329"), new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5755), "Phó phòng", new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5756) },
                    { new Guid("c8d73b25-3605-415a-8d0c-dca692a260d6"), new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5721), "Giám đốc", new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5723) },
                    { new Guid("ee255b75-f5af-4778-95ae-f5a3b740ad95"), new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5751), "Trưởng phòng", new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5752) },
                    { new Guid("f86c83c7-abba-4e89-b3eb-c9d850e3355e"), new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5764), "Nhân viên IT", new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5764) }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "RoleAccountId", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { new Guid("990a0ff2-843d-4ae6-bfb7-a0a6c985f171"), new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5992), "haunhpr024@gmail.com", "$2a$11$inaTR/gdzUEJ0f.G2mqXMe8IlGmgUgKt5ybuaAW8gUiiTWL8rhTS2", new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 10, 15, 9, 584, DateTimeKind.Utc).AddTicks(5993), "admin" },
                    { new Guid("cc3e2505-39ac-4fcf-b1fc-787ec6fadfa8"), new DateTime(2025, 4, 15, 10, 15, 9, 991, DateTimeKind.Utc).AddTicks(56), "haunhpr024@gmail.com", "$2a$11$aNsWx.Amm9k5iQ43PKF7HuKydL2Qn/bVfFcbn6xsCFPLpPevNZDei", new Guid("7a0c9b26-55a0-4197-a8d9-513b983b39b2"), new DateTime(2025, 4, 15, 10, 15, 9, 991, DateTimeKind.Utc).AddTicks(67), "vicedirector" },
                    { new Guid("f9290bcb-7512-49cc-b39f-7d9579bffe56"), new DateTime(2025, 4, 15, 10, 15, 9, 789, DateTimeKind.Utc).AddTicks(7215), "haunhpr024@gmail.com", "$2a$11$imtBcpYESbP793xBVjJgVOqqTODprwv0E5tYz6h2yLX9HQcpShGr.", new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 10, 15, 9, 789, DateTimeKind.Utc).AddTicks(7224), "ceo" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "PermissionId", "RoleAccountId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("07f2cf94-d15b-4784-80a6-1c32579e535c"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6463), new Guid("b608ff06-a7f4-479f-8033-3813125870fc"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6464) },
                    { new Guid("0d0a3807-ef4e-40ab-a7a6-33114c1701b4"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6356), new Guid("247ed2fa-2812-4d60-bb94-5131fe5ad65d"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6356) },
                    { new Guid("292babcf-2338-4279-9bf8-4937551a14dc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6325), new Guid("f1831356-fe9d-4f74-a66d-50c42da4efd6"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6326) },
                    { new Guid("36f3756f-2513-4c80-b7b7-488fee3a9c2f"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6422), new Guid("b608ff06-a7f4-479f-8033-3813125870fc"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6422) },
                    { new Guid("37447490-3ac2-4ac9-a3b6-1be6953bfdef"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6396), new Guid("95c66136-aca8-4b32-a852-c973b4cd2586"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6397) },
                    { new Guid("3abe4fe0-38a6-47ac-9e07-aa22db745ba9"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6367), new Guid("b608ff06-a7f4-479f-8033-3813125870fc"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6368) },
                    { new Guid("3fd99a79-4577-43b1-8551-d09bbef31018"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6363), new Guid("952dea7b-d16c-4548-a84d-e2cfbe810747"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6364) },
                    { new Guid("40b12ccf-a049-4ab0-997e-4b5f8f9da629"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6451), new Guid("96a52b57-258b-4cb4-bd21-47ebf9d04a70"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6452) },
                    { new Guid("41fa3a2c-6402-487a-8d4c-1efcab27b202"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6375), new Guid("0197e334-3431-426e-ae7e-60374118707b"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6375) },
                    { new Guid("431780db-629e-4680-b5fa-b4ec3ec027bb"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6348), new Guid("c120becb-15d6-420b-aae7-b429709c391b"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6348) },
                    { new Guid("54b51895-72c7-4d57-a56f-8e53ea80f1cc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6455), new Guid("c6fc4c0b-5238-4698-8378-2b20500087aa"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6456) },
                    { new Guid("596bdcce-9569-4293-a50f-93e203969684"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6371), new Guid("abc92f4e-980a-42fe-b62f-25c1ad27f8dd"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6372) },
                    { new Guid("62a4d49b-3ffb-4d0b-9bf5-947396bb013f"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6389), new Guid("520bcdbd-8129-4dc6-8151-c16f238fda12"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6389) },
                    { new Guid("62de43d1-cb62-4428-a638-5fce2df34ecd"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6400), new Guid("c120becb-15d6-420b-aae7-b429709c391b"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6401) },
                    { new Guid("66d0ef3f-ca57-4a61-9bdb-3a116b70a299"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6418), new Guid("952dea7b-d16c-4548-a84d-e2cfbe810747"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6419) },
                    { new Guid("75995e1c-a3c7-4420-8742-bdefab5e207c"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6414), new Guid("c6fc4c0b-5238-4698-8378-2b20500087aa"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6415) },
                    { new Guid("7a47aecc-d2b4-45aa-84ec-08d50b8b3292"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6352), new Guid("96a52b57-258b-4cb4-bd21-47ebf9d04a70"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6352) },
                    { new Guid("7ce75136-0695-46fa-b8d9-63f902200e7e"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6408), new Guid("247ed2fa-2812-4d60-bb94-5131fe5ad65d"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6409) },
                    { new Guid("7de01e49-0c95-4fcd-aef9-e68f3c3f4369"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6430), new Guid("0197e334-3431-426e-ae7e-60374118707b"), new Guid("7a0c9b26-55a0-4197-a8d9-513b983b39b2"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6430) },
                    { new Guid("8002a07e-94b9-4e43-905a-a0b85b9241ea"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6337), new Guid("2de135a5-3149-425c-a73e-02d372367380"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6338) },
                    { new Guid("8a11a6a1-3d43-4203-94ac-03b04e9fce5f"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6426), new Guid("abc92f4e-980a-42fe-b62f-25c1ad27f8dd"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6426) },
                    { new Guid("8b499ff8-a78c-4b22-b7aa-577f35df8aaa"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6433), new Guid("f1831356-fe9d-4f74-a66d-50c42da4efd6"), new Guid("7a0c9b26-55a0-4197-a8d9-513b983b39b2"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6434) },
                    { new Guid("8e82fda1-09d5-42d0-9688-7cf8ecaa4ddb"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6320), new Guid("0197e334-3431-426e-ae7e-60374118707b"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6321) },
                    { new Guid("92068ee1-b455-49aa-b7f6-6be1c71d8f6f"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6329), new Guid("0c24ccff-1cde-47a0-b58d-50ea0e8bc652"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6330) },
                    { new Guid("b44abf4d-ceb3-4030-ab8b-8306af81b866"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6393), new Guid("2de135a5-3149-425c-a73e-02d372367380"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6393) },
                    { new Guid("b60b4d13-ed83-4f12-b498-ecc8fc77cb48"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6437), new Guid("520bcdbd-8129-4dc6-8151-c16f238fda12"), new Guid("7a0c9b26-55a0-4197-a8d9-513b983b39b2"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6438) },
                    { new Guid("bf363cb9-10bd-42e8-b89c-cf9f42cb0d93"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6381), new Guid("f1831356-fe9d-4f74-a66d-50c42da4efd6"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6382) },
                    { new Guid("d017acae-a0e6-4790-bd2e-75e50a88adbc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6447), new Guid("c120becb-15d6-420b-aae7-b429709c391b"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6448) },
                    { new Guid("d0889dd4-7161-4d0f-a204-98f6115380fe"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6333), new Guid("520bcdbd-8129-4dc6-8151-c16f238fda12"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6334) },
                    { new Guid("df0c222a-f21e-420c-8047-529d58c66d15"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6341), new Guid("95c66136-aca8-4b32-a852-c973b4cd2586"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6342) },
                    { new Guid("e5d0a801-ac09-4565-acac-3bcc51e5f715"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6441), new Guid("2de135a5-3149-425c-a73e-02d372367380"), new Guid("7a0c9b26-55a0-4197-a8d9-513b983b39b2"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6442) },
                    { new Guid("edfbdfee-eb1a-4105-9340-8fc6429e2bb6"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6359), new Guid("c6fc4c0b-5238-4698-8378-2b20500087aa"), new Guid("b23584d9-748c-4248-abbe-0a553c4716bc"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6360) },
                    { new Guid("f1294ef7-edd3-4ea0-9270-5931fa78aa1d"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6385), new Guid("0c24ccff-1cde-47a0-b58d-50ea0e8bc652"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6386) },
                    { new Guid("f64638ab-9ed3-4309-a9df-8409b9822103"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6459), new Guid("952dea7b-d16c-4548-a84d-e2cfbe810747"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6460) },
                    { new Guid("fc941eb4-b4e3-4300-bc02-75fc188f6eab"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6404), new Guid("96a52b57-258b-4cb4-bd21-47ebf9d04a70"), new Guid("e44ce3f9-0218-41b6-94b2-44df2e845785"), new DateTime(2025, 4, 15, 3, 15, 10, 195, DateTimeKind.Utc).AddTicks(6405) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RoleAccountId",
                table: "Accounts",
                column: "RoleAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelHistories_PersonnelId",
                table: "PersonnelHistories",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_Personnels_NumberId",
                table: "Personnels",
                column: "NumberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personnels_RolePersonnel",
                table: "Personnels",
                column: "RolePersonnel");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleAccountId",
                table: "RolePermissions",
                column: "RoleAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_AccountId",
                table: "Tokens",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_AccountId1",
                table: "Tokens",
                column: "AccountId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonnelHistories");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "Personnels");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "RolePersonnels");

            migrationBuilder.DropTable(
                name: "RoleAccounts");
        }
    }
}
