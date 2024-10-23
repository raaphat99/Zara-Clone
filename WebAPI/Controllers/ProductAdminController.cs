using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.DTOs.ProductDTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductAdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var productsQuery = _unitOfWork.Products.GetAllWithVariantsAndImages().Where(p => p.StockQuantity > 0);

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
                        .FirstOrDefault(),
                })
                .ToListAsync();
            foreach (var product in products)
            {
                if (product.CategoryId.HasValue)
                {
                    var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryId.Value); // استخدام Value هنا

                    if (category != null)
                    {
                        if (category.ParentCategoryId.HasValue)
                        {
                            var parentCategoryId = category.ParentCategoryId.Value;
                            var parentCategory = await _unitOfWork.Categories.GetByIdAsync(parentCategoryId);

                            if (parentCategory != null)
                            {
                                if (parentCategory.ParentCategoryId.HasValue)
                                {
                                    var parentCategory2 = await _unitOfWork.Categories.GetByIdAsync(parentCategory.ParentCategoryId.Value);
                                    product.CategoryName = parentCategory2.Name;
                                }
                                else
                                {
                                    product.CategoryName = parentCategory.Name;
                                }
                            }
                        }
                        else
                        {
                            product.CategoryName = category.Name;
                        }
                    }
                }
            }

            return Ok(products);
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductByID(int id)
        {
            // جلب المنتج بناءً على الـ id
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            // إذا لم يتم العثور على المنتج
            if (product == null)
            {
                return NotFound();
            }

            // قائمة لأسماء الفلاتر
            List<string> filterNames = new List<string>();

            // التحقق مما إذا كان للمنتج CategoryId
            if (product.CategoryId.HasValue)
            {
                // جلب الفلاتر بناءً على CategoryId
                var filters = await _unitOfWork.Filters.GetFiltersByCategoryIdAsync(product.CategoryId.Value);

                // استخراج أسماء الفلاتر وإضافتها إلى القائمة
                filterNames = filters.Select(f => f.Name).ToList();
            }

            // إنشاء الـ DTO للمنتج
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
                FilterName = filterNames,  // تعيين أسماء الفلاتر
                MainImageUrl = product.ProductVariants
                    .SelectMany(pv => pv.ProductImage)
                    .FirstOrDefault()?.ImageUrl,  // تعيين الرابط للصورة الأساسية
            };

            // التحقق مما إذا كان المنتج ينتمي إلى فئة معينة
            if (product.CategoryId.HasValue)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryId.Value);

                // إذا كانت الفئة موجودة ولها فئة أب
                if (category != null && category.ParentCategoryId.HasValue)
                {
                    var parentCategoryId = category.ParentCategoryId.Value;
                    var parentCategory = await _unitOfWork.Categories.GetByIdAsync(parentCategoryId);

                    // إذا كانت الفئة الأب موجودة
                    if (parentCategory != null)
                    {
                        // التحقق مما إذا كانت الفئة الأب لها فئة أب أخرى
                        if (parentCategory.ParentCategoryId.HasValue)
                        {
                            var parentCategory2 = await _unitOfWork.Categories.GetByIdAsync(parentCategory.ParentCategoryId.Value);
                            productDto.CategoryName = parentCategory2?.Name;
                        }
                        else
                        {
                            productDto.CategoryName = parentCategory.Name;
                        }
                    }
                }
            }

            // إرجاع المنتج DTO
            return Ok(productDto);
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
                SizeId = pv.SizeId, // Make sure this is set correctly
                SizeName = pv.Size?.Value.ToString() ?? string.Empty, // Assuming Size has a Name property
                CategoryId = product.CategoryId, // Extracting CategoryId from the product
                ImageUrls = pv.ProductImage.Select(img => img.ImageUrl).ToList()
            }).ToList();

            return Ok(productVariantDtos);
        }

        //[HttpGet("{id:int}/variants")]
        //public async Task<IActionResult> GetProductVariants(int id)
        //{
        //    var product = await _unitOfWork.Products
        //        .Find(prd => prd.Id == id)
        //        .Include(prd => prd.ProductVariants)
        //        .ThenInclude(pv => pv.ProductImage)
        //        .FirstOrDefaultAsync();

        //    if (product == null)
        //        return NotFound();

        //    var x = _unitOfWork.Products.FindSingle(s => s.Id == id);
        //    var productVariantDtos = product.ProductVariants.Select(pv => new VariantForDetailsScreenDto
        //    {
        //        Id = pv.Id,
        //        Price = pv.Price,
        //        DiscountPercentage = pv.DiscountPercentage,
        //        DiscountedPrice = pv.DiscountedPrice,
        //        StockQuantity = pv.StockQuantity,
        //        Created = pv.Created,
        //        Updated = pv.Updated,
        //        ProductColor = pv.ProductColor.ToString(),
        //        ProductMaterial = pv.ProductMaterial.ToString(),
        //        SizeName = pv.Size.Value.ToString(),
        //        ImageUrls = pv.ProductImage.Select(img => img.ImageUrl).ToList()
        //    }).ToList();

        //    return Ok(productVariantDtos);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await _unitOfWork.Products
                .FindAsync(p => p.Name == product.Name && p.CategoryId == product.CategoryId);

            if (existingProduct.Count() != 0)
            {
                return Conflict("Product with the same name and category already exists.");
            }

            product.Created = DateTime.UtcNow;
            var newProduct = new Domain.Models.Product()
            {
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                StockQuantity = product.StockQuantity,
                Price = product.Price,
                Created = DateTime.UtcNow,
            };

            await _unitOfWork.Products.AddAsync(newProduct); // Assuming you have a DbSet<Product> in your DbContext
            await _unitOfWork.Complete();

            return CreatedAtAction(nameof(GetProductByID), new { id = newProduct.Id }, newProduct);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProduct(int id, [FromBody] ProductDto product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await _unitOfWork.Products.FindSingle(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }

            var conflictingProducts = await _unitOfWork.Products
                .FindAsync(p => p.Name == product.Name && p.CategoryId == product.CategoryId && p.Id != id);

            if (conflictingProducts.Any())
            {
                return Conflict("Product with the same name and category already exists.");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Updated = DateTime.UtcNow;

            _unitOfWork.Products.Update(existingProduct);
            await _unitOfWork.Complete();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingProduct = await _unitOfWork.Products.FindSingle(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }

            _unitOfWork.Products.Remove(existingProduct);
            await _unitOfWork.Complete();

            return Ok("Product Deleted");
        }

        [HttpGet("colors")]
        public IActionResult GetAllColors()
        {
            var colors = _unitOfWork.ProductVariant.GetAllColors();
            return Ok(colors);
        }
        [HttpGet("material")]
        public IActionResult GetAllMaterial()
        {
            var material = _unitOfWork.ProductVariant.GetAllMaterial();
            return Ok(material);
        }
        [HttpGet("size")]
        public IActionResult GetAllSize()
        {
            var size = _unitOfWork.SizeType.GetAll();
            var prosize = new List<SizeDTO>();
            foreach (var size1 in size)
            {
                prosize.Add(new SizeDTO
                {
                    Id = size1.Id,
                    // تحويل Type إلى string باستخدام ToString
                    sizevalue = size1.Type.ToString()
                });
            }
            return Ok(prosize);
        }

    }

}
