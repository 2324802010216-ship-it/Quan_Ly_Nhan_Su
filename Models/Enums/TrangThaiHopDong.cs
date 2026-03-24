using System.ComponentModel.DataAnnotations;

namespace WebQuanLyNhanSu.Models.Enums;
public enum TrangThaiHopDong
{
    [Display(Name = "Hiệu lực")] HieuLuc,
    [Display(Name = "Hết hạn")] HetHan,
    [Display(Name = "Đã thanh lý")] DaThanhLy,
    [Display(Name = "Tạm hoãn")] TamHoan
}
