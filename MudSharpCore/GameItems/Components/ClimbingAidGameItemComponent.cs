#nullable enable

using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ClimbingAidGameItemComponent : GameItemComponent, IClimbingAid
{
	private ClimbingAidGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;
	public double CheckBonus => _prototype.CheckBonus;
	public bool WorksHeld => _prototype.WorksHeld;
	public bool WorksWorn => _prototype.WorksWorn;
	public ClimbingAidGameItemComponent(ClimbingAidGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary) => _prototype = proto;
	public ClimbingAidGameItemComponent(MudSharp.Models.GameItemComponent component, ClimbingAidGameItemComponentProto proto, IGameItem parent) : base(component, parent) => _prototype = proto;
	private ClimbingAidGameItemComponent(ClimbingAidGameItemComponent source, IGameItem parent, bool temporary) : base(source, parent, temporary) => _prototype = source._prototype;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new ClimbingAidGameItemComponent(this, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) => _prototype = (ClimbingAidGameItemComponentProto)newProto;
	protected override string SaveToXml() => "<Definition/>";
}
