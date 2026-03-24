using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;

namespace WebQuanLyNhanSu.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    public ReportService(ApplicationDbContext context) => _context = context;

    public async Task<byte[]> ExportEmployeeReport(int? departmentId)
    {
        var query = _context.Employees
            .Include(e => e.Department).Include(e => e.Position).AsNoTracking();

        if (departmentId.HasValue) query = query.Where(e => e.DepartmentId == departmentId);

        var employees = await query.OrderBy(e => e.MaNV).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Danh sách nhân viên");
        ws.Cell(1, 1).Value = "Mã NV"; ws.Cell(1, 2).Value = "Họ tên";
        ws.Cell(1, 3).Value = "Phòng ban"; ws.Cell(1, 4).Value = "Chức vụ";
        ws.Cell(1, 5).Value = "SĐT"; ws.Cell(1, 6).Value = "Email";
        ws.Cell(1, 7).Value = "Ngày vào làm"; ws.Cell(1, 8).Value = "Trạng thái";
        ws.Row(1).Style.Font.Bold = true;

        for (int i = 0; i < employees.Count; i++)
        {
            var e = employees[i];
            ws.Cell(i + 2, 1).Value = e.MaNV; ws.Cell(i + 2, 2).Value = e.HoTen;
            ws.Cell(i + 2, 3).Value = e.Department.TenPhongBan;
            ws.Cell(i + 2, 4).Value = e.Position.TenChucVu;
            ws.Cell(i + 2, 5).Value = e.SoDienThoai; ws.Cell(i + 2, 6).Value = e.Email;
            ws.Cell(i + 2, 7).Value = e.NgayVaoLam.ToString("dd/MM/yyyy");
            ws.Cell(i + 2, 8).Value = e.TrangThai.ToString();
        }
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms); return ms.ToArray();
    }

    public async Task<byte[]> ExportSalaryReport(int thang, int nam)
    {
        var salaries = await _context.Salaries
            .Include(s => s.Employee).ThenInclude(e => e.Department)
            .Where(s => s.Thang == thang && s.Nam == nam)
            .OrderBy(s => s.Employee.MaNV).AsNoTracking().ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet($"Lương T{thang}/{nam}");
        ws.Cell(1, 1).Value = "Mã NV"; ws.Cell(1, 2).Value = "Họ tên";
        ws.Cell(1, 3).Value = "Phòng ban"; ws.Cell(1, 4).Value = "Lương CB";
        ws.Cell(1, 5).Value = "Phụ cấp"; ws.Cell(1, 6).Value = "Tăng ca";
        ws.Cell(1, 7).Value = "BHXH"; ws.Cell(1, 8).Value = "Khấu trừ";
        ws.Cell(1, 9).Value = "Thực lãnh";
        ws.Row(1).Style.Font.Bold = true;

        for (int i = 0; i < salaries.Count; i++)
        {
            var s = salaries[i];
            ws.Cell(i + 2, 1).Value = s.Employee.MaNV; ws.Cell(i + 2, 2).Value = s.Employee.HoTen;
            ws.Cell(i + 2, 3).Value = s.Employee.Department.TenPhongBan;
            ws.Cell(i + 2, 4).SetValue((double)s.LuongCoBan); ws.Cell(i + 2, 5).SetValue((double)s.PhuCap);
            ws.Cell(i + 2, 6).SetValue((double)s.TangCa); ws.Cell(i + 2, 7).SetValue((double)s.BHXH);
            ws.Cell(i + 2, 8).SetValue((double)s.KhauTru); ws.Cell(i + 2, 9).SetValue((double)s.ThucLanh);
        }
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms); return ms.ToArray();
    }

    public async Task<byte[]> ExportAttendanceReport(int thang, int nam, int? departmentId)
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Attendances.Where(a => a.Ngay.Month == thang && a.Ngay.Year == nam))
            .AsNoTracking();

        if (departmentId.HasValue) query = query.Where(e => e.DepartmentId == departmentId);
        var employees = await query.ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet($"Chấm công T{thang}/{nam}");
        ws.Cell(1, 1).Value = "Mã NV"; ws.Cell(1, 2).Value = "Họ tên";
        ws.Cell(1, 3).Value = "Phòng ban"; ws.Cell(1, 4).Value = "Đi làm";
        ws.Cell(1, 5).Value = "Trễ"; ws.Cell(1, 6).Value = "Vắng";
        ws.Cell(1, 7).Value = "Nghỉ phép"; ws.Cell(1, 8).Value = "Tổng giờ";
        ws.Row(1).Style.Font.Bold = true;

        int row = 2;
        foreach (var e in employees)
        {
            var atts = e.Attendances;
            ws.Cell(row, 1).Value = e.MaNV; ws.Cell(row, 2).Value = e.HoTen;
            ws.Cell(row, 3).Value = e.Department.TenPhongBan;
            ws.Cell(row, 4).Value = atts.Count(a => a.TrangThai == Models.Enums.TrangThaiChamCong.DungGio);
            ws.Cell(row, 5).Value = atts.Count(a => a.TrangThai == Models.Enums.TrangThaiChamCong.TreMuon);
            ws.Cell(row, 6).Value = atts.Count(a => a.TrangThai == Models.Enums.TrangThaiChamCong.VangMat);
            ws.Cell(row, 7).Value = atts.Count(a => a.TrangThai == Models.Enums.TrangThaiChamCong.NghiPhep);
            ws.Cell(row, 8).SetValue(atts.Where(a => a.GioVao != null && a.GioRa != null)
                .Sum(a => (a.GioRa!.Value.ToTimeSpan() - a.GioVao!.Value.ToTimeSpan()).TotalHours));
            row++;
        }
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms); return ms.ToArray();
    }

    public async Task<byte[]> ExportLeaveReport(int? thang, int? nam, int? departmentId)
    {
        var query = _context.LeaveRequests
            .Include(lr => lr.Employee).ThenInclude(e => e.Department)
            .AsNoTracking();

        if (thang.HasValue && nam.HasValue)
            query = query.Where(lr => lr.NgayBatDau.Month == thang && lr.NgayBatDau.Year == nam);
        else if (nam.HasValue)
            query = query.Where(lr => lr.NgayBatDau.Year == nam);

        if (departmentId.HasValue)
            query = query.Where(lr => lr.Employee.DepartmentId == departmentId);

        var leaves = await query.OrderByDescending(lr => lr.NgayBatDau).ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Nghỉ phép");
        ws.Cell(1, 1).Value = "Mã NV"; ws.Cell(1, 2).Value = "Họ tên";
        ws.Cell(1, 3).Value = "Phòng ban"; ws.Cell(1, 4).Value = "Loại nghỉ";
        ws.Cell(1, 5).Value = "Từ ngày"; ws.Cell(1, 6).Value = "Đến ngày";
        ws.Cell(1, 7).Value = "Số ngày"; ws.Cell(1, 8).Value = "Trạng thái";
        ws.Cell(1, 9).Value = "Lý do";
        ws.Row(1).Style.Font.Bold = true;

        for (int i = 0; i < leaves.Count; i++)
        {
            var lr = leaves[i];
            ws.Cell(i + 2, 1).Value = lr.Employee.MaNV;
            ws.Cell(i + 2, 2).Value = lr.Employee.HoTen;
            ws.Cell(i + 2, 3).Value = lr.Employee.Department.TenPhongBan;
            ws.Cell(i + 2, 4).Value = lr.LoaiNghi.ToString();
            ws.Cell(i + 2, 5).Value = lr.NgayBatDau.ToString("dd/MM/yyyy");
            ws.Cell(i + 2, 6).Value = lr.NgayKetThuc.ToString("dd/MM/yyyy");
            ws.Cell(i + 2, 7).Value = lr.SoNgay;
            ws.Cell(i + 2, 8).Value = lr.TrangThai.ToString();
            ws.Cell(i + 2, 9).Value = lr.LyDo;
        }
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms); return ms.ToArray();
    }
}
