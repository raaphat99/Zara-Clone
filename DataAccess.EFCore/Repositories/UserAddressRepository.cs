using DataAccess.EFCore.Data;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Repositories
{
    public class UserAddressRepository : GenericRepository<UserAddress, int>, IUserAddressRepository
    {
        private readonly ApplicationContext _context;

        public UserAddressRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserAddress>> GetAllByUserIdAsync(string userId)
        {
            return await _context.UserAddresses
                .Where(address => address.UserId == userId)
                .ToListAsync();
        }
    }
}
