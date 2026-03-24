using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "HRAccess")]
public class DepartmentController : BaseController
{
    public DepartmentController(ApplicationDbContext ctx, UserManager<ApplicationUser> um) : base(ctx, um) { }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Departments.Include(d => d.TruongPhong).Include(d => d.Employees).AsQueryable();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(d => d.TenPhongBan.Contains(search));
        var total = await query.CountAsync();
        ViewBag.Search = search; ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 15.0);
        return View(await query.OrderBy(d => d.TenPhongBan).Skip((page - 1) * 15).Take(15).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var dept = await _context.Departments.Include(d => d.TruongPhong)
            .Include(d => d.Employees).ThenInclude(e => e.Position)
            .FirstOrDefaultAsync(d => d.Id == id);
        return dept == null ? NotFound() : View(dept);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Department model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Departments.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm phòng ban thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dept = await _context.Departments.FindAsync(id);
        return dept == null ? NotFound() : View(dept);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Department model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        _context.Departments.Update(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật phòng ban thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var dept = await _context.Departments.Include(d => d.Employees).FirstOrDefaultAsync(d => d.Id == id);
        if (dept == null) return NotFound();
        if (dept.Employees.Any()) { TempData["Error"] = "Không thể xóa — phòng ban còn nhân viên!"; return RedirectToAction(nameof(Index)); }
        _context.Departments.Remove(dept);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa phòng ban thành công!";
        return RedirectToAction(nameof(Index));
    }
}
