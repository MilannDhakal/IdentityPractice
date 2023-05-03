using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace IdentityPractice.Filters
{
    public class AllowOnlyAdminAttribute : IAuthorizationFilter
    {
        public AllowOnlyAdminAttribute()
        {

        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
           
        }
    }
}
