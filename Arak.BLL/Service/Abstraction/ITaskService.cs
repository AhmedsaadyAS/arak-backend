using Arak.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    public interface ITaskService
    {
        Task<IEnumerable<Assignment>> GetAllAsync();
        Task<Assignment> GetByIdAsync(int id);
        Task<Assignment> CreateAsync(Assignment entity);
        Task<Assignment> UpdateAsync(Assignment entity);
        Task<bool> DeleteAsync(int id);
    }
}
