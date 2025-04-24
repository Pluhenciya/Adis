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

                entity.HasIndex(p => p.Status)
                    .HasDatabaseName("ix_projects_status");

                entity.HasIndex(p => new { p.StartDate, p.EndDate })
                    .HasDatabaseName("ix_projects_dates");

                entity.ToTable(t => t.HasCheckConstraint("chk_projects_budget", "budget >= 0"));

                entity.ToTable(t => t.HasCheckConstraint("chk_projects_dates", "start_date <= end_date"));
            });
        }
    }
}
