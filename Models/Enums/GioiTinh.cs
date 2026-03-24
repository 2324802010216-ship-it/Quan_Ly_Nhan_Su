using System.ComponentModel.DataAnnotations;

namespace WebQuanLyNhanSu.Models.Enums;
public enum GioiTinh
{
    [Display(Name = "Nam")] Nam,
    [Display(Name = "Nữ")] Nu,
    [Display(Name = "Khác")] Khac
}
