using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Inventory;

public class OutfitItem : IOutfitItem
{
	public long Id { get; set; }
	public string ItemDescription { get; set; }
	public long? PreferredContainerId { get; set; }
	public string PreferredContainerDescription { get; set; }
	public int WearOrder { get; set; }
	public IWearProfile DesiredProfile { get; set; }

	public XElement SaveToXml()
	{
		return new XElement("Item",
			new XElement("Id", Id),
			new XElement("PreferredContainerId", PreferredContainerId ?? 0),
			new XElement("PreferredContainerDescription", PreferredContainerDescription ?? ""),
			new XElement("WearOrder", WearOrder),
			new XElement("DesiredProfile", DesiredProfile?.Id ?? 0),
			new XElement("ItemDescription", new XCData(ItemDescription))
		);
	}

	public OutfitItem()
	{
	}

	public OutfitItem(IOutfitItem rhs)
	{
		Id = rhs.Id;
		PreferredContainerId = rhs.PreferredContainerId;
		WearOrder = rhs.WearOrder;
		ItemDescription = rhs.ItemDescription;
		DesiredProfile = rhs.DesiredProfile;
		PreferredContainerDescription = rhs.PreferredContainerDescription;
	}

	public OutfitItem(XElement element, IFuturemud gameworld)
	{
		Id = long.Parse(element.Element("Id").Value);
		PreferredContainerId = long.Parse(element.Element("PreferredContainerId").Value);
		if (PreferredContainerId == 0)
		{
			PreferredContainerId = null;
		}

		WearOrder = int.Parse(element.Element("WearOrder").Value);
		ItemDescription = element.Element("ItemDescription").Value;
		PreferredContainerDescription = element.Element("PreferredContainerDescription").Value;
		DesiredProfile = gameworld.WearProfiles.Get(long.Parse(element.Element("DesiredProfile").Value));
	}

	#region Implementation of IKeyworded

	public IEnumerable<string> Keywords => ItemDescription.Strip_A_An().Split(' ', ',', '-');

	#endregion


	#region IFutureProgVariable Implementation

	public FutureProgVariableTypes Type => FutureProgVariableTypes.OutfitItem;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "containerid":
				return PreferredContainerId.HasValue
					? (IFutureProgVariable)new NumberVariable(PreferredContainerId.Value)
					: new NullVariable(FutureProgVariableTypes.Number);
			case "order":
				return new NumberVariable(WearOrder);
			case "desc":
			case "description":
				return new TextVariable(ItemDescription);
			case "containerdesc":
			case "containerdescription":
				return new TextVariable(PreferredContainerDescription);
			case "profile":
				return DesiredProfile != null
					? (IFutureProgVariable)new TextVariable(DesiredProfile?.Name)
					: new NullVariable(FutureProgVariableTypes.Text);
		}

		throw new NotImplementedException();
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "containerid", FutureProgVariableTypes.Number },
			{ "order", FutureProgVariableTypes.Number },
			{ "desc", FutureProgVariableTypes.Text },
			{ "description", FutureProgVariableTypes.Text },
			{ "containerdesc", FutureProgVariableTypes.Text },
			{ "containerdescription", FutureProgVariableTypes.Text },
			{ "profile", FutureProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "containerid", "" },
			{ "order", "" },
			{ "desc", "" },
			{ "description", "" },
			{ "containerdesc", "" },
			{ "containerdescription", "" },
			{ "profile", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.OutfitItem, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}