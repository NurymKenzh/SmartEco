using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SmartEco.Common.Enums;

namespace SmartEco.API.Helpers.Attributes
{
    public class AuthorizeEnumAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        readonly Role[] _requiredRoles;

        public AuthorizeEnumAttribute(params Role[] roles)
        {
            _requiredRoles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated;
            if (isAuthenticated is false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var user = context.HttpContext.User;
            var hasAllRequredClaims = _requiredRoles.Any(role => context.HttpContext.User.HasClaim(x => x.Value == nameof(role)));
            if (hasAllRequredClaims is false)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
