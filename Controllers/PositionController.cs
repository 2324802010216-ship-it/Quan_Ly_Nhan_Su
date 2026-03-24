using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "HRAccess")]
public class PositionController : BaseController
{
    public PositionController(ApplicationDbContext ctx, UserManager<ApplicationUser> um) : base(ctx, um) { }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Positions.Include(p => p.Employees).AsQueryable();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.TenChucVu.Contains(search));
        var total = await query.CountAsync();
        ViewBag.Search = search; ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 15.0);
        return View(await query.OrderBy(p => p.CapBac).Skip((page - 1) * 15).Take(15).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var pos = await _context.Positions.Include(p => p.Employees).ThenInclude(e => e.Department)
            .FirstOrDefaultAsync(p => p.Id == id);
        return pos == null ? NotFound() : View(pos);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Position model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Positions.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm chức vụ thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var pos = await _context.Positions.FindAsync(id);
        return pos == null ? NotFound() : View(pos);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Position model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        _context.Positions.Update(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật chức vụ thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pos = await _context.Positions.Include(p => p.Employees).FirstOrDefaultAsync(p => p.Id == id);
        if (pos == null) return NotFound();
        if (pos.Employees.Any()) { TempData["Error"] = "Không thể xóa — chức vụ đang có nhân viên!"; return RedirectToAction(nameof(Index)); }
        _context.Positions.Remove(pos);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa chức vụ thành công!";
        return RedirectToAction(nameof(Index));
    }
}
