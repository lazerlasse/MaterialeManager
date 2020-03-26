using MaterialeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialeManager.Data
{
	public class SeedData
	{
		public static async Task SeedDatabaseAsync(ApplicationDbContext context)
		{
			if (context.CaseStates.Any())
			{
				return;     // Status table are seeded...
			}

			// Seed database table "Status" with static data...
			var caseStates = new CaseState[]
			{
				new CaseState { State = "Oprettet" },
				new CaseState { State = "Klippes" },
				new CaseState { State = "Fejlet" },
				new CaseState { State = "Udgivet" }
			};

			await context.CaseStates.AddRangeAsync(caseStates);
			await context.SaveChangesAsync();
		}
	}
}
