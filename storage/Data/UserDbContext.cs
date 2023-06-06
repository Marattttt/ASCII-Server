using Microsoft.EntityFrameworkCore;

using storage.Models;

namespace storage.Data;

public class UserDbContext : DbContext {
    public required DbSet<User> Users { get; set; }

    public UserDbContext (DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(
            usr => {
                usr.HasKey(user => user.UserId);
                usr.HasMany(user => user.Uploads)
                    .WithOne(img => img.Owner)
                    .HasForeignKey(img => img.UserId)
                    .IsRequired();
            }
        );
        modelBuilder.Entity<ImageData>(
            img => {
                img.HasKey(img => img.ImageId);
            }
        );
    }
}