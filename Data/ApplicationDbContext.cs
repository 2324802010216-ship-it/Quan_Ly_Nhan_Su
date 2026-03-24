using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebQuanLyNhanSu.Models;

namespace WebQuanLyNhanSu.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Salary> Salaries => Set<Salary>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<Recruitment> Recruitments => Set<Recruitment>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<Training> Trainings => Set<Training>();
    public DbSet<TrainingEmployee> TrainingEmployees => Set<TrainingEmployee>();
    public DbSet<RewardDiscipline> RewardDisciplines => Set<RewardDiscipline>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // === UNIQUE INDEXES ===
        builder.Entity<Employee>().HasIndex(e => e.MaNV).IsUnique();
        builder.Entity<Contract>().HasIndex(c => c.MaHopDong).IsUnique();
        builder.Entity<Attendance>().HasIndex(a => new { a.EmployeeId, a.Ngay }).IsUnique();
        builder.Entity<Salary>().HasIndex(s => new { s.EmployeeId, s.Thang, s.Nam }).IsUnique();
        builder.Entity<TrainingEmployee>().HasIndex(te => new { te.TrainingId, te.EmployeeId }).IsUnique();

        // === FK CASCADE RULES ===
        builder.Entity<Department>()
            .HasOne(d => d.TruongPhong).WithMany()
            .HasForeignKey(d => d.TruongPhongId).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Employee>()
            .HasOne(e => e.Department).WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(e => e.Position).WithMany(p => p.Employees)
            .HasForeignKey(e => e.PositionId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Employee).WithMany()
            .HasForeignKey(u => u.EmployeeId).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<LeaveRequest>()
            .HasOne(lr => lr.Employee).WithMany(e => e.LeaveRequests)
            .HasForeignKey(lr => lr.EmployeeId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<LeaveRequest>()
            .HasOne(lr => lr.NguoiDuyet).WithMany()
            .HasForeignKey(lr => lr.NguoiDuyetId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Recruitment>()
            .HasOne(r => r.Department).WithMany(d => d.Recruitments)
            .HasForeignKey(r => r.PhongBanId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Recruitment>()
            .HasOne(r => r.Position).WithMany(p => p.Recruitments)
            .HasForeignKey(r => r.PositionId).OnDelete(DeleteBehavior.Restrict);

        // === DECIMAL PRECISION ===
        builder.Entity<Salary>(e =>
        {
            e.Property(s => s.LuongCoBan).HasColumnType("decimal(18,2)");
            e.Property(s => s.PhuCap).HasColumnType("decimal(18,2)");
            e.Property(s => s.TangCa).HasColumnType("decimal(18,2)");
            e.Property(s => s.BHXH).HasColumnType("decimal(18,2)");
            e.Property(s => s.BHYT).HasColumnType("decimal(18,2)");
            e.Property(s => s.BHTN).HasColumnType("decimal(18,2)");
            e.Property(s => s.KhauTru).HasColumnType("decimal(18,2)");
            e.Property(s => s.ThucLanh).HasColumnType("decimal(18,2)");
        });
    }
}
