﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflec tion;

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
