using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Controllers;

[Authorize(Policy = "AdminOnly")]
public class UserManagementController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public UserManagementController(UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm, ApplicationDbContext ctx)
    { _userManager = um; _roleManager = rm; _context = ctx; }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.Include(u => u.Employee).ToListAsync();
        var model = new List<UserViewModel>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            model.Add(new UserViewModel
            {
                Id = u.Id, Email = u.Email!, HoTen = u.HoTen,
                Role = roles.FirstOrDefault() ?? "—",
                EmployeeName = u.Employee?.HoTen,
                EmployeeMaNV = u.Employee?.MaNV
            });
        }
        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        ViewBag.Employees = new SelectList(
            await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.Employees = new SelectList(
                await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
            return View(model);
        }
        var user = new ApplicationUser
        {
            UserName = model.Email, Email = model.Email,
            HoTen = model.HoTen, EmployeeId = model.EmployeeId
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            ViewBag.Employees = new SelectList(
                await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
            return View(model);
        }
        if (!string.IsNullOrEmpty(model.Role))
            await _userManager.AddToRoleAsync(user, model.Role);
        TempData["Success"] = $"Tạo tài khoản {model.Email} thành công!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.Users.Include(u => u.Employee).FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();
        var roles = await _userManager.GetRolesAsync(user);
        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        ViewBag.Employees = new SelectList(
            await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
        return View(new EditUserViewModel
        {
            Id = user.Id, Email = user.Email!, HoTen = user.HoTen,
            Role = roles.FirstOrDefault() ?? "", EmployeeId = user.EmployeeId
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound();

        user.HoTen = model.HoTen;
        user.EmployeeId = model.EmployeeId;
        await _userManager.UpdateAsync(user);

        // Update role
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!string.IsNullOrEmpty(model.Role))
            await _userManager.AddToRoleAsync(user, model.Role);

        // Reset password if requested
        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
                ViewBag.Employees = new SelectList(
                    await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
                return View(model);
            }
        }
        TempData["Success"] = $"Cập nhật tài khoản {user.Email} thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["Error"] = "Không thể xóa tài khoản đang đăng nhập!";
            return RedirectToAction(nameof(Index));
        }
        await _userManager.DeleteAsync(user);
        TempData["Success"] = $"Xóa tài khoản {user.Email} thành công!";
        return RedirectToAction(nameof(Index));
    }
}

// ViewModels
public class UserViewModel
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string? HoTen { get; set; }
    public string Role { get; set; } = "";
    public string? EmployeeName { get; set; }
    public string? EmployeeMaNV { get; set; }
}

public class CreateUserViewModel
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string Email { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.MinLength(6)]
    public string Password { get; set; } = "";

    public string? HoTen { get; set; }
    public string? Role { get; set; }
    public int? EmployeeId { get; set; }
}

public class EditUserViewModel
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string? HoTen { get; set; }
    public string? Role { get; set; }
    public int? EmployeeId { get; set; }

    [System.ComponentModel.DataAnnotations.MinLength(6)]
    public string? NewPassword { get; set; }
}
