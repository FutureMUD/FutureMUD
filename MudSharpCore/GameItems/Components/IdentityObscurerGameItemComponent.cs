using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class IdentityObscurerGameItemComponent : GameItemComponent, IObscureIdentity
{
	protected IdentityObscurerGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (IdentityObscurerGameItemComponentProto)newProto;
	}

	#region Constructors

	public IdentityObscurerGameItemComponent(IdentityObscurerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public IdentityObscurerGameItemComponent(MudSharp.Models.GameItemComponent component,
		IdentityObscurerGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public IdentityObscurerGameItemComponent(IdentityObscurerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new IdentityObscurerGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	#region IObscureIdentity

	private static Regex KeywordSubstitutionRegex = new("@(?<option>item|name|sub|key)", RegexOptions.IgnoreCase);
	public virtual bool CurrentlyApplies => Parent.GetItemType<IWearable>()?.GloballyTransparent != true;

	public string OverriddenShortDescription
	{
		get
		{
			return KeywordSubstitutionRegex.Replace(_prototype.OverriddenShortDescription, match =>
			{
				if (match.Groups["option"].Value.EqualTo("item"))
				{
					return Parent.HowSeen(Parent, flags: PerceiveIgnoreFlags.IgnoreSelf);
				}

				if (match.Groups["option"].Value.EqualTo("name"))
				{
					return Parent.Name.ToLowerInvariant();
				}

				if (match.Groups["option"].Value.EqualTo("key"))
				{
					return _prototype.KeywordSubstitutions.ContainsKey(Parent.Name)
						? _prototype.KeywordSubstitutions[Parent.Name]
						: $"{Parent.Name}ed";
				}

				return
					_prototype.KeywordSubstitutions.ContainsKey(Parent.Name)
						? Parent
						  .HowSeen(Parent, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)
						  .Replace(Parent.Name, _prototype.KeywordSubstitutions[Parent.Name])
						  .Strip_A_An()
						: Parent
						  .HowSeen(Parent, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)
						  .Strip_A_An();
			});
		}
	}

	public string OverriddenFullDescription
	{
		get
		{
			return KeywordSubstitutionRegex.Replace(_prototype.OverriddenFullDescription, match =>
			{
				if (match.Groups["option"].Value.EqualTo("item"))
				{
					return Parent.HowSeen(Parent, flags: PerceiveIgnoreFlags.IgnoreSelf);
				}

				if (match.Groups["option"].Value.EqualTo("name"))
				{
					return Parent.Name.ToLowerInvariant();
				}

				return
					_prototype.KeywordSubstitutions.ContainsKey(Parent.Name)
						? Parent
						  .HowSeen(Parent, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)
						  .Replace(Parent.Name, _prototype.KeywordSubstitutions[Parent.Name])
						  .Strip_A_An()
						: Parent
						  .HowSeen(Parent, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)
						  .Strip_A_An();
			});
		}
	}

	public Difficulty SeeThroughDisguiseDifficulty => _prototype.SeeThroughDisguiseDifficulty;

	public virtual bool ObscuresCharacteristic(ICharacteristicDefinition type)
	{
		return _prototype.ObscuredCharacteristics.Contains(type);
	}

	public virtual string DescribeCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur)
	{
		return Parent.ParseCharacteristics(_prototype.GetDescription(type), voyeur);
	}

	public string RemovalEcho => _prototype.RemovalEcho;

	#endregion
}