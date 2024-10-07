using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartHome.Controllers.Security.Attributes;
using SmartHome.Data.Interfaces;
using SmartHome.Data.Sqlite;
using SmartHome.Security.Oauth2;
using System.Reflection;

namespace SmartHome.Controllers.Security
{
    public class SecureAccessController : Controller
    {
        private string AuthenticatedUseremail = "";
        private IDatabaseProcessor _database = new SqliteProcessor();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Type controllerType = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ControllerTypeInfo.AsType();
            var actionName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ActionName;
            var action = controllerType.GetMethod(actionName);
                   
            AuthenticationRequiredAttribute _authRequired = (AuthenticationRequiredAttribute)action.GetCustomAttribute(typeof(AuthenticationRequiredAttribute), true);

            if (_authRequired != null)
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();

                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                {
                    context.Result = Json(new { Access = false, Message = "Access token not found." });
                }
                else
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();

                    // Pass the token for further validation and user lookup
                    var userEmail = TokenProcessor.ValidateAndExtractEmail(token);

                    if (userEmail == null)
                    {
                        context.Result = Json(new { Access = false, Message = "Invalid token or user not found." });
                    }
                    else
                    {
                        // All OK.
                        this.AuthenticatedUseremail = userEmail;
                    }
                }                  
            }

            base.OnActionExecuting(context);
        }
    }
}
