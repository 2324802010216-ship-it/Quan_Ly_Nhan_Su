using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Controllers;

public class AccountController : BaseController
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(ApplicationDbContext ctx, UserManager<ApplicationUser> um,
        SignInManager<ApplicationUser> sim) : base(ctx, um) { _signInManager = sim; }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, bool rememberMe, string? returnUrl)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            TempData["Error"] = "Email hoặc mật khẩu không đúng!";
            return View();
        }
        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, false);
        if (result.Succeeded)
            return LocalRedirect(returnUrl ?? "/Dashboard");

        TempData["Error"] = "Email hoặc mật khẩu không đúng!";
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        ViewBag.Employees = new SelectList(
            await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
        return View();
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string email, string hoTen, string password, string role, int? employeeId)
    {
        var user = new ApplicationUser
        {
            UserName = email, Email = email,
            HoTen = hoTen, EmployeeId = employeeId,
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, role);
            TempData["Success"] = "Tạo tài khoản thành công!";
            return RedirectToAction("Index", "Dashboard");
        }
        foreach (var err in result.Errors)
            TempData["Error"] = err.Description;

        ViewBag.Employees = new SelectList(
            await _context.Employees.OrderBy(e => e.MaNV).ToListAsync(), "Id", "HoTen");
        return View();
    }

    public IActionResult AccessDenied() => View();
}
