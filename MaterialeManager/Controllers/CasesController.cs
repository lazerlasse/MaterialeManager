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

namespace MaterialeManager
{
    [Authorize]
    public class CasesController : CaseControllerBaseModel
    {
        public CasesController(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager)
            :base(context, authorizationService, userManager)
        {
        }

        // GET: Cases
        public async Task<IActionResult> Index()
        {
            var cases = Context.Case
                .Include(s => s.CaseState)
                .Include(p => p.Photographer)
                .Include(o => o.CaseOperator)
                .AsNoTracking();

            return View(await cases.ToListAsync());
        }

        // GET: Cases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caseToView = await Context.Case
                .Include(s => s.CaseState)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CaseID == id);

            if (caseToView == null)
            {
                return NotFound();
            }

            return View(caseToView);
        }

        // GET: Cases/Create
        public async Task<IActionResult> Create()
        {
            var newCase = new Case
            {
                Created = DateTime.Now
            };

            ViewBag.CaseStates = await PopulateCaseStatesDropDownList();
            
            return View(newCase);
        }

        // POST: Cases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CaseID,Titel,Description,Comments,Created,CaseStateID")] Case caseToCreate)
        {
            if (ModelState.IsValid)
            {
                caseToCreate.PhotographerID = UserManager.GetUserId(User);
                Context.Add(caseToCreate);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CaseStates = await PopulateCaseStatesDropDownList();

            return View(caseToCreate);
        }

        // GET: Cases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caseToEdit = await Context.Case.FirstOrDefaultAsync(i => i.CaseID == id);

            if (caseToEdit == null)
            {
                return NotFound();
            }

            ViewBag.CaseStates = await PopulateCaseStatesDropDownList();

            return View(caseToEdit);
        }

        // POST: Cases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CaseID,Titel,Description,Comments,Created,CaseStateID")] Case caseToUpdate)
        {
            if (id != caseToUpdate.CaseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(caseToUpdate);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CaseExists(caseToUpdate.CaseID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.CaseStates = await PopulateCaseStatesDropDownList();

            return View(caseToUpdate);
        }

        // GET: Cases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caseToDelete = await Context.Case
                .FirstOrDefaultAsync(m => m.CaseID == id);

            if (caseToDelete == null)
            {
                return NotFound();
            }

            return View(caseToDelete);
        }

        // POST: Cases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var caseToDelete = await Context.Case
                .FirstOrDefaultAsync(i => i.CaseID == id);

            Context.Case.Remove(caseToDelete);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CaseExists(int id)
        {
            return Context.Case.Any(e => e.CaseID == id);
        }
    }
}