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
    public class TimetableRepository : GenericRepository<TimeTable>, ITimetableRepository
    {
        public TimetableRepository(AppDbContext context) : base(context) { }

        public async Task<ICollection<TimeTable>> GetTimetableByClassIdAsync(int classId)
        {
            return await _context.TimeTables
                .Include(t => t.Subject)
                .Include(t => t.Semester)
                .Where(x => x.ClassId == classId)
                .ToListAsync();
        }

        public async Task<ICollection<TimeTable>> GetTimetableByTeacherIdAsync(int teacherId)
        {
            return await _context.TimeTables
                .Include(t => t.Subject)
                .Include(t => t.Semester)
                .Where(x => x.TeacherId == teacherId)
                .ToListAsync();
        }
    }
}
