using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Models;

public class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
    [StringLength(20)]
    [Display(Name = "Mã NV")]
    public string MaNV { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime NgaySinh { get; set; }

    [Display(Name = "Giới tính")]
    public GioiTinh GioiTinh { get; set; }

    [StringLength(12)]
    [Display(Name = "CCCD")]
    public string? CCCD { get; set; }

    [StringLength(300)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [StringLength(15)]
    [Phone]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [StringLength(100)]
    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(500)]
    [Display(Name = "Ảnh đại diện")]
    public string? AnhDaiDien { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày vào làm")]
    public DateTime NgayVaoLam { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiNhanVien TrangThai { get; set; } = TrangThaiNhanVien.DangLamViec;

    [Required]
    [Display(Name = "Phòng ban")]
    public int DepartmentId { get; set; }

    [Required]
    [Display(Name = "Chức vụ")]
    public int PositionId { get; set; }

    [ForeignKey("DepartmentId")]
    public Department Department { get; set; } = null!;

    [ForeignKey("PositionId")]
    public Position Position { get; set; } = null!;

    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Salary> Salaries { get; set; } = new List<Salary>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<RewardDiscipline> RewardDisciplines { get; set; } = new List<RewardDiscipline>();
    public ICollection<TrainingEmployee> TrainingEmployees { get; set; } = new List<TrainingEmployee>();
}
