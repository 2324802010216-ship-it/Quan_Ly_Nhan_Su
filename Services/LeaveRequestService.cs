using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Models.Enums;
using WebQuanLyNhanSu.ViewModels;

namespace WebQuanLyNhanSu.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ApplicationDbContext _context;
    public LeaveRequestService(ApplicationDbContext context) => _context = context;

    public async Task<LeaveBalanceViewModel> GetLeaveBalance(int employeeId, int nam)
    {
        var emp = await _context.Employees.FindAsync(employeeId);
        if (emp == null) return new LeaveBalanceViewModel();

        // ★ Fix #6: Chỉ đếm NghiPhepNam, không đếm NghiOm/NghiKhongLuong
        var daDung = await _context.LeaveRequests
            .Where(lr => lr.EmployeeId == employeeId
                && lr.NgayBatDau.Year == nam
                && lr.LoaiNghi == LoaiNghiPhep.NghiPhepNam
                && (lr.TrangThai == TrangThaiDonNghi.DaDuyet || lr.TrangThai == TrangThaiDonNghi.ChoDuyet))
            .SumAsync(lr => lr.SoNgay);

        return new LeaveBalanceViewModel
        {
            EmployeeId = employeeId,
            HoTen = emp.HoTen,
            SoNgayPhepNam = 12,
            SoNgayDaDung = daDung
        };
    }

    public async Task<bool> ApproveLeaveRequest(int leaveId, int approverId)
    {
        var leave = await _context.LeaveRequests.FindAsync(leaveId);
        if (leave == null || leave.TrangThai != TrangThaiDonNghi.ChoDuyet) return false;

        leave.TrangThai = TrangThaiDonNghi.DaDuyet;
        leave.NguoiDuyetId = approverId;
        leave.NgayDuyet = DateTime.Now;

        // ★ Fix #9: Tự động tạo Attendance NghiPhep cho mỗi ngày
        for (var d = leave.NgayBatDau; d <= leave.NgayKetThuc; d = d.AddDays(1))
        {
            if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                continue;

            var exists = await _context.Attendances
                .AnyAsync(a => a.EmployeeId == leave.EmployeeId && a.Ngay.Date == d.Date);
            if (!exists)
            {
                _context.Attendances.Add(new Attendance
                {
                    EmployeeId = leave.EmployeeId,
                    Ngay = d.Date,
                    TrangThai = TrangThaiChamCong.NghiPhep,
                    GhiChu = $"Nghỉ phép (Đơn #{leave.Id})"
                });
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectLeaveRequest(int leaveId, string lyDo)
    {
        var leave = await _context.LeaveRequests.FindAsync(leaveId);
        if (leave == null || leave.TrangThai != TrangThaiDonNghi.ChoDuyet) return false;

        leave.TrangThai = TrangThaiDonNghi.TuChoi;
        leave.LyDoTuChoi = lyDo;
        leave.NgayDuyet = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
}
