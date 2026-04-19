using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepository<Subject> _repository;
        private readonly IGenericRepository<Teacher> _teacherRepository;

        public SubjectService(IGenericRepository<Subject> repository, IGenericRepository<Teacher> teacherRepository)
        {
            _repository = repository;
            _teacherRepository = teacherRepository;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<Subject?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<Subject> CreateAsync(Subject entity)
        {
            var created = await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
            return created;
        }
        public async Task<Subject> UpdateAsync(Subject entity)
        {
            var updated = await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return updated;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            // Unlink all assigned teachers first
            var teachers = await _teacherRepository.GetAllAsync();
            foreach (var t in teachers)
            {
                if (t.SubjectId == id)
                {
                    t.SubjectId = null;
                    await _teacherRepository.UpdateAsync(t);
                }
            }
            await _teacherRepository.SaveChangesAsync();

            var result = await _repository.DeleteAsync(id);
            if (result)
            {
                try
                {
                    await _repository.SaveChangesAsync();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    throw new System.InvalidOperationException("Cannot delete this entity as it is currently referenced by other active records. Please re-assign or remove them first.");
                }
            }
            return result;
        }
    }
}
