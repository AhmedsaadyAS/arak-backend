using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class TeacherService : ITeacherService
    {
        private readonly IGenericRepository<Teacher> _repository;

        public TeacherService(IGenericRepository<Teacher> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<Teacher?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<Teacher> CreateAsync(Teacher entity)
        {
            var created = await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
            return created;
        }
        public async Task<Teacher> UpdateAsync(Teacher entity)
        {
            var updated = await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return updated;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);
            if (result) await _repository.SaveChangesAsync();
            return result;
        }
    }
}
