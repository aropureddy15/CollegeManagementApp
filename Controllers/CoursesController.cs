using CollegeManagementApp.Data;
using CollegeManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementApp.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly ApplicationDbContext _db;
    public CoursesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index() => View(await _db.Courses.OrderBy(c => c.Title).ToListAsync());
    public async Task<IActionResult> Details(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        return course is null ? NotFound() : View(course);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new Course());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid) return View(course);
        course.CreatedAtUtc = course.UpdatedAtUtc = DateTime.UtcNow;
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        return course is null ? NotFound() : View(course);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Course input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return View(input);
        var course = await _db.Courses.FindAsync(id);
        if (course is null) return NotFound();
        course.Title = input.Title;
        course.Department = input.Department;
        course.Fee = input.Fee;
        course.StartDate = input.StartDate;
        course.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        return course is null ? NotFound() : View(course);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course is not null) { _db.Courses.Remove(course); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
