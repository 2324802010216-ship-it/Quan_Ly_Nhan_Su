using System.Security.Claims;
using WebQuanLyNhanSu.Models;
using WebQuanLyNhanSu.ViewModels;

namespace WebQuanLyNhanSu.Services;

public interface ISalaryService
{
    Task<Salary> CalculateSalary(int employeeId, int thang, int nam, decimal phuCap, decimal soGioTangCa, decimal khauTru, string nguoiTinh);
}

public interface ILeaveRequestService
{
    Task<LeaveBalanceViewModel> GetLeaveBalance(int employeeId, int nam);
    Task<bool> ApproveLeaveRequest(int leaveId, int approverId);
    Task<bool> RejectLeaveRequest(int leaveId, string lyDo);
}

public interface IAttendanceService
{
    Task<AttendanceReportViewModel> GetMonthlyReport(int thang, int nam, int? departmentId);
}

public interface INotificationService
{
    Task<int> GetNotificationCount(ClaimsPrincipal user);
}

public interface IReportService
{
    Task<byte[]> ExportEmployeeReport(int? departmentId);
    Task<byte[]> ExportSalaryReport(int thang, int nam);
    Task<byte[]> ExportAttendanceReport(int thang, int nam, int? departmentId);
    Task<byte[]> ExportLeaveReport(int? thang, int? nam, int? departmentId);
}

public interface IFileUploadService
{
    Task<string?> UploadFile(IFormFile file, string subFolder);
    void DeleteFile(string? filePath);
}
