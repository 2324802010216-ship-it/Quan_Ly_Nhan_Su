using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "HRAccess")]
public class RecruitmentController : BaseController
{
    public RecruitmentController(ApplicationDbContext ctx, UserManager<ApplicationUser> um) : base(ctx, um) { }

    public async Task<IActionResult> Index(int? trangThai, int page = 1)
    {
        var query = _context.Recruitments.Include(r => r.Department).Include(r => r.Position)
            .Include(r => r.Candidates).AsNoTracking();
        if (trangThai.HasValue)
            query = query.Where(r => (int)r.TrangThai == trangThai);
        var total = await query.CountAsync();
        ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 15.0); ViewBag.TrangThai = trangThai;
        return View(await query.OrderByDescending(r => r.NgayDang).Skip((page - 1) * 15).Take(15).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var r = await _context.Recruitments.Include(r => r.Department).Include(r => r.Position)
            .Include(r => r.Candidates).FirstOrDefaultAsync(r => r.Id == id);
        return r == null ? NotFound() : View(r);
    }

    public async Task<IActionResult> Create()
    {
        await LoadDropdowns(); return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Recruitment model)
    {
        ModelState.Remove("Department"); ModelState.Remove("Position"); ModelState.Remove("Candidates");
        if (!ModelState.IsValid) { await LoadDropdowns(); return View(model); }
        _context.Recruitments.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm tin tuyển dụng thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var r = await _context.Recruitments.FindAsync(id);
        if (r == null) return NotFound();
        await LoadDropdowns(); return View(r);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Recruitment model)
    {
        if (id != model.Id) return NotFound();
        ModelState.Remove("Department"); ModelState.Remove("Position"); ModelState.Remove("Candidates");
        if (!ModelState.IsValid) { await LoadDropdowns(); return View(model); }
        _context.Recruitments.Update(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var r = await _context.Recruitments.Include(r => r.Candidates).FirstOrDefaultAsync(r => r.Id == id);
        if (r == null) return NotFound();
        if (r.Candidates.Any())
        {
            TempData["Error"] = "Không thể xóa — tin tuyển dụng còn ứng viên!";
            return RedirectToAction(nameof(Index));
        }
        _context.Recruitments.Remove(r);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa tin tuyển dụng thành công!";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns()
    {
        ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "TenPhongBan");
        ViewBag.Positions = new SelectList(await _context.Positions.ToListAsync(), "Id", "TenChucVu");
    }
}
