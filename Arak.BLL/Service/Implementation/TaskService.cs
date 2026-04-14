using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly IGenericRepository<Assignment> _repository;

        public TaskService(IGenericRepository<Assignment> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Assignment>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<Assignment?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<Assignment> CreateAsync(Assignment entity)
        {
            var created = await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
            return created;
        }
        public async Task<Assignment> UpdateAsync(Assignment entity)
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
