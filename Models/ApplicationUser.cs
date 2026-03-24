using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebQuanLyNhanSu.Models;

public class ApplicationUser : IdentityUser
{
    [Required, StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Display(Name = "Nhân viên")]
    public int? EmployeeId { get; set; }

    [StringLength(500)]
    [Display(Name = "Ảnh đại diện")]
    public string? AvatarUrl { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }
}
