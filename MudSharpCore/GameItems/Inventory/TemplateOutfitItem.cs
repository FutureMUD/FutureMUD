
namespace MudSharp.GameItems.Inventory;

public sealed class TemplateOutfitItem : IOutfitTemplateItem
{
	public TemplateOutfitItem()
	{
	}

	public TemplateOutfitItem(Models.OutfitTemplateItem dbitem, IFuturemud gameworld)
	{
		TemplateKey = dbitem.TemplateKey;
		GameItemProto = gameworld.ItemProtos.Get(dbitem.GameItemProtoId);
		DesiredProfile = dbitem.WearProfileId is null ? null : gameworld.WearProfiles.Get(dbitem.WearProfileId.Value);
		Placement = (OutfitTemplateItemPlacement)dbitem.Placement;
		ContainerKey = dbitem.ContainerKey;
		LoadArguments = dbitem.LoadArguments ?? string.Empty;
		WearOrder = dbitem.WearOrder;
	}

	public TemplateOutfitItem(IOutfitTemplateItem rhs)
	{
		TemplateKey = rhs.TemplateKey;
		GameItemProto = rhs.GameItemProto;
		DesiredProfile = rhs.DesiredProfile;
		Placement = rhs.Placement;
		ContainerKey = rhs.ContainerKey;
		LoadArguments = rhs.LoadArguments;
		WearOrder = rhs.WearOrder;
	}

	public string TemplateKey { get; set; }
	public IGameItemProto GameItemProto { get; set; }
	public IWearProfile DesiredProfile { get; set; }
	public OutfitTemplateItemPlacement Placement { get; set; }
	public string ContainerKey { get; set; }
	public string LoadArguments { get; set; } = string.Empty;
	public int WearOrder { get; set; }

	public IEnumerable<string> Keywords => new[] { TemplateKey };
}
