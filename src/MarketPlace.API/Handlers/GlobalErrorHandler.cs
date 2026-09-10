using MarketPlace.Domain.Common.BaseErrors;
using MarketPlace.Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;


namespace PortfolioApp.WebApi.Handlers;


public static class GlobalErrorHandler
{
    public static IActionResult ToActionResult(this BaseError baseError) 
    {
     var problemDetails = new ProblemDetails
     {
         Title = baseError.Title,
         Status = (int)baseError.StatusCode,
         Detail = baseError.Message,
        
     };

return baseError.StatusCode switch
{
    
    StatusCode.BadRequest => new BadRequestObjectResult(problemDetails),
    StatusCode.NotFound => new NotFoundObjectResult(problemDetails),
    StatusCode.Unauthorized => new UnauthorizedObjectResult(problemDetails),
    StatusCode.Forbidden => new ObjectResult(problemDetails) { StatusCode = (int)StatusCode.Forbidden },
    StatusCode.InternalServerError => new ObjectResult(problemDetails) { StatusCode = (int)StatusCode.InternalServerError },
    StatusCode.Validation => new ObjectResult(problemDetails) { StatusCode = (int)StatusCode.Validation },
    StatusCode.Conflict => new ConflictObjectResult(problemDetails),
    StatusCode.ServiceUnavailable => new ObjectResult(problemDetails) { StatusCode = (int)StatusCode.ServiceUnavailable },
    _ => new ObjectResult(problemDetails) { StatusCode = (int)StatusCode.InternalServerError }  
};
    }
}