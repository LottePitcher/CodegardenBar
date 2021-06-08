using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CgBarBackend.Authorization
{
    public class RequireConfigPasswordFilter : Attribute, IActionFilter
    {
        private readonly string _configItem;

        public RequireConfigPasswordFilter(string configItem)
        {
            _configItem = configItem;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var _configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            if (context.HttpContext.Request.Headers["adminPassword"] != _configuration[_configItem])
            {
                throw new AuthenticationException("Invalid admin password");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
