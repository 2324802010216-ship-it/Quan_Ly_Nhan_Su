using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQuanLyNhanSu.Models;

public class Salary
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Range(1, 12)]
    [Display(Name = "Tháng")]
    public int Thang { get; set; }

    [Display(Name = "Năm")]
    public int Nam { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Lương cơ bản")]
    public decimal LuongCoBan { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Phụ cấp")]
    public decimal PhuCap { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tăng ca")]
    public decimal TangCa { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "BHXH (8%)")]
    public decimal BHXH { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "BHYT (1.5%)")]
    public decimal BHYT { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "BHTN (1%)")]
    public decimal BHTN { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Khấu trừ")]
    public decimal KhauTru { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Thực lãnh")]
    public decimal ThucLanh { get; set; }

    [Display(Name = "Ngày tính lương")]
    public DateTime NgayTinhLuong { get; set; }

    [StringLength(100)]
    [Display(Name = "Người tính lương")]
    public string? NguoiTinhLuong { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; } = null!;
}
