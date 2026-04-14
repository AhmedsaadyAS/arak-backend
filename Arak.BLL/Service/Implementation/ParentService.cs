using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class ParentService : IParentService
    {
        private readonly IGenericRepository<Parent> _repository;

        public ParentService(IGenericRepository<Parent> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Parent>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<Parent?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<Parent> CreateAsync(Parent entity)
        {
            var created = await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
            return created;
        }
        public async Task<Parent> UpdateAsync(Parent entity)
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
