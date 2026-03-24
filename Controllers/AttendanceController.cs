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
public class AttendanceController : BaseController
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(ApplicationDbContext ctx, UserManager<ApplicationUser> um, IAttendanceService svc)
        : base(ctx, um) { _attendanceService = svc; }

    public async Task<IActionResult> Index(int? thang, int? nam, int page = 1)
    {
        thang ??= DateTime.Now.Month; nam ??= DateTime.Now.Year;
        var query = _context.Attendances
            .Include(a => a.Employee).ThenInclude(e => e.Department)
            .Where(a => a.Ngay.Month == thang && a.Ngay.Year == nam).AsNoTracking();

        if (User.IsInRole("Employee"))
        { var empId = await GetCurrentEmployeeId(); query = query.Where(a => a.EmployeeId == empId); }
        else if (User.IsInRole("DeptManager"))
        { var deptId = await GetCurrentDepartmentId(); query = query.Where(a => a.Employee.DepartmentId == deptId); }

        var total = await query.CountAsync();
        ViewBag.Thang = thang; ViewBag.Nam = nam; ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 20.0);
        return View(await query.OrderByDescending(a => a.Ngay).Skip((page - 1) * 20).Take(20).ToListAsync());
    }

    [Authorize(Policy = "ManagerAccess")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
        return View(new Attendance { Ngay = DateTime.Today });
    }

    [Authorize(Policy = "ManagerAccess")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Attendance model)
    {
        var exists = await _context.Attendances
            .AnyAsync(a => a.EmployeeId == model.EmployeeId && a.Ngay.Date == model.Ngay.Date);
        if (exists) { TempData["Error"] = "Đã có bản ghi chấm công ngày này!"; return RedirectToAction(nameof(Create)); }

        if (model.GioVao != null && model.GioVao > new TimeOnly(8, 15))
            model.TrangThai = TrangThaiChamCong.TreMuon;

        ModelState.Remove("Employee");
        if (!ModelState.IsValid)
        {
            ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
            return View(model);
        }
        _context.Attendances.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Chấm công thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> MonthlyReport(int? thang, int? nam, int? departmentId)
    {
        thang ??= DateTime.Now.Month; nam ??= DateTime.Now.Year;
        var model = await _attendanceService.GetMonthlyReport(thang.Value, nam.Value, departmentId);
        if (User.IsInRole("DeptManager"))
        {
            var deptId = await GetCurrentDepartmentId();
            model.Summaries = model.Summaries.Where(s => s.Employee.DepartmentId == deptId).ToList();
        }
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var att = await _context.Attendances.Include(a => a.Employee)
            .ThenInclude(e => e.Department).FirstOrDefaultAsync(a => a.Id == id);
        return att == null ? NotFound() : View(att);
    }

    [Authorize(Policy = "ManagerAccess")]
    public async Task<IActionResult> Edit(int id)
    {
        var att = await _context.Attendances.FindAsync(id);
        if (att == null) return NotFound();
        ViewBag.Employees = new SelectList(
            await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
        return View(att);
    }

    [Authorize(Policy = "ManagerAccess")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Attendance model)
    {
        if (id != model.Id) return NotFound();
        if (model.GioVao != null && model.GioVao > new TimeOnly(8, 15))
            model.TrangThai = TrangThaiChamCong.TreMuon;
        ModelState.Remove("Employee");
        if (!ModelState.IsValid)
        {
            ViewBag.Employees = new SelectList(
                await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
            return View(model);
        }
        _context.Attendances.Update(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật chấm công thành công!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "ManagerAccess")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var att = await _context.Attendances.FindAsync(id);
        if (att == null) return NotFound();
        _context.Attendances.Remove(att);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa bản ghi chấm công thành công!";
        return RedirectToAction(nameof(Index));
    }
}
