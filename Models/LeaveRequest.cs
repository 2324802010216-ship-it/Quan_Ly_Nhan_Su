using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Models;

public class LeaveRequest
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Nhân viên")]
    public int EmployeeId { get; set; }

    [Display(Name = "Loại nghỉ")]
    public LoaiNghiPhep LoaiNghi { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime NgayBatDau { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc")]
    public DateTime NgayKetThuc { get; set; }

    [Display(Name = "Số ngày")]
    public int SoNgay { get; set; }

    [Required(ErrorMessage = "Lý do là bắt buộc")]
    [StringLength(500)]
    [Display(Name = "Lý do")]
    public string LyDo { get; set; } = string.Empty;

    [Display(Name = "Trạng thái")]
    public TrangThaiDonNghi TrangThai { get; set; } = TrangThaiDonNghi.ChoDuyet;

    [Display(Name = "Người duyệt")]
    public int? NguoiDuyetId { get; set; }

    [StringLength(500)]
    [Display(Name = "Lý do từ chối")]
    public string? LyDoTuChoi { get; set; }

    [Display(Name = "Ngày duyệt")]
    public DateTime? NgayDuyet { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; } = null!;

    [ForeignKey("NguoiDuyetId")]
    public Employee? NguoiDuyet { get; set; }
}
