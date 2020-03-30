using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaterialeManager.Data;
using MaterialeManager.Models;
using MaterialeManager.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MaterialeManager.Authorization;

namespace MaterialeManager
{
	[Authorize]
	public class CasesController : CaseControllerBaseModel
	{
		public CasesController(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager)
			: base(context, authorizationService, userManager)
		{
		}

		// GET: Cases
		public async Task<IActionResult> Index()
		{
			var isAuthorized = User.IsInRole(Constants.CaseAdministratorsRole) ||
				User.IsInRole(Constants.CaseOperatorsRole) ||
				User.IsInRole(Constants.CasePhotographerRole);

			if (!isAuthorized)
			{
				return Forbid();
			}

			var cases = Context.Case
				.Include(p => p.Photographer)
				.Include(o => o.CaseOperator)
				.Where(
						c => c.CaseState == Case.States.Oprettet
						|| c.CaseState == Case.States.Klippes
						|| c.CaseState == Case.States.Fejlet)
				.AsNoTracking().OrderByDescending(c => c.Created);

			return View(await cases.ToListAsync());
		}

		// GET: Cases/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			// Check id not null.
			if (id == null)
			{
				return NotFound();
			}

			// Load case from db and include related data.
			var caseToView = await Context.Case
				.Include(o => o.CaseOperator)
				.Include(p => p.Photographer)
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.CaseID == id);

			// Check loaded case not null.
			if (caseToView == null)
			{
				return NotFound();
			}

			// Validate current user have read rights.
			var isAuthorized = await AuthorizationService.AuthorizeAsync(User, caseToView, CaseOperations.Read);

			// If Authorization failed return forbid.
			if (!isAuthorized.Succeeded)
			{
				return Forbid();
			}

			// Authorization succeded return view.
			return View(caseToView);
		}


		// ---------------------   Create methods   ----------------------------- //
		// GET: Cases/Create
		public async Task<IActionResult> CreateAsync()
		{
			// Create new empty case and set datetime to now.
			var newCase = new Case
			{
				Created = DateTime.Now
			};

			// Check if user can create.
			var canCreate = await AuthorizationService.AuthorizeAsync(User, newCase, CaseOperations.Create);

			// If authorization failed return forbid.
			if (!canCreate.Succeeded)
			{
				return Forbid();
			}

			// Return view.
			return View(newCase);
		}

		// POST: Cases/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("CaseID,Titel,Description,Comments,Created")] Case caseToCreate)
		{
			// Check model state is valid.
			if (ModelState.IsValid)
			{
				// Set properties for new case to create.
				caseToCreate.PhotographerID = UserManager.GetUserId(User);
				caseToCreate.CaseState = Case.States.Oprettet;

				// Check user can create.
				var canCreate = await AuthorizationService.AuthorizeAsync(User, caseToCreate, CaseOperations.Create);

				// Check authorization succeded or return forbid.
				if (!canCreate.Succeeded)
				{
					return Forbid();
				}

				// Save case to db.
				Context.Add(caseToCreate);
				await Context.SaveChangesAsync();

				// Return to index.
				return RedirectToAction(nameof(Index));
			}

			// Model state not valid so return view.
			return View(caseToCreate);
		}


		// -------------------------------- Edit methods --------------------------------
		// GET: Cases/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			// Check id not null.
			if (id == null)
			{
				return NotFound();
			}

			// Load case and related data to edit...
			var caseToEdit = await Context.Case
				.Include(o => o.CaseOperator)
				.Include(p => p.Photographer)
				.FirstOrDefaultAsync(i => i.CaseID == id);

			// Check loaded case not null.
			if (caseToEdit == null)
			{
				return NotFound();
			}

			// Check if user can edit.
			var canEdit = await AuthorizationService.AuthorizeAsync(User, caseToEdit, CaseOperations.Update);

			// Check authorization succeded or return forbid.
			if (!canEdit.Succeeded)
			{
				return Forbid();
			}

			// Authorization succeded return view and case to edit.
			return View(caseToEdit);
		}

		// POST: Cases/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditCase(int? id)
		{
			// Check id not null.
			if (id == null)
			{
				return NotFound();
			}

			// Load case to edit.
			var caseToEdit = await Context.Case
				.Include(o => o.CaseOperator)
				.Include(p => p.Photographer)
				.FirstOrDefaultAsync(c => c.CaseID == id);

			// Check case is loaded.
			if (caseToEdit == null)
			{
				return NotFound();
			}

			// Check user can edit.
			var canEdit = await AuthorizationService.AuthorizeAsync(User, caseToEdit, CaseOperations.Update);

			// Validate authorization.
			if (!canEdit.Succeeded)
			{
				return Forbid();
			}

			// Try update model async.
			if (await TryUpdateModelAsync<Case>(
				caseToEdit,
				"",
				c => c.Titel, c => c.Description, c => c.Comments))
			{
				// Try save changes to db.
				try
				{
					await Context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					//Log the error (uncomment ex variable name and write a log.)
					ModelState.AddModelError("", "Unable to save changes. " +
						"Try again, and if the problem persists, " +
						"see your system administrator.");
				}

				// Succeded return to index.
				return RedirectToAction(nameof(Index));
			}

			// Update failed return view.
			return View(caseToEdit);
		}


		// ----------------------------- Delete methods  --------------------------------------
		// GET: Cases/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			// Check id not null.
			if (id == null)
			{
				return NotFound();
			}

			// Load case to delete from db and include related data.
			var caseToDelete = await Context.Case
				.Include(o => o.CaseOperator)
				.Include(p => p.Photographer)
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.CaseID == id);

			// Check loaded case not null.
			if (caseToDelete == null)
			{
				return NotFound();
			}

			// Check user can delete.
			var canDelete = await AuthorizationService.AuthorizeAsync(User, caseToDelete, CaseOperations.Delete);

			// Validate authorization succeded or return forbid.
			if (!canDelete.Succeeded)
			{
				return Forbid();
			}

			// Authorization succeded return View.
			return View(caseToDelete);
		}

		// POST: Cases/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			// Load case to delete from db.
			var caseToDelete = await Context.Case
				.AsNoTracking()
				.FirstOrDefaultAsync(i => i.CaseID == id);

			// Check loaded case not null.
			if (caseToDelete == null)
			{
				return NotFound();
			}

			// Check the user can delete.
			var canDelete = await AuthorizationService.AuthorizeAsync(User, caseToDelete, CaseOperations.Delete);

			// Check Authorization succeded or return forbid.
			if (!canDelete.Succeeded)
			{
				return Forbid();
			}

			// Authorization succeded - remove case from db.
			Context.Case.Remove(caseToDelete);
			await Context.SaveChangesAsync();

			// Return to index.
			return RedirectToAction(nameof(Index));
		}



		// --------------------------------- Accept case section ---------------------------- //
		// Get: Cases/Accept/5
		[HttpGet]
		public async Task<IActionResult> Accept(int? id)
		{
			// Check id not null..
			if (id == null)
			{
				return NotFound();
			}

			// Get the case to accept from db and include related data.
			var caseToAccept = await Context.Case
				.Include(p => p.Photographer)
				.FirstOrDefaultAsync(c => c.CaseID == id);

			// Check if loaded case not null.
			if (caseToAccept == null)
			{
				return NotFound();
			}

			// Check user can accept.
			var canAccept = await AuthorizationService.AuthorizeAsync(User, caseToAccept, CaseOperations.Accept);

			// Validate authentication.
			if (!canAccept.Succeeded)
			{
				return Forbid();
			}

			// If case already accepted.
			if (caseToAccept.CaseState == Case.States.Klippes)
			{
				// Check current user is the operator of selected case.
				if (caseToAccept.CaseOperatorID == UserManager.GetUserId(User))
				{
					return View(caseToAccept);
				}
				else
				{
					return Forbid();
				}
			}

			// Set case state to accepted and set operator name to current user.
			caseToAccept.CaseState = Case.States.Klippes;
			caseToAccept.CaseOperatorID = UserManager.GetUserId(User);

			// Try update case...
			try
			{
				Context.Case.Update(caseToAccept);
				await Context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				throw;
			}

			// Return view.
			return View(caseToAccept);
		}

		// Get: Action Published on Accept Page.
		public async Task<IActionResult> Published(int? id)
		{
			// Check id not null.
			if (id == null)
			{
				return NotFound();
			}

			// Get case to publish.
			var caseToPublish = await Context.Case
				.FirstOrDefaultAsync(i => i.CaseID == id);

			// Check user have accept rights..
			var canAccept = await AuthorizationService.AuthorizeAsync(User, caseToPublish, CaseOperations.Accept);

			// Validate authentication.
			if (!canAccept.Succeeded)
			{
				Forbid();
			}

			// Set case to published.
			caseToPublish.CaseState = Case.States.Udgivet;

			// Try update data.
			try
			{
				Context.Case.Update(caseToPublish);
				await Context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{

				throw ex;
			}

			// Return to index view of cases to handle.
			return RedirectToAction(nameof(Index));
		}



		// --------------------------- Set case error section --------------------------------
		// Get: Cases/SetCaseError/5.
		public async Task<IActionResult> SetCaseError(int? id)
		{
			// Check id not null.
			if (id == null)
			{
				return NotFound();
			}

			// Get the case from db to update.
			var caseToUpdate = await Context.Case
				.Include(p => p.Photographer)
				.Include(o => o.CaseOperator)
				.FirstOrDefaultAsync(c => c.CaseID == id);

			// Check Case not null.
			if (caseToUpdate == null)
			{
				return NotFound();
			}

			// Check user can error operate.
			var canErrorOperate = await AuthorizationService.AuthorizeAsync(User, caseToUpdate, CaseOperations.Error);

			// Validate authorization.
			if (!canErrorOperate.Succeeded)
			{
				return Forbid();
			}

			// Succeded return view.
			return View(caseToUpdate);
		}

		// Post: Cases/SetCaseError/5
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> SetCaseError(int id)
		{
			// Check model state is valid.
			if (!ModelState.IsValid)
			{
				return NotFound();
			}

			// Load case to update from db and include related data.
			var caseToSetErrorOn = await Context.Case
				.Include(o => o.CaseOperator)
				.Include(p => p.Photographer)
				.FirstOrDefaultAsync(c => c.CaseID == id);

			// Check loaded case not null.
			if (caseToSetErrorOn == null)
			{
				return NotFound();
			}

			// Check user can set error.
			var canSetError = await AuthorizationService.AuthorizeAsync(User, caseToSetErrorOn, CaseOperations.Error);

			// Validate Authentication.
			if (!canSetError.Succeeded)
			{
				return Forbid();
			}

			// Authentication succeded set new case state.
			caseToSetErrorOn.CaseState = Case.States.Fejlet;

			// Try update model async..
			if (await TryUpdateModelAsync(
				caseToSetErrorOn,
				"",
				c => c.ErrorDescription))
			{
				try
				{
					Context.Case.Update(caseToSetErrorOn);
					await Context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException ex)
				{
					if (!CaseExists(caseToSetErrorOn.CaseID))
					{
						return NotFound();
					}
					else
					{
						throw ex;
					}
				}

				// Saving changes succeded return to index.
				return RedirectToAction(nameof(Index));
			}


			// Operation failed return view.
			return View(caseToSetErrorOn);
		}



		// ------------------------------- Running Cases section ------------------------//
		// Get: Cases/RunningCases
		public async Task<IActionResult> RunningCases()
		{
			// Check user have permissions to view.
			var isAuthorized = User.IsInRole(Constants.CaseOperatorsRole);

			// Validate Authentication.
			if (!isAuthorized)
			{
				return Forbid();
			}

			// Load Cases to view.
			var runningCases = Context.Case
				.Include(o => o.CaseOperator)
				.Include(p => p.Photographer)
				.Where(c => c.CaseState == Case.States.Klippes)
				.Where(c => c.CaseOperatorID == UserManager.GetUserId(User))
				.OrderByDescending(c => c.Created).AsNoTracking();

			// Return view and load cases async.
			return View(await runningCases.ToListAsync());
		}

		public async Task<IActionResult> MyCases()
		{
			// Check user have permissions to view.
			var isAuthorized = User.IsInRole(Constants.CasePhotographerRole) ||
								User.IsInRole(Constants.CaseOperatorsRole);

			// Validate authentication.
			if (!isAuthorized)
			{
				return Forbid();
			}

			// Validation succeded load cases from db and include related data.
			var myCases = Context.Case
				.Include(o => o.CaseOperator)
				.Include(p => p.Photographer)
				.Where(c => c.PhotographerID == UserManager.GetUserId(User))
				.AsNoTracking().OrderBy(c => c.Created);

			// Succeded return view and load content async.
			return View(await myCases.ToListAsync());
		}

		private bool CaseExists(int id)
		{
			return Context.Case.Any(e => e.CaseID == id);
		}
	}
}