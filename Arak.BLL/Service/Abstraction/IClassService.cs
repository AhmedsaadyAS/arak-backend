using Arak.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    public interface IClassService
    {
        Task<IEnumerable<Class>> GetAllAsync();
        Task<Class> GetByIdAsync(int id);
        Task<Class> CreateAsync(Class entity);
        Task<Class> UpdateAsync(Class entity);
        Task<bool> DeleteAsync(int id);
    }
}
