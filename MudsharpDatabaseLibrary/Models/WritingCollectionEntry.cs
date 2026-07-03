namespace MudSharp.Models
{
	public partial class WritingCollectionEntry
	{
		public long Id { get; set; }
		public long WritingCollectionId { get; set; }
		public int PageNumber { get; set; }
		public int DisplayOrder { get; set; }
		public long? WritingId { get; set; }
		public long? DrawingId { get; set; }

		public virtual WritingCollection WritingCollection { get; set; }
		public virtual Writing Writing { get; set; }
		public virtual Drawing Drawing { get; set; }
	}
}
