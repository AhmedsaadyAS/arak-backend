using Arak.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    public interface ITeacherService
    {
        Task<IEnumerable<Teacher>> GetAllAsync();
        Task<Teacher> GetByIdAsync(int id);
        Task<Teacher> CreateAsync(Teacher entity);
        Task<Teacher> UpdateAsync(Teacher entity);
        Task<bool> DeleteAsync(int id);
    }
}
