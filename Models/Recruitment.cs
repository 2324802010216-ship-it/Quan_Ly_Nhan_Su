using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Models;

public class Recruitment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    [StringLength(200)]
    [Display(Name = "Tiêu đề")]
    public string TieuDe { get; set; } = string.Empty;

    [Display(Name = "Phòng ban")]
    public int PhongBanId { get; set; }

    [Display(Name = "Chức vụ")]
    public int PositionId { get; set; }

    [Range(1, 100)]
    [Display(Name = "Số lượng")]
    public int SoLuong { get; set; }

    [StringLength(2000)]
    [Display(Name = "Mô tả công việc")]
    public string? MoTaCongViec { get; set; }

    [StringLength(1000)]
    [Display(Name = "Yêu cầu")]
    public string? YeuCau { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày đăng")]
    public DateTime NgayDang { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Hạn nộp")]
    public DateTime HanNop { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiTuyenDung TrangThai { get; set; } = TrangThaiTuyenDung.MoTuyenDung;

    [ForeignKey("PhongBanId")]
    public Department Department { get; set; } = null!;

    [ForeignKey("PositionId")]
    public Position Position { get; set; } = null!;

    public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();

    [NotMapped]
    public int SoUngVien => Candidates?.Count ?? 0;
}
