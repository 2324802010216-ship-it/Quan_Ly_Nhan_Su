using System.ComponentModel.DataAnnotations;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.ViewModels;

public class DashboardViewModel
{
    public int TongNhanVien { get; set; }
    public int TongPhongBan { get; set; }
    public int HopDongSapHetHan { get; set; }
    public int DonPhepChoDuyet { get; set; }
    public List<Contract> HopDongCanhBao { get; set; } = new();
    public List<LeaveRequest> DonNghiChoDuyet { get; set; } = new();
    public List<string> TenPhongBans { get; set; } = new();
    public List<int> SoNVTheoPhong { get; set; } = new();
    public List<string> ThangLabels { get; set; } = new();
    public List<int> SoTuyenDungTheoThang { get; set; } = new();
}

public class EmployeeFilterViewModel
{
    public string? Search { get; set; }
    public int? DepartmentId { get; set; }
    public int? PositionId { get; set; }
    public int? TrangThai { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public int TotalPages { get; set; }
    public List<Employee> Employees { get; set; } = new();
    public List<Department> Departments { get; set; } = new();
    public List<Position> Positions { get; set; } = new();
}

public class SalaryCalculationViewModel
{
    public int EmployeeId { get; set; }
    public int Thang { get; set; } = DateTime.Now.Month;
    public int Nam { get; set; } = DateTime.Now.Year;
    public decimal PhuCap { get; set; }
    public decimal SoGioTangCa { get; set; }
    public decimal KhauTru { get; set; }
    public List<Employee>? Employees { get; set; }
}

public class LeaveBalanceViewModel
{
    public int EmployeeId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public int SoNgayPhepNam { get; set; } = 12;
    public int SoNgayDaDung { get; set; }
    public int SoNgayConLai => SoNgayPhepNam - SoNgayDaDung;
}

public class AttendanceReportViewModel
{
    public int Thang { get; set; }
    public int Nam { get; set; }
    public int? DepartmentId { get; set; }
    public List<Department> Departments { get; set; } = new();
    public List<AttendanceSummary> Summaries { get; set; } = new();
}

public class AttendanceSummary
{
    public Employee Employee { get; set; } = null!;
    public int NgayDiLam { get; set; }
    public int NgayTreMuon { get; set; }
    public int NgayVangMat { get; set; }
    public int NgayNghiPhep { get; set; }
    public int NgayCongTac { get; set; }
    public double TongGioLam { get; set; }
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu hiện tại")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ReportExportViewModel
{
    public string? ReportType { get; set; }
    public int? Thang { get; set; }
    public int? Nam { get; set; }
    public int? DepartmentId { get; set; }
}

public class ProfileViewModel
{
    public string Id { get; set; } = string.Empty;
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    [Display(Name = "Ảnh đại diện")]
    public string? AvatarUrl { get; set; }
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public string? DepartmentName { get; set; }
    public string? PositionName { get; set; }
}
