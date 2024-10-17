using Domain.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IFilterRepository
    {
        public Task<IEnumerable<Filter>> GetFiltersByCategoryIdAsync(int categoryId);
    }
}
