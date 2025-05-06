using Adis.Dm;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Adis.Dal.Data
{
    /// <summary>
    /// Позволяет использовать EFCore
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        /// <summary>
        /// Основной конструктор для контекста БД
        /// </summary>
        public AppDbContext() { }

        /// <summary>
        /// Конструктор с указанием опций подключения к БД
        /// </summary>
        /// <param name="options">Опции подключения</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Проекты
        /// </summary>
        public virtual DbSet<Project> Projects { get; set; }

        /// <summary>
        /// Задачи
        /// </summary>
        public virtual DbSet<ProjectTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("projects");

                entity.HasKey(p => p.IdProject)
                    .HasName("PRIMARY");

                entity.Property(p => p.IdProject)
                    .HasColumnName("id_project");

                entity.Property(p => p.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(p => p.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(p => p.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(p => p.Status)
                    .HasColumnName("status")
                    .HasConversion(new EnumToStringConverter<ProjectStatus>())
                    .HasColumnType("enum('designing', 'contractorSearch', 'inExecution', 'completed')")
                    .IsRequired();

                entity.Property(p => p.NameWorkObject)
                    .HasColumnName("name_work_object")
                    .IsRequired();

                entity.Property(p => p.IdUser)
                    .HasColumnName("id_user")
                    .IsRequired();

                entity.Property(p => p.IdLocation)
                    .HasColumnName("id_location");

                entity.HasIndex(p => p.Status)
                    .HasDatabaseName("ix_projects_status");

                entity.HasIndex(p => new { p.StartDate, p.EndDate })
                    .HasDatabaseName("ix_projects_dates");

                entity.ToTable(t => t.HasCheckConstraint("chk_projects_dates", "start_date <= end_date"));

                entity.HasOne(p => p.User)
                    .WithMany(u => u.Projects)
                    .HasForeignKey(p => p.IdUser)
                    .HasConstraintName("fk_projects_user");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("locations");

                entity.HasKey(l => l.IdLocation)
                    .HasName("PRIMARY");

                entity.Property(l => l.IdLocation)
                    .HasColumnName("id_location");

                entity.Property(l => l.Geometry)
                    .HasColumnType("GEOMETRY")
                    .HasColumnName("geometry")
                    .IsRequired();

                entity.HasIndex(l => l.Geometry)
                    .HasDatabaseName("ix_locations_geometry")
                    .IsSpatial();

                entity.HasMany(l => l.Projects)
                    .WithOne(p => p.Location) 
                    .HasForeignKey(p => p.IdLocation) 
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_location_project");
            });

            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.ToTable("tasks");

                // Primary Key configuration
                entity.HasKey(t => t.IdTask)
                    .HasName("PRIMARY");

                // Properties configuration
                entity.Property(t => t.IdTask)
                    .HasColumnName("id_task");

                entity.Property(t => t.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(t => t.Description)
                    .HasColumnName("description")
                    .HasColumnType("text")
                    .IsRequired();

                entity.Property(t => t.IdProject)
                    .HasColumnName("id_project")
                    .IsRequired();

                // Indexes
                entity.HasIndex(t => t.IdProject)
                    .HasDatabaseName("ix_id_project");

                // Relationships
                entity.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.IdProject)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_project_tasks");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(entity => entity.Id)
                    .HasName("PRIMARY");

                entity.Property(u => u.Id)
                    .HasColumnName("id_user")
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.UserName)
                    .HasColumnName("username")
                    .HasMaxLength(50);

                entity.Property(u => u.NormalizedUserName)
                    .HasColumnName("normalized_username")
                    .HasMaxLength(50);

                entity.Property(u => u.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(u => u.NormalizedEmail)
                    .HasColumnName("normalized_email")
                    .HasMaxLength(100);

                entity.Property(u => u.EmailConfirmed)
                    .HasColumnName("email_confirmed");

                entity.Property(u => u.PasswordHash)
                    .HasColumnName("password_hash")
                    .HasMaxLength(256);

                entity.Property(u => u.SecurityStamp)
                    .HasColumnName("security_stamp")
                    .HasMaxLength(256);

                entity.Property(u => u.ConcurrencyStamp)
                    .HasColumnName("concurrency_stamp")
                    .HasMaxLength(256);

                entity.Property(u => u.PhoneNumber)
                    .HasColumnName("phone_number")
                    .HasMaxLength(20);

                entity.Property(u => u.PhoneNumberConfirmed)
                    .HasColumnName("phone_number_confirmed");

                entity.Property(u => u.TwoFactorEnabled)
                    .HasColumnName("two_factor_enabled");

                entity.Property(u => u.LockoutEnd)
                    .HasColumnName("lockout_end")
                    .HasColumnType("datetime");

                entity.Property(u => u.LockoutEnabled)
                    .HasColumnName("lockout_enabled");

                entity.Property(u => u.AccessFailedCount)
                    .HasColumnName("access_failed_count");

                entity.Property(u => u.FullName)
                    .HasColumnName("full_name")
                    .HasMaxLength(255);

                entity.Property(u => u.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd();

                // Индексы
                entity.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("ix_users_email");

                // Связь с проектами
                entity.HasMany(u => u.Projects)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.IdUser)
                    .HasConstraintName("fk_projects_user")
                    .OnDelete(DeleteBehavior.NoAction);

                // Проверочные ограничения
                entity.ToTable(t => t.HasCheckConstraint(
                    "chk_users_email_format",
                    "email LIKE '%@%'"));
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.HasKey(entity => entity.Id)
                    .HasName("PRIMARY");

                entity.Property(r => r.Id)
                    .HasColumnName("id_role");

                entity.Property(r => r.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(r => r.NormalizedName)
                    .HasColumnName("normalized_name")
                    .HasMaxLength(50);

                entity.Property(r => r.ConcurrencyStamp)
                    .HasColumnName("concurrency_stamp");

                entity.HasMany(r => r.Users)
                    .WithMany(u => u.Roles)
                    .UsingEntity<IdentityUserRole<int>>();
            });

            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("user_claims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("user_logins");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("user_tokens");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("role_claims");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("user_roles");

            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.Property(ul => ul.LoginProvider)
                    .HasColumnName("login_provider");

                entity.Property(ul => ul.ProviderKey)
                    .HasColumnName("provider_key");

                entity.Property(ul => ul.ProviderDisplayName)
                    .HasColumnName("provider_dysplay_name");

                entity.Property(ul => ul.UserId)
                    .HasColumnName("id_user");
            });

            modelBuilder.Entity<IdentityUserToken<int>>(entity =>
            {

                entity.Property(ut => ut.LoginProvider)
                    .HasColumnName("login_provider");

                entity.Property(ut => ut.Name)
                    .HasColumnName("token_name");

                entity.Property(ut => ut.UserId)
                    .HasColumnName("id_user");

                entity.Property(ut => ut.Value)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.Property(uc => uc.Id)
                    .HasColumnName("id_user_claims");

                entity.Property(uc => uc.UserId)
                    .HasColumnName("id_user");

                entity.Property(uc => uc.ClaimType)
                    .HasColumnName("claim_type");

                entity.Property(uc => uc.ClaimValue)
                    .HasColumnName("claim_value");
            });

            modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.Property(rc => rc.Id)
                    .HasColumnName("id_role_claims");

                entity.Property(rc => rc.ClaimType)
                    .HasColumnName("claim_type");

                entity.Property(rc => rc.ClaimValue)
                    .HasColumnName("claim_value");

                entity.Property(rc => rc.RoleId)
                    .HasColumnName("id_role");
            });

            modelBuilder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.Property(ur => ur.RoleId)
                    .HasColumnName("id_role");

                entity.Property(ur => ur.UserId)
                    .HasColumnName("id_user");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens");

                // Первичный ключ
                entity.HasKey(rt => rt.IdRefreshToken)
                    .HasName("PRIMARY");

                // Конфигурация свойств
                entity.Property(rt => rt.IdRefreshToken)
                    .HasColumnName("id_refresh_token")
                    .ValueGeneratedOnAdd();

                entity.Property(rt => rt.Token)
                    .HasColumnName("token")
                    .HasMaxLength(512)
                    .IsRequired();

                entity.Property(rt => rt.Expires)
                    .HasColumnName("expires_at")
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(rt => rt.Created)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(rt => rt.CreatedByIp)
                    .HasColumnName("created_by_ip")
                    .HasMaxLength(45);

                entity.Property(rt => rt.Revoked)
                    .HasColumnName("revoked_at")
                    .HasColumnType("datetime");

                entity.Property(rt => rt.RevokedByIp)
                    .HasColumnName("revoked_by_ip")
                    .HasMaxLength(45);

                entity.Property(rt => rt.ReplacedByToken)
                    .HasColumnName("replaced_by_token")
                    .HasMaxLength(512);

                // Внешний ключ к пользователю
                entity.Property(rt => rt.IdUser)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.IdUser)
                    .HasConstraintName("fk_refresh_tokens_users")
                    .OnDelete(DeleteBehavior.Cascade);

                // Индексы
                entity.HasIndex(rt => rt.Token)
                    .HasDatabaseName("ix_refresh_tokens_token")
                    .IsUnique();

                entity.HasIndex(rt => rt.IdUser)
                    .HasDatabaseName("ix_refresh_tokens_user_id");

                // Проверочные ограничения
                entity.ToTable(t => t.HasCheckConstraint(
                    "chk_refresh_tokens_expiration",
                    "expires_at > created_at"));

                entity.ToTable(t => t.HasCheckConstraint(
                    "chk_refresh_tokens_revoked",
                    "revoked_at IS NULL OR revoked_at > created_at"));
            });
        }
    }
}
