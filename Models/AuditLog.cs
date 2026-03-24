using System.ComponentModel.DataAnnotations;

namespace WebQuanLyNhanSu.Models;

public class AuditLog
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = "";

    [StringLength(100)]
    public string UserName { get; set; } = "";

    [Required, StringLength(20)]
    [Display(Name = "Hành động")]
    public string Action { get; set; } = "";  // Create / Update / Delete / Login

    [Required, StringLength(100)]
    [Display(Name = "Đối tượng")]
    public string EntityName { get; set; } = "";

    public int? EntityId { get; set; }

    [StringLength(500)]
    [Display(Name = "Chi tiết")]
    public string? Details { get; set; }

    [Display(Name = "Thời gian")]
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
