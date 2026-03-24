using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Models;

public class Contract
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Nhân viên")]
    public int EmployeeId { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "Mã hợp đồng")]
    public string MaHopDong { get; set; } = string.Empty;

    [Display(Name = "Loại hợp đồng")]
    public LoaiHopDong LoaiHopDong { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime NgayBatDau { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc")]
    public DateTime? NgayKetThuc { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Lương hợp đồng")]
    [DisplayFormat(DataFormatString = "{0:N0} ₫")]
    public decimal LuongHopDong { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiHopDong TrangThai { get; set; } = TrangThaiHopDong.HieuLuc;

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; } = null!;

    [NotMapped]
    public int? SoNgayConLai => NgayKetThuc.HasValue
        ? (NgayKetThuc.Value - DateTime.Today).Days
        : null;
}
