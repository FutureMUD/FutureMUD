#nullable enable

using System.Globalization;
using MudSharp.Character;
using MudSharp.Framework.Scheduling;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class CreateProjectionEffect : IMagicSpellEffectTemplate, IMagicSpellEffectAdmission, IMagicSpellEffectPreparedSelection
{
	private XElement _definition;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public const string HelpText = @"Projection options:
	#3plane <plane>#0 - exact occupied plane
	#3effigy <prototype>#0 - Sand Effigy's approved plain holdable item
	#3lifetime <seconds per grade>#0 - authored duration, at most one day at grade seven
	#3range <0-32 room edges>#0 - Walking Shadow's range; Sand Effigy is immobile
	#3backlash <0-1000 damage>#0 - native cellular damage on severing or damage collapse
	#3closeddoors true|false#0 - Walking Shadow can cross closed ordinary doors
	Both preserve canonical identity, prohibit physical manipulation and teleportation, and collapse on logout/reboot.";
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("createprojection", (root, spell) => new CreateProjectionEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("createprojection", (_, spell) =>
			(new CreateProjectionEffect(Definition(new(SpellProjectionKind.WalkingShadow, 0, 0, 120, 3, 0)), spell), ""),
			"Creates a bounded owned presence of the caster's identity", HelpText, true, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes.Contains("character")).ToArray());
	}
	public CreateProjectionEffect(XElement root, IMagicSpell spell) { Spell = spell; _definition = new(root); }
	public static XElement Definition(SpellProjectionConfiguration c) => new("Effect", new XAttribute("type", "createprojection"), new XAttribute("version", 1), SpellProjectionAnchor.Definition(c));
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new CreateProjectionEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes.Contains("character");
	internal SpellProjectionConfiguration Configuration()
	{
		if ((string?)_definition.Attribute("version") != "1" || _definition.Elements().Count() != 1) throw new FormatException("Invalid projection effect schema.");
		return SpellProjectionAnchor.ReadPolicy(_definition.Element("Policy")!);
	}
	public string? DefinitionError { get { try { return Configuration().Error(7); } catch (Exception ex) { return "Invalid projection configuration: " + ex.Message; } } }
	private bool Validate(ICharacter caster, IPerceivable target, out SpellProjectionConfiguration? c, out int grade, out string? error)
	{
		c = null; grade = 0; error = DefinitionError; if (error is not null) return false;
		if (!ReferenceEquals(caster, target) || Spell is not MagicSpell { InvocationGrade: { } selected } || Gameworld.SpellOwnedProjections is null)
		{ error = "Owned projection creation requires a selected-grade invocation targeting yourself."; return false; }
		grade = selected; c = Configuration(); error = Gameworld.SpellOwnedProjections.AdmissionError(caster, c, grade); return error is null;
	}
	private sealed record Selection(ICharacter Caster, IPerceivable Recipient, int Grade, string Definition, Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{ public bool IsCurrent => Check(); }
	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (!Validate(caster, recipient, out var c, out var grade, out var error)) throw new InvalidOperationException(error);
		var body = caster.Body; var room = caster.Location; var layer = caster.RoomLayer; var instance = caster.InstanceId; var state = caster.State;
		var plane = Gameworld.Planes.Get(c!.PlaneId); var proto = Gameworld.ItemProtos.Get(c.EffigyPrototypeId);
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var token = new Selection(caster, recipient, grade, definition, () => ReferenceEquals(caster.Body, body) && ReferenceEquals(caster.Location, room) &&
			caster.RoomLayer == layer && caster.InstanceId == instance && caster.State == state && caster.RoutePositionMetres is null && caster.Movement is null &&
			ReferenceEquals(Gameworld.Planes.Get(c.PlaneId), plane) && ReferenceEquals(Gameworld.ItemProtos.Get(c.EffigyPrototypeId), proto) && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		_selections[recipient] = token; return token;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared projection source or policy changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Recipient, recipient) || !token.IsCurrent ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition || !Validate(caster, recipient, out _, out var grade, out error) || grade != token.Grade) return false;
		_selections[recipient] = token; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared projection source or policy changed.";
		return _selections.TryGetValue(recipient, out var token) && ReferenceEquals(token.Caster, caster) && token.IsCurrent && Validate(caster, recipient, out _, out var grade, out error) && grade == token.Grade;
	}
	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null; if (!Validate(caster, target, out var c, out var grade, out error) || _selections.ContainsKey(target) && !TryConfirmPreparedSelection(caster, target, out error)) return false;
		application = new Creation(Guid.NewGuid(), this, caster, c!, grade); return true;
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null || !TryPrepareApplication(caster, target, outcome, power, TimeSpan.Zero, out var application, out var error)) throw new InvalidOperationException("Projection was not admitted.");
		return application!.Create(parent);
	}
	private sealed record Creation(Guid Id, CreateProjectionEffect Effect, ICharacter Caster, SpellProjectionConfiguration Policy, int Grade) : IMagicSpellEffectApplicationOperation
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => Apply(parent).Effect!;
		public MagicEffectOperation Apply(IMagicSpellEffectParent parent)
		{
			if (Effect._selections.ContainsKey(Caster) && !Effect.TryConfirmPreparedSelection(Caster, Caster, out var error)) throw new InvalidOperationException(error);
			var now = RuntimeClock.UtcNow;
			var instance = Effect.Gameworld.SpellOwnedProjections!.Create(Caster, Policy, new(Id, Effect.Spell.Id, Grade, Caster.Identity.Id,
				SpellProjectionAnchor.Family, SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(Policy.SecondsPerGrade * Grade)));
			var index = Caster.Identity.Instances.OrderByDescending(x => x.IsPrimaryInstance).ThenBy(x => x.InstanceId).ToList().FindIndex(x => x.InstanceId == instance.InstanceId) + 1;
			Caster.OutputHandler.Send($"Your {Policy.Kind.DescribeEnum().ColourName()} takes shape as instance #{instance.InstanceId.ToString("N0", Caster).ColourValue()}. Use {($"focus {index}").ColourCommand()} to observe through it.");
			return new(MagicEffectOperationStatus.Applied, null);
		}
	}
	public string Show(ICharacter actor) => DefinitionError is { } error ? error.ColourError() : $"{Configuration().Kind.DescribeEnum().ColourName()}, lifetime {TimeSpan.FromSeconds(Configuration().SecondsPerGrade).Describe(actor)} per grade; range {Configuration().MaximumRoomDistance.ToString("N0", actor)}, backlash {Configuration().BacklashDamage.ToString("N2", actor)}. " + HelpText.SubstituteANSIColour();
	public bool BuildingCommand(ICharacter actor, StringStack commands)
	{
		var field = commands.PopSpeech().ToLowerInvariant(); var text = commands.SafeRemainingArgument; var policy = _definition.Element("Policy")!;
		string? name = null; object? value = null;
		switch (field)
		{
			case "plane": var plane = Gameworld.Planes.GetByIdOrName(text); if (plane is not null) { name = "Plane"; value = plane.Id; } break;
			case "effigy": var proto = Gameworld.ItemProtos.GetByIdOrName(text); if (proto is not null) { name = "EffigyPrototype"; value = proto.Id; } break;
			case "lifetime": case "backlash": if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && double.IsFinite(number)) { name = field == "lifetime" ? "SecondsPerGrade" : "BacklashDamage"; value = number; } break;
			case "range": if (int.TryParse(text, out var distance)) { name = "MaximumRoomDistance"; value = distance; } break;
			case "closeddoors": if (bool.TryParse(text, out var doors)) { name = "CrossClosedDoors"; value = doors; } break;
		}
		if (name is null) { actor.OutputHandler.Send(HelpText.SubstituteANSIColour()); return false; }
		var old = policy.Element(name)!.Value; policy.SetElementValue(name, value);
		if (DefinitionError is { } error) { policy.SetElementValue(name, old); actor.OutputHandler.Send(error.ColourError()); return false; }
		Spell.Changed = true; actor.OutputHandler.Send("Projection policy updated."); return true;
	}
}
