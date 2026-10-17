#nullable enable

using System.Globalization;
using MudSharp.Character;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Magic.Lifecycle;
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
			SpellTriggerFactory.MagicTriggerTypes.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes.Contains("character") || SpellTriggerFactory.BuilderInfoForType(x).TargetTypes.Contains("item")).ToArray());
	}
	public CreateProjectionEffect(XElement root, IMagicSpell spell) { Spell = spell; _definition = new(root); }
	public static XElement Definition(SpellProjectionConfiguration c) => new("Effect", new XAttribute("type", "createprojection"), new XAttribute("version", 1), SpellProjectionAnchor.Definition(c));
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new CreateProjectionEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => DefinitionError is null &&
		trigger.TargetTypes.Contains(Configuration().RequiresHeldSourceItem ? "item" : "character");
	internal SpellProjectionConfiguration Configuration()
	{
		if ((string?)_definition.Attribute("version") != "1" || _definition.Elements().Count() != 1) throw new FormatException("Invalid projection effect schema.");
		return SpellProjectionAnchor.ReadPolicy(_definition.Element("Policy")!);
	}
	public string? DefinitionError { get { try { return Configuration().Error(7); } catch (Exception ex) { return "Invalid projection configuration: " + ex.Message; } } }
	private bool Validate(ICharacter caster, IPerceivable target, out SpellProjectionConfiguration? c, out int grade, out string? error)
	{
		c = null; grade = 0; error = DefinitionError; if (error is not null) return false;
		c = Configuration();
		if (Spell is not MagicSpell { InvocationGrade: { } selected } || Gameworld.SpellOwnedProjections is null)
		{ error = "Owned projection creation requires a selected-grade invocation targeting yourself."; return false; }
		grade = selected;
		if (c.RequiresHeldSourceItem)
		{
			if (target is not IGameItem source) { error = "Select the exact ordinary held item supplying this effigy."; return false; }
			SpellOwnedProjectionService.SourceFrame frame;
			try { frame = SpellOwnedProjectionService.CaptureSourceFrame(caster, source, Gameworld); }
			catch (InvalidOperationException exception) { error = exception.Message; return false; }
			error = Gameworld.SpellOwnedProjections.AdmissionError(caster, c, grade, source);
			if (error is null && !frame.IsCurrent) error = "The borrowed item changed during projection admission.";
			return error is null;
		}
		if (!ReferenceEquals(caster, target)) { error = "This projection targets your existing primary identity."; return false; }
		error = Gameworld.SpellOwnedProjections.AdmissionError(caster, c, grade); return error is null;
	}
	private sealed record Selection(ICharacter Caster, IPerceivable Recipient, int Grade, string Definition, Func<bool> Check,
		SpellOwnedProjectionService.SourceFrame? Source) : IMagicSpellEffectPreparedSelectionRawToken
	{ public bool IsCurrent => Check(); }
	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		var c = Configuration();
		var source = c.RequiresHeldSourceItem && recipient is IGameItem item ? SpellOwnedProjectionService.CaptureSourceFrame(caster, item, Gameworld) : null;
		var grade = 0; string? error;
		if (source is null && !Validate(caster, recipient, out _, out grade, out error)) throw new InvalidOperationException(error);
		var body = caster.Body; var room = caster.Location; var layer = caster.RoomLayer; var instance = caster.InstanceId; var state = caster.State;
		var identity = caster.Identity; var plane = Gameworld.Planes.Get(c.PlaneId); var proto = Gameworld.ItemProtos.Get(c.EffigyPrototypeId);
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		bool CurrentFrame() => ReferenceEquals(caster.Body, body) && ReferenceEquals(caster.Identity, identity) && ReferenceEquals(caster.Location, room) &&
			caster.RoomLayer == layer && caster.InstanceId == instance && caster.State == state && caster.RoutePositionMetres is null && caster.Movement is null &&
			ReferenceEquals(Gameworld.Planes.Get(c.PlaneId), plane) && ReferenceEquals(Gameworld.ItemProtos.Get(c.EffigyPrototypeId), proto) &&
			(source is null || source.IsCurrent) && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition;
		if (source is not null && !Validate(caster, recipient, out _, out grade, out error)) throw new InvalidOperationException(error);
		if (!CurrentFrame()) throw new InvalidOperationException("The projection's original source or physical frame changed during admission.");
		var token = new Selection(caster, recipient, grade, definition, CurrentFrame, source);
		_selections[recipient] = token; return token;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared projection source or policy changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Recipient, recipient) || !token.IsCurrent ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition || !Validate(caster, recipient, out _, out var grade, out error) || grade != token.Grade || !token.IsCurrent) return false;
		_selections[recipient] = token; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared projection source or policy changed.";
		return _selections.TryGetValue(recipient, out var token) && ReferenceEquals(token.Caster, caster) && token.IsCurrent && Validate(caster, recipient, out _, out var grade, out error) && grade == token.Grade && token.IsCurrent;
	}
	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		try { if (Configuration().RequiresHeldSourceItem && !_selections.ContainsKey(target)) CapturePreparedSelection(caster, target); }
		catch (InvalidOperationException exception) { error = exception.Message; return false; }
		if (!Validate(caster, target, out var c, out var grade, out error) || _selections.ContainsKey(target) && !TryConfirmPreparedSelection(caster, target, out error)) return false;
		_selections.TryGetValue(target, out var selection);
		application = new Creation(Guid.NewGuid(), this, caster, target, c!, grade, selection); return true;
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null || !TryPrepareApplication(caster, target, outcome, power, TimeSpan.Zero, out var application, out var error)) throw new InvalidOperationException("Projection was not admitted.");
		return application!.Create(parent);
	}
	private sealed record Creation(Guid Id, CreateProjectionEffect Effect, ICharacter Caster, IPerceivable Recipient,
		SpellProjectionConfiguration Policy, int Grade, Selection? Prepared) : IMagicSpellEffectApplicationOperation
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => Apply(parent).Effect!;
		public MagicEffectOperation Apply(IMagicSpellEffectParent parent)
		{
			if (Prepared is not null && (!Prepared.IsCurrent || !Effect.TryConfirmPreparedSelection(Caster, Recipient, out var error)))
				throw new InvalidOperationException("The original paid projection source or physical frame changed.");
			var now = RuntimeClock.UtcNow;
			var policy = Policy.ResolveForGrade(Grade);
			var origin = new SpellLifecycleOrigin(Id, Effect.Spell.Id, Grade, Caster.Identity.Id,
				SpellProjectionAnchor.Family, SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(policy.SecondsPerGrade * Grade));
			var service = Effect.Gameworld.SpellOwnedProjections!;
			var instance = Prepared?.Source is { } source ? service.Create(Caster, policy, origin, source.Item) : service.Create(Caster, policy, origin);
			if (Prepared is not null && !Prepared.IsCurrent)
			{
				if (instance is ICharacter projection) service.RequestRetirement(projection, SpellRetirementReason.AnchorSevered);
				throw new InvalidOperationException("Projection creation changed its paid frame; retain retirement and do not replay.");
			}
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
