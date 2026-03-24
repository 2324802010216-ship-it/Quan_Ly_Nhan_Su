using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQuanLyNhanSu.Models;

public class Department
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên phòng ban là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên phòng ban tối đa 100 ký tự")]
    [Display(Name = "Tên phòng ban")]
    public string TenPhongBan { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Ngày thành lập")]
    [DataType(DataType.Date)]
    public DateTime NgayThanhLap { get; set; }

    [Display(Name = "Trưởng phòng")]
    public int? TruongPhongId { get; set; }

    [ForeignKey("TruongPhongId")]
    public Employee? TruongPhong { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();
}
