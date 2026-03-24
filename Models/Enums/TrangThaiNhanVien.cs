using System.ComponentModel.DataAnnotations;

namespace WebQuanLyNhanSu.Models.Enums;
public enum TrangThaiNhanVien
{
    [Display(Name = "Đang làm việc")] DangLamViec,
    [Display(Name = "Nghỉ việc")] NghiViec,
    [Display(Name = "Thử việc")] ThuViec,
    [Display(Name = "Nghỉ phép")] NghiPhep
}
