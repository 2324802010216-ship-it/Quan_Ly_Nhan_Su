using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "HRAccess")]
public class ContractController : BaseController
{
    public ContractController(ApplicationDbContext ctx, UserManager<ApplicationUser> um) : base(ctx, um) { }

    public async Task<IActionResult> Index(int? trangThai, int page = 1)
    {
        var query = _context.Contracts.Include(c => c.Employee).ThenInclude(e => e.Department)
            .AsNoTracking();
        if (trangThai.HasValue)
            query = query.Where(c => (int)c.TrangThai == trangThai);
        var total = await query.CountAsync();
        ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 15.0); ViewBag.TrangThai = trangThai;
        return View(await query.OrderByDescending(c => c.NgayBatDau).Skip((page - 1) * 15).Take(15).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var c = await _context.Contracts.Include(c => c.Employee).ThenInclude(e => e.Department)
            .FirstOrDefaultAsync(c => c.Id == id);
        return c == null ? NotFound() : View(c);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Contract model)
    {
        ModelState.Remove("Employee");
        if (!ModelState.IsValid)
        {
            ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
            return View(model);
        }
        _context.Contracts.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm hợp đồng thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var c = await _context.Contracts.FindAsync(id);
        if (c == null) return NotFound();
        ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
        return View(c);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Contract model)
    {
        if (id != model.Id) return NotFound();
        ModelState.Remove("Employee");
        if (!ModelState.IsValid)
        {
            ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
            return View(model);
        }
        _context.Contracts.Update(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật hợp đồng thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Renew(int id)
    {
        var old = await _context.Contracts.FindAsync(id);
        if (old == null) return NotFound();
        old.TrangThai = TrangThaiHopDong.HetHan;

        var renewed = new Contract
        {
            EmployeeId = old.EmployeeId,
            MaHopDong = $"HD{DateTime.Now:yyyyMMddHHmmss}",
            LoaiHopDong = old.LoaiHopDong,
            NgayBatDau = old.NgayKetThuc?.AddDays(1) ?? DateTime.Today,
            NgayKetThuc = old.NgayKetThuc?.AddYears(1),
            LuongHopDong = old.LuongHopDong,
            TrangThai = TrangThaiHopDong.HieuLuc
        };
        _context.Contracts.Add(renewed);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Gia hạn hợp đồng thành công!";
        return RedirectToAction(nameof(Details), new { id = renewed.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var c = await _context.Contracts.FindAsync(id);
        if (c == null) return NotFound();
        if (c.TrangThai == TrangThaiHopDong.HieuLuc)
        {
            TempData["Error"] = "Không thể xóa hợp đồng đang hiệu lực!";
            return RedirectToAction(nameof(Index));
        }
        _context.Contracts.Remove(c);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa hợp đồng thành công!";
        return RedirectToAction(nameof(Index));
    }
}
