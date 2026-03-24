using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQuanLyNhanSu.Models;

public class Position
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên chức vụ là bắt buộc")]
    [StringLength(100)]
    [Display(Name = "Tên chức vụ")]
    public string TenChucVu { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Range(1, 10, ErrorMessage = "Cấp bậc từ 1-10")]
    [Display(Name = "Cấp bậc")]
    public int CapBac { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Mức lương cơ sở")]
    [DisplayFormat(DataFormatString = "{0:N0} ₫")]
    public decimal MucLuongCoSo { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();
}
