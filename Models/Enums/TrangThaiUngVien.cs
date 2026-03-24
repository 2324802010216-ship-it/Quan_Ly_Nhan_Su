using System.ComponentModel.DataAnnotations;

namespace WebQuanLyNhanSu.Models.Enums;
public enum TrangThaiUngVien
{
    [Display(Name = "Mới ứng tuyển")] MoiUngTuyen,
    [Display(Name = "Đang phỏng vấn")] DangPhongVan,
    [Display(Name = "Đạt vòng")] DatVong,
    [Display(Name = "Loại vòng")] LoaiVong,
    [Display(Name = "Trúng tuyển")] TrungTuyen,
    [Display(Name = "Đã nhận việc")] DaNhanViec
}
