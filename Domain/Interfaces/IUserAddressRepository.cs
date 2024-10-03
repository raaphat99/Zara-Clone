using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserAddressRepository : IGenericRepository<UserAddress, int>
    {
        Task<IEnumerable<UserAddress>> GetAllByUserIdAsync(string userId);
    }
}
