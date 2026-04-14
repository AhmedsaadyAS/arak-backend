using Arak.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    public interface IParentService
    {
        Task<IEnumerable<Parent>> GetAllAsync();
        Task<Parent> GetByIdAsync(int id);
        Task<Parent> CreateAsync(Parent entity);
        Task<Parent> UpdateAsync(Parent entity);
        Task<bool> DeleteAsync(int id);
    }
}
