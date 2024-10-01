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
        public UserAddressRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }
    }
}
