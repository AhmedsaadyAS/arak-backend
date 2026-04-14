using Arak.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    public interface ISubjectService
    {
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject> GetByIdAsync(int id);
        Task<Subject> CreateAsync(Subject entity);
        Task<Subject> UpdateAsync(Subject entity);
        Task<bool> DeleteAsync(int id);
    }
}
