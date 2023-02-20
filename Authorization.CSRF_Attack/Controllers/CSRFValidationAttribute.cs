using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Authorization.CSRF_Attack.Controllers
{
    public class CSRFValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("X-CSRF-Key", out var csrfFromClient))
            {
                context.Result = new BadRequestObjectResult("Missing CSRF parameter.");
                return;
            }

            // Get csrf key from token and compare it with value from client.
            // We trust token because we generated it and it cannot be faked.
            var identity = context.HttpContext.User.Identity as ClaimsIdentity;
            var csrfFromToken = identity.Claims.First(c => c.Type == "csrf").Value;

            if (csrfFromToken != csrfFromClient)
            {
                context.Result = new BadRequestObjectResult("CSRF parameter mismatch.");
            }
        }
    }
}
