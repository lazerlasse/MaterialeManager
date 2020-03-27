using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialeManager.Models
{
	public class Case
	{
		[Key]
		public int CaseID { get; set; }

		[Required]
		public string Titel { get; set; }
		
		[Required, Display(Name = "Beskrivelse")]
		public string Description { get; set; }

		[Display(Name = "Kommentar/Bemærkninger")]
		public string Comments { get; set; }

		[DataType(DataType.DateTime), Display(Name = "Oprettet")]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:MM}", ApplyFormatInEditMode = true)]
		public DateTime Created { get; set; }

		[Display(Name = "Fejlmeddelelse")]
		public string ErrorDescription { get; set; }


		// Navigation Property for Case State.
		[ForeignKey("CaseState"), Display(Name = "Status")]
		public int CaseStateID { get; set; }			// FK for CaseState.
		public CaseState CaseState { get; set; }		// Navigation Property for CaseState.


		// Navigation Property for operator.
		[Display(Name = "Sagsbehandler")]
		public string CaseOperatorID { get; set; }						// FK for CaseOperator IdentityUser.
		public virtual IdentityUser CaseOperator { get; set; }			// Navigation property for IdentityUser.
		
		
		// Navigation Property for photographer.
		[Display(Name = "Fotograf")]
		public string PhotographerID { get; set; }						// FK for CasePhotographer IdentityUser.
		public virtual IdentityUser Photographer { get; set; }			// Navigation property for IdentityUser.
	}
}