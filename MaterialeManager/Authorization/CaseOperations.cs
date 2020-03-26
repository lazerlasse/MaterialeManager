using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialeManager.Authorization
{
	public static class CaseOperations
	{
		public static OperationAuthorizationRequirement Create =
		  new OperationAuthorizationRequirement { Name = Constants.CreateOperationName };
		public static OperationAuthorizationRequirement Read =
		  new OperationAuthorizationRequirement { Name = Constants.ReadOperationName };
		public static OperationAuthorizationRequirement Update =
		  new OperationAuthorizationRequirement { Name = Constants.UpdateOperationName };
		public static OperationAuthorizationRequirement Delete =
		  new OperationAuthorizationRequirement { Name = Constants.DeleteOperationName };
		public static OperationAuthorizationRequirement Accept =
		  new OperationAuthorizationRequirement { Name = Constants.AcceptOperationName };
		public static OperationAuthorizationRequirement Reject =
		  new OperationAuthorizationRequirement { Name = Constants.RejectOperationName };
	}

	public class Constants
	{
		public static readonly string CreateOperationName = "Create";
		public static readonly string ReadOperationName = "Read";
		public static readonly string UpdateOperationName = "Update";
		public static readonly string DeleteOperationName = "Delete";
		public static readonly string AcceptOperationName = "Accept";
		public static readonly string RejectOperationName = "Reject";

		public static readonly string CaseAdministratorsRole = "Administrator";
		public static readonly string CaseOperatorsRole = "Klipper";
		public static readonly string CasePhotographerRole = "Fotograf";
	}
}
