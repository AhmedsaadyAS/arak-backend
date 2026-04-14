using Arak.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    public interface IEventService
    {
        Task<IEnumerable<ArakEvent>> GetAllAsync();
        Task<ArakEvent> GetByIdAsync(int id);
        Task<ArakEvent> CreateAsync(ArakEvent entity);
        Task<ArakEvent> UpdateAsync(ArakEvent entity);
        Task<bool> DeleteAsync(int id);
    }
}
