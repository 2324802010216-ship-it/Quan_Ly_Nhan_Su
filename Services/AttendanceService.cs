using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models.Enums;
using WebQuanLyNhanSu.ViewModels;

namespace WebQuanLyNhanSu.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _context;
    public AttendanceService(ApplicationDbContext context) => _context = context;

    public async Task<AttendanceReportViewModel> GetMonthlyReport(int thang, int nam, int? deptId)
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Attendances.Where(a => a.Ngay.Month == thang && a.Ngay.Year == nam))
            .AsNoTracking();

        if (deptId.HasValue)
            query = query.Where(e => e.DepartmentId == deptId);

        var employees = await query.ToListAsync();

        var summaries = employees.Select(e => new AttendanceSummary
        {
            Employee = e,
            NgayDiLam = e.Attendances.Count(a => a.TrangThai == TrangThaiChamCong.DungGio),
            NgayTreMuon = e.Attendances.Count(a => a.TrangThai == TrangThaiChamCong.TreMuon),
            NgayVangMat = e.Attendances.Count(a => a.TrangThai == TrangThaiChamCong.VangMat),
            NgayNghiPhep = e.Attendances.Count(a => a.TrangThai == TrangThaiChamCong.NghiPhep),
            NgayCongTac = e.Attendances.Count(a => a.TrangThai == TrangThaiChamCong.CongTac),
            TongGioLam = e.Attendances
                .Where(a => a.GioVao != null && a.GioRa != null)
                .Sum(a => (a.GioRa!.Value.ToTimeSpan() - a.GioVao!.Value.ToTimeSpan()).TotalHours)
        }).ToList();

        return new AttendanceReportViewModel
        {
            Thang = thang, Nam = nam, DepartmentId = deptId,
            Summaries = summaries,
            Departments = await _context.Departments.ToListAsync()
        };
    }
}
