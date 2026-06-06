namespace MudSharp.Models
{
	public partial class OutfitTemplateItem
	{
		public long Id { get; set; }
		public long OutfitTemplateId { get; set; }
		public string TemplateKey { get; set; }
		public long GameItemProtoId { get; set; }
		public long? WearProfileId { get; set; }
		public int Placement { get; set; }
		public string ContainerKey { get; set; }
		public string LoadArguments { get; set; }
		public int WearOrder { get; set; }

		public virtual OutfitTemplate OutfitTemplate { get; set; }
		public virtual WearProfile WearProfile { get; set; }
	}
}
