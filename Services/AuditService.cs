using System.Security.Claims;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Services;

public interface IAuditService
{
    Task LogAsync(ClaimsPrincipal user, string action, string entityName, int? entityId = null, string? details = null);
}

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context) { _context = context; }

    public async Task LogAsync(ClaimsPrincipal user, string action, string entityName, int? entityId = null, string? details = null)
    {
        _context.AuditLogs.Add(new AuditLog
        {
            UserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
            UserName = user.Identity?.Name ?? "System",
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Details = details,
            Timestamp = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }
}
