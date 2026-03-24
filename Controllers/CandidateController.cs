using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Services;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "HRAccess")]
public class CandidateController : BaseController
{
    private readonly IFileUploadService _fileService;

    public CandidateController(ApplicationDbContext ctx, UserManager<ApplicationUser> um, IFileUploadService fs)
        : base(ctx, um) { _fileService = fs; }

    public async Task<IActionResult> Index(int? trangThai, int page = 1)
    {
        var query = _context.Candidates.Include(c => c.Recruitment).AsNoTracking();
        if (trangThai.HasValue)
            query = query.Where(c => (int)c.TrangThai == trangThai);
        var total = await query.CountAsync();
        ViewBag.Page = page; ViewBag.TotalPages = (int)Math.Ceiling(total / 20.0); ViewBag.TrangThai = trangThai;
        return View(await query.OrderByDescending(c => c.NgayUngTuyen).Skip((page - 1) * 20).Take(20).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var c = await _context.Candidates.Include(c => c.Recruitment).ThenInclude(r => r.Department)
            .FirstOrDefaultAsync(c => c.Id == id);
        return c == null ? NotFound() : View(c);
    }

    [HttpGet]
    public IActionResult Create(int recruitmentId)
    {
        ViewBag.RecruitmentId = recruitmentId;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Candidate model, IFormFile? cv)
    {
        if (cv != null) model.CVFilePath = await _fileService.UploadFile(cv, "uploads/cvs");
        ModelState.Remove("Recruitment");
        if (!ModelState.IsValid) { ViewBag.RecruitmentId = model.RecruitmentId; return View(model); }
        _context.Candidates.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Thêm ứng viên thành công!";
        return RedirectToAction("Details", "Recruitment", new { id = model.RecruitmentId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var c = await _context.Candidates.FindAsync(id);
        return c == null ? NotFound() : View(c);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Candidate model, IFormFile? cv)
    {
        if (id != model.Id) return NotFound();
        if (cv != null)
        {
            _fileService.DeleteFile(model.CVFilePath);
            model.CVFilePath = await _fileService.UploadFile(cv, "uploads/cvs");
        }
        ModelState.Remove("Recruitment");
        if (!ModelState.IsValid) return View(model);
        _context.Candidates.Update(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cập nhật ứng viên thành công!";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string trangThai)
    {
        var c = await _context.Candidates.FindAsync(id);
        if (c == null) return NotFound();
        if (Enum.TryParse<Models.Enums.TrangThaiUngVien>(trangThai, out var status))
        {
            c.TrangThai = status;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật trạng thái thành công!";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var c = await _context.Candidates.FindAsync(id);
        if (c == null) return NotFound();
        _fileService.DeleteFile(c.CVFilePath);
        _context.Candidates.Remove(c);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Xóa ứng viên thành công!";
        return RedirectToAction(nameof(Index));
    }
}
