using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQuanLyNhanSu.Models;

public class Training
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "Tên khóa học")]
    public string TenKhoaHoc { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime NgayBatDau { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc")]
    public DateTime NgayKetThuc { get; set; }

    [StringLength(200)]
    [Display(Name = "Địa điểm")]
    public string? DiaDiem { get; set; }

    [StringLength(100)]
    [Display(Name = "Người đào tạo")]
    public string? NguoiDaoTao { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Chi phí")]
    public decimal ChiPhi { get; set; }

    public ICollection<TrainingEmployee> TrainingEmployees { get; set; } = new List<TrainingEmployee>();
}
