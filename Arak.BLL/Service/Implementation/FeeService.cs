using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class FeeService : IFeeService
    {
        private readonly IGenericRepository<Fee> _repository;

        public FeeService(IGenericRepository<Fee> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Fee>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<Fee?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<Fee> CreateAsync(Fee entity)
        {
            var created = await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
            return created;
        }
        public async Task<Fee> UpdateAsync(Fee entity)
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
