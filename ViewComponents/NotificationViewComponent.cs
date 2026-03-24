using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models.Enums;
using WebQuanLyNhanSu.Services;

namespace WebQuanLyNhanSu.ViewComponents;

public class NotificationViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public NotificationViewComponent(ApplicationDbContext context, INotificationService ns)
    {
        _context = context;
        _notificationService = ns;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var notifCount = await _notificationService.GetNotificationCount(HttpContext.User);

        var expiringContracts = await _context.Contracts
            .Include(c => c.Employee)
            .Where(c => c.TrangThai == TrangThaiHopDong.HieuLuc
                && c.NgayKetThuc != null
                && c.NgayKetThuc < DateTime.Today.AddDays(30))
            .OrderBy(c => c.NgayKetThuc).Take(5).ToListAsync();

        var pendingLeaves = await _context.LeaveRequests
            .Include(l => l.Employee)
            .Where(l => l.TrangThai == TrangThaiDonNghi.ChoDuyet)
            .OrderByDescending(l => l.NgayBatDau).Take(5).ToListAsync();

        return View(new NotificationViewModel
        {
            Count = notifCount,
            ExpiringContracts = expiringContracts,
            PendingLeaves = pendingLeaves
        });
    }
}

public class NotificationViewModel
{
    public int Count { get; set; }
    public List<WebQuanLyNhanSu.Models.Contract> ExpiringContracts { get; set; } = new();
    public List<WebQuanLyNhanSu.Models.LeaveRequest> PendingLeaves { get; set; } = new();
}
