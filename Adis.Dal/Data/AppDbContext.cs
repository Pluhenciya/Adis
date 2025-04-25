using Adis.Dm;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Adis.Dal.Data
{
    /// <summary>
    /// Позволяет использовать EFCore
    /// </summary>
    public class AppDbContext : DbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("projects");

                entity.HasKey(p => p.IdProduct)
                    .HasName("PRIMARY");

                entity.Property(p => p.IdProduct)
                    .HasColumnName("id_product");

                entity.Property(p => p.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(p => p.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");

                entity.Property(p => p.Budget)
                    .HasColumnName("budget")
                    .HasColumnType("DECIMAL(15,2)")
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
                    .HasConversion(new EnumToStringConverter<Status>())
                    .HasColumnType("enum('draft', 'inProgress', 'completed', 'overdue')")
                    .IsRequired();

                entity.Property(p => p.IdUser)
                    .HasColumnName("id_user")
                    .IsRequired();

                entity.HasIndex(p => p.Status)
                    .HasDatabaseName("ix_projects_status");

                entity.HasIndex(p => new { p.StartDate, p.EndDate })
                    .HasDatabaseName("ix_projects_dates");

                entity.ToTable(t => t.HasCheckConstraint("chk_projects_budget", "budget >= 0"));

                entity.ToTable(t => t.HasCheckConstraint("chk_projects_dates", "start_date <= end_date"));
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(u => u.IdUser)
                    .HasName("PRIMARY");

                entity.Property(u => u.IdUser)
                    .HasColumnName("id_user")
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(u => u.PasswordHash)
                    .HasColumnName("password_hash")
                    .HasMaxLength(256) 
                    .IsRequired();

                entity.Property(u => u.Role)
                    .HasColumnName("role")
                    .HasConversion(new EnumToStringConverter<Role>())
                    .HasColumnType("enum('admin', 'projecter')")
                    .IsRequired();

                entity.Property(u => u.FullName)
                    .HasColumnName("full_name")
                    .HasMaxLength(255);

                entity.Property(u => u.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd();

                entity.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("ix_users_email");

                entity.HasIndex(u => u.Role)
                    .HasDatabaseName("ix_users_role");

                entity.ToTable(t => t.HasCheckConstraint(
                    "chk_users_email_format",
                    "email LIKE '%@%'"));

                entity.HasMany(u => u.Projects)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.IdUser);
            });
        }
    }
}
