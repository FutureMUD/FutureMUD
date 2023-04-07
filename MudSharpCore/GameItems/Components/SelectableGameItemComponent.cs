using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class SelectableGameItemComponent : GameItemComponent, ISelectable
{
	protected SelectableGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SelectableGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var options =
			_prototype.Options.Where(x => (bool?)x.CanSelectProg.Execute(voyeur, Parent) ?? true).ToList();
		return !options.Any()
			? description
			: $"{description}\n\nIt has the following selections available (see {"help select".FluentTagMXP("send", "href='help select' hint='show the helpfile for the select command'")} for more info):\n{options.Select(x => $"{x.Description} [{x.Keyword.Proper().Colour(Telnet.Yellow)}]").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n", article: "\t")}";
	}

	public override int DecorationPriority => int.MaxValue;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SelectableGameItemComponentProto)newProto;
	}

	#region Constructors

	public SelectableGameItemComponent(SelectableGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public SelectableGameItemComponent(SelectableGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public SelectableGameItemComponent(MudSharp.Models.GameItemComponent component,
		SelectableGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	protected override string SaveToXml()
	{
		return "<Definition></Definition>";
	}

	protected void LoadFromXml(XElement root)
	{
		// Do nothing
	}

	#endregion

	#region Implementation of ISelectable

	public bool CanSelect(ICharacter character, string argument)
	{
		return
			_prototype.Options.Where(x => (bool?)x.CanSelectProg.Execute(character, Parent) ?? true)
			          .Any(x => x.Keyword.StartsWith(argument, StringComparison.InvariantCultureIgnoreCase));
	}

	public bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)
	{
		var options =
			_prototype.Options.Where(x => (bool?)x.CanSelectProg.Execute(character, Parent) ?? true).ToList();
		var option =
			options.FirstOrDefault(x => x.Keyword.Equals(argument, StringComparison.InvariantCultureIgnoreCase)) ??
			options.FirstOrDefault(x => x.Keyword.StartsWith(argument, StringComparison.InvariantCultureIgnoreCase));
		if (option == null)
		{
			character.Send("That is not a valid selection to make.");
			return false;
		}

		if (!silent)
		{
			character.OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote(
						$"@ select|selects the '{option.Keyword.Proper().Colour(Telnet.Green)}' option on $1",
						character, character, Parent)).Append(playerEmote));
		}

		option.OnSelectProg.Execute(character, Parent);
		return true;
	}

	#endregion
}