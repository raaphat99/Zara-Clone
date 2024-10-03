using DataAccess.EFCore.Repositories;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public partial class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //    #region Expensive ProductDto
        //    var expensiveProducts = _unitOfWork.Products.GetMostExpensiveProducts(count);
        //    List<ProductDto> productdto = new List<ProductDto>();
        //        foreach (var product in expensiveProducts)
        //        {
        //            var x = new ProductDto()
        //            {
        //                Id = product.Id,
        //                Name = product.Name,
        //                Price = product.Price,
        //                StockQuntity = product.StockQuntity
        //            };
        //    productdto.Add(x);
        //        }
        //        return Ok(productdto);
        //#endregion

        [HttpGet("/api/products")]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Products.GetAll();
            return Ok(products);
        }



        [HttpGet("/api/products/{id:int}")]
        public async Task<IActionResult> GetProductByID(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }



        [HttpGet("/api/products/category/{categoryId:int}")]
        public IActionResult GetProductsByCategory(int categoryID)
        {
            var products = _unitOfWork.Products.Find(prd => prd.CategoryId == categoryID);
            return Ok(products);
        }



        [HttpGet("/api/products/{id:int}/variants")]
        public async Task<IActionResult> GetProductVariants(int id)
        {
            var product = await _unitOfWork.Products
                .Find(prd => prd.Id == id)
                .Include(prd => prd.ProductVariants)
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product.ProductVariants);
        }



        // Get all products with id=7 woman and all dress with id=10
        [HttpGet("/api/products/{mainCategoryId:int}")]
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

            return Ok(products);
        }
        [HttpGet("variant/{id:int}")]
        public async Task<IActionResult> GetProductVariantById(int id)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByIdAsync(id);

            if (productVariant == null)
            {
                return NotFound($"Product variant with ID {id} not found.");
            }
            var productvariantdto = new ProductVariantforC_M_P()
            {
                Id = productVariant.Id,
                ProductId = productVariant.ProductId??0,
                ProductName = productVariant.Product.Name,
                Price = productVariant.Price,
                ProductColor = productVariant.ProductColor,
                // SizeName = productVariant.Product.,
               // SizeId = productVariant.SizeId,
                StockQuantity = productVariant.StockQuntity,
                ProductMaterial = productVariant.ProductMaterial,
                Created = productVariant.Created,
                Updated = productVariant.Updated
            };
            return Ok(productvariantdto);
        }
        [HttpPost("variant")]
        public async Task<IActionResult> AddProductVariant([FromBody] ProductVariantforC_M_P productVariantDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productVariant = new ProductVariant
            {
                ProductId = productVariantDto.ProductId,
                //SizeId = productVariantDto.SizeId,
                Price = productVariantDto.Price,
                StockQuntity = productVariantDto.StockQuantity,  // Ensure this matches the DTO
                ProductColor = productVariantDto.ProductColor,
                ProductMaterial = productVariantDto.ProductMaterial,
                Created = DateTime.UtcNow,  // It's a good practice to use UtcNow for date/time
                Updated = DateTime.UtcNow
            };

            await _unitOfWork.ProductVariant.AddAsync(productVariant);
            await _unitOfWork.Complete();

            return CreatedAtAction(nameof(GetProductVariantById), new { id = productVariant.Id }, productVariant);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchProductVariants([FromQuery] List<string>? colors, [FromQuery] List<string>? materials, [FromQuery] double? priceFrom, [FromQuery] double? priceTo)
        {
            // Convert color strings to enum values
            List<Color> colorEnums = new List<Color>();
            if (colors != null && colors.Count > 0)
            {
                foreach (var color in colors)
                {
                    if (Enum.TryParse(color, true, out Color parsedColor))
                    {
                        colorEnums.Add(parsedColor);
                    }
                }
            }

            // Convert material strings to enum values
            List<Material> materialEnums = new List<Material>();
            if (materials != null && materials.Count > 0)
            {
                foreach (var material in materials)
                {
                    if (Enum.TryParse(material, true, out Material parsedMaterial))
                    {
                        materialEnums.Add(parsedMaterial);
                    }
                }
            }

            // Get all product variants
            var allVariants = await _unitOfWork.ProductVariant.GetAllAsync();

            // Filter product variants based on color, material, and price
            var productVariants = await _unitOfWork.ProductVariant.FindAsync(pv =>
                (colorEnums.Count == 0 || colorEnums.Contains(pv.ProductColor)) &&
                (materialEnums.Count == 0 || materialEnums.Contains(pv.ProductMaterial)) &&
                (!priceFrom.HasValue || pv.Price >= priceFrom) &&
                (!priceTo.HasValue || pv.Price <= priceTo)
            );

            // If no variants are found, return NotFound
            if (productVariants == null || !productVariants.Any())
            {
                return NotFound("No product variants found.");
            }

            // Prepare the result DTO
            List<ProductVariantforC_M_P> productvariantdto = new List<ProductVariantforC_M_P>();
            foreach (var variant in productVariants)
            {
                productvariantdto.Add(new ProductVariantforC_M_P()
                {
                    Id = variant.Id,
                    ProductName = variant.Product.Name,
                    ProductColor = variant.ProductColor,
                    ProductMaterial = variant.ProductMaterial,
                    Price = variant.Price,
                    ProductId = variant.ProductId ?? 0,
                    StockQuantity = variant.StockQuntity
                });
            }

            return Ok(productvariantdto);
        }

        [HttpGet("variants")]
        public async Task<IActionResult> GetAllProductVariants()
        {
            var productVariants = await _unitOfWork.ProductVariant.GetAllAsync();
            return Ok(productVariants);
        }
        [HttpDelete("variant/{id:int}")]
        public async Task<IActionResult> DeleteProductVariant(int id)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByIdAsync(id);

            if (productVariant == null)
            {
                return NotFound($"Product Variant with ID {id} not found.");
            }

            _unitOfWork.ProductVariant.Remove(productVariant);
            await _unitOfWork.Complete();

            return NoContent();
        }


        [HttpPut("variant/{id:int}/stock")] //  دي لما يحصل order هنادي عليها علشان تقلل العدد المتاح منها 
        public async Task<IActionResult> UpdateStockQuantity(int id, [FromBody] int newStockQuantity)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByIdAsync(id);

            if (productVariant == null)
            {
                return NotFound($"Product Variant with ID {id} not found.");
            }

            productVariant.StockQuntity = newStockQuantity;
            productVariant.Updated = DateTime.UtcNow;

            _unitOfWork.ProductVariant.Update(productVariant);
            await _unitOfWork.Complete();

            return NoContent();
        }

        [HttpPut("variant/{id:int}")] //  مش جربتها 
        public async Task<IActionResult> UpdateProductVariant(int id, [FromBody] ProductVariantforC_M_P productVariantDto)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByIdAsync(id);

            if (productVariant == null)
            {
                return NotFound($"Product Variant with ID {id} not found.");
            }

            productVariant.Price = productVariantDto.Price;
            productVariant.StockQuntity = productVariantDto.StockQuantity;
            productVariant.ProductColor = productVariantDto.ProductColor;
            productVariant.ProductMaterial = productVariantDto.ProductMaterial;
            productVariant.Updated = DateTime.UtcNow;

            _unitOfWork.ProductVariant.Update(productVariant);
            await _unitOfWork.Complete();

            return NoContent();
        }


    }
}
