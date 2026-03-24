using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationService(ApplicationDbContext ctx, UserManager<ApplicationUser> um)
    { _context = ctx; _userManager = um; }

    public async Task<int> GetNotificationCount(ClaimsPrincipal user)
    {
        var hopDongSapHet = await _context.Contracts
            .CountAsync(c => c.TrangThai == TrangThaiHopDong.HieuLuc
                && c.NgayKetThuc != null
                && c.NgayKetThuc < DateTime.Today.AddDays(30));

        var donChoDuyet = await _context.LeaveRequests
            .CountAsync(lr => lr.TrangThai == TrangThaiDonNghi.ChoDuyet);

        if (user.IsInRole("Employee"))
        {
            var userId = _userManager.GetUserId(user);
            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            hopDongSapHet = await _context.Contracts
                .CountAsync(c => c.EmployeeId == appUser!.EmployeeId
                    && c.TrangThai == TrangThaiHopDong.HieuLuc
                    && c.NgayKetThuc != null && c.NgayKetThuc < DateTime.Today.AddDays(30));
            donChoDuyet = 0;
        }

        return hopDongSapHet + donChoDuyet;
    }
}
