using Microsoft.EntityFrameworkCore;
using PersonnelManagement.Models.Entities;

namespace PersonnelManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<RoleAccount> RoleAccounts { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Permission> EndpointPermissions { get; set; }
        public DbSet<Personnel> Personnels { get; set; }
        public DbSet<RolePersonnel> RolePersonnels { get; set; }
        public DbSet<PersonnelHistory> PersonnelHistories { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình các mối quan hệ giữa các entity

            // Account - RoleAccount (M:1)
            modelBuilder.Entity<Account>()
                .HasOne(a => a.RoleAccount)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleAccountId);

            // Account - Token (1:M)
            modelBuilder.Entity<Token>()
                .HasOne<Account>()
                .WithMany(a => a.RefreshTokens)
                .HasForeignKey("AccountId");

            // Personnel - RolePersonnel (M:1)
            modelBuilder.Entity<Personnel>()
                .HasOne(p => p.RolePersonnels)
                .WithMany(r => r.Personnels)
                .HasForeignKey(p => p.RolePersonnel);

            // RoleAccount - Permission (M:M thông qua RolePermission)
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.RoleAccount)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleAccountId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            // Personnel - CheckInOutHistory (1:M)
            modelBuilder.Entity<PersonnelHistory>()
                .HasOne(h => h.Personnel)
                .WithMany(p => p.CheckHistory)
                .HasForeignKey(h => h.PersonnelId);

            // Cấu hình các thuộc tính đặc biệt, ràng buộc, index, etc.
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Personnel>()
                .HasIndex(p => p.NumberId)
                .IsUnique();

            // Seed data mặc định nếu cần
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // Seed dữ liệu mặc định cho RolePersonnel
            modelBuilder.Entity<RolePersonnel>().HasData(
                new RolePersonnel { Id = Guid.NewGuid(), RoleName = "Giám đốc", CreatedAt = DateTime.UtcNow.AddHours(7), UpdatedAt = DateTime.UtcNow.AddHours(7) },
                new RolePersonnel { Id = Guid.NewGuid(), RoleName = "Phó giám đốc", CreatedAt = DateTime.UtcNow.AddHours(7), UpdatedAt = DateTime.UtcNow.AddHours(7) },
                new RolePersonnel { Id = Guid.NewGuid(), RoleName = "Trưởng phòng", CreatedAt = DateTime.UtcNow.AddHours(7), UpdatedAt = DateTime.UtcNow.AddHours(7) },
                new RolePersonnel { Id = Guid.NewGuid(), RoleName = "Phó phòng", CreatedAt = DateTime.UtcNow.AddHours(7), UpdatedAt = DateTime.UtcNow.AddHours(7) },
                new RolePersonnel { Id = Guid.NewGuid(), RoleName = "Thực tập sinh", CreatedAt = DateTime.UtcNow.AddHours(7), UpdatedAt = DateTime.UtcNow.AddHours(7) },
                new RolePersonnel { Id = Guid.NewGuid(), RoleName = "Nhân viên IT", CreatedAt = DateTime.UtcNow.AddHours(7), UpdatedAt = DateTime.UtcNow.AddHours(7) },
                new RolePersonnel { Id = Guid.NewGuid(), RoleName = "Nhân viên nhân sự", CreatedAt = DateTime.UtcNow.AddHours(7), UpdatedAt = DateTime.UtcNow.AddHours(7) },
                new RolePersonnel { Id = Guid.NewGuid(), RoleName = "Kế toán", CreatedAt = DateTime.UtcNow.AddHours(7), UpdatedAt = DateTime.UtcNow.AddHours(7) }
            );

            // Seed dữ liệu mặc định cho RoleAccount
            var adminRoleId = Guid.NewGuid();
            var ceoRoleId = Guid.NewGuid();
            var viceDirectorRoleId = Guid.NewGuid();

            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "haunhpr024@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                    RoleAccountId = adminRoleId,
                },

                new Account
                {
                    Id = Guid.NewGuid(),
                    Username = "ceo",
                    Email = "haunhpr024@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("ceo"),
                    RoleAccountId = ceoRoleId,
                },

                new Account
                {
                    Id = Guid.NewGuid(),
                    Username = "vicedirector",
                    Email = "haunhpr024@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("vicedirector"),
                    RoleAccountId = viceDirectorRoleId,
                }
            );

            modelBuilder.Entity<RoleAccount>().HasData(
                new RoleAccount
                {
                    Id = adminRoleId,
                    RoleName = "Quản trị viên",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new RoleAccount
                {
                    Id = ceoRoleId,
                    RoleName = "Giám đốc",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new RoleAccount
                {
                    Id = viceDirectorRoleId,
                    RoleName = "Phó giám đốc",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            var editPersonnelId = Guid.NewGuid();
            var deletePersonnelId = Guid.NewGuid();
            var addPersonnelId = Guid.NewGuid();

            var editRoleAccountId = Guid.NewGuid();
            var deleteRoleAccountId = Guid.NewGuid();
            var addRoleAccountId = Guid.NewGuid();

            var editAccountId = Guid.NewGuid();
            var deleteAccountId = Guid.NewGuid();
            var addAccountId = Guid.NewGuid();

            var editRolePersonnelId = Guid.NewGuid();
            var deleteRolePersonnelId = Guid.NewGuid();
            var addRolePersonnelId = Guid.NewGuid();

            var viewHistoryId = Guid.NewGuid();

            modelBuilder.Entity<Permission>().HasData(
               new Permission { Id = addPersonnelId,        Name = "Cho phép thêm nhân viên",           Code = "ADD_PERSONNEl" },
               new Permission { Id = editPersonnelId,       Name = "Cho phép sửa thông tin nhân viên",  Code = "EDIT_PERSONNEl" },
               new Permission { Id = deletePersonnelId,     Name = "Cho phép xóa nhân viên",            Code = "DELETE_PERSONNEl" },

               new Permission { Id = addRolePersonnelId,    Name = "Cho phép thêm phân nhóm nhân viên", Code = "ADD_PERSONNEl_GROUP" },
               new Permission { Id = editRolePersonnelId,   Name = "Cho phép sửa phân nhóm nhân viên",  Code = "EDIT_PERSONNEl_GROUP" },
               new Permission { Id = deleteRolePersonnelId, Name = "Cho phép xóa phân nhóm nhân viên",  Code = "DELETE_PERSONNEl_GROUP" },

               new Permission { Id = viewHistoryId,         Name = "Cho phép xem lịch sử",              Code = "VIEW_HISTORY" },

               new Permission { Id = addAccountId,          Name = "Cho phép thêm tài khoản đăng nhập", Code = "ADD_ACCOUNT" },
               new Permission { Id = editAccountId,         Name = "Cho phép sửa tài khoản đăng nhập",  Code = "EDIT_ACCOUNT" },
               new Permission { Id = deleteAccountId,       Name = "Cho phép xóa tài khoản đăng nhập",  Code = "DELETE_ACCOUNT" },

               new Permission { Id = addRoleAccountId,      Name = "Cho phép thêm nhóm phân quyền",     Code = "ADD_ROLE_ACCOUNT" },
               new Permission { Id = editRoleAccountId,     Name = "Cho phép sửa nhóm phân quyền",      Code = "EDIT_ROLE_ACCOUNT" },
               new Permission { Id = deleteRoleAccountId,   Name = "Cho phép xóa nhóm phân quyền",      Code = "DELETE_ROLE_ACCOUNT" }
           );

            // Seed dữ liệu mặc định cho RolePermission
            modelBuilder.Entity<RolePermission>().HasData(
                // ADMIN
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = addPersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = editPersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = deletePersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = addRolePersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = editRolePersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = deleteRolePersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = addRoleAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = editRoleAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = deleteRoleAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = viewHistoryId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = addAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = editAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = adminRoleId, PermissionId = deleteAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                // CEO
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = addPersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = editPersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = deletePersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = addRolePersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = editRolePersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = deleteRolePersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = addRoleAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = editRoleAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = deleteRoleAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = viewHistoryId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = addAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = editAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = deleteAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                // VICE DIRECTOR
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = viceDirectorRoleId, PermissionId = addPersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = viceDirectorRoleId, PermissionId = editPersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = viceDirectorRoleId, PermissionId = addRolePersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = viceDirectorRoleId, PermissionId = editRolePersonnelId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = addRoleAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = editRoleAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = viewHistoryId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = addAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RolePermission { Id = Guid.NewGuid(), RoleAccountId = ceoRoleId, PermissionId = editAccountId, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }
     }
}
