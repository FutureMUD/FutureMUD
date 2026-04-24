namespace MudSharp.Models
{
	public partial class Plane
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Alias { get; set; }
		public string Description { get; set; }
		public int DisplayOrder { get; set; }
		public bool IsDefault { get; set; }
	}
}
