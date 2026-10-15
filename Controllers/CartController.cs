using System.Security.Claims;
using CollegeManagementApp.Data;
using CollegeManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagementApp.Controllers;

[Authorize(Roles = "Admin")]
public class CartController : Controller
{
    private readonly ApplicationDbContext _db;
    public CartController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var items = await _db.CartItems.Include(x => x.Course)
            .Where(x => x.UserId == userId).OrderByDescending(x => x.AddedAtUtc).ToListAsync();
        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int courseId)
    {
        var course = await _db.Courses.FindAsync(courseId);
        if (course is null) return NotFound();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var exists = await _db.CartItems.AnyAsync(x => x.UserId == userId && x.CourseId == courseId);
        if (!exists)
            _db.CartItems.Add(new CartItem { UserId = userId, CourseId = courseId, AddedAtUtc = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var item = await _db.CartItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (item is not null) { _db.CartItems.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
