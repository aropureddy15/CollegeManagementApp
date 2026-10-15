using System.ComponentModel.DataAnnotations;

namespace CollegeManagementApp.Models;

public class Student
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string FullName { get; set; } = "";
    [Required, EmailAddress, StringLength(256)] public string Email { get; set; } = "";
    [StringLength(80)] public string? Department { get; set; }
    [DataType(DataType.Date)] public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow.Date;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
