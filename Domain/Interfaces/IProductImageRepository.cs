﻿using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductImageRepository : IGenericRepository<ProductImage, int>
    {
        Task<IEnumerable<ProductImage>> GetImagesByVariantIdAsync(int variantId);
        public Task DeleteImagesByVariantIdAsync(int variantId);

    }
}
