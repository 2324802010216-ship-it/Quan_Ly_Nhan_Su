using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Services;
using WebQuanLyNhanSu.ViewModels;

namespace WebQuanLyNhanSu.Controllers;

[Authorize]
public class ProfileController : BaseController
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IFileUploadService _fileService;

    public ProfileController(ApplicationDbContext ctx, UserManager<ApplicationUser> um,
        SignInManager<ApplicationUser> sim, IFileUploadService fs)
        : base(ctx, um) { _signInManager = sim; _fileService = fs; }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var model = new ProfileViewModel
        {
            Id = user.Id,
            HoTen = user.HoTen,
            Email = user.Email ?? "",
            AvatarUrl = user.AvatarUrl,
            EmployeeId = user.EmployeeId
        };

        if (user.EmployeeId.HasValue)
        {
            var emp = await _context.Employees
                .Include(e => e.Department).Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == user.EmployeeId);
            if (emp != null)
            {
                model.EmployeeName = emp.HoTen;
                model.DepartmentName = emp.Department?.TenPhongBan;
                model.PositionName = emp.Position?.TenChucVu;
            }
        }
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var path = await _fileService.UploadFile(avatar, "uploads/avatars");
        if (path != null)
        {
            _fileService.DeleteFile(user.AvatarUrl);
            user.AvatarUrl = path;
            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Cập nhật ảnh đại diện thành công!";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangePassword() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction(nameof(Index));
        }
        foreach (var err in result.Errors) ModelState.AddModelError("", err.Description);
        return View(model);
    }
}
