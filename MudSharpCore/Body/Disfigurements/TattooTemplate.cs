using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Body.Disfigurements;

public class TattooTemplate : DisfigurementTemplate, ITattooTemplate
{
	private static readonly Regex TemplateTextSlotRegex = new(@"\$template\{(?<name>[^}]+)\}",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	public TattooTemplate(MudSharp.Models.DisfigurementTemplate dbitem, IFuturemud gameworld) : base(dbitem, gameworld)
	{
		var root = XElement.Parse(dbitem.Definition);
		Size = (SizeCategory)int.Parse(root.Element("MinBodypartsize").Value);
		CanSelectInChargen = bool.Parse(root.Element("CanBeSelectedInChargen").Value);
		CanSelectInChargenProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("CanBeSelectedInChargenProg")?.Value ?? "0"));
		foreach (var element in root.Element("ChargenCosts")?.Elements() ?? Enumerable.Empty<XElement>())
		{
			ChargenCosts[Gameworld.ChargenResources.Get(long.Parse(element.Attribute("resource").Value))] =
				int.Parse(element.Attribute("amount").Value);
		}

		_requiredKnowledgeToTattoo =
			Gameworld.Knowledges.Get(long.Parse(root.Element("RequiredKnowledge")?.Value ?? "0"));
		_minimumSkill = double.Parse(root.Element("MinimumSkill")?.Value ?? "0.0");
		foreach (var element in root.Element("Shapes").Elements())
		{
			_permittedShapes.Add(Gameworld.BodypartShapes.Get(long.Parse(element.Value)));
		}

		foreach (var element in root.Element("Inks").Elements())
		{
			_inkColours[Gameworld.Colours.Get(long.Parse(element.Value))] =
				double.Parse(element.Attribute("weight").Value);
		}

		_overrideCharacteristicPlain =
			root.Element("OverrideCharacteristicPlain")?.Value ??
			root.Element("OverrideCharcteristicPlain")?.Value;
		_overrideCharacteristicWith = root.Element("OverrideCharacteristicWith")?.Value;
		foreach (var element in root.Element("TextSlots")?.Elements("TextSlot") ?? Enumerable.Empty<XElement>())
		{
			_textSlots.Add(new TattooTemplateTextSlot(element, Gameworld));
		}
	}


	protected override XElement SaveDefinition()
	{
		return new XElement("Tattoo",
			new XElement("MinBodypartsize", (int)Size),
			new XElement("CanBeSelectedInChargen", CanSelectInChargen),
			new XElement("CanBeSelectedInChargenProg", CanSelectInChargenProg?.Id ?? 0),
			new XElement("ChargenCosts",
				from cost in ChargenCosts
				select new XElement("Cost", new XAttribute("resource", cost.Key.Id),
					new XAttribute("amount", cost.Value))
			),
			new XElement("RequiredKnowledge", _requiredKnowledgeToTattoo?.Id ?? 0),
			new XElement("MinimumSkill", _minimumSkill),
			new XElement("OverrideCharacteristicPlain", _overrideCharacteristicPlain ?? string.Empty),
			new XElement("OverrideCharacteristicWith", _overrideCharacteristicWith ?? string.Empty),
			new XElement("Inks",
				from ink in _inkColours
				select new XElement("Ink", new XAttribute("weight", ink.Value), ink.Key.Id)),
			new XElement("Shapes",
				from shape in _permittedShapes
				select new XElement("Shape", shape.Id)
			),
			new XElement("TextSlots",
				from slot in _textSlots
				select slot.SaveToXml()
			)
		);
	}

	public TattooTemplate(IAccount originator, string name) : base(originator, name)
	{
	}

	public TattooTemplate(TattooTemplate rhs, IAccount originator, string name) : base(rhs, originator, name)
	{
		Size = rhs.Size;
		CanSelectInChargen = rhs.CanSelectInChargen;
		CanSelectInChargenProg = rhs.CanSelectInChargenProg;
		foreach (var cost in rhs.ChargenCosts)
		{
			ChargenCosts[cost.Key] = cost.Value;
		}

		_requiredKnowledgeToTattoo = rhs._requiredKnowledgeToTattoo;
		_permittedShapes = rhs._permittedShapes.ToList();
		_minimumSkill = rhs._minimumSkill;
		_inkColours = new DoubleCounter<IColour>(rhs._inkColours);
		_overrideCharacteristicWith = rhs._overrideCharacteristicWith;
		_overrideCharacteristicPlain = rhs._overrideCharacteristicPlain;
		_textSlots.AddRange(rhs._textSlots.Select(x => new TattooTemplateTextSlot(x.Name, x.MaximumLength,
			x.RequiredCustomText, x.DefaultLanguage, x.DefaultScript, x.DefaultStyle, x.DefaultColour,
			x.DefaultMinimumSkill, x.DefaultText, x.DefaultAlternateText)));
		Changed = true;
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.DisfigurementTemplates.GetAll(Id).OfType<ITattooTemplate>();
	}

	public override IDisfigurementTemplate Clone(IAccount originator, string newName)
	{
		return new TattooTemplate(this, originator, newName);
	}

	private static ILiquid _inkLiquid;

	public static ILiquid InkLiquid
	{
		get
		{
			if (_inkLiquid == null)
			{
				_inkLiquid = Futuremud.Games.First().Liquids
				                      .Get(Futuremud.Games.First().GetStaticLong("TattooInkLiquid"));
			}

			return _inkLiquid;
		}
	}

	private static ITraitDefinition _tattooistTrait;

	public static ITraitDefinition TattooistTrait
	{
		get
		{
			if (_tattooistTrait == null)
			{
				_tattooistTrait = Futuremud.Games.First().Traits
				                           .Get(Futuremud.Games.First().GetStaticLong("TattooistTrait"));
			}

			return _tattooistTrait;
		}
	}

	#region Overrides of EditableItem

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(EditHeader());
		sb.AppendLine($"Short Description: {ShortDescription}");
		sb.AppendLine($"Permitted in Chargen: {CanSelectInChargen.ToColouredString()}");
		sb.AppendLine(
			$"Chargen Prog: {CanSelectInChargenProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Chargen Costs: {(ChargenCosts.Any(x => x.Value > 0) ? ChargenCosts.Select(x => $"{x.Value.ToString("N0", actor)} {x.Key.Alias}".ColourValue()).ListToString() : "None".Colour(Telnet.Red))}");
		sb.AppendLine($"Minimum Bodypart Size: {Size.Describe().ColourValue()}");
		sb.AppendLine(
			$"Required Tattooist Knowledge: {_requiredKnowledgeToTattoo?.Name.Colour(Telnet.Cyan) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine(
			$"Minimum Skill: {(actor.IsAdministrator() ? _minimumSkill.ToString("N2", actor).ColourValue() : TattooistTrait.Decorator.Decorate(_minimumSkill).ColourValue())}");
		sb.AppendLine(
			$"Permitted Bodyparts: {(_permittedShapes.Any() ? _permittedShapes.Select(x => x.Name.ColourValue()).ListToString() : "Any".ColourValue())}");
		sb.AppendLine(
			$"Required Inks: {_inkColours.Select(x => $"{x.Key.Name} [w{x.Value.ToString("N2")}]".ColourValue()).ListToString()}");
		sb.AppendLine(
			$"Text Slots: {(_textSlots.Any() ? _textSlots.Select(DescribeTextSlotForShow).ListToString() : "None".Colour(Telnet.Red))}");
		if (HasSpecialTattooCharacteristicOverride)
		{
			sb.AppendLine($"Override Characteristic (Plain): {_overrideCharacteristicPlain.ColourCharacter()}");
			sb.AppendLine($"Override Characteristic (With): {_overrideCharacteristicWith.ColourCharacter()}");
		}

		sb.AppendLine($"Full Description:\n\n{FullDescription.Wrap(actor.InnerLineFormatLength)}");
		return sb.ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return new TattooTemplate(this, initiator.Account, null);
	}

	public override string EditHeader()
	{
		return $"Tattoo Template #{Id:N0}r{RevisionNumber:N0}";
	}

	#region Overrides of DisfigurementTemplate

	protected override string BuildingHelpText =>
		$@"{base.BuildingHelpText}
	#3size <size>#0 - sets the size of this tattoo
	#3bodypart <shape>#0 - sets the target bodypart shapes
	#3knowledge <knowledge>#0 - sets the knowledge required to ink this tattoo
	#3skill <minimum>#0 - sets the minimum skill to ink this tattoo
	#3ink <colour> [<weight>]#0 - toggles an ink colour required for this tattoo
	#3chargen [<amount> <resource>]#0 - toggles tattoo selectable in chargen. Admins only.
	#3prog <prog>#0 - sets the prog that controls appearance in chargen. Admins only.
	#3chargenprog <prog>#0 - sets the chargen prog. Admins only.
	#3textslot add <name> <maxlength> [required]#0 - adds a named tattoo text slot
	#3textslot remove <name>#0 - removes a named tattoo text slot
	#3textslot <name> required <true|false>#0 - sets whether the slot requires custom text
	#3textslot <name> maxlength <length>#0 - sets the slot's max text length
	#3textslot <name> language <language>#0 - sets the default fallback language
	#3textslot <name> script <script>#0 - sets the default fallback script
	#3textslot <name> style <style>[,<style>...]#0 - sets the default fallback writing style flags
	#3textslot <name> colour <colour>#0 - sets the default fallback writing colour
	#3textslot <name> minskill <value>#0 - sets the default fallback minimum reading skill
	#3textslot <name> text <text>#0 - sets the default fallback readable text
	#3textslot <name> alt <text>#0 - sets the default fallback unreadable text
	#3override ""<plain>"" ""<with>""#0 - special override descriptions for sdescs, both a plain form and a with form.";

	/// <summary>Handles OLC Building related commands from an Actor</summary>
	/// <param name="actor">The ICharacter requesting the edit</param>
	/// <param name="command">The command they have entered</param>
	/// <returns>True if anything was changed, false if the command was invalid or did not change anything</returns>
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "override":
				return BuildingCommandOverride(actor, command);
			case "size":
				return BuildingCommandSize(actor, command);
			case "bodypart":
			case "part":
			case "shape":
				return BuildingCommandBodypart(actor, command);
			case "chargen":
			case "charactercreation":
			case "character_creation":
			case "character creation":
				if (!actor.IsAdministrator())
				{
					break;
				}

				return BuildingCommandChargen(actor, command);
			case "knowledge":
				return BuildingCommandKnowledge(actor, command);
			case "ink":
				return BuildingCommandInk(actor, command);
			case "skill":
			case "minskill":
			case "min_skill":
			case "min skill":
			case "minimumskill":
			case "minimum_skill":
			case "minimum skill":
				return BuildingCommandSkill(actor, command);
			case "chargenprog":
			case "chargen prog":
			case "chargen_prog":
				return BuildingCommandChargenProg(actor, command);
			case "textslot":
			case "text":
			case "slot":
				return BuildingCommandTextSlot(actor, command);
		}

		return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
	}

	private bool BuildingCommandOverride(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What is the plain form (e.g. ritually-tattooed) you want this tattoo to provide?");
			return false;
		}

		var plain = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What is the with form (e.g. with a facial tattoo) you want this tattoo to provide?");
			return false;
		}

		_overrideCharacteristicWith = command.PopSpeech();
		_overrideCharacteristicPlain = plain;
		Changed = true;
		actor.OutputHandler.Send(
			$"This tattoo will now provide an override of {_overrideCharacteristicPlain.ColourCharacter()} with the &tattoos sdesc tag and {_overrideCharacteristicWith.ColourCharacter()} with the &withtattoos sdesc tag.");
		return true;
	}

	private bool BuildingCommandSkill(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What minimum level of skill do you want to set for this tattoo?");
			return false;
		}

		var skillText = command.PopSpeech();
		var rtvd = TattooistTrait.Decorator as IRangeTraitValueDecorator;
		if ((rtvd == null || actor.IsAdministrator()) && double.TryParse(skillText, out var value))
		{
			if (value < 0)
			{
				actor.OutputHandler.Send("The value you set must be zero or greater.");
				return false;
			}

			_minimumSkill = value;
			Changed = true;
			actor.OutputHandler.Send(
				$"This tattoo will now require a minimum skill of {_minimumSkill.ToString("N2", actor).ColourValue()} to inscribe.");
			return true;
		}

		if (rtvd == null)
		{
			actor.OutputHandler.Send("You must enter a valid number for the tattooist minimum skill.");
			return false;
		}

		var range = TattooistTrait.Decorator.OrderedDescriptors.FirstOrDefault(x => x.EqualTo(skillText));
		_minimumSkill = rtvd.MinimumValueForDescriptor(range);
		Changed = true;
		actor.OutputHandler.Send(
			$"This tattoo will now require a tattooist with a minimum skill of {range.ColourValue()} to inscribe.");
		return true;
	}

	private bool BuildingCommandInk(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify an ink colour to include or exclude. See SHOW COLOURS.");
			return false;
		}

		var colour = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Colours.Get(value)
			: Gameworld.Colours.GetByName(command.Last);
		if (colour == null)
		{
			actor.OutputHandler.Send("There is no such colour.");
			return false;
		}

		if (command.IsFinished && _inkColours.ContainsKey(colour))
		{
			_inkColours.Remove(colour);
			Changed = true;
			actor.OutputHandler.Send($"This tattoo will no longer require any {colour.Name} ink.");
			return true;
		}

		var weighting = 1.0;
		if (!command.IsFinished)
		{
			if (!double.TryParse(command.PopSpeech(), out var dvalue) || dvalue <= 0.0)
			{
				actor.OutputHandler.Send("If you specify a weight for the colour, it must be a valid number.");
				return false;
			}

			weighting = dvalue;
		}

		_inkColours[colour] = weighting;
		Changed = true;
		actor.OutputHandler.Send(
			$"This tattoo will now require {colour.Name} ink at a weighting of {weighting.ToString("N2", actor).ColourValue()}, which works out to be {Gameworld.UnitManager.DescribeMostSignificantExact(InkColours.First(x => x.Colour == colour).Amount, Framework.Units.UnitType.FluidVolume, actor).ColourValue()} of ink.");
		return true;
	}

	private bool BuildingCommandKnowledge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which knowledge do you want tattooists to have to have in order to inscribe this tattoo?");
			return false;
		}

		if (command.Peek().EqualToAny("none", "clear"))
		{
			actor.OutputHandler.Send(
				"This tattoo will no longer require any particular knowledge on the part of the tattooist.");
			_requiredKnowledgeToTattoo = null;
			Changed = true;
			return true;
		}

		var knowledges = actor.IsAdministrator() ? Gameworld.Knowledges.ToList() : actor.Knowledges.ToList();
		var knowledge = long.TryParse(command.PopSpeech(), out var value)
			? knowledges.FirstOrDefault(x => x.Id == value)
			: knowledges.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
			  knowledges.FirstOrDefault(x =>
				  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (knowledge == null)
		{
			if (actor.IsAdministrator())
			{
				actor.OutputHandler.Send("There is no such knowledge.");
			}
			else
			{
				actor.OutputHandler.Send(
					"You do not possess any such knowledge. You can only use knowledges that you personally know.");
			}

			return false;
		}

		_requiredKnowledgeToTattoo = knowledge;
		actor.OutputHandler.Send(
			$"This tattoo will now require the tattooist to have the {_requiredKnowledgeToTattoo.Name.Colour(Telnet.Cyan)} knowledge in order to inscribe it.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandChargen(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"This tattoo will {(CanSelectInChargen ? "no longer" : "now")} be selectable in character creation.");
			CanSelectInChargen = !CanSelectInChargen;
			ChargenCosts.Clear();
			Changed = true;
			return true;
		}

		var costsRegex = new Regex("(?<amount>\\d+) (?<resource>\\D+)");
		if (!costsRegex.IsMatch(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send(
				"Those are not valid chargen resource costs. If you specify anything, it must be chargen resource costs e.g. 3 RPP.");
			return false;
		}

		var costs = new Counter<IChargenResource>();
		foreach (Match match in costsRegex.Matches(command.SafeRemainingArgument))
		{
			var resource =
				Gameworld.ChargenResources.FirstOrDefault(x => x.Alias.EqualTo(match.Groups["resource"].Value)) ??
				Gameworld.ChargenResources.FirstOrDefault(x => x.Name.EqualTo(match.Groups["resource"].Value));
			if (resource == null)
			{
				actor.OutputHandler.Send($"There is no chargen resource like \"{match.Groups["resource"].Value}\".");
				return false;
			}

			costs[resource] += int.Parse(match.Groups["amount"].Value);
		}

		ChargenCosts.Clear();
		foreach (var cost in costs)
		{
			ChargenCosts[cost.Key] = cost.Value;
		}

		CanSelectInChargen = true;
		actor.OutputHandler.Send(
			$"This tattoo is now selectable in chargen for a cost of {ChargenCosts.Select(x => $"{x.Value} {x.Key.Alias}".ColourValue()).ListToString()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandBodypart(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which bodypart shape do you want to toggle being able to take this tattoo?");
			return false;
		}

		var shape = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.BodypartShapes.Get(value)
			: Gameworld.BodypartShapes.GetByName(command.Last);
		if (shape == null)
		{
			actor.OutputHandler.Send(
				"There is no such bodypart shape. To see a list of bodypart shapes please see SHOW BODYPARTSHAPES.");
			return false;
		}

		if (_permittedShapes.Contains(shape))
		{
			actor.OutputHandler.Send(
				$"This tattoo will no longer be permitted to be inscribed on bodyparts of type {shape.Name.Colour(Telnet.Yellow)}.");
			_permittedShapes.Remove(shape);
			Changed = true;
			return true;
		}

		actor.OutputHandler.Send(
			$"This tattoo will now be permitted to be inscribed on bodyparts of type {shape.Name.Colour(Telnet.Yellow)}.");
		_permittedShapes.Add(shape);
		Changed = true;
		return true;
	}

	private bool BuildingCommandSize(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the minimum size of the bodypart on which this tattoo can be inscribed?");
			return false;
		}

		if (!GameItemEnumExtensions.TryParseSize(command.PopSpeech(), out var size))
		{
			actor.OutputHandler.Send("That is not a valid size. See SHOW SIZES for a list of valid sizes.");
			return false;
		}

		Size = size;
		Changed = true;
		actor.OutputHandler.Send(
			$"This tattoo will now require a bodypart of size {size.Describe().ColourValue()} or larger to be inscribed.");
		return true;
	}


	private bool BuildingCommandChargenProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog, or 'none' to remove an existing one.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear", "remove", "delete"))
		{
			CanSelectInChargenProg = null;
			Changed = true;
			actor.OutputHandler.Send(
				"This tattoo will no longer use a prog to determine who can select it in chargen. This means it will always be selectable if it has been approved for chargen.");
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send("You must specify a prog that returns a boolean value.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Chargen }))
		{
			actor.OutputHandler.Send("You must specify a prog that accepts a single Chargen as a parameter.");
			return false;
		}

		CanSelectInChargenProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This tattoo will now use the prog {prog.MXPClickableFunctionNameWithId()} to determine whether it can be selected in chargen.");
		return true;
	}

	#endregion

	public override bool CanSubmit()
	{
		if (!_inkColours.Any())
		{
			return false;
		}

		if (ShortDescription.EqualTo(DefaultShortDescription))
		{
			return false;
		}

		if (FullDescription.EqualTo(DefaultFullDescription))
		{
			return false;
		}

		if (GetInvalidTextSlotReferences().Any())
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (!_inkColours.Any())
		{
			return "You must set at least one ink colour to be required for this tattoo.";
		}

		if (ShortDescription.EqualTo(DefaultShortDescription))
		{
			return
				"You must change the short description of this tattoo to something different to the default placeholder.";
		}

		if (FullDescription.EqualTo(DefaultFullDescription))
		{
			return
				"You must change the full description of this tattoo to something different to the default placeholder.";
		}

		var invalidSlots = GetInvalidTextSlotReferences().ToList();
		if (invalidSlots.Any())
		{
			return
				$"This tattoo references undefined text slots: {invalidSlots.Select(x => x.ColourName()).ListToString()}.";
		}

		return base.WhyCannotSubmit();
	}

	#endregion

	#region Overrides of DisfigurementTemplate

	protected override IEnumerable<IDisfigurementTemplate> TemplatesForCharacter(ICharacter builder, bool currentOnly)
	{
		if (builder.IsAdministrator())
		{
			if (currentOnly)
			{
				return Gameworld.DisfigurementTemplates
				                .OfType<ITattooTemplate>()
				                .Where(x => x.Status == RevisionStatus.Current)
				                .ToList();
			}

			return Gameworld.DisfigurementTemplates
			                .OfType<ITattooTemplate>()
			                .ToList();
		}

		if (currentOnly)
		{
			return Gameworld.DisfigurementTemplates.OfType<ITattooTemplate>()
			                .Where(x => x.CanSeeTattooInList(builder))
			                .Where(x => x.Status == RevisionStatus.Current)
			                .ToList();
		}

		return Gameworld.DisfigurementTemplates.OfType<ITattooTemplate>()
		                .Where(x => x.CanSeeTattooInList(builder))
		                .ToList();
	}

	protected override string SubtypeName => "Tattoo";
	protected override string DefaultShortDescription => "a generic tattoo";
	protected override string DefaultFullDescription => "This is a generic tattoo, which is not yet described.";

	private readonly List<IBodypartShape> _permittedShapes = new();
	private readonly List<TattooTemplateTextSlot> _textSlots = new();
	public override IEnumerable<IBodypartShape> BodypartShapes => _permittedShapes;

	private IKnowledge _requiredKnowledgeToTattoo;

	public override bool CanBeAppliedToBodypart(IBody body, IBodypart part)
	{
		return (!_permittedShapes.Any() || _permittedShapes.Contains(part.Shape)) &&
		       body.Race.ModifiedSize(part) >= Size;
		// TODO - maximum tattoos per bodypart?
	}

	#endregion

	#region Implementation of ITattooTemplate

	public bool CanSeeTattooInList(ICharacter character)
	{
		return character.IsAdministrator() ||
		       _requiredKnowledgeToTattoo == null ||
		       character.Knowledges.Contains(_requiredKnowledgeToTattoo);
	}

	public bool CanProduceTattoo(ICharacter character)
	{
		return CanSeeTattooInList(character) && character.GetTrait(TattooistTrait)?.Value >= _minimumSkill;
	}

	public ITattoo ProduceTattoo(ICharacter tattooist, ICharacter target, IBodypart bodypart,
		IEnumerable<ITattooTextValue> textValues = null, bool hasUnreadableCopyPenalty = false)
	{
		return new Tattoo(this, Gameworld, tattooist, tattooist.GetTrait(TattooistTrait)?.Value ?? 0.0, bodypart,
			tattooist.Location.DateTime(), textValues, hasUnreadableCopyPenalty);
	}

	public IInventoryPlan GetInkPlan(ICharacter tattooist)
	{
		return new InventoryPlanTemplate(Gameworld, from ink in InkColours
		                                            select new InventoryPlanActionConsumeLiquid(Gameworld, 0, 0,
			                                            item => true, null,
			                                            liquid => liquid != null && liquid.Instances
				                                            .OfType<ColourLiquidInstance>().Any(x =>
					                                            x.Colour == ink.Colour && x.Liquid == InkLiquid &&
					                                            x.Amount >= ink.Amount),
			                                            new LiquidMixture(
				                                            new[]
				                                            {
					                                            new ColourLiquidInstance(InkLiquid, ink.Colour,
						                                            ink.Amount)
				                                            }, Gameworld)
		                                            )).CreatePlan(tattooist);
	}


	/// <summary>
	/// Whether or not this tattoo has a special form that overrides the default "heavily-tattooed" type description, e.g. "ritually-tattooed", "facially-tattooed" etc
	/// </summary>
	public bool HasSpecialTattooCharacteristicOverride => !string.IsNullOrWhiteSpace(_overrideCharacteristicPlain);

	private string _overrideCharacteristicWith;
	private string _overrideCharacteristicPlain;

	/// <summary>
	/// Returns a special overriding description, e.g. "ritually-tattooed", "with maori facial tattoos" etc
	/// </summary>
	/// <param name="withForm">If true, it's in the form "with ...", otherwise it's a participle e.g. "tattooed"</param>
	/// <returns>The description</returns>
	public string SpecialTattooCharacteristicOverride(bool withForm)
	{
		return withForm ? _overrideCharacteristicWith : _overrideCharacteristicPlain;
	}

	public SizeCategory Size { get; private set; } = SizeCategory.Nanoscopic;

	private double _minimumSkill;

	private DoubleCounter<IColour> _inkColours = new();

	private static double _tickMultiplierPerSize;

	public static double TickMultiplierPerSize
	{
		get
		{
			if (_tickMultiplierPerSize == 0)
			{
				_tickMultiplierPerSize = Futuremud.Games.First().GetStaticDouble("TattooingTicksPerSize");
			}

			return _tickMultiplierPerSize;
		}
	}

	public int TicksToCompleteTattoo => (int)(Math.Pow((int)Size, 2.0) * TickMultiplierPerSize);

	public IEnumerable<(IColour Colour, double Amount)> InkColours
	{
		get
		{
			var totalInkVolume = Gameworld.GetStaticDouble("TattooInkTotalPerSize") * (int)Size / TicksToCompleteTattoo;
			var totalInkWeights = _inkColours.Sum(x => x.Value);
			return _inkColours.Select(x => (x.Key, x.Value * totalInkVolume / totalInkWeights));
		}
	}

	public IEnumerable<ITattooTemplateTextSlot> TextSlots => _textSlots;
	public bool HasRequiredTextSlots => _textSlots.Any(x => x.RequiredCustomText);

	public ITattooTemplateTextSlot GetTextSlot(string name)
	{
		return _textSlots.FirstOrDefault(x => x.Name.EqualTo(name));
	}

	public string ResolveDescription(string description, IReadOnlyDictionary<string, ITattooTextValue> textValues)
	{
		if (string.IsNullOrEmpty(description))
		{
			return string.Empty;
		}

		return TemplateTextSlotRegex.Replace(description, match =>
		{
			var slot = _textSlots.FirstOrDefault(x => x.Name.EqualTo(match.Groups["name"].Value));
			if (slot == null)
			{
				return match.Value;
			}

			if (textValues != null &&
			    textValues.TryGetValue(slot.Name, out var value) &&
			    value != null)
			{
				return (value as TattooTextValue ?? new TattooTextValue(value.Name, value.Language, value.Script,
					value.Style, value.Colour, value.MinimumSkill, value.Text, value.AlternateText,
					value.IsCopiedFromSource, value.WasCopiedWithoutUnderstanding)).ToWritingMarkup();
			}

			return new TattooTextValue(slot).ToWritingMarkup();
		});
	}

	public string ResolveShortDescription(IReadOnlyDictionary<string, ITattooTextValue> textValues, IPerceiver voyeur)
	{
		return ResolveDescription(ShortDescription, textValues).SubstituteWrittenLanguage(voyeur, Gameworld);
	}

	public string ResolveFullDescription(IReadOnlyDictionary<string, ITattooTextValue> textValues, IPerceiver voyeur)
	{
		return ResolveDescription(FullDescription, textValues).SubstituteWrittenLanguage(voyeur, Gameworld);
	}

	#endregion

	private IEnumerable<string> GetInvalidTextSlotReferences()
	{
		return GetTextSlotReferences()
			.Where(x => !_textSlots.Any(y => y.Name.EqualTo(x)))
			.Distinct(StringComparer.InvariantCultureIgnoreCase);
	}

	private IEnumerable<string> GetTextSlotReferences()
	{
		return TemplateTextSlotRegex.Matches($"{ShortDescription}\n{FullDescription}")
			.OfType<Match>()
			.Select(x => x.Groups["name"].Value);
	}

	private string DescribeTextSlotForShow(TattooTemplateTextSlot slot)
	{
		var fallbackText = string.IsNullOrWhiteSpace(slot.DefaultText) ? "None".ColourError() : slot.DefaultText.ColourValue();
		return
			$"{slot.Name.ColourName()} (max {slot.MaximumLength.ToString("N0").ColourValue()}, {(slot.RequiredCustomText ? "required".ColourError() : "optional".ColourValue())}, fallback {fallbackText})";
	}

	private bool BuildingCommandTextSlot(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var rows = _textSlots.Select(x => new[]
			{
				x.Name.ColourName(),
				x.MaximumLength.ToString("N0", actor).ColourValue(),
				x.RequiredCustomText.ToColouredString(),
				x.DefaultLanguage?.Name.ColourName() ?? "None".ColourError(),
				x.DefaultScript?.Name.ColourName() ?? "None".ColourError(),
				string.IsNullOrWhiteSpace(x.DefaultText) ? "None".ColourError() : x.DefaultText.ColourValue()
			});
			actor.OutputHandler.Send(StringUtilities.GetTextTable(rows,
				new[] { "Name", "Max", "Required", "Language", "Script", "Fallback Text" }, actor,
				Telnet.Cyan));
			return false;
		}

		var firstArgument = command.PopSpeech();
		if (firstArgument.EqualTo("add"))
		{
			return BuildingCommandTextSlotAdd(actor, command);
		}

		if (firstArgument.EqualToAny("remove", "delete"))
		{
			return BuildingCommandTextSlotRemove(actor, command);
		}

		var slot = _textSlots.FirstOrDefault(x => x.Name.EqualTo(firstArgument));
		if (slot == null)
		{
			actor.OutputHandler.Send("There is no such tattoo text slot.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Slot {slot.Name.ColourName()}: max {slot.MaximumLength.ToString("N0", actor).ColourValue()}, required {slot.RequiredCustomText.ToColouredString()}, language {slot.DefaultLanguage?.Name.ColourName() ?? "None".ColourError()}, script {slot.DefaultScript?.Name.ColourName() ?? "None".ColourError()}, style {slot.DefaultStyle.Describe().ColourValue()}, colour {slot.DefaultColour?.Name.ColourName() ?? "None".ColourError()}, minimum skill {slot.DefaultMinimumSkill.ToString("N2", actor).ColourValue()}, text {slot.DefaultText.ColourValue()}, alternate {slot.DefaultAlternateText.ColourValue()}.");
			return false;
		}

		return BuildingCommandTextSlotEdit(actor, slot, command);
	}

	private bool BuildingCommandTextSlotAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give the new tattoo text slot?");
			return false;
		}

		var name = command.PopSpeech();
		if (!Regex.IsMatch(name, @"^[a-z][a-z0-9_]*$", RegexOptions.IgnoreCase))
		{
			actor.OutputHandler.Send(
				"Text slot names must start with a letter and contain only letters, numbers, or underscores.");
			return false;
		}

		if (_textSlots.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a text slot with that name.");
			return false;
		}

		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var maximumLength) || maximumLength <= 0)
		{
			actor.OutputHandler.Send("You must specify a positive maximum length for the slot.");
			return false;
		}

		var required = false;
		if (!command.IsFinished)
		{
			if (!bool.TryParse(command.PopSpeech(), out required))
			{
				actor.OutputHandler.Send("If you specify whether the slot is required, it must be true or false.");
				return false;
			}
		}

		_textSlots.Add(new TattooTemplateTextSlot(name, maximumLength, required, null, null,
			WritingStyleDescriptors.None, null, 0.0, string.Empty, string.Empty));
		Changed = true;
		actor.OutputHandler.Send(
			$"This tattoo now has a text slot called {name.ColourName()} with a max length of {maximumLength.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTextSlotRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tattoo text slot do you want to remove?");
			return false;
		}

		var slot = _textSlots.FirstOrDefault(x => x.Name.EqualTo(command.PopSpeech()));
		if (slot == null)
		{
			actor.OutputHandler.Send("There is no such tattoo text slot.");
			return false;
		}

		var references = GetTextSlotReferences().Where(x => x.EqualTo(slot.Name)).ToList();
		if (references.Any())
		{
			actor.OutputHandler.Send(
				$"You cannot remove the {slot.Name.ColourName()} slot while it is still referenced by $template{{{slot.Name}}} in this tattoo's descriptions.");
			return false;
		}

		_textSlots.Remove(slot);
		Changed = true;
		actor.OutputHandler.Send($"This tattoo no longer has a text slot called {slot.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandTextSlotEdit(ICharacter actor, TattooTemplateTextSlot slot, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "required":
				return BuildingCommandTextSlotRequired(actor, slot, command);
			case "maxlength":
			case "length":
			case "max":
				return BuildingCommandTextSlotMaxLength(actor, slot, command);
			case "language":
				return BuildingCommandTextSlotLanguage(actor, slot, command);
			case "script":
				return BuildingCommandTextSlotScript(actor, slot, command);
			case "style":
				return BuildingCommandTextSlotStyle(actor, slot, command);
			case "colour":
			case "color":
				return BuildingCommandTextSlotColour(actor, slot, command);
			case "minskill":
			case "minimumskill":
			case "skill":
				return BuildingCommandTextSlotMinimumSkill(actor, slot, command);
			case "text":
				slot.DefaultText = command.SafeRemainingArgument;
				Changed = true;
				actor.OutputHandler.Send(
					$"The {slot.Name.ColourName()} slot now defaults to the text {slot.DefaultText.ColourValue()}.");
				return true;
			case "alt":
			case "alternate":
				slot.DefaultAlternateText = command.SafeRemainingArgument;
				Changed = true;
				actor.OutputHandler.Send(
					$"The {slot.Name.ColourName()} slot now defaults to alternate text {slot.DefaultAlternateText.ColourValue()}.");
				return true;
		}

		actor.OutputHandler.Send(
			"You can edit text slots with REQUIRED, MAXLENGTH, LANGUAGE, SCRIPT, STYLE, COLOUR, MINSKILL, TEXT, or ALT.");
		return false;
	}

	private bool BuildingCommandTextSlotRequired(ICharacter actor, TattooTemplateTextSlot slot, StringStack command)
	{
		if (command.IsFinished || !bool.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must specify whether the slot is required as true or false.");
			return false;
		}

		slot.RequiredCustomText = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {slot.Name.ColourName()} text slot will {(value ? "now".ColourValue() : "no longer".ColourError())} require custom text.");
		return true;
	}

	private bool BuildingCommandTextSlotMaxLength(ICharacter actor, TattooTemplateTextSlot slot, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must specify a positive maximum text length.");
			return false;
		}

		slot.MaximumLength = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {slot.Name.ColourName()} text slot now has a maximum length of {value.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTextSlotLanguage(ICharacter actor, TattooTemplateTextSlot slot, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which language should be the default fallback for this slot?");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear"))
		{
			slot.DefaultLanguage = null;
			Changed = true;
			actor.OutputHandler.Send($"The {slot.Name.ColourName()} text slot no longer has a fallback language.");
			return true;
		}

		var language = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Languages.Get(value)
			: Gameworld.Languages.GetByName(command.Last);
		if (language == null)
		{
			actor.OutputHandler.Send("There is no such language.");
			return false;
		}

		slot.DefaultLanguage = language;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {slot.Name.ColourName()} text slot now uses {language.Name.ColourName()} as its fallback language.");
		return true;
	}

	private bool BuildingCommandTextSlotScript(ICharacter actor, TattooTemplateTextSlot slot, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which script should be the default fallback for this slot?");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear"))
		{
			slot.DefaultScript = null;
			Changed = true;
			actor.OutputHandler.Send($"The {slot.Name.ColourName()} text slot no longer has a fallback script.");
			return true;
		}

		var script = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Scripts.Get(value)
			: Gameworld.Scripts.GetByName(command.Last);
		if (script == null)
		{
			actor.OutputHandler.Send("There is no such script.");
			return false;
		}

		slot.DefaultScript = script;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {slot.Name.ColourName()} text slot now uses {script.Name.ColourName()} as its fallback script.");
		return true;
	}

	private bool BuildingCommandTextSlotStyle(ICharacter actor, TattooTemplateTextSlot slot, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What writing style flags should be the fallback for this slot? Use a comma-separated list, or NONE.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear"))
		{
			slot.DefaultStyle = WritingStyleDescriptors.None;
			Changed = true;
			actor.OutputHandler.Send($"The {slot.Name.ColourName()} text slot now has no fallback writing styles.");
			return true;
		}

		var style = WritingStyleDescriptors.None;
		foreach (var item in command.SafeRemainingArgument.Split(' ', StringSplitOptions.RemoveEmptyEntries))
		{
			foreach (var subItem in item.Split(',', StringSplitOptions.RemoveEmptyEntries))
			{
				style |= WritingStyleDescriptors.None.Parse(subItem.Trim());
			}
		}

		slot.DefaultStyle = style;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {slot.Name.ColourName()} text slot now uses {style.Describe().ColourValue()} as its fallback style.");
		return true;
	}

	private bool BuildingCommandTextSlotColour(ICharacter actor, TattooTemplateTextSlot slot, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which colour should be the default fallback for this slot?");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear"))
		{
			slot.DefaultColour = null;
			Changed = true;
			actor.OutputHandler.Send($"The {slot.Name.ColourName()} text slot no longer has a fallback colour.");
			return true;
		}

		var colour = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Colours.Get(value)
			: Gameworld.Colours.GetByName(command.Last);
		if (colour == null)
		{
			actor.OutputHandler.Send("There is no such colour.");
			return false;
		}

		slot.DefaultColour = colour;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {slot.Name.ColourName()} text slot now uses {colour.Name.ColourName()} as its fallback colour.");
		return true;
	}

	private bool BuildingCommandTextSlotMinimumSkill(ICharacter actor, TattooTemplateTextSlot slot, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must specify a minimum skill of zero or greater.");
			return false;
		}

		slot.DefaultMinimumSkill = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {slot.Name.ColourName()} text slot now has a fallback minimum skill of {value.ToString("N2", actor).ColourValue()}.");
		return true;
	}
}
