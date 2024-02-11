using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NZWALKS.API.CustomActionFilters
{

    public class ApiError
    {
        public IEnumerable<string> Errors { get; set; }

        public ApiError(ModelStateDictionary modelState)
        {
            Errors = modelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        }
    }
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                // Handle model validation errors:

                // 1. Create an error response object:
                var errorResponse = new BadRequestObjectResult(new ApiError(context.ModelState));

                // 2. Set the response status code and content:
                errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.ContentTypes.Add(MediaTypeNames.Application.Json);

                // 3. Set the response context's result to the error response:
                context.Result = errorResponse;

                // 4. Prevent further action execution:
                return;
            }

            // If model is valid, continue with action execution:
            base.OnActionExecuting(context);
        }
    }
}
