using System.ComponentModel.DataAnnotations;

namespace WebQuanLyNhanSu.Models.Enums;
public enum LoaiNghiPhep
{
    [Display(Name = "Nghỉ phép năm")] NghiPhepNam,
    [Display(Name = "Nghỉ ốm")] NghiOm,
    [Display(Name = "Nghỉ không lương")] NghiKhongLuong,
    [Display(Name = "Nghỉ lễ")] NghiLe,
    [Display(Name = "Nghỉ thai sản")] NghiThai
}
