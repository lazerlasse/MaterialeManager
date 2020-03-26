using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialeManager.Models
{
	public class CaseState
	{
		[Key]
		public int CaseStateID { get; set; }

		[Required, Display(Name = "Status")]
		public string State { get; set; }
	}
}
