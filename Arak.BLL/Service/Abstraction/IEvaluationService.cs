using Arak.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    public interface IEvaluationService
    {
        Task<IEnumerable<Evaluation>> GetAllAsync();
        Task<Evaluation> GetByIdAsync(int id);
        Task<Evaluation> CreateAsync(Evaluation entity);
        Task<Evaluation> UpdateAsync(Evaluation entity);
        Task<bool> DeleteAsync(int id);
    }
}
