using System.Collections.Generic;

namespace MudSharp.Models
{
	public partial class WritingCollection
	{
		public WritingCollection()
		{
			WritingCollectionEntries = new HashSet<WritingCollectionEntry>();
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string DefaultTitle { get; set; }

		public virtual ICollection<WritingCollectionEntry> WritingCollectionEntries { get; set; }
	}
}
