using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MudSharp.Database;
using MudSharp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Terrain_API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class TerrainController : ControllerBase
	{
		private readonly FuturemudDatabaseContext _context;

		public TerrainController(FuturemudDatabaseContext context)
		{
			_context = context;
		}

		// GET: <TerrainController>
		[HttpGet]
		public IEnumerable<MudSharp.Models.Terrain> Get()
		{
			return _context.Terrains.ToList();
		}
	}
}
