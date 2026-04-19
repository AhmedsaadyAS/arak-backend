using Arak.BLL.DTOs;

namespace Arak.BLL.Service.Abstraction
{
    public interface IAttendanceService
    {
        Task<AttendanceDto> MarkAttendanceAsync(MarkAttendanceDto dto);
        Task<IEnumerable<AttendanceDto>> BulkMarkAttendanceAsync(BulkMarkAttendanceDto dto);
        Task<PagedResult<AttendanceDto>> GetClassAttendanceByDateAsync(int classId, DateOnly date, int page, int pageSize);
        Task<IEnumerable<AttendanceDto>> GetStudentAttendanceByMonthAsync(int studentId, int month, int year);
        Task<StudentAttendanceStatsDto> GetStudentStatsAsync(int studentId);
        Task<AttendanceDto> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto);
        Task<int> BulkUpdateTimeOutAsync(BulkUpdateTimeOutDto dto);
    }
}
