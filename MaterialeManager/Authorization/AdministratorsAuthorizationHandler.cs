using MaterialeManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialeManager.Authorization
{
	public class AdministratorsAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Case>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Case resource)
		{
			if (context.User == null)
			{
				return Task.CompletedTask;
			}

			if (context.User.IsInRole(Constants.CaseAdministratorsRole))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
