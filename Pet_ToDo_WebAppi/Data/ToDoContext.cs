using Microsoft.EntityFrameworkCore;
using Pet_ToDo_WebApi.Models;

namespace Pet_ToDo_WebApi.Data
{
    public class ToDoContext: DbContext
    {
        public DbSet<HashPasswordModel> HashPasswords { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<UserModel> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
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
        }
    }
}
