using MaterialeManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialeManager.Authorization
{
	public class IsOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Case>
	{
		UserManager<IdentityUser> _userManager;

		public IsOwnerAuthorizationHandler(UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Case resource)
		{
			if (context.User == null || resource == null)
			{
				// Return Task.FromResult(0) if targeting a version of
				// .NET Framework older than 4.6:
				return Task.CompletedTask;
			}


			// If we're not asking for CRUD permission, return.
			if (requirement.Name != Constants.UpdateOperationName &&
				requirement.Name != Constants.DeleteOperationName)
			{
				return Task.CompletedTask;
			}


			if (resource.PhotographerID == _userManager.GetUserId(context.User))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}