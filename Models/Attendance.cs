using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Models;

public class Attendance
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Nhân viên")]
    public int EmployeeId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày")]
    public DateTime Ngay { get; set; }

    [Display(Name = "Giờ vào")]
    public TimeOnly? GioVao { get; set; }

    [Display(Name = "Giờ ra")]
    public TimeOnly? GioRa { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiChamCong TrangThai { get; set; }

    [StringLength(300)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; } = null!;

    [NotMapped]
    public double? SoGioLam => (GioVao != null && GioRa != null)
        ? (GioRa.Value.ToTimeSpan() - GioVao.Value.ToTimeSpan()).TotalHours
        : null;
}
