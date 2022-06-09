// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using DAL.Core;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Web.Authorization
{
    public class ViewRoleAuthorizationRequirement : IAuthorizationRequirement
    {

    }



    public class ViewRoleAuthorizationHandler : AuthorizationHandler<ViewRoleAuthorizationRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewRoleAuthorizationRequirement requirement, string roleName)
        {
            if (context.User == null)
                return Task.CompletedTask;

            return Task.CompletedTask;
        }
    }
}
