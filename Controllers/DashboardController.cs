using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Models.Enums;
using WebQuanLyNhanSu.Services;
using WebQuanLyNhanSu.ViewModels;

namespace WebQuanLyNhanSu.Controllers;

[Authorize]
public class DashboardController : BaseController
{
    public DashboardController(ApplicationDbContext ctx, UserManager<ApplicationUser> um)
        : base(ctx, um) { }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            TongNhanVien = await _context.Employees.CountAsync(e => e.TrangThai == TrangThaiNhanVien.DangLamViec),
            TongPhongBan = await _context.Departments.CountAsync(),
            HopDongSapHetHan = await _context.Contracts.CountAsync(c =>
                c.TrangThai == TrangThaiHopDong.HieuLuc && c.NgayKetThuc != null
                && c.NgayKetThuc < DateTime.Today.AddDays(30)),
            DonPhepChoDuyet = await _context.LeaveRequests.CountAsync(lr =>
                lr.TrangThai == TrangThaiDonNghi.ChoDuyet),
            HopDongCanhBao = await _context.Contracts
                .Include(c => c.Employee)
                .Where(c => c.TrangThai == TrangThaiHopDong.HieuLuc && c.NgayKetThuc != null
                    && c.NgayKetThuc < DateTime.Today.AddDays(30))
                .OrderBy(c => c.NgayKetThuc).Take(5).ToListAsync(),
            DonNghiChoDuyet = await _context.LeaveRequests
                .Include(lr => lr.Employee)
                .Where(lr => lr.TrangThai == TrangThaiDonNghi.ChoDuyet)
                .OrderByDescending(lr => lr.NgayBatDau).Take(5).ToListAsync(),
        };

        // Chart data
        var depts = await _context.Departments
            .Select(d => new { d.TenPhongBan, Count = d.Employees.Count(e => e.TrangThai == TrangThaiNhanVien.DangLamViec) })
            .ToListAsync();
        model.TenPhongBans = depts.Select(d => d.TenPhongBan).ToList();
        model.SoNVTheoPhong = depts.Select(d => d.Count).ToList();

        var now = DateTime.Now;
        model.ThangLabels = Enumerable.Range(0, 6).Select(i => now.AddMonths(-i).ToString("MM/yyyy")).Reverse().ToList();
        model.SoTuyenDungTheoThang = Enumerable.Range(0, 6).Select(i =>
        {
            var m = now.AddMonths(-i);
            return _context.Recruitments.Count(r => r.NgayDang.Month == m.Month && r.NgayDang.Year == m.Year);
        }).Reverse().ToList();

        return View(model);
    }

    [AllowAnonymous]
    public IActionResult Error(int? code)
    {
        ViewBag.Code = code;
        var viewName = code switch
        {
            403 => "~/Views/Shared/Error403.cshtml",
            404 => "~/Views/Shared/Error404.cshtml",
            500 => "~/Views/Shared/Error500.cshtml",
            _ => "~/Views/Shared/Error.cshtml"
        };
        return View(viewName);
    }
}
