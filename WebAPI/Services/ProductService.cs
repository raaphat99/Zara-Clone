using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Services
{
    public class ProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> UpdateStockQuantity(int variantId, int orderedQuantity)
        {
            var productVariant = await _unitOfWork.ProductVariant.GetByIdAsync(variantId);

            if (productVariant == null)
            {
                throw new ArgumentException("Product variant not found");
            }

            // Check if there's enough stock
            if (productVariant.StockQuantity < orderedQuantity)
            {
                throw new InvalidOperationException("Not enough stock available");
            }

            // Update the stock quantity
            productVariant.StockQuantity -= orderedQuantity;

            // Update the product variant in the repository
            _unitOfWork.ProductVariant.Update(productVariant);

            // Save changes to the database
            return await _unitOfWork.Complete() > 0;
        }
    }
}
