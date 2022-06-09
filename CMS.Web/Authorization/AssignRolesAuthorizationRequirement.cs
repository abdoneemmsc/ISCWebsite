// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using DAL.Core;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Web.Authorization
{
    public class AssignRolesAuthorizationRequirement : IAuthorizationRequirement
    {

    }



    public class AssignRolesAuthorizationHandler : AuthorizationHandler<AssignRolesAuthorizationRequirement, (string[] newRoles, string[] currentRoles)>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AssignRolesAuthorizationRequirement requirement, (string[] newRoles, string[] currentRoles) roles)
        {
            if (!GetIsRolesChanged(roles.newRoles, roles.currentRoles))
            {
                context.Succeed(requirement);
            }


            return Task.CompletedTask;
        }


        private bool GetIsRolesChanged(string[] newRoles, string[] currentRoles)
        {
            if (newRoles == null)
                newRoles = new string[] { };

            if (currentRoles == null)
                currentRoles = new string[] { };


            bool roleAdded = newRoles.Except(currentRoles).Any();
            bool roleRemoved = currentRoles.Except(newRoles).Any();

            return roleAdded || roleRemoved;
        }


        private bool GetIsUserInAllAddedRoles(ClaimsPrincipal contextUser, string[] newRoles, string[] currentRoles)
        {
            if (newRoles == null)
                newRoles = new string[] { };

            if (currentRoles == null)
                currentRoles = new string[] { };


            var addedRoles = newRoles.Except(currentRoles);

            return addedRoles.All(role => contextUser.IsInRole(role));
        }
    }
}