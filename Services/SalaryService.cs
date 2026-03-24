using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Services;

public class SalaryService : ISalaryService
{
    private readonly ApplicationDbContext _context;
    public SalaryService(ApplicationDbContext context) => _context = context;

    public async Task<Salary> CalculateSalary(int employeeId, int thang, int nam,
        decimal phuCap, decimal soGioTangCa, decimal khauTru, string nguoiTinh)
    {
        var employee = await _context.Employees
            .Include(e => e.Contracts.Where(c => c.TrangThai == TrangThaiHopDong.HieuLuc))
            .FirstOrDefaultAsync(e => e.Id == employeeId)
            ?? throw new Exception("Nhân viên không tồn tại");

        var contract = employee.Contracts.FirstOrDefault()
            ?? throw new Exception("Nhân viên chưa có hợp đồng hiệu lực");

        decimal luongCoBan = contract.LuongHopDong;

        // Số ngày công thực tế
        int ngayCong = await _context.Attendances
            .CountAsync(a => a.EmployeeId == employeeId
                && a.Ngay.Month == thang && a.Ngay.Year == nam
                && (a.TrangThai == TrangThaiChamCong.DungGio || a.TrangThai == TrangThaiChamCong.TreMuon));

        // Tăng ca = SoGioTangCa * (LuongCoBan / 26 / 8) * 1.5
        decimal luongGio = luongCoBan / 26 / 8;
        decimal tangCa = soGioTangCa * luongGio * 1.5m;

        // Bảo hiểm
        decimal bhxh = luongCoBan * 0.08m;
        decimal bhyt = luongCoBan * 0.015m;
        decimal bhtn = luongCoBan * 0.01m;

        // Lương theo ngày công
        decimal luongThucTe = (luongCoBan / 26) * ngayCong;

        decimal thucLanh = luongThucTe + phuCap + tangCa - bhxh - bhyt - bhtn - khauTru;

        // Check duplicate
        var existing = await _context.Salaries
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.Thang == thang && s.Nam == nam);

        if (existing != null)
        {
            existing.LuongCoBan = luongCoBan;
            existing.PhuCap = phuCap; existing.TangCa = tangCa;
            existing.BHXH = bhxh; existing.BHYT = bhyt; existing.BHTN = bhtn;
            existing.KhauTru = khauTru; existing.ThucLanh = thucLanh;
            existing.NgayTinhLuong = DateTime.Now; existing.NguoiTinhLuong = nguoiTinh;
            _context.Salaries.Update(existing);
        }
        else
        {
            existing = new Salary
            {
                EmployeeId = employeeId, Thang = thang, Nam = nam,
                LuongCoBan = luongCoBan, PhuCap = phuCap, TangCa = tangCa,
                BHXH = bhxh, BHYT = bhyt, BHTN = bhtn,
                KhauTru = khauTru, ThucLanh = thucLanh,
                NgayTinhLuong = DateTime.Now, NguoiTinhLuong = nguoiTinh
            };
            _context.Salaries.Add(existing);
        }
        await _context.SaveChangesAsync();
        return existing;
    }
}
