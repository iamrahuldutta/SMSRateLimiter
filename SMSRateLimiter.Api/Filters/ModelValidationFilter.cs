using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SMSRateLimiter.Api.Filters
{
    public class ModelValidationFilter : IActionFilter
    {
        private readonly ILogger<ModelValidationFilter> _logger;

        public ModelValidationFilter(ILogger<ModelValidationFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                // Get action details
                var actionName = context.ActionDescriptor.DisplayName;
                // Alternatively, you can extract specific route values:
                // var controllerName = context.RouteData.Values["controller"]?.ToString();
                // var actionName = context.RouteData.Values["action"]?.ToString();

                foreach (var error in context.ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("Validation error in {Action}: {ErrorMessage}", actionName, error.ErrorMessage);
                }
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action required after execution.
        }
    }
}
