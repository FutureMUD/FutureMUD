using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
	public partial class CulturesNameCultures
	{
		public long CultureId { get; set; }
		public long NameCultureId { get; set; }
		public short Gender { get; set; }

		public virtual Culture Culture { get; set; }
		public virtual NameCulture NameCulture { get; set; }
	}
}
