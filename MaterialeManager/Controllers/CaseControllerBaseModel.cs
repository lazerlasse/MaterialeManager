using MaterialeManager.Authorization;
using MaterialeManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialeManager.Controllers
{
	public class CaseControllerBaseModel : Controller
	{
		protected ApplicationDbContext Context { get; }
		protected IAuthorizationService AuthorizationService { get; }
		protected UserManager<IdentityUser> UserManager { get; }


		public CaseControllerBaseModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager)
			: base()
		{
			Context = context;
			AuthorizationService = authorizationService;
			UserManager = userManager;
		}

		public async Task<SelectList> PopulateOperatorsDropDownListAsync(string selectedOperator = null)
		{
			var operatorsList = await UserManager.GetUsersInRoleAsync(Constants.CaseOperatorsRole);

			return new SelectList(operatorsList, "Id", "UserName", selectedOperator);
		}
	}
}
