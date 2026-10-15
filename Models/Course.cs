using System.ComponentModel.DataAnnotations;

namespace CollegeManagementApp.Models;

public class Course
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string Title { get; set; } = "";
    [StringLength(80)] public string? Department { get; set; }
    [Range(0, 10000000)] public decimal Fee { get; set; }
    [DataType(DataType.Date)] public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
