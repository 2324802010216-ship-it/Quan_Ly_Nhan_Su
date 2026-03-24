using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "HRAccess")]
public class RewardDisciplineController : BaseController
{
    public RewardDisciplineController(ApplicationDbContext ctx, UserManager<ApplicationUser> um) : base(ctx, um) { }

    public async Task<IActionResult> Index(int? loai, int page = 1)
    {
        var query = _context.RewardDisciplines.Include(r => r.Employee).AsNoTracking();
        if (loai.HasValue)
            query = query.Where(r => (int)r.Loai == loai);
        var total = await query.CountAsync();
        ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 20.0); ViewBag.Loai = loai;
        return View(await query.OrderByDescending(r => r.NgayQuyetDinh).Skip((page - 1) * 20).Take(20).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var rd = await _context.RewardDisciplines.Include(r => r.Employee)
            .FirstOrDefaultAsync(r => r.Id == id);
        return rd == null ? NotFound() : View(rd);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RewardDiscipline model)
    {
        ModelState.Remove("Employee");
        if (!ModelState.IsValid)
        {
            ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
            return View(model);
        }
        _context.RewardDisciplines.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var rd = await _context.RewardDisciplines.FindAsync(id);
        if (rd == null) return NotFound();
        ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
        return View(rd);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RewardDiscipline model)
    {
        if (id != model.Id) return NotFound();
        ModelState.Remove("Employee");
        if (!ModelState.IsValid)
        {
            ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
            return View(model);
        }
        _context.RewardDisciplines.Update(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var rd = await _context.RewardDisciplines.FindAsync(id);
        if (rd == null) return NotFound();
        _context.RewardDisciplines.Remove(rd);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa thành công!";
        return RedirectToAction(nameof(Index));
    }
}
