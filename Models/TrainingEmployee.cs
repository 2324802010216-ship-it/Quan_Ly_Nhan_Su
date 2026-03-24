using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQuanLyNhanSu.Models;

public class TrainingEmployee
{
    public int Id { get; set; }

    [Required]
    public int TrainingId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [StringLength(100)]
    [Display(Name = "Kết quả")]
    public string? KetQua { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    [Display(Name = "Điểm số")]
    public decimal? DiemSo { get; set; }

    [StringLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [ForeignKey("TrainingId")]
    public Training Training { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; } = null!;
}
