using System.ComponentModel.DataAnnotations;

namespace WebQuanLyNhanSu.Models.Enums;
public enum TrangThaiDonNghi
{
    [Display(Name = "Chờ duyệt")] ChoDuyet,
    [Display(Name = "Đã duyệt")] DaDuyet,
    [Display(Name = "Từ chối")] TuChoi,
    [Display(Name = "Đã hủy")] DaHuy
}
