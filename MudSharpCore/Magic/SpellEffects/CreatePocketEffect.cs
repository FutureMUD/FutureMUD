#nullable enable

using System.Globalization;
using MudSharp.Character;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class CreatePocketEffect : IMagicSpellEffectTemplate, IMagicSpellEffectAdmission, IMagicSpellEffectPreparedSelection
{
	private XElement _definition;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public const string HelpText = "Pocket options: prototype <item>, fallback <room>, capacity <mass per grade>, size <maximum size>, lifetime <seconds per grade>, access Bearer|Creator.";
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("createpocket", (root, spell) => new CreatePocketEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("createpocket", (_, spell) =>
			(new CreatePocketEffect(new XElement("Effect", new XAttribute("type", "createpocket"), new XAttribute("version", 1),
				new SpellPocketConfiguration(0, 1, SizeCategory.Normal, 300, SpellPocketAccess.Bearer, 0).Save()), spell), ""),
			"Creates finite portable storage with exact foreign-content conservation", HelpText, true, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "item").ToArray());
	}
	public CreatePocketEffect(XElement root, IMagicSpell spell) { Spell = spell; _definition = new(root); }
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new CreatePocketEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes == "item";
	internal SpellPocketConfiguration Configuration(bool validate = true)
	{
		if (_definition.Name != "Effect" || (string?)_definition.Attribute("type") != "createpocket" ||
			(string?)_definition.Attribute("version") != "1" || _definition.Attributes().Count() != 2 ||
			_definition.Elements().Count() != 1 || _definition.Element("Pocket") is not { } pocket)
			throw new FormatException("Unsupported pocket effect envelope.");
		return SpellPocketConfiguration.Load(pocket, validate);
	}
	public string? DefinitionError { get { try { Configuration(); return null; } catch (Exception ex) { return "Invalid pocket configuration: " + ex.Message; } } }
	private bool Validate(ICharacter caster, IPerceivable target, out SpellPocketConfiguration? configuration, out int grade, out string? error)
	{
		configuration = null; grade = 0; error = DefinitionError;
		if (error is not null) return false;
		if (target is not IGameItem source || Spell is not MagicSpell { InvocationGrade: { } selected } || Gameworld.SpellOwnedPockets is null)
		{ error = "Pocket creation requires a selected-grade item invocation."; return false; }
		configuration = Configuration(); grade = selected;
		error = Gameworld.SpellOwnedPockets.AdmissionError(caster, source, configuration, grade); return error is null;
	}
	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{ if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error); return existing; }
		if (!Validate(caster, recipient, out var configuration, out var grade, out var diagnostic)) throw new InvalidOperationException(diagnostic);
		var source = (IGameItem)recipient; var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var prototype = Gameworld.ItemProtos.Get(configuration!.PrototypeId); var revision = prototype.RevisionNumber;
		var fallback = Gameworld.Rooms.Get(configuration.FallbackRoomId);
		var sourcePrototype = source.Prototype; var body = caster.Body; var instance = caster.InstanceId;
		var location = caster.Location; var layer = caster.RoomLayer; var position = caster.RoutePositionMetres;
		var selection = new Selection(caster, source, grade, definition, () => ReferenceEquals(caster.Body, body) && caster.InstanceId == instance &&
			ReferenceEquals(caster.Location, location) && caster.RoomLayer == layer && caster.RoutePositionMetres == position &&
			ReferenceEquals(source.Prototype, sourcePrototype) && ReferenceEquals(Gameworld.ItemProtos.Get(configuration.PrototypeId), prototype) &&
			prototype.RevisionNumber == revision && ReferenceEquals(Gameworld.Rooms.Get(configuration.FallbackRoomId), fallback) &&
			SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		_selections.Add(recipient, selection); return selection;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared pocket frame or policy changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Source, recipient) ||
			!token.IsCurrent || token.Definition != SaveToXml().ToString(SaveOptions.DisableFormatting) ||
			!Validate(caster, recipient, out _, out var grade, out error) || grade != token.Grade) return false;
		_selections[recipient] = token; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared pocket frame or policy changed.";
		return _selections.TryGetValue(recipient, out var token) && ReferenceEquals(token.Caster, caster) && token.IsCurrent &&
			Validate(caster, recipient, out _, out var grade, out error) && grade == token.Grade;
	}
	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power,
		TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		if (!Validate(caster, target, out var configuration, out var grade, out error) ||
			_selections.ContainsKey(target) && !TryConfirmPreparedSelection(caster, target, out error)) return false;
		application = new Creation(Guid.NewGuid(), this, caster, (IGameItem)target, configuration!, grade); return true;
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null || !TryPrepareApplication(caster, target, outcome, power, TimeSpan.Zero, out var application, out _))
			throw new InvalidOperationException("Pocket application was not admitted.");
		return application!.Create(parent);
	}
	private sealed record Selection(ICharacter Caster, IGameItem Source, int Grade, string Definition, Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{ public bool IsCurrent => Check(); }
	private sealed record Creation(Guid Id, CreatePocketEffect Effect, ICharacter Caster, IGameItem Source,
		SpellPocketConfiguration Configuration, int Grade) : IMagicSpellEffectApplicationOperation
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => Apply(parent).Effect!;
		public MagicEffectOperation Apply(IMagicSpellEffectParent parent)
		{
			var now = new DateTime(RuntimeClock.UtcNow.Ticks / 10 * 10, DateTimeKind.Utc);
			var anchor = new SpellPocketAnchor(Source.Id, Grade, Configuration);
			try
			{
				if (Effect._selections.ContainsKey(Source) && !Effect.TryConfirmPreparedSelection(Caster, Source, out var error)) throw new InvalidOperationException(error);
				Effect.Gameworld.SpellOwnedPockets!.Create(Caster, Source, Configuration, new SpellLifecycleOrigin(Id, Effect.Spell.Id,
					Grade, CharacterInstanceIdentityComparer.IdentityId(Caster), SpellPocketAnchor.Family, SpellLifecycleMode.TemporaryCleanup,
					now, now + Configuration.DurationForGrade(Grade), anchor.Save()));
			}
			finally { Effect._selections.Remove(Source); }
			return new(MagicEffectOperationStatus.Applied, null);
		}
	}
	public string Show(ICharacter actor) => "Creates a finite native pocket: " + SaveToXml().ToString(SaveOptions.DisableFormatting).ColourValue();
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		try
		{
			var c = Configuration(false); var option = command.PopSpeech().ToLowerInvariant(); var value = command.SafeRemainingArgument;
			switch (option)
			{
				case "prototype": c = c with { PrototypeId = Gameworld.ItemProtos.GetByIdOrName(value)?.Id ?? throw new ArgumentException("No such item prototype.") }; break;
				case "fallback": c = c with { FallbackRoomId = Gameworld.Rooms.GetByIdOrName(value)?.Id ?? throw new ArgumentException("No such fallback room.") }; break;
				case "capacity":
					if (!Gameworld.UnitManager.TryGetBaseUnits(value, UnitType.Mass, actor, out var capacity) || !double.IsFinite(capacity) || capacity <= 0) throw new ArgumentException("Use a positive finite mass per grade.");
					c = c with { CapacityPerGrade = capacity }; break;
				case "lifetime": c = c with { SecondsPerGrade = double.Parse(value, CultureInfo.InvariantCulture) }; break;
				case "size": c = c with { MaximumSize = Enum.Parse<SizeCategory>(value, true) }; break;
				case "access": c = c with { Access = Enum.Parse<SpellPocketAccess>(value, true) }; break;
				default: actor.OutputHandler.Send(HelpText.ColourCommand()); return false;
			}
			(c with { PrototypeId = Math.Max(1, c.PrototypeId), FallbackRoomId = Math.Max(1, c.FallbackRoomId) }).Validate();
			_definition = new XElement("Effect", new XAttribute("type", "createpocket"), new XAttribute("version", 1), c.Save());
			Spell.Changed = true; actor.OutputHandler.Send("Pocket configuration updated; existing pockets retain their original binding and deadline."); return true;
		}
		catch (Exception ex) when (ex is ArgumentException or FormatException or OverflowException)
		{ actor.OutputHandler.Send(ex.Message.ColourError()); return false; }
	}
}
