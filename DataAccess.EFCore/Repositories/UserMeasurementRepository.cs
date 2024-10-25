using DataAccess.EFCore.Data;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Repositories
{
    public class UserMeasurementRepository : GenericRepository<UserMeasurement, int>, IUserMeasurementRepository
    {
        public UserMeasurementRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }
    }
}
