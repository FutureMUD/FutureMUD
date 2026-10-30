#nullable enable

using System.Collections.Generic;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public sealed class KeywordDescriptionsGameItemComponent : GameItemComponent, IKeywordDescriptionProvider
{
	private KeywordDescriptionsGameItemComponentProto _prototype;
	public KeywordDescriptionsGameItemComponent(KeywordDescriptionsGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary) => _prototype = proto;
	public KeywordDescriptionsGameItemComponent(MudSharp.Models.GameItemComponent component,
		KeywordDescriptionsGameItemComponentProto proto, IGameItem parent) : base(component, parent) => _prototype = proto;
	private KeywordDescriptionsGameItemComponent(KeywordDescriptionsGameItemComponent rhs, IGameItem parent, bool temporary)
		: base(rhs, parent, temporary) => _prototype = rhs._prototype;
	public override IGameItemComponentProto Prototype => _prototype;
	public IReadOnlyCollection<KeywordDescription> KeywordDescriptions => _prototype.KeywordDescriptions;
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => new KeywordDescriptionsGameItemComponent(this, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) => _prototype = (KeywordDescriptionsGameItemComponentProto)newProto;
	protected override string SaveToXml() => "<Definition/>";
}
