using Arak.DAL.Database;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arak.DAL.Repository.Implementation
{
    public class TimetableRepository : GenericRepository<TimeTable> ,ITimetableRepository
    {
        public TimetableRepository(AppDbContext context) : base(context) { }

        public async Task<ICollection<TimeTable>> GetTimetableByClassId(int classId)
        {
            var timetables = await _context.TimeTables.Where(x => x.ClassId == classId).ToListAsync();
            return timetables;
        }

        public async Task<ICollection<TimeTable>> GetTimetableByTeacherId(int teacherId)
        {
            var timetables = await _context.TimeTables.Where(x => x.TeacherId == teacherId).ToListAsync();
            return timetables;
        }

        public async Task<ICollection<TimeTable>> GetTimetableInStudent(int TimeClassId)
        {
            //var StdclsId = await _context.Students.Where(n => n.ClassId == StudentClassId).ToListAsync();

            var timetables = await _context.TimeTables.Where(x => x.ClassId == TimeClassId).ToListAsync();
            return timetables;
            
        }
    }
}
