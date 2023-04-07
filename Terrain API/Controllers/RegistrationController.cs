using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_API.Controllers
{
	public record RegistrationRequest
	{
		public long AccountId { get; init; }
		public string RegistrationCode { get; init; }
	}

	[Route("[controller]")]
	[ApiController]
	public class RegistrationController
	{
		private readonly FuturemudDatabaseContext _context;

		public RegistrationController(FuturemudDatabaseContext context)
		{
			_context = context;
		}

		[HttpPut]
		public void Put([FromRoute]RegistrationRequest request)
		{
			var account = _context.Accounts.Find(request.AccountId);
			if (account == null)
			{
				return;
			}

			if (account.RegistrationCode.Equals(request.RegistrationCode))
			{
				account.IsRegistered = true;
				_context.SaveChanges();
			}
		}
	}
}
