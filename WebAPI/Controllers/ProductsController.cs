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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var productsQuery = _unitOfWork.Products.GetAllWithVariantsAndImages();

            var products = await productsQuery
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Created = p.Created,
                    Updated = p.Updated,
                    CategoryId = p.CategoryId,
                    MainImageUrl = p.ProductVariants
                    .SelectMany(v => v.ProductImage)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }



        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductByID(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            string productSizeType = _unitOfWork.Products.GetSizeTypeByProductId(id);

            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Created = product.Created,
                Updated = product.Updated,
                CategoryId = product.CategoryId,
                MainImageUrl = product.ProductVariants
                    .SelectMany(pv => pv.ProductImage)
                    .FirstOrDefault()?.ImageUrl,
                SizeType = productSizeType
            };

            return Ok(productDto);
        }



        [HttpGet("{id:int}/colors")]
        public async Task<IActionResult> GetProductDistinctColors(int id)
        {
            var distinctColors = await _unitOfWork.ProductVariant
                .GetAll()
                .Where(pv => pv.ProductId == id)
                .Select(pv => pv.ProductColor.ToString())
                .Distinct()
                .ToListAsync();

            return Ok(distinctColors);
        }



        [HttpGet("{id:int}/variants")]
        public async Task<IActionResult> GetProductVariants(int id)
        {
            var product = await _unitOfWork.Products
                .Find(prd => prd.Id == id)
                .Include(prd => prd.ProductVariants)
                .ThenInclude(pv => pv.ProductImage)
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            // Map the product variants to DTOs
            var productVariantDtos = product.ProductVariants.Select(pv => new VariantForDetailsScreenDto
            {
                Id = pv.Id,
                Price = pv.Price,
                DiscountPercentage = pv.DiscountPercentage,
                DiscountedPrice = pv.DiscountedPrice,
                StockQuantity = pv.StockQuantity,
                Created = pv.Created,
                Updated = pv.Updated,
                ProductColor = pv.ProductColor.ToString(),
                ProductMaterial = pv.ProductMaterial.ToString(),
                SizeName = pv.Size.Value.ToString(),
                ImageUrls = pv.ProductImage.Select(img => img.ImageUrl).ToList()
            }).ToList();

            return Ok(productVariantDtos);
        }



        [HttpGet("category/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _unitOfWork.Products
                .Find(prd => prd.CategoryId == categoryId)
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return NotFound($"No products found for category ID {categoryId}");
            }

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
                        ProductId = variant.ProductId ?? 0,
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
