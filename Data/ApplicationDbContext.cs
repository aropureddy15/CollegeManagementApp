using CollegeManagementApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementApp.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Student>().Property(x => x.FullName).HasMaxLength(120).IsRequired();
        builder.Entity<Student>().Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.Entity<Course>().Property(x => x.Title).HasMaxLength(120).IsRequired();
        builder.Entity<Course>().Property(x => x.Fee).HasPrecision(10, 2);
        builder.Entity<CartItem>().HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();
        builder.Entity<CartItem>().HasOne(x => x.Course).WithMany()
            .HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Cascade);
    }
}
