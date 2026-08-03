using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.GameItems.Components;

public class ArtilleryMountGameItemComponent : GameItemComponent, IArtilleryMount
{
	private ArtilleryMountGameItemComponentProto _prototype;
	private IArtilleryPiece? _mountedPiece;

	public ArtilleryMountGameItemComponent(ArtilleryMountGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary) => _prototype = proto;
	public ArtilleryMountGameItemComponent(MudSharp.Models.GameItemComponent component, ArtilleryMountGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_mountedPiece = Gameworld.TryGetItem(long.Parse(XElement.Parse(component.Definition).Element("MountedPiece")?.Value ?? "0"), true)
			?.GetItemType<IArtilleryPiece>();
	}
	private ArtilleryMountGameItemComponent(ArtilleryMountGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary) => _prototype = rhs._prototype;

	public override IGameItemComponentProto Prototype => _prototype;
	public bool IsFixed => _prototype.Fixed;
	public double TraverseArc => _prototype.TraverseArc;
	public double ElevationArc => _prototype.ElevationArc;
	public IGameItem? InstalledPiece => _mountedPiece?.Parent;
	public bool CanInstall(IArtilleryPiece piece) => _mountedPiece is null && piece.Parent.ContainedIn is null;
	public string WhyCannotInstall(IArtilleryPiece piece) => _mountedPiece is null
		? piece.Parent.ContainedIn is null ? string.Empty : "That artillery piece is already installed or contained elsewhere."
		: "That artillery mount already has a piece installed.";
	public bool Install(IArtilleryPiece piece)
	{
		if (!CanInstall(piece)) return false;
		_mountedPiece = piece;
		piece.Parent.ContainedIn = Parent;
		Changed = true;
		return true;
	}
	public IGameItem? Remove()
	{
		var piece = _mountedPiece;
		_mountedPiece = null;
		if (piece is not null)
		{
			piece.Parent.ContainedIn = null;
		}
		Changed = piece is not null;
		return piece?.Parent;
	}
	public override bool PreventsMovement() => IsFixed && _mountedPiece is not null;
	public override string WhyPreventsMovement(ICharacter mover) =>
		"it is a fixed artillery mount with a piece installed";
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new ArtilleryMountGameItemComponent(this, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) => _prototype = (ArtilleryMountGameItemComponentProto)newProto;
	protected override string SaveToXml() => new XElement("Definition", new XElement("MountedPiece", _mountedPiece?.Parent.Id ?? 0)).ToString();
}
