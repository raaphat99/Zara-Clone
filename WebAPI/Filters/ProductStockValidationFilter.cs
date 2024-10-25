using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Filters
{
    public class ProductStockValidationFilter : IActionFilter
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductStockValidationFilter(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // This will be called before the action executes
            if (context.HttpContext.Request.Method == "GET")
            {
                var productsWithZeroStock = _unitOfWork.Products.GetAll().Where(p => p.StockQuantity == 0).ToList();

                if (productsWithZeroStock.Any())
                {
                    // Remove products with zero stock from the response
                    context.Result = new BadRequestObjectResult("Some products have zero stock and cannot be returned.");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

    }
}
