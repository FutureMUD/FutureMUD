using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public sealed class ArtilleryAmmunitionGameItemComponent : AmmunitionGameItemComponent, IArtilleryAmmunition
{
	private ArtilleryAmmunitionGameItemComponentProto _artilleryPrototype;
	public ArtilleryAmmunitionGameItemComponent(ArtilleryAmmunitionGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary) => _artilleryPrototype = proto;
	public ArtilleryAmmunitionGameItemComponent(MudSharp.Models.GameItemComponent component, ArtilleryAmmunitionGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent) => _artilleryPrototype = proto;
	private ArtilleryAmmunitionGameItemComponent(ArtilleryAmmunitionGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary) => _artilleryPrototype = rhs._artilleryPrototype;
	public ArtilleryPayloadType PayloadType => _artilleryPrototype.PayloadType;
	public string ArtilleryProfile => _artilleryPrototype.ArtilleryProfile;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new ArtilleryAmmunitionGameItemComponent(this, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_artilleryPrototype = (ArtilleryAmmunitionGameItemComponentProto)newProto;
	}
}
