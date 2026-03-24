using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AuditLogController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuditLogController(ApplicationDbContext ctx) { _context = ctx; }

    public async Task<IActionResult> Index(string? user, string? entity, string? action, int page = 1)
    {
        var query = _context.AuditLogs.AsNoTracking();
        if (!string.IsNullOrEmpty(user))
            query = query.Where(a => a.UserName.Contains(user));
        if (!string.IsNullOrEmpty(entity))
            query = query.Where(a => a.EntityName == entity);
        if (!string.IsNullOrEmpty(action))
            query = query.Where(a => a.Action == action);

        var total = await query.CountAsync();
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total / 25.0);
        ViewBag.User = user; ViewBag.Entity = entity; ViewBag.Action = action;
        ViewBag.Entities = await _context.AuditLogs.Select(a => a.EntityName).Distinct().OrderBy(e => e).ToListAsync();

        return View(await query.OrderByDescending(a => a.Timestamp).Skip((page - 1) * 25).Take(25).ToListAsync());
    }
}
