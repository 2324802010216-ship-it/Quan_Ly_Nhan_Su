using Microsoft.AspNetCore.Identity;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.Models.Enums;

namespace WebQuanLyNhanSu.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // 1. Roles
        string[] roles = { "Admin", "HRManager", "DeptManager", "Employee" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // 2. Admin account
        if (await userManager.FindByEmailAsync("admin@hrms.com") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@hrms.com",
                Email = "admin@hrms.com",
                HoTen = "Quản Trị Viên",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin@123456");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // 3. Sample data
        if (!context.Departments.Any())
        {
            var departments = new[]
            {
                new Department { TenPhongBan = "Phòng Nhân sự", MoTa = "Quản lý nhân sự toàn công ty", NgayThanhLap = new DateTime(2020, 1, 1) },
                new Department { TenPhongBan = "Phòng Kỹ thuật", MoTa = "Phát triển sản phẩm", NgayThanhLap = new DateTime(2020, 1, 1) },
                new Department { TenPhongBan = "Phòng Kinh doanh", MoTa = "Bán hàng và marketing", NgayThanhLap = new DateTime(2020, 3, 1) },
                new Department { TenPhongBan = "Phòng Kế toán", MoTa = "Tài chính kế toán", NgayThanhLap = new DateTime(2020, 1, 1) },
            };
            context.Departments.AddRange(departments);
            await context.SaveChangesAsync();

            var positions = new[]
            {
                new Position { TenChucVu = "Giám đốc", CapBac = 10, MucLuongCoSo = 50_000_000 },
                new Position { TenChucVu = "Trưởng phòng", CapBac = 8, MucLuongCoSo = 30_000_000 },
                new Position { TenChucVu = "Phó phòng", CapBac = 7, MucLuongCoSo = 25_000_000 },
                new Position { TenChucVu = "Nhân viên", CapBac = 5, MucLuongCoSo = 15_000_000 },
                new Position { TenChucVu = "Thực tập sinh", CapBac = 1, MucLuongCoSo = 5_000_000 },
            };
            context.Positions.AddRange(positions);
            await context.SaveChangesAsync();

            var hrDept = context.Departments.First(d => d.TenPhongBan == "Phòng Nhân sự");
            var techDept = context.Departments.First(d => d.TenPhongBan == "Phòng Kỹ thuật");
            var tpPos = context.Positions.First(p => p.TenChucVu == "Trưởng phòng");
            var nvPos = context.Positions.First(p => p.TenChucVu == "Nhân viên");

            var employees = new[]
            {
                new Employee
                {
                    MaNV = "NV001", HoTen = "Nguyễn Văn An",
                    NgaySinh = new DateTime(1990, 5, 15), GioiTinh = GioiTinh.Nam,
                    CCCD = "012345678901", Email = "an.nv@hrms.com",
                    SoDienThoai = "0901234567", NgayVaoLam = new DateTime(2020, 1, 15),
                    DepartmentId = hrDept.Id, PositionId = tpPos.Id,
                    TrangThai = TrangThaiNhanVien.DangLamViec
                },
                new Employee
                {
                    MaNV = "NV002", HoTen = "Trần Thị Bình",
                    NgaySinh = new DateTime(1992, 8, 20), GioiTinh = GioiTinh.Nu,
                    CCCD = "012345678902", Email = "binh.tt@hrms.com",
                    SoDienThoai = "0901234568", NgayVaoLam = new DateTime(2020, 3, 1),
                    DepartmentId = techDept.Id, PositionId = nvPos.Id,
                    TrangThai = TrangThaiNhanVien.DangLamViec
                },
            };
            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();

            // Link Admin → Employee
            var adminUser = await userManager.FindByEmailAsync("admin@hrms.com");
            var empAn = context.Employees.First(e => e.MaNV == "NV001");
            adminUser!.EmployeeId = empAn.Id;
            await userManager.UpdateAsync(adminUser);
            hrDept.TruongPhongId = empAn.Id;
            await context.SaveChangesAsync();

            // Contracts
            context.Contracts.AddRange(
                new Contract
                {
                    EmployeeId = empAn.Id, MaHopDong = "HD001",
                    LoaiHopDong = LoaiHopDong.KhongThoiHan,
                    NgayBatDau = new DateTime(2020, 1, 15),
                    LuongHopDong = 30_000_000,
                    TrangThai = TrangThaiHopDong.HieuLuc
                },
                new Contract
                {
                    EmployeeId = employees[1].Id, MaHopDong = "HD002",
                    LoaiHopDong = LoaiHopDong.CoThoiHan,
                    NgayBatDau = new DateTime(2024, 1, 1),
                    NgayKetThuc = DateTime.Today.AddDays(20),
                    LuongHopDong = 15_000_000,
                    TrangThai = TrangThaiHopDong.HieuLuc
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
