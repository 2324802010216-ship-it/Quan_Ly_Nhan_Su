using System.ComponentModel.DataAnnotations;

namespace WebQuanLyNhanSu.Models.Enums;
public enum LoaiHopDong
{
    [Display(Name = "Thử việc")] ThuViec,
    [Display(Name = "Có thời hạn")] CoThoiHan,
    [Display(Name = "Không thời hạn")] KhongThoiHan
}
