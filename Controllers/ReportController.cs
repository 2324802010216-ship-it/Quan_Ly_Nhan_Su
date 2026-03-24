using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Services;
using WebQuanLyNhanSu.ViewModels;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "HRAccess")]
public class ReportController : BaseController
{
    private readonly IReportService _reportService;

    public ReportController(ApplicationDbContext ctx, UserManager<ApplicationUser> um, IReportService rs)
        : base(ctx, um) { _reportService = rs; }

    public async Task<IActionResult> Index()
    {
        ViewBag.Departments = await _context.Departments.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Export(ReportExportViewModel model)
    {
        byte[] data;
        string fileName;
        switch (model.ReportType)
        {
            case "employees":
                data = await _reportService.ExportEmployeeReport(model.DepartmentId);
                fileName = "BaoCao_NhanVien.xlsx";
                break;
            case "salary":
                data = await _reportService.ExportSalaryReport(model.Thang ?? DateTime.Now.Month, model.Nam ?? DateTime.Now.Year);
                fileName = $"BaoCao_Luong_T{model.Thang}_{model.Nam}.xlsx";
                break;
            case "attendance":
                data = await _reportService.ExportAttendanceReport(model.Thang ?? DateTime.Now.Month, model.Nam ?? DateTime.Now.Year, model.DepartmentId);
                fileName = $"BaoCao_ChamCong_T{model.Thang}_{model.Nam}.xlsx";
                break;
            case "leave":
                data = await _reportService.ExportLeaveReport(model.Thang, model.Nam, model.DepartmentId);
                fileName = $"BaoCao_NghiPhep_T{model.Thang}_{model.Nam}.xlsx";
                break;
            default:
                TempData["Error"] = "Loại báo cáo không hợp lệ!";
                return RedirectToAction(nameof(Index));
        }
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
