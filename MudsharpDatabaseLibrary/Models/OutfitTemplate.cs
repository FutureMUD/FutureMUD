using System.Collections.Generic;

namespace MudSharp.Models
{
	public partial class OutfitTemplate
	{
		public OutfitTemplate()
		{
			OutfitTemplateItems = new HashSet<OutfitTemplateItem>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Exclusivity { get; set; }

		public virtual ICollection<OutfitTemplateItem> OutfitTemplateItems { get; set; }
	}
}
