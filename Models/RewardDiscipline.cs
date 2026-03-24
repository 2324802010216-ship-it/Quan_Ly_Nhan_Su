using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Models;

public class RewardDiscipline
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Display(Name = "Loại")]
    public LoaiKhenThuongKyLuat Loai { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày quyết định")]
    public DateTime NgayQuyetDinh { get; set; }

    [StringLength(50)]
    [Display(Name = "Số quyết định")]
    public string? SoQuyetDinh { get; set; }

    [Required(ErrorMessage = "Lý do là bắt buộc")]
    [StringLength(500)]
    [Display(Name = "Lý do")]
    public string LyDo { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Số tiền")]
    [DisplayFormat(DataFormatString = "{0:N0} ₫")]
    public decimal SoTien { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; } = null!;
}
