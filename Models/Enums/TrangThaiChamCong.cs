using System.ComponentModel.DataAnnotations;

namespace WebQuanLyNhanSu.Models.Enums;
public enum TrangThaiChamCong
{
    [Display(Name = "Đúng giờ")] DungGio,
    [Display(Name = "Trễ muộn")] TreMuon,
    [Display(Name = "Vắng mặt")] VangMat,
    [Display(Name = "Nghỉ phép")] NghiPhep,
    [Display(Name = "Công tác")] CongTac
}
