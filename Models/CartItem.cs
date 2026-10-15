using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CollegeManagementApp.Models;

public class CartItem
{
    public int Id { get; set; }
    [Required] public string UserId { get; set; } = "";
    public IdentityUser? User { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;
}
