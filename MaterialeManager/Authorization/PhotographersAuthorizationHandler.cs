using MaterialeManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialeManager.Authorization
{
	public class PhotographersAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Case>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Case resource)
		{
			if (context.User == null || resource == null)
			{
				// Return Task.FromResult(0) if targeting a version of
				// .NET Framework older than 4.6:
				return Task.CompletedTask;
			}


			// If we're not asking for CRUD permission or approval/reject, return.
			if (requirement.Name != Constants.CreateOperationName &&
				requirement.Name != Constants.ReadOperationName &&
				requirement.Name != Constants.IsPhotographerOperationName)
			{
				return Task.CompletedTask;
			}


			if (context.User.IsInRole(Constants.CasePhotographerRole))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
