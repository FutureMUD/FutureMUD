using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class ArtilleryChamberGameItemComponent : GameItemComponent, IArtilleryChamber
{
	private ArtilleryChamberGameItemComponentProto _prototype;
	private IArtilleryAmmunition? _loadedAmmunition;

	public ArtilleryChamberGameItemComponent(ArtilleryChamberGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary) => _prototype = proto;

	public ArtilleryChamberGameItemComponent(MudSharp.Models.GameItemComponent component,
		ArtilleryChamberGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_loadedAmmunition = Gameworld.TryGetItem(long.TryParse(XElement.Parse(component.Definition).Element("Loaded")?.Value, out var id) ? id : 0, true)
			?.GetItemType<IArtilleryAmmunition>();
	}

	private ArtilleryChamberGameItemComponent(ArtilleryChamberGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary) => _prototype = rhs._prototype;

	public override IGameItemComponentProto Prototype => _prototype;
	public string ArtilleryProfile => _prototype.ArtilleryProfile;
	public bool IsLoaded => _loadedAmmunition is not null;
	public IArtilleryAmmunition? LoadedAmmunition => _loadedAmmunition;

	public bool TryLoad(IArtilleryAmmunition ammunition)
	{
		if (IsLoaded || !ProfilesAreCompatible(ArtilleryProfile, ammunition.ArtilleryProfile))
		{
			return false;
		}

		_loadedAmmunition = ammunition;
		ammunition.Parent.ContainedIn = Parent;
		Changed = true;
		return true;
	}

	public IArtilleryAmmunition? Unload()
	{
		var ammunition = _loadedAmmunition;
		if (ammunition is not null)
		{
			ammunition.Parent.ContainedIn = null;
			_loadedAmmunition = null;
			Changed = true;
		}

		return ammunition;
	}

	public override double ComponentWeight => _loadedAmmunition?.Parent.Weight ?? 0.0;

	public override double ComponentBuoyancy(double fluidDensity) =>
		_loadedAmmunition?.Parent.Buoyancy(fluidDensity) ?? 0.0;

	public override void FinaliseLoad()
	{
		_loadedAmmunition?.Parent.FinaliseLoadTimeTasks();
	}

	public override void Login()
	{
		_loadedAmmunition?.Parent.Login();
	}

	public override void Quit()
	{
		base.Quit();
		_loadedAmmunition?.Parent.Quit();
	}

	public override void Delete()
	{
		base.Delete();
		_loadedAmmunition?.Parent.Delete();
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new ArtilleryChamberGameItemComponent(this, newParent, temporary);

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) =>
		_prototype = (ArtilleryChamberGameItemComponentProto)newProto;

	protected override string SaveToXml() => new XElement("Definition",
		new XElement("Loaded", _loadedAmmunition?.Parent.Id ?? 0)).ToString();

	private static bool ProfilesAreCompatible(string chamberProfile, string ammunitionProfile)
	{
		var chamberProfiles = chamberProfile.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		var ammunitionProfiles = ammunitionProfile.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		return chamberProfiles.Any(x => x == "*" || ammunitionProfiles.Any(y => y == "*" || x.EqualTo(y)));
	}
}
