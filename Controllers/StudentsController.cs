using CollegeManagementApp.Data;
using CollegeManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementApp.Controllers;

[Authorize]
public class StudentsController : Controller
{
    private readonly ApplicationDbContext _db;
    public StudentsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index() => View(await _db.Students.OrderBy(s => s.FullName).ToListAsync());
    public async Task<IActionResult> Details(int id)
    {
        var student = await _db.Students.FindAsync(id);
        return student is null ? NotFound() : View(student);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new Student());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Student student)
    {
        if (!ModelState.IsValid) return View(student);
        student.CreatedAtUtc = student.UpdatedAtUtc = DateTime.UtcNow;
        _db.Students.Add(student);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var student = await _db.Students.FindAsync(id);
        return student is null ? NotFound() : View(student);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Student input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return View(input);
        var student = await _db.Students.FindAsync(id);
        if (student is null) return NotFound();
        student.FullName = input.FullName;
        student.Email = input.Email;
        student.Department = input.Department;
        student.EnrollmentDate = input.EnrollmentDate;
        student.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _db.Students.FindAsync(id);
        return student is null ? NotFound() : View(student);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student is not null) { _db.Students.Remove(student); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
