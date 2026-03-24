using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Models;

public class Candidate
{
    public int Id { get; set; }

    [Required]
    public int RecruitmentId { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [StringLength(100), EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(15), Phone]
    [Display(Name = "SĐT")]
    public string? SDT { get; set; }

    [StringLength(500)]
    [Display(Name = "CV")]
    public string? CVFilePath { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày ứng tuyển")]
    public DateTime NgayUngTuyen { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiUngVien TrangThai { get; set; } = TrangThaiUngVien.MoiUngTuyen;

    [ForeignKey("RecruitmentId")]
    public Recruitment Recruitment { get; set; } = null!;
}
