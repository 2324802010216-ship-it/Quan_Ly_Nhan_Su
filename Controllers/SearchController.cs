using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Controllers;

[Authorize]
public class SearchController : BaseController
{
    public SearchController(ApplicationDbContext ctx, UserManager<ApplicationUser> um)
        : base(ctx, um) { }

    [HttpGet]
    public async Task<IActionResult> GlobalSearch(string term)
    {
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            return Json(Array.Empty<object>());

        var results = await _context.Employees
            .Include(e => e.Department).Include(e => e.Position)
            .Where(e => e.MaNV.Contains(term) || e.HoTen.Contains(term)
                || e.Email!.Contains(term) || e.SoDienThoai!.Contains(term))
            .Take(8)
            .Select(e => new
            {
                e.Id, e.MaNV, e.HoTen, e.AnhDaiDien,
                PhongBan = e.Department.TenPhongBan,
                ChucVu = e.Position.TenChucVu
            })
            .ToListAsync();

        return Json(results);
    }
}
