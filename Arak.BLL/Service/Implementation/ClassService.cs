using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class ClassService : IClassService
    {
        private readonly IGenericRepository<Class> _repository;

        public ClassService(IGenericRepository<Class> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Class>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<Class?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<Class> CreateAsync(Class entity)
        {
            var created = await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
            return created;
        }
        public async Task<Class> UpdateAsync(Class entity)
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
