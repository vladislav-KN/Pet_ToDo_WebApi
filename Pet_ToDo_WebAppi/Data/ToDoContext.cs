using Microsoft.EntityFrameworkCore;
using Pet_ToDo_WebApi.Models;

namespace Pet_ToDo_WebApi.Data
{
    public class ToDoContext: DbContext
    {
        public DbSet<HashPasswordModel> HashPasswords { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<UserModel> Users { get; set; }
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
            modelBuilder.Entity<UserModel>()
                        .HasOne(e => e.Password)
                        .WithOne(e => e.User)
                        .HasForeignKey<HashPasswordModel>();

            modelBuilder.Entity<UserModel>()
                        .HasMany(e => e.Tasks)
                        .WithOne(e => e.Owner)
                        .HasForeignKey("UserId")
                        .IsRequired();

            modelBuilder.Entity<UserModel>()
                        .HasIndex(u => u.Name)
                        .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}
