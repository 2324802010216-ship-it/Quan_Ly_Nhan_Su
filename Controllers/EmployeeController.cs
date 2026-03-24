using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Models.Enums;
using WebQuanLyNhanSu.Services;
using WebQuanLyNhanSu.ViewModels;

namespace WebQuanLyNhanSu.Controllers;

[Authorize]
public class EmployeeController : BaseController
{
    private readonly IFileUploadService _fileService;

    public EmployeeController(ApplicationDbContext ctx, UserManager<ApplicationUser> um, IFileUploadService fs)
        : base(ctx, um) { _fileService = fs; }

    public async Task<IActionResult> Index(EmployeeFilterViewModel filter)
    {
        var query = _context.Employees
            .Include(e => e.Department).Include(e => e.Position).AsNoTracking();

        if (User.IsInRole("DeptManager"))
        {
            var deptId = await GetCurrentDepartmentId();
            query = query.Where(e => e.DepartmentId == deptId);
        }

        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(e => e.MaNV.Contains(filter.Search) || e.HoTen.Contains(filter.Search));
        if (filter.DepartmentId.HasValue)
            query = query.Where(e => e.DepartmentId == filter.DepartmentId);
        if (filter.PositionId.HasValue)
            query = query.Where(e => e.PositionId == filter.PositionId);
        if (filter.TrangThai.HasValue)
            query = query.Where(e => (int)e.TrangThai == filter.TrangThai);

        var total = await query.CountAsync();
        filter.TotalPages = (int)Math.Ceiling(total / (double)filter.PageSize);
        filter.Employees = await query.OrderBy(e => e.MaNV)
            .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        filter.Departments = await _context.Departments.ToListAsync();
        filter.Positions = await _context.Positions.ToListAsync();

        return View(filter);
    }

    public async Task<IActionResult> Details(int id)
    {
        var emp = await _context.Employees
            .Include(e => e.Department).Include(e => e.Position)
            .Include(e => e.Contracts).Include(e => e.LeaveRequests)
            .Include(e => e.RewardDisciplines).Include(e => e.TrainingEmployees).ThenInclude(te => te.Training)
            .FirstOrDefaultAsync(e => e.Id == id);
        return emp == null ? NotFound() : View(emp);
    }

    [Authorize(Policy = "HRAccess")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View();
    }

    [Authorize(Policy = "HRAccess")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee model, IFormFile? avatar)
    {
        ModelState.Remove("Department"); ModelState.Remove("Position");
        if (!ModelState.IsValid) { await LoadDropdowns(); return View(model); }
        if (avatar != null) model.AnhDaiDien = await _fileService.UploadFile(avatar, "uploads/avatars");
        _context.Employees.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm nhân viên thành công!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "HRAccess")]
    public async Task<IActionResult> Edit(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) return NotFound();
        await LoadDropdowns();
        return View(emp);
    }

    [Authorize(Policy = "HRAccess")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee model, IFormFile? avatar)
    {
        if (id != model.Id) return NotFound();
        ModelState.Remove("Department"); ModelState.Remove("Position");
        if (!ModelState.IsValid) { await LoadDropdowns(); return View(model); }
        if (avatar != null)
        {
            var old = await _context.Employees.AsNoTracking().FirstAsync(e => e.Id == id);
            _fileService.DeleteFile(old.AnhDaiDien);
            model.AnhDaiDien = await _fileService.UploadFile(avatar, "uploads/avatars");
        }
        _context.Employees.Update(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật nhân viên thành công!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) return NotFound();
        _fileService.DeleteFile(emp.AnhDaiDien);
        _context.Employees.Remove(emp);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa nhân viên thành công!";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns()
    {
        ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "TenPhongBan");
        ViewBag.Positions = new SelectList(await _context.Positions.ToListAsync(), "Id", "TenChucVu");
    }
}
