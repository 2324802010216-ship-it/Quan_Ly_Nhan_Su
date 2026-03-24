using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Models.Enums;
using WebQuanLyNhanSu.Services;

namespace WebQuanLyNhanSu.Controllers;

[Authorize]
public class LeaveRequestController : BaseController
{
    private readonly ILeaveRequestService _leaveService;

    public LeaveRequestController(ApplicationDbContext ctx, UserManager<ApplicationUser> um, ILeaveRequestService ls)
        : base(ctx, um) { _leaveService = ls; }

    public async Task<IActionResult> Index(int? trangThai, int page = 1)
    {
        var query = _context.LeaveRequests.Include(lr => lr.Employee).AsNoTracking();
        if (User.IsInRole("Employee"))
        { var empId = await GetCurrentEmployeeId(); query = query.Where(lr => lr.EmployeeId == empId); }
        else if (User.IsInRole("DeptManager"))
        { var deptId = await GetCurrentDepartmentId(); query = query.Where(lr => lr.Employee.DepartmentId == deptId); }
        if (trangThai.HasValue)
            query = query.Where(lr => (int)lr.TrangThai == trangThai);
        var total = await query.CountAsync();
        ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 20.0); ViewBag.TrangThai = trangThai;
        return View(await query.OrderByDescending(lr => lr.NgayBatDau).Skip((page - 1) * 20).Take(20).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var lr = await _context.LeaveRequests.Include(l => l.Employee).ThenInclude(e => e.Department)
            .Include(l => l.NguoiDuyet).FirstOrDefaultAsync(l => l.Id == id);
        if (lr == null) return NotFound();

        var balance = await _leaveService.GetLeaveBalance(lr.EmployeeId, lr.NgayBatDau.Year);
        ViewBag.Balance = balance;
        return View(lr);
    }

    public async Task<IActionResult> Create()
    {
        if (User.IsInRole("Employee"))
        {
            var empId = await GetCurrentEmployeeId();
            if (empId.HasValue)
            {
                var balance = await _leaveService.GetLeaveBalance(empId.Value, DateTime.Now.Year);
                ViewBag.Balance = balance;
            }
        }
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveRequest model)
    {
        if (User.IsInRole("Employee"))
        {
            var empId = await GetCurrentEmployeeId();
            model.EmployeeId = empId ?? 0;
        }
        model.SoNgay = (model.NgayKetThuc - model.NgayBatDau).Days + 1;

        // Check leave balance for NghiPhepNam
        if (model.LoaiNghi == LoaiNghiPhep.NghiPhepNam)
        {
            var balance = await _leaveService.GetLeaveBalance(model.EmployeeId, model.NgayBatDau.Year);
            if (model.SoNgay > balance.SoNgayConLai)
            {
                TempData["Error"] = $"Số ngày phép còn lại: {balance.SoNgayConLai}, không đủ!";
                return View(model);
            }
        }

        ModelState.Remove("Employee"); ModelState.Remove("NguoiDuyet");
        if (!ModelState.IsValid) return View(model);
        _context.LeaveRequests.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Gửi đơn nghỉ phép thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var lr = await _context.LeaveRequests.FindAsync(id);
        if (lr == null) return NotFound();
        if (lr.TrangThai != TrangThaiDonNghi.ChoDuyet)
        {
            TempData["Error"] = "Chỉ có thể sửa đơn đang chờ duyệt!";
            return RedirectToAction(nameof(Details), new { id });
        }
        return View(lr);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LeaveRequest model)
    {
        if (id != model.Id) return NotFound();
        var existing = await _context.LeaveRequests.FindAsync(id);
        if (existing == null) return NotFound();
        if (existing.TrangThai != TrangThaiDonNghi.ChoDuyet)
        {
            TempData["Error"] = "Chỉ có thể sửa đơn đang chờ duyệt!";
            return RedirectToAction(nameof(Details), new { id });
        }

        existing.LoaiNghi = model.LoaiNghi;
        existing.NgayBatDau = model.NgayBatDau;
        existing.NgayKetThuc = model.NgayKetThuc;
        existing.SoNgay = (model.NgayKetThuc - model.NgayBatDau).Days + 1;
        existing.LyDo = model.LyDo;

        // Check leave balance for NghiPhepNam
        if (existing.LoaiNghi == LoaiNghiPhep.NghiPhepNam)
        {
            var balance = await _leaveService.GetLeaveBalance(existing.EmployeeId, existing.NgayBatDau.Year);
            if (existing.SoNgay > balance.SoNgayConLai)
            {
                TempData["Error"] = $"Số ngày phép còn lại: {balance.SoNgayConLai}, không đủ!";
                return View(model);
            }
        }

        ModelState.Remove("Employee"); ModelState.Remove("NguoiDuyet");
        if (!ModelState.IsValid) return View(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật đơn nghỉ phép thành công!";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "CanApproveLeave")]
    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        var approverId = (await GetCurrentEmployeeId()) ?? 0;
        var result = await _leaveService.ApproveLeaveRequest(id, approverId);
        TempData[result ? "Success" : "Error"] = result ? "Đã duyệt đơn!" : "Không thể duyệt!";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = "CanApproveLeave")]
    [HttpPost]
    public async Task<IActionResult> Reject(int id, string lyDoTuChoi)
    {
        var result = await _leaveService.RejectLeaveRequest(id, lyDoTuChoi);
        TempData[result ? "Success" : "Error"] = result ? "Đã từ chối đơn!" : "Không thể từ chối!";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var lr = await _context.LeaveRequests.FindAsync(id);
        if (lr == null) return NotFound();
        if (lr.TrangThai != TrangThaiDonNghi.ChoDuyet)
        {
            TempData["Error"] = "Chỉ có thể xóa đơn đang chờ duyệt!";
            return RedirectToAction(nameof(Index));
        }
        _context.LeaveRequests.Remove(lr);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa đơn nghỉ phép thành công!";
        return RedirectToAction(nameof(Index));
    }
}
