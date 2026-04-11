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

        public async Task<ICollection<TimeTable>> GetTimetableByClassId(int classId)
        {
            return await _timetableRepository.GetTimetableByClassId(classId);
        }

        public async Task<ICollection<TimeTable>> GetTimetableByTeacherId(int classId)
        {
            return await _timetableRepository.GetTimetableByTeacherId(classId);
        }

        public async Task<ICollection<TimeTable>> GetTimetableInStudent(int TimeClassId)
        {
            return await _timetableRepository.GetTimetableInStudent(TimeClassId);
        }
        public async Task<TimeTable> AddLesson(TimeTable timeTable)
        {

          return  await _timetableRepository.CreateAsync(timeTable);
        }

        public async Task<TimeTable> UpdateAsync(TimeTable timeTable)
        {
            return await _timetableRepository.UpdateAsync(timeTable);
        }

        public async Task<bool> DeleteAsync(int Id)
        {

            var result = await _timetableRepository.DeleteAsync(Id);
            return result;
        }
    }
}
