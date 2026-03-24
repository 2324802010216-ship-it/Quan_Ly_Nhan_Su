using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Data;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Controllers;

public abstract class BaseController : Controller
{
    protected readonly ApplicationDbContext _context;
    protected readonly UserManager<ApplicationUser> _userManager;

    protected BaseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    protected async Task<int?> GetCurrentEmployeeId()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return user?.EmployeeId;
    }

    protected async Task<int?> GetCurrentDepartmentId()
    {
        var empId = await GetCurrentEmployeeId();
        if (empId == null) return null;
        var emp = await _context.Employees.FindAsync(empId);
        return emp?.DepartmentId;
    }
}
