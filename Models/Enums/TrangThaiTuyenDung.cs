using System.ComponentModel.DataAnnotations;

namespace WebQuanLyNhanSu.Models.Enums;
public enum TrangThaiTuyenDung
{
    [Display(Name = "Mở tuyển dụng")] MoTuyenDung,
    [Display(Name = "Đã đóng")] DaDong,
    [Display(Name = "Tạm dừng")] TamDung
}
