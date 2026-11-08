#nullable enable

using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public sealed class TransientItemGameItemComponent : GameItemComponent, ITransientItemPolicy
{
	private TransientItemGameItemComponentProto _prototype;
	private readonly bool _loaded;
	public override IGameItemComponentProto Prototype => _prototype;
	public bool RetireOnLoad => _loaded && _prototype.RetireOnReload;
	public long RetirementPlaneId => _prototype.RetirementPlaneId;
	public ItemRetirementTrigger Triggers => _prototype.Triggers;
	public bool RetireOnLogout => _prototype.RetireOnLogout;
	public TransientItemGameItemComponent(TransientItemGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary) => _prototype = proto;
	public TransientItemGameItemComponent(MudSharp.Models.GameItemComponent component, TransientItemGameItemComponentProto proto, IGameItem parent) : base(component, parent) { _prototype = proto; _loaded = true; }
	private TransientItemGameItemComponent(TransientItemGameItemComponent source, IGameItem parent, bool temporary) : base(source, parent, temporary) => _prototype = source._prototype;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new TransientItemGameItemComponent(this, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) => _prototype = (TransientItemGameItemComponentProto)newProto;
	protected override string SaveToXml() => "<Definition/>";
}
