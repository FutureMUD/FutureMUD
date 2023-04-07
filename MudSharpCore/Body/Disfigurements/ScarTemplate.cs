using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;

namespace MudSharp.Body.Disfigurements;

public class ScarTemplate : DisfigurementTemplate, IScarTemplate
{
	public ScarTemplate(MudSharp.Models.DisfigurementTemplate dbitem, IFuturemud gameworld) : base(dbitem, gameworld)
	{
		var root = XElement.Parse(dbitem.Definition);
		CanSelectInChargen = bool.Parse(root.Element("CanBeSelectedInChargen").Value);
		CanSelectInChargenProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("CanBeSelectedInChargenProg")?.Value ?? "0"));
		foreach (var element in root.Element("ChargenCosts")?.Elements() ?? Enumerable.Empty<XElement>())
		{
			ChargenCosts[Gameworld.ChargenResources.Get(long.Parse(element.Attribute("resource").Value))] =
				int.Parse(element.Attribute("amount").Value);
		}

		foreach (var element in root.Element("Shapes").Elements())
		{
			_permittedShapes.Add(Gameworld.BodypartShapes.Get(long.Parse(element.Value)));
		}

		_overrideCharacteristicPlain = root.Element("OverrideCharacteristicPlain")?.Value;
		_overrideCharacteristicWith = root.Element("OverrideCharacteristicWith")?.Value;
		SizeSteps = int.Parse(root.Element("SizeSteps").Value);
		Unique = bool.Parse(root.Element("Unique").Value);
		Distinctiveness = int.Parse(root.Element("Distinctiveness").Value);
		foreach (var element in root.Element("Surgeries").Elements())
		{
			_surgicalProcedureTypes.Add((SurgicalProcedureType)int.Parse(element.Value));
		}

		foreach (var element in root.Element("Damages").Elements())
		{
			_damageTypesAndSeverities[(DamageType)int.Parse(element.Attribute("type").Value)] =
				(WoundSeverity)int.Parse(element.Attribute("severity").Value);
		}
	}

	public ScarTemplate(IAccount originator, string name) : base(originator, name)
	{
	}

	public ScarTemplate(ScarTemplate rhs, IAccount originator, string name) : base(rhs, originator, name)
	{
		_permittedShapes = rhs._permittedShapes.ToList();
		_overrideCharacteristicPlain = rhs._overrideCharacteristicPlain;
		_overrideCharacteristicWith = rhs._overrideCharacteristicWith;
		_damageTypesAndSeverities = rhs._damageTypesAndSeverities.ToDictionary(x => x.Key, x => x.Value);
		CanSelectInChargen = rhs.CanSelectInChargen;
		CanSelectInChargenProg = rhs.CanSelectInChargenProg;
		foreach (var cost in rhs.ChargenCosts)
		{
			ChargenCosts[cost.Key] = cost.Value;
		}

		SizeSteps = rhs.SizeSteps;
		Unique = rhs.Unique;
		Distinctiveness = rhs.Distinctiveness;
		Changed = true;
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.DisfigurementTemplates.GetAll(Id).OfType<IScarTemplate>();
	}

	protected override IEnumerable<IDisfigurementTemplate> TemplatesForCharacter(ICharacter builder, bool currentOnly)
	{
		if (currentOnly)
		{
			return Gameworld.DisfigurementTemplates
			                .OfType<IScarTemplate>()
			                .Where(x => x.Status == RevisionStatus.Current)
			                .ToList();
		}

		return Gameworld.DisfigurementTemplates
		                .OfType<IScarTemplate>()
		                .ToList();
	}

	protected override string SubtypeName => "Scar";
	protected override string DefaultShortDescription => "a generic scar";
	protected override string DefaultFullDescription => "This is a generic scar, which is not yet described.";

	protected override XElement SaveDefinition()
	{
		return new XElement("Scar",
			new XElement("CanBeSelectedInChargen", CanSelectInChargen),
			new XElement("CanBeSelectedInChargenProg", CanSelectInChargenProg?.Id ?? 0),
			new XElement("ChargenCosts",
				from cost in ChargenCosts
				select new XElement("Cost", new XAttribute("resource", cost.Key.Id),
					new XAttribute("amount", cost.Value))
			),
			new XElement("OverrideCharcteristicPlain", _overrideCharacteristicPlain ?? string.Empty),
			new XElement("OverrideCharacteristicWith", _overrideCharacteristicWith ?? string.Empty),
			new XElement("SizeSteps", SizeSteps),
			new XElement("Distinctiveness", Distinctiveness),
			new XElement("Unique", Unique),
			new XElement("Shapes",
				from shape in _permittedShapes
				select new XElement("Shape", shape.Id)
			),
			new XElement("Surgeries",
				from surgery in _surgicalProcedureTypes
				select new XElement("Surgery", (int)surgery)),
			new XElement("Damages",
				from damage in _damageTypesAndSeverities
				select new XElement("Damage", new XAttribute("severity", (int)damage.Value),
					new XAttribute("type", (int)damage.Key)))
		);
	}

	public override IDisfigurementTemplate Clone(IAccount originator, string newName)
	{
		return new ScarTemplate(this, originator, newName);
	}

	private readonly List<IBodypartShape> _permittedShapes = new();
	public override IEnumerable<IBodypartShape> BodypartShapes => _permittedShapes;

	public override bool CanBeAppliedToBodypart(IBody body, IBodypart part)
	{
		return (!_permittedShapes.Any() || _permittedShapes.Contains(part.Shape)) &&
		       (!Unique || body.Scars.All(x => x.Template != this));
	}

	public override string EditHeader()
	{
		return $"Scar Template #{Id:N0}r{RevisionNumber:N0}";
	}

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
		sb.AppendLine($"Can Only Have One: {Unique.ToColouredString()}");
		sb.AppendLine($"Sizes Below Bodypart Size: {SizeSteps.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Distinctiveness: {Distinctiveness.ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Permitted Bodyparts: {(_permittedShapes.Any() ? _permittedShapes.Select(x => x.Name.ColourValue()).ListToString() : "Any".ColourValue())}");
		sb.AppendLine(
			$"Permitted Surgeries: {(_surgicalProcedureTypes.Any() ? _surgicalProcedureTypes.Select(x => x.DescribeEnum().ColourValue()).ListToString() : "None".Colour(Telnet.Red))}");
		sb.AppendLine(
			$"Damage Types: {(_damageTypesAndSeverities.Any() ? _damageTypesAndSeverities.Select(x => $"{x.Value.Describe()} {x.Key.Describe()}".ColourValue()).ListToString() : "None".Colour(Telnet.Red))}");
		if (HasSpecialScarCharacteristicOverride)
		{
			sb.AppendLine($"Override Characteristic (Plain): {_overrideCharacteristicPlain.ColourCharacter()}");
			sb.AppendLine($"Override Characteristic (With): {_overrideCharacteristicWith.ColourCharacter()}");
		}

		sb.AppendLine($"Full Description:\n\n{FullDescription.Wrap(actor.InnerLineFormatLength)}");
		return sb.ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return new ScarTemplate(this, initiator.Account, null);
	}

	public bool CanBeAppliedFromDamage(DamageType damageType, WoundSeverity severity)
	{
		if (!_damageTypesAndSeverities.ContainsKey(damageType))
		{
			return false;
		}

		return _damageTypesAndSeverities[damageType] <= severity;
	}

	public bool CanBeAppliedFromSurgery(SurgicalProcedureType type)
	{
		return _surgicalProcedureTypes.Contains(type);
	}

	public IScar ProduceScar(ICharacter target, IBodypart bodypart)
	{
		return new Scar(this, Gameworld, target, bodypart, target.Location.DateTime());
	}

	public int SizeSteps { get; protected set; }
	public int Distinctiveness { get; protected set; }
	public bool Unique { get; protected set; }
	private readonly List<SurgicalProcedureType> _surgicalProcedureTypes = new();
	private readonly Dictionary<DamageType, WoundSeverity> _damageTypesAndSeverities = new();

	/// <summary>
	/// Whether or not this scar has a special form that overrides the default "heavily-scarred" type description, e.g. "facially-scarred", "facially-disfigured" etc
	/// </summary>
	public bool HasSpecialScarCharacteristicOverride => !string.IsNullOrWhiteSpace(_overrideCharacteristicPlain);

	private string _overrideCharacteristicWith;
	private string _overrideCharacteristicPlain;

	/// <summary>
	/// Returns a special overriding description, e.g. "facially-disfigured", "with an eye scar" etc
	/// </summary>
	/// <param name="withForm">If true, it's in the form "with ...", otherwise it's a participle e.g. "scarred"</param>
	/// <returns>The description</returns>
	public string SpecialScarCharacteristicOverride(bool withForm)
	{
		return withForm ? _overrideCharacteristicWith : _overrideCharacteristicPlain;
	}

	public override bool CanSubmit()
	{
		if (ShortDescription.EqualTo(DefaultShortDescription))
		{
			return false;
		}

		if (FullDescription.EqualTo(DefaultFullDescription))
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
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

		return base.WhyCannotSubmit();
	}

	#region Building Commands

	protected override string BuildingHelpText =>
		$"{base.BuildingHelpText}\n\tsize <steps> - sets the number of sizes smaller than the bodypart this scar is\n\tsurgery <type> - toggle the surgery types that can apply this scar\n\tdamage <type> <severity> - set a damage type and minimum severity for this scar\n\tdamage <type> - clears a damage type\n\tbodypart <shape> - sets the target bodypart shapes\n\tchargen [<amount> <resource>] - toggles tattoo selectable in chargen. Admins only.\n\tprog <prog> - sets the prog that controls appearance in chargen. Admins only.\n\toverride \"<plain>\" \"<with>\" - special override descriptions for sdescs, both a plain form and a with form.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "size":
				return BuildingCommandSize(actor, command);
			case "surgery":
				return BuildingCommandSurgery(actor, command);
			case "damage":
				return BuildingCommandDamage(actor, command);
			case "override":
				return BuildingCommandOverride(actor, command);
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
			case "chargenprog":
			case "chargen prog":
			case "chargen_prog":
				if (!actor.IsAdministrator())
				{
					break;
				}

				return BuildingCommandChargenProg(actor, command);
		}

		return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
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
				"This scar will no longer use a prog to determine who can select it in chargen. This means it will always be selectable if it has been approved for chargen.");
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

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send("You must specify a prog that returns a boolean value.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Chargen }))
		{
			actor.OutputHandler.Send("You must specify a prog that accepts a single Chargen as a parameter.");
			return false;
		}

		CanSelectInChargenProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This scar will now use the prog {prog.MXPClickableFunctionNameWithId()} to determine whether it can be selected in chargen.");
		return true;
	}

	private bool BuildingCommandDamage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which damage type did you want to add, edit or toggle?");
			return false;
		}

		if (!Utilities.TryParseEnum<DamageType>(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid damage type. See SHOW DAMAGES for a list of valid damage types.");
			return false;
		}

		if (command.IsFinished)
		{
			if (!_damageTypesAndSeverities.ContainsKey(value))
			{
				actor.OutputHandler.Send(
					$"This scar is not currently set to be applied by the {value.Describe().ColourValue()} damage type. You must supply a minimum severity if you were hoping to add this damage type.");
				return false;
			}

			_damageTypesAndSeverities.Remove(value);
			actor.OutputHandler.Send(
				$"The {value.Describe().ColourValue()} damage type will no longer cause this scar.");
			Changed = true;
			return true;
		}

		if (!Utilities.TryParseEnum<WoundSeverity>(command.PopSpeech(), out var sValue))
		{
			actor.OutputHandler.Send(
				$"That is not a valid wound severity. The valid values are {Enum.GetValues(typeof(WoundSeverity)).OfType<WoundSeverity>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		_damageTypesAndSeverities[value] = sValue;
		Changed = true;
		actor.OutputHandler.Send(
			$"This scar can now be caused by {value.Describe().ColourValue()} damage of severity {sValue.Describe().ColourValue()} or worse.");
		return true;
	}

	private bool BuildingCommandSurgery(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which surgical procedure type do you want to toggle? See SHOW SURGERIES to see a list of possible types.");
			return false;
		}

		if (!Utilities.TryParseEnum<SurgicalProcedureType>(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send(
				"That is not a valid surgical procedure type. See SHOW SURGERIES to see a list of possible types.");
			return false;
		}

		if (_surgicalProcedureTypes.Contains(value))
		{
			_surgicalProcedureTypes.Remove(value);
			Changed = true;
			actor.OutputHandler.Send(
				$"Surgeries of type {value.DescribeEnum().ColourValue()} will no longer be able to cause this scar.");
			return true;
		}

		_surgicalProcedureTypes.Add(value);
		Changed = true;
		actor.OutputHandler.Send(
			$"Surgeries of type {value.DescribeEnum().ColourValue()} will now be able to cause this scar.");
		return true;
	}

	private bool BuildingCommandSize(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value > 0)
		{
			actor.OutputHandler.Send(
				"You must supply a valid negative number of steps smaller than the bodypart for the size of the scar (e.g. 0 would be a scar that is as big as the bodypart itself, -1 a little smaller etc)");
			return false;
		}

		SizeSteps = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This scar is now {SizeSteps.ToString("N0", actor).ColourValue()} steps smaller than its parent bodypart. For example, this scar on a size {SizeCategory.Normal.Describe().ColourValue()} bodypart would be size {SizeCategory.Normal.ChangeSize(SizeSteps).Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandChargen(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"This scar will {(CanSelectInChargen ? "no longer" : "now")} be selectable in character creation.");
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
			$"This scar is now selectable in chargen for a cost of {ChargenCosts.Select(x => $"{x.Value} {x.Key.Alias}".ColourValue()).ListToString()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandBodypart(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which bodypart shape do you want to toggle being able to receive this scar?");
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
				$"This scar will no longer be possible to occur on bodyparts of type {shape.Name.Colour(Telnet.Yellow)}.");
			_permittedShapes.Remove(shape);
			Changed = true;
		}

		actor.OutputHandler.Send(
			$"This scar will now be possible to occur on bodyparts of type {shape.Name.Colour(Telnet.Yellow)}.");
		_permittedShapes.Add(shape);
		Changed = true;
		return true;
	}

	private bool BuildingCommandOverride(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the plain form (e.g. facially-scarred) you want this scar to provide?");
			return false;
		}

		var plain = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What is the with form (e.g. with an across-the-eye scar) you want this scar to provide?");
			return false;
		}

		_overrideCharacteristicWith = command.PopSpeech();
		_overrideCharacteristicPlain = plain;
		Changed = true;
		actor.OutputHandler.Send(
			$"This scar will now provide an override of {_overrideCharacteristicPlain.ColourCharacter()} with the &scars sdesc tag and {_overrideCharacteristicWith.ColourCharacter()} with the &withscars sdesc tag.");
		return true;
	}

	#endregion
}