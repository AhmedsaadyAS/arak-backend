using Arak.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Repository.Abstraction
{
    public interface ITimetableRepository : IGenericRepository<TimeTable>
    {
        Task<ICollection<TimeTable>> GetTimetableByClassIdAsync(int classId);
        Task<ICollection<TimeTable>> GetTimetableByTeacherIdAsync(int teacherId);
    }
}
