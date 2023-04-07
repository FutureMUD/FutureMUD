using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ImplantTraitChangerGameItemComponent : ImplantBaseGameItemComponent, IImplantTraitChange
{
	protected ImplantTraitChangerGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ImplantTraitChangerGameItemComponentProto)newProto;
	}

	#region Constructors

	public ImplantTraitChangerGameItemComponent(ImplantTraitChangerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public ImplantTraitChangerGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantTraitChangerGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImplantTraitChangerGameItemComponent(ImplantTraitChangerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImplantTraitChangerGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return SaveToXmlNoTextConversion()
			.ToString();
	}

	#endregion

	public double BonusForTrait(ITrait trait, TraitBonusContext context)
	{
		var (traitdef, addition, multiplier) = _prototype.Bonuses.FirstOrDefault(x => x.Trait == trait.Definition);
		if (traitdef == null)
		{
			return 0.0;
		}

		return FunctionFactor * (addition + multiplier * trait.RawValue);
	}

	public string ImplantFunctionReport()
	{
		return _prototype.Bonuses.Select(x => $"+{x.Trait.Name.ToLowerInvariant()}").ListToCommaSeparatedValues(" ");
	}
}