using Arak.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    public interface ITimetableService
    {
        Task<IEnumerable<TimeTable>> GetAllAsync();
        Task<TimeTable?> GetByIdAsync(int id);
        Task<TimeTable> AddLesson(TimeTable timeTable);
        Task<ICollection<TimeTable>> GetTimetableByClassId(int classId);
        Task<ICollection<TimeTable>> GetTimetableByTeacherId(int teacherId);
        Task<TimeTable> UpdateAsync(TimeTable timeTable);
        Task<bool> DeleteAsync(int Id);
    }
}
