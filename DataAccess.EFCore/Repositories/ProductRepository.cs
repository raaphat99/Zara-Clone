using DataAccess.EFCore.Data;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Color = Domain.Enums.Color;

namespace DataAccess.EFCore.Repositories
{
    public class ProductRepository : GenericRepository<Product, int>, IProductRepository
    {
        public ProductRepository(ApplicationContext applicationContext) : base(applicationContext)
        { }

        public IQueryable<Product> GetAllWithVariantsAndImages()
        {
            var products = _dbSet;
            return products
                .Include(prd => prd.ProductVariants)
                .ThenInclude(variant => variant.ProductImage);
        }

        public IQueryable<ProductVariant> GetAllVariants()
        {
            return _dbContext.ProductVariants.AsQueryable();
        }

        public IQueryable<ProductVariant> GetVariantsByProductId(int productId)
        {
            return _dbContext.ProductVariants
                    .Include(variant => variant.Size)
                    .Include(variant => variant.ProductImage)
                    .Where(variant => variant.ProductId == productId);
        }


        public async Task<ICollection<Product>> SearchProductsAsync(
            string searchTerm,
            string? category = null,
            double? minPrice = null,
            double? maxPrice = null,
            string? size = null,
            Color? color = null,
            Material? material = null)
        {
            // Split the search query into individual keywords
            var keywords = searchTerm?.Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            // Retrieve all products with their variants, category included
            var allProducts = await _dbSet
                .Include(prd => prd.ProductVariants)
                .ThenInclude(v => v.Size)  // Include Size if necessary
                .Include(prd => prd.Category) // Include Category
                .ToListAsync();

            // Prepare lists of valid materials and colors
            var validProductMaterials = Enum.GetNames(typeof(Material)).Select(name => name.ToLowerInvariant()).ToList();
            var validProductColors = Enum.GetNames(typeof(Color)).Select(name => name.ToLowerInvariant()).ToList();

            // Apply filters in-memory
            var filteredProducts = allProducts.AsQueryable();

            // If there are keywords, search for them in various fields
            if (keywords.Length != 0)
            {
                foreach (var keyword in keywords)
                {
                    var keywordLower = keyword.ToLowerInvariant();
                    filteredProducts = filteredProducts.Where(p =>
                        p.Name.ToLowerInvariant().Contains(keywordLower) ||
                        p.Description.ToLowerInvariant().Contains(keywordLower) ||
                        p.Category.Name.ToLowerInvariant().Contains(keywordLower) ||
                        p.ProductVariants.Any(v =>
                            validProductMaterials.Contains(v.ProductMaterial.ToString().ToLowerInvariant()) &&
                            v.ProductMaterial.ToString().ToLowerInvariant().Contains(keywordLower) ||
                            validProductColors.Contains(v.ProductColor.ToString().ToLowerInvariant()) &&
                            v.ProductColor.ToString().ToLowerInvariant().Contains(keywordLower)
                        ));
                }
            }

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                filteredProducts = filteredProducts.Where(prd => prd.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.Price >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.Price <= maxPrice.Value));
            }

            // Filter by color
            if (color.HasValue)
            {
                filteredProducts = filteredProducts.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.ProductColor == color.Value));
            }

            // Filter by material
            if (material.HasValue)
            {
                filteredProducts = filteredProducts.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.ProductMaterial == material.Value));
            }

            // Filter by size
            if (!string.IsNullOrEmpty(size))
            {
                filteredProducts = filteredProducts.Where(p => p.ProductVariants.Any(v => v.Size.Value.ToString() == size));
            }

            return filteredProducts.ToList(); // Return the filtered list of products
        }

        public string GetSizeTypeByProductId(int productId)
        {
            var product = _dbContext.Products
                .Include(p => p.Category)
                .ThenInclude(c => c.SizeType)
                .FirstOrDefault(p => p.Id == productId);

            return product?.Category?.SizeType?.Type.ToString();
        }

        //#region Old implemetation for search functionality
        //public async Task<ICollection<Product>> SearchProductsAsync(
        //    string searchTerm,
        //    string? category = null,
        //    double? minPrice = null,
        //    double? maxPrice = null,
        //    string? size = null,
        //    Color? color = null,
        //    Material? material = null)
        //{

        //    // Split the search query into individual keywords
        //    var keywords = searchTerm?.Split(" ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        //    IQueryable<Product> products = _dbSet.Include(prd => prd.ProductVariants).AsQueryable();

        //    var validProductMaterials = Enum.GetNames(typeof(Material)).Select(name => name.ToLowerInvariant()).ToList();
        //    var validProductColors = Enum.GetNames(typeof(Color)).Select(name => name.ToLowerInvariant()).ToList();


        //    // If there are keywords, search for them in various fields
        //    if (keywords.Length != 0)
        //    {
        //        foreach (var keyword in keywords)
        //        {
        //            var keywordLower = keyword.ToLowerInvariant();

        //            products = products.Where(p =>
        //                p.Name.Contains(keyword) ||
        //                p.Description.Contains(keyword) ||
        //                p.Category.Name.Contains(keyword));
        //            //||
        //            //p.ProductVariants.Any(v => v.ProductMaterial.ToString() == keyword ||
        //            //                          v.ProductColor.ToString() == keyword));
        //        }
        //    }

        //    // Filter by category
        //    if (!string.IsNullOrEmpty(category))
        //    {
        //        products = products.Where(prd => prd.Category.Name == category);
        //    }

        //    // Filter by price range
        //    if (minPrice.HasValue)
        //    {
        //        products = products.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.Price >= minPrice.Value));
        //    }

        //    if (maxPrice.HasValue)
        //    {
        //        products = products.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.Price <= maxPrice.Value));
        //    }

        //    // Filter by color
        //    if (color.HasValue)
        //    {
        //        products = products.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.ProductColor == color.Value));
        //    }

        //    // Filter by material
        //    if (material.HasValue)
        //    {
        //        products = products.Where(prd => prd.ProductVariants.Any(prdVariant => prdVariant.ProductMaterial == material.Value));
        //    }

        //    // Filter by size
        //    if (!string.IsNullOrEmpty(size))
        //    {
        //        products = products.Where(p => p.ProductVariants.Any(v => v.Size.Value.ToString() == size));
        //    }

        //    return await products.ToListAsync();
        //}
        //#endregion
    }
}
