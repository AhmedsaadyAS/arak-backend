using Arak.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arak.BLL.Service.Abstraction
{
    public interface IFeeService
    {
        Task<IEnumerable<Fee>> GetAllAsync();
        Task<Fee> GetByIdAsync(int id);
        Task<Fee> CreateAsync(Fee entity);
        Task<Fee> UpdateAsync(Fee entity);
        Task<bool> DeleteAsync(int id);
    }
}
