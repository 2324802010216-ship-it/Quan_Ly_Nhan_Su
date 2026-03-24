using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Services;
using WebQuanLyNhanSu.ViewModels;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "HRAccess")]
public class SalaryController : BaseController
{
    private readonly ISalaryService _salaryService;

    public SalaryController(ApplicationDbContext ctx, UserManager<ApplicationUser> um, ISalaryService ss)
        : base(ctx, um) { _salaryService = ss; }

    public async Task<IActionResult> Index(int? thang, int? nam, int page = 1)
    {
        thang ??= DateTime.Now.Month; nam ??= DateTime.Now.Year;
        var query = _context.Salaries
            .Include(s => s.Employee).ThenInclude(e => e.Department)
            .Where(s => s.Thang == thang && s.Nam == nam)
            .OrderBy(s => s.Employee.MaNV).AsNoTracking();
        var total = await query.CountAsync();
        ViewBag.Thang = thang; ViewBag.Nam = nam; ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 20.0);
        return View(await query.Skip((page - 1) * 20).Take(20).ToListAsync());
    }

    public async Task<IActionResult> Calculate()
    {
        var model = new SalaryCalculationViewModel
        {
            Employees = await _context.Employees.OrderBy(e => e.MaNV).ToListAsync()
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Calculate(SalaryCalculationViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        var salary = await _salaryService.CalculateSalary(
            model.EmployeeId, model.Thang, model.Nam,
            model.PhuCap, model.SoGioTangCa, model.KhauTru,
            user?.HoTen ?? "System");
        TempData["Success"] = "Tính lương thành công!";
        return RedirectToAction(nameof(PaySlip), new { id = salary.Id });
    }

    public async Task<IActionResult> PaySlip(int id)
    {
        var salary = await _context.Salaries
            .Include(s => s.Employee).ThenInclude(e => e.Department)
            .Include(s => s.Employee).ThenInclude(e => e.Position)
            .FirstOrDefaultAsync(s => s.Id == id);
        return salary == null ? NotFound() : View(salary);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var salary = await _context.Salaries.Include(s => s.Employee).FirstOrDefaultAsync(s => s.Id == id);
        return salary == null ? NotFound() : View(salary);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, decimal phuCap, decimal tangCa, decimal khauTru)
    {
        var salary = await _context.Salaries.FindAsync(id);
        if (salary == null) return NotFound();
        salary.PhuCap = phuCap;
        salary.TangCa = tangCa;
        salary.KhauTru = khauTru;
        salary.ThucLanh = salary.LuongCoBan + phuCap + tangCa - salary.BHXH - salary.BHYT - salary.BHTN - khauTru;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật bảng lương thành công!";
        return RedirectToAction(nameof(PaySlip), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var salary = await _context.Salaries.FindAsync(id);
        if (salary == null) return NotFound();
        _context.Salaries.Remove(salary);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa bảng lương thành công!";
        return RedirectToAction(nameof(Index));
    }
}
