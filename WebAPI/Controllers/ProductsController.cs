using DataAccess.EFCore.Data;
using DataAccess.EFCore.Repositories;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using WebAPI.DTOs;
using WebAPI.DTOs.ProductDTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            // Fetch products by categoryId
            var products = await _unitOfWork.Products
                .Find(prd => prd.CategoryId == categoryId)
                .ToListAsync();

            var filters = await _unitOfWork.Filters.GetFiltersByCategoryIdAsync(categoryId);
            // Fetch filters by categoryId

            // Check if there are no products
            if (products == null || products.Count == 0)
            {
                return NotFound($"No products found for category ID {categoryId}");
            }
            var filterNames = filters.Select(f => f.Name).ToList();


            // Convert filter data into a list of filter names
            // Create product DTOs including the filter names
            var productDtos = products.Select(prd => new ProductDto
            {
                Id = prd.Id,
                Name = prd.Name,
                Description = prd.Description,
                Price = prd.Price,
                StockQuantity = prd.StockQuantity,
                Created = prd.Created,
                Updated = prd.Updated,
                CategoryId = prd.CategoryId,
                FilterName = filterNames,
                MainImageUrl = prd.ProductVariants
                    .SelectMany(pv => pv.ProductImage)
                    .FirstOrDefault()?.ImageUrl
            }).ToList();

            return Ok(productDtos);
        }


        [HttpGet("any-category/{mainCategoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int mainCategoryId, int? subCategoryId = null)
        {
            var mainCategory = await _unitOfWork.Categories.GetByIdAsync(mainCategoryId);

            if (mainCategory == null)
            {
                return NotFound($"No category found with ID {mainCategoryId}.");
            }

            IQueryable<Product> products;
            if (subCategoryId.HasValue)
            {
                var subCategory = await _unitOfWork.Categories.GetByIdAsync(subCategoryId.Value);
                if (subCategory == null || subCategory.ParentCategoryId != mainCategoryId)
                {
                    return NotFound($"No subcategory found with ID {subCategoryId} under main category ID {mainCategoryId}.");
                }
                products = _unitOfWork.Products.Find(p => p.CategoryId == subCategoryId);
            }
            else
            {
                products = _unitOfWork.Products.Find(p => p.CategoryId == mainCategoryId);
            }

            if (products == null || !products.Any())
            {
                return NotFound("No products found for the specified category.");
            }

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Created = p.Created,
                Updated = p.Updated,
                CategoryId = p.CategoryId,
                MainImageUrl = p.ProductVariants.SelectMany(pv => pv.ProductImage).FirstOrDefault().ImageUrl
            }).ToList();

            return Ok(productDtos);
        }



        [HttpGet("size/{sizeId:int}", Name = "GetProductsBySize")]
        public IActionResult GetProductsBySize(int sizeId)
        {
            // Query all products that have at least one variant with the specified sizeId
            var products = _unitOfWork.Products
                .Find(prd => prd.ProductVariants.Any(variant => variant.SizeId == sizeId))
                .Include(prd => prd.ProductVariants)
                .ToList();

            if (products == null || !products.Any())
                return NotFound("No products found with the selected size.");


            var productDtos = products.Select(prd => new ProductDto
            {
                Id = prd.Id,
                Name = prd.Name,
                Description = prd.Description,
                Price = prd.Price,
                StockQuantity = prd.StockQuantity,
                Created = prd.Created,
                Updated = prd.Updated,
                CategoryId = prd.CategoryId,
                MainImageUrl = prd.ProductVariants.SelectMany(pv => pv.ProductImage).FirstOrDefault().ImageUrl
            }).ToList();

            return Ok(productDtos);
        }

        [HttpGet("subcategory/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsBySubCategory(int categoryId)
        {
            // البحث عن الفئة باستخدام ID
            var category = await _unitOfWork.Categories.FindSingle(s => s.Id == categoryId);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            // قائمة لحفظ كل المنتجات الخاصة بالفئات الفرعية
            List<ProductDto> productsList = new List<ProductDto>();

            // البحث عن جميع الفئات الفرعية التي ParentCategoryId يساوي parentId للفئة الرئيسية
            var subcategories = await _unitOfWork.Categories.FindAsync(c => c.ParentCategoryId == category.ParentCategoryId);

            if (!subcategories.Any())
            {
                return NotFound("No subcategories found for the given category.");
            }

            foreach (var subcategory in subcategories)
            {
                var products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == subcategory.Id);

                if (products != null && products.Any())
                {
                    var productDtos = products.Select(prd => new ProductDto
                    {
                        Id = prd.Id,
                        Name = prd.Name,
                        Description = prd.Description,
                        Price = prd.Price,
                        StockQuantity = prd.StockQuantity,
                        Created = prd.Created,
                        Updated = prd.Updated,
                        CategoryId = prd.CategoryId,
                        MainImageUrl = prd.ProductVariants
                            .SelectMany(pv => pv.ProductImage)
                            .FirstOrDefault()?.ImageUrl
                    }).ToList();

                    productsList.AddRange(productDtos);
                }
            }

            if (!productsList.Any())
            {
                return NotFound("No products found for the given subcategories.");
            }

            return Ok(productsList);
        }


        [HttpPut("deactivate/{productId}")]
        public async Task<IActionResult> DeactivateProductStock(int productId)
        {
            // البحث عن المنتج باستخدام ID
            var product = await _unitOfWork.Products.GetByIdAsync(productId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // تعيين StockQuantity إلى 0
            product.StockQuantity = 0;

            // تحديث المنتج في قاعدة البيانات
            _unitOfWork.Products.Update(product);
            await _unitOfWork.Complete(); // تأكد من حفظ التغييرات

            return Ok("Product stock quantity set to zero.");
        }
        [HttpPut("deactivateVariant/{productVId}")]
        public async Task<IActionResult> DeactivateProductVariantStock(int productVId)
        {
            // البحث عن المنتج باستخدام ID
            var productVariant = await _unitOfWork.ProductVariant.GetByIdAsync(productVId);

            if (productVariant == null)
            {
                return NotFound("Product variant not found.");
            }

            // تعيين StockQuantity إلى 0
            productVariant.StockQuantity = 0;

            // تحديث المنتج في قاعدة البيانات
            _unitOfWork.ProductVariant.Update(productVariant);
            await _unitOfWork.Complete(); // تأكد من حفظ التغييرات

            return Ok("Product variant stock quantity set to zero.");
        }


        [HttpGet("{productId:int}/sizes")]
        public async Task<IActionResult> GetAllProductSizes(int productId)
        {
            // Query the product variants and join to their sizes, filtering by productId
            var productVariants = await _unitOfWork.ProductVariant
                .Find(variant => variant.ProductId == productId)
                .Include(variant => variant.Size)
                .ToListAsync();

            // Extract distinct sizes from product variants
            var productSizes = productVariants
                .Select(variant => variant.Size)
                .Distinct()
                .ToList();

            if (productSizes == null || !productSizes.Any())
                return NotFound("No sizes found for this product.");

            var productSizeDtos = productSizes.Select(ps => new SizeDTO()
            {
                Id = ps.Id,
                Value = ps.Value
            }).ToList();

            return Ok(productSizeDtos);
        }



        [HttpGet("filter")]
        public async Task<IActionResult> FilterProductVariants(
            [FromQuery] int? productId,   // معلمة productId
            [FromQuery] int? categoryId,  // إضافة معلمة categoryId
            [FromQuery] List<string>? colors, [FromQuery] List<string>? materials,
            [FromQuery] double? priceFrom, [FromQuery] double? priceTo,
            [FromQuery] List<string>? sizes)
        {
            List<Color> colorEnums = colors?.Select(c => Enum.TryParse(c, true, out Color parsedColor) ? parsedColor : (Color?)null)
                                            .Where(c => c.HasValue)
                                            .Select(c => c.Value).ToList() ?? new List<Color>();

            List<Material> materialEnums = materials?.Select(m => Enum.TryParse(m, true, out Material parsedMaterial) ? parsedMaterial : (Material?)null)
                                                    .Where(m => m.HasValue)
                                                    .Select(m => m.Value).ToList() ?? new List<Material>();

            List<SizeValue> sizeEnums = sizes?.Select(s => Enum.TryParse(s, true, out SizeValue parsedSize) ? parsedSize : (SizeValue?)null)
                                            .Where(s => s.HasValue)
                                            .Select(s => s.Value).ToList() ?? new List<SizeValue>();

            // الحصول على جميع الـ ProductVariants
            var allVariants = await _unitOfWork.ProductVariant.GetAllAsync();

            // تصفية البيانات باستخدام productId، categoryId والمعلمات الأخرى
            var productVariants = await _unitOfWork.ProductVariant.FindAsync(
                pv => (!productId.HasValue ||
                       pv.ProductId == productId) &&  // تحقق من productId
                      (!categoryId.HasValue ||
                       pv.Product.CategoryId ==
                           categoryId) &&  // تحقق من categoryId
                      (colorEnums.Count == 0 ||
                       colorEnums.Contains(pv.ProductColor)) &&
                      (materialEnums.Count == 0 ||
                       materialEnums.Contains(pv.ProductMaterial)) &&
                      (!priceFrom.HasValue || pv.Price >= priceFrom) &&
                      (!priceTo.HasValue || pv.Price <= priceTo) &&
                      (sizeEnums.Count == 0 ||
                       sizeEnums.Contains(pv.Size.Value)));

            // التحقق من وجود أي نتائج
            if (productVariants == null || !productVariants.Any())
            {
                return NotFound("No product variants found.");
            }

            // تحويل النتائج إلى Dtos
            var productVariantDtos =
                productVariants
                    .Select(variant => new ProductVariantforC_M_P()
                    {
                        Id = variant.Id,
                        ProductName = variant.Product.Name,
                        ProductColor = variant.ProductColor,
                        ProductMaterial = variant.ProductMaterial,
                        Price = variant.Price,
                        productId = variant.ProductId ?? 0,
                        StockQuantity = variant.StockQuantity,
                        SizeId = variant.SizeId,
                        SizeValue = variant.Size.Value.ToString(),
                    })
                    .ToList();

            // إعادة النتائج
            return Ok(productVariantDtos);
        }
    }
}
