using Arak.BLL.DTOs;

namespace Arak.BLL.Service.Abstraction
{
    public interface IAttendanceService
    {
        Task<AttendanceDto> MarkAttendanceAsync(MarkAttendanceDto dto);
        Task<int> BulkMarkAttendanceAsync(BulkMarkAttendanceDto dto, int teacherId);
        Task<ClassAttendanceResponseDto> GetClassAttendanceAsync(int classId, DateOnly date);
        Task<AttendanceSummaryDto> GetClassSummaryAsync(int classId, DateOnly date);
        Task<StudentAttendanceDetailDto> GetStudentAttendanceDetailsAsync(int studentId, int month, int year, string userId, string role);
        Task<AttendanceDto> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto);
        Task<int> BulkUpdateTimeoutAsync(BulkTimeoutDto dto, int teacherId);
    }
}
