using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic.SpellEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

#nullable enable
namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellChangeCharacteristicEffect : MagicSpellEffectBase, IChangeCharacteristicEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellChangeCharacteristic", (effect, owner) => new SpellChangeCharacteristicEffect(effect, owner));
	}

	#region Constructors and Saving
	public SpellChangeCharacteristicEffect(IPerceivable owner, IMagicSpellEffectParent parent, ICharacteristicValue changed, IFutureProg? prog) : base(owner, parent, prog)
	{
		_changedValueId = changed.Id;
		_changedValue = changed;
	}

	protected SpellChangeCharacteristicEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		var value = long.Parse(trueRoot.Element("ChangedValue").Value);
		_changedValueId = value;
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("ChangedValue", ChangedValue.Id)
		);
	}
	#endregion

	public override string Describe(IPerceiver voyeur)
	{
		return $"SpellChangeCharacteristic";
	}

	protected override string SpecificEffectType => "SpellChangeCharacteristic";
	private readonly long _changedValueId;
	private ICharacteristicValue? _changedValue;
	private ICharacteristicValue ChangedValue
	{
		get
		{
			_changedValue ??= Gameworld.CharacteristicValues.Get(_changedValueId);
			return _changedValue!;
		}
	}

	public bool ChangesCharacteristic(ICharacteristicDefinition characteristic)
	{
		return ((ChangeCharacteristicEffect)ParentEffect).WhichCharacteristic == characteristic;
	}

	public ICharacteristicValue GetChangedCharacteristic(ICharacteristicDefinition characteristic)
	{
		return ChangedValue;
	}
}