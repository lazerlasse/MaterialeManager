using MaterialeManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialeManager.Authorization
{
	public class OperatorsAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Case>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Case resource)
		{
			if (context.User == null || resource == null)
			{
				// Return Task.FromResult(0) if targeting a version of
				// .NET Framework older than 4.6:
				return Task.CompletedTask;
			}


			// If we're not asking for following permission, return.
			if (requirement.Name != Constants.ReadOperationName &&
				requirement.Name != Constants.CreateOperationName &&
				requirement.Name != Constants.AcceptOperationName &&
				requirement.Name != Constants.ErrorOperationName &&
				requirement.Name != Constants.IsOperatorOperationName)
			{
				return Task.CompletedTask;
			}


			if (context.User.IsInRole(Constants.CaseOperatorsRole))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
