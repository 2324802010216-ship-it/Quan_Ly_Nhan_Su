using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "HRAccess")]
public class TrainingController : BaseController
{
    public TrainingController(ApplicationDbContext ctx, UserManager<ApplicationUser> um) : base(ctx, um) { }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _context.Trainings.Include(t => t.TrainingEmployees).AsNoTracking();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(t => t.TenKhoaHoc.Contains(search));
        var total = await query.CountAsync();
        ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 15.0); ViewBag.Search = search;
        return View(await query.OrderByDescending(t => t.NgayBatDau).Skip((page - 1) * 15).Take(15).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var t = await _context.Trainings.Include(t => t.TrainingEmployees).ThenInclude(te => te.Employee)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (t == null) return NotFound();
        var enrolledIds = t.TrainingEmployees.Select(te => te.EmployeeId).ToList();
        ViewBag.Employees = await _context.Employees
            .Where(e => !enrolledIds.Contains(e.Id))
            .OrderBy(e => e.MaNV).ToListAsync();
        return View(t);
    }

    public IActionResult Create() => View(new Training { NgayBatDau = DateTime.Today, NgayKetThuc = DateTime.Today.AddMonths(1) });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Training model)
    {
        if (!ModelState.IsValid) return View(model);
        _context.Trainings.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm khóa đào tạo thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var t = await _context.Trainings.FindAsync(id);
        return t == null ? NotFound() : View(t);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Training model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);
        _context.Trainings.Update(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Enroll(int trainingId, int employeeId)
    {
        var exists = await _context.TrainingEmployees
            .AnyAsync(te => te.TrainingId == trainingId && te.EmployeeId == employeeId);
        if (exists) { TempData["Error"] = "Nhân viên đã đăng ký!"; return RedirectToAction(nameof(Details), new { id = trainingId }); }

        _context.TrainingEmployees.Add(new TrainingEmployee { TrainingId = trainingId, EmployeeId = employeeId });
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đăng ký thành công!";
        return RedirectToAction(nameof(Details), new { id = trainingId });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateResult(int id, string? ketQua, decimal? diemSo)
    {
        var te = await _context.TrainingEmployees.FindAsync(id);
        if (te == null) return NotFound();
        te.KetQua = ketQua; te.DiemSo = diemSo;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật kết quả thành công!";
        return RedirectToAction(nameof(Details), new { id = te.TrainingId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var t = await _context.Trainings.Include(t => t.TrainingEmployees).FirstOrDefaultAsync(t => t.Id == id);
        if (t == null) return NotFound();
        if (t.TrainingEmployees.Any())
        {
            TempData["Error"] = "Không thể xóa — khóa đào tạo còn học viên!";
            return RedirectToAction(nameof(Index));
        }
        _context.Trainings.Remove(t);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa khóa đào tạo thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Unenroll(int id)
    {
        var te = await _context.TrainingEmployees.FindAsync(id);
        if (te == null) return NotFound();
        var trainingId = te.TrainingId;
        _context.TrainingEmployees.Remove(te);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Hủy đăng ký thành công!";
        return RedirectToAction(nameof(Details), new { id = trainingId });
    }
}
