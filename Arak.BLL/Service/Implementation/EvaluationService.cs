using Arak.BLL.Service.Abstraction;
using Arak.DAL.Entities;
using Arak.DAL.Repository.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Implementation
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IGenericRepository<Evaluation> _repository;

        public EvaluationService(IGenericRepository<Evaluation> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Evaluation>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task<Evaluation?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        public async Task<Evaluation> CreateAsync(Evaluation entity)
        {
            var created = await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
            return created;
        }
        public async Task<Evaluation> UpdateAsync(Evaluation entity)
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
