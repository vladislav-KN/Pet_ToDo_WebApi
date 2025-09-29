using Microsoft.EntityFrameworkCore;
using Pet_ToDo_WebApi.Entities;

namespace Pet_ToDo_WebApi.Data
{
    public class ToDoContext: DbContext
    {
        public DbSet<HashPasswordEntity> HashPasswords { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public string DbPath { get; }

        public ToDoContext()
        {
            var folder = Environment.CurrentDirectory; 
            DbPath = System.IO.Path.Join(folder, "DataBase\\ToDo.db");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>()
                        .HasOne(e => e.Password)
                        .WithOne(e => e.User)
                        .HasForeignKey<HashPasswordEntity>();

            modelBuilder.Entity<UserEntity>()
                        .HasMany(e => e.Tasks)
                        .WithOne(e => e.Owner)
                        .HasForeignKey("UserId")
                        .IsRequired();

            modelBuilder.Entity<UserEntity>()
                        .HasIndex(u => u.Name)
                        .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}
