using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using Arak.DAL.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class TimetableService : ITimetableService
    {
        private readonly ITimetableRepository _timetableRepository;
        public TimetableService(ITimetableRepository timetableRepository)
        {
            _timetableRepository = timetableRepository;
        }

        public async Task<IEnumerable<TimeTable>> GetAllAsync() => await _timetableRepository.GetAllAsync();
        public async Task<TimeTable> GetByIdAsync(int id) => await _timetableRepository.GetByIdAsync(id);

        public async Task<ICollection<TimeTable>> GetTimetableByClassId(int classId)
        {
            return await _timetableRepository.GetTimetableByClassIdAsync(classId);
        }

        public async Task<ICollection<TimeTable>> GetTimetableByTeacherId(int teacherId)
        {
            return await _timetableRepository.GetTimetableByTeacherIdAsync(teacherId);
        }
        public async Task<TimeTable> AddLesson(TimeTable timeTable)
        {
            var created = await _timetableRepository.CreateAsync(timeTable);
            await _timetableRepository.SaveChangesAsync();
            return created;
        }

        public async Task<TimeTable> UpdateAsync(TimeTable timeTable)
        {
            var updated = await _timetableRepository.UpdateAsync(timeTable);
            await _timetableRepository.SaveChangesAsync();
            return updated;
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            var result = await _timetableRepository.DeleteAsync(Id);
            if (result) await _timetableRepository.SaveChangesAsync();
            return result;
        }
    }
}
