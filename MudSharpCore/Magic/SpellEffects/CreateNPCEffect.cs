using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.NPC.Templates;
using MudSharp.RPG.Checks;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework.Scheduling;
using MudSharp.Effects.Concrete;
using MudSharp.Magic.Lifecycle;

#nullable enable annotations

namespace MudSharp.Magic.SpellEffects;

public sealed partial class CreateNPCEffect : IMagicSpellEffectTemplate, IMagicSpellEffectAdmission, IMagicSpellEffectPreparedSelection
{
    public static void RegisterFactory()
    {
        SpellEffectFactory.RegisterLoadTimeFactory("createnpc", (root, spell) => new CreateNPCEffect(root, spell));
        SpellEffectFactory.RegisterBuilderFactory("createnpc", BuilderFactory,
            "Loads a new NPC",
            HelpText,
            true,
            true,
            SpellTriggerFactory.MagicTriggerTypes.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
    }

    private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
        IMagicSpell spell)
    {
        return (new CreateNPCEffect(new XElement("Effect",
            new XAttribute("type", "createnpc"),
            new XElement("NPCPrototypeId", 0),
            new XElement("OnLoadProg", 0)
        ), spell), string.Empty);
    }

    public CreateNPCEffect(XElement root, IMagicSpell spell)
    {
        Spell = spell;
        _npcPrototypeId = long.Parse(root.Element("NPCPrototypeId").Value);
        _onLoadProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnLoadProg").Value));
		if (root.Element("Lifecycle") is { } lifetime)
		{
			if (!int.TryParse((string?)lifetime.Attribute("version"), out var version) || version != 1 ||
				!Enum.TryParse<SpellLifecycleMode>((string?)lifetime.Attribute("mode"), true, out var mode) || !Enum.IsDefined(mode))
			{
				_loadError = "Invalid NPC lifecycle schema or mode.";
				_unreadableLifecycle = new XElement(lifetime);
			}
			else LifecycleMode = mode;
			LifecycleFamily = lifetime.Element("Family")?.Value ?? string.Empty;
			_lifetimeFormula = lifetime.Element("Seconds")?.Value;
			if (_lifetimeFormula is not null) LifetimeExpression = new TraitExpression(_lifetimeFormula, Gameworld);
		}
    }

    public IFuturemud Gameworld => Spell.Gameworld;

    public IMagicSpell Spell { get; }

    private long _npcPrototypeId;

    public INPCTemplate NPCTemplate => Gameworld.NpcTemplates.Get(_npcPrototypeId);

    private IFutureProg _onLoadProg;
	private string? _loadError;
	private XElement? _unreadableLifecycle;
	private string? _lifetimeFormula;
	public SpellLifecycleMode? LifecycleMode { get; private set; }
	public string LifecycleFamily { get; private set; } = string.Empty;
	public ITraitExpression? LifetimeExpression { get; internal set; }
	public string? DefinitionError => _loadError ?? (LifecycleMode is null ? null :
		string.IsNullOrWhiteSpace(LifecycleFamily) || LifecycleFamily.Length > 128 ? "Set a lifecycle family of at most 128 characters." :
		NativeNpcCreationEligibility.TemplateError(NPCTemplate, Gameworld) is { } templateError ? templateError :
		Gameworld.SpellOwnedNpcs is null ? "Spell-owned native NPC creation is unavailable." :
		LifecycleMode != SpellLifecycleMode.Permanent && (LifetimeExpression is null || LifetimeExpression.HasErrors()) ? "Set a valid temporary lifetime expression in real seconds." : null);

    public XElement SaveToXml()
    {
        return new XElement("Effect",
            new XAttribute("type", "createnpc"),
            new XElement("NPCPrototypeId", _npcPrototypeId),
            new XElement("OnLoadProg", _onLoadProg?.Id ?? 0),
			_unreadableLifecycle is not null ? new XElement(_unreadableLifecycle) : LifecycleMode is { } mode ? new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", mode),
				new XElement("Family", LifecycleFamily), _lifetimeFormula is not null ? new XElement("Seconds", _lifetimeFormula) : null) : null
        );
    }

    public bool IsInstantaneous => true;
    public bool RequiresTarget => true;

    public bool IsCompatibleWithTrigger(IMagicTrigger types)
    {
        return IsCompatibleWithTrigger(types.TargetTypes);
    }

    public static bool IsCompatibleWithTrigger(string types)
    {
        switch (types)
        {
            case "room":
            case "rooms":
                return true;
            default:
                return false;
        }
    }

    public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
        SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
    {
        INPCTemplate template = NPCTemplate;
        if (template is null)
        {
            return null;
        }

        IRoom room = target as IRoom ?? caster.Location;
		var casterLocation = RouteSpatialService.Instance.GetEffectiveLocation(caster);
		var spawnLocation = ReferenceEquals(room, casterLocation.Room)
			? casterLocation
			: CharacterInstanceService.CreateDefaultSpawnLocation(room, RoomLayer.GroundLevel);

		if (LifecycleMode is not null || _loadError is not null)
		{
			if (!TryPrepareApplication(caster, target, outcome, power, TimeSpan.Zero, out var application, out var error))
				throw new InvalidOperationException(error);
			return application!.Create(parent);
		}
		return FinishLoading(template.CreateNewCharacter(spawnLocation), template, caster);
	}

	private IMagicSpellEffect FinishLoading(ICharacter newCharacter, INPCTemplate template, ICharacter caster)
	{
        Gameworld.Add(newCharacter, true);

        template.ApplyTemplateLoadAdditions(newCharacter);
		if (LifecycleMode is not null && newCharacter.State.HasFlag(CharacterState.Dead)) return null;
        template.OnLoadProg?.Execute(newCharacter);
		if (LifecycleMode is not null && newCharacter.State.HasFlag(CharacterState.Dead)) return null;
        _onLoadProg?.Execute(newCharacter, caster, Spell);
		if (LifecycleMode is not null && newCharacter.State.HasFlag(CharacterState.Dead)) return null;

        if (newCharacter.Location.IsSwimmingLayer(newCharacter.RoomLayer) && newCharacter.Race.CanSwim)
        {
            newCharacter.PositionState = PositionSwimming.Instance;
        }
        else if (newCharacter.RoomLayer.IsHigherThan(RoomLayer.GroundLevel) && newCharacter.CanFly().Truth)
        {
            newCharacter.PositionState = PositionFlying.Instance;
        }

        newCharacter.Location.Login(newCharacter);
        newCharacter.HandleEvent(EventType.NPCOnGameLoadFinished, newCharacter);
        return null;
    }

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null; error = DefinitionError;
		if (error is not null) return false;
		if (LifecycleMode is null)
		{ application = new LegacyCreation(this, caster, target, outcome, power); return true; }
		if (Gameworld.SpellOwnedNpcs is null || Spell is not MagicSpell { InvocationGrade: { } grade } native)
		{ error = "Lifecycle NPC creation requires a configured selected-grade native casting invocation."; return false; }
		if (NPCTemplate is not { } template || template.Status != RevisionStatus.Current)
		{ error = "The NPC template is missing or not approved."; return false; }
		if (_preparedSelections.ContainsKey(target) && !TryConfirmPreparedSelection(caster, target, out error)) return false;
		var casterLocation = RouteSpatialService.Instance.GetEffectiveLocation(caster);
		var room = target as IRoom ?? casterLocation.Room;
		var location = ReferenceEquals(room, casterLocation.Room) ? casterLocation : CharacterInstanceService.CreateDefaultSpawnLocation(room, RoomLayer.GroundLevel);
		if (!ReferenceEquals(caster.Gameworld, Gameworld) || !ReferenceEquals(room.Gameworld, Gameworld) ||
			!RouteSpatialService.Instance.TryValidateLocation(location, out error)) return false;
		try
		{
			double? seconds = null;
			if (LifecycleMode != SpellLifecycleMode.Permanent)
			{
				seconds = LifetimeExpression!.EvaluateWith(caster, native.CastingTrait, TraitBonusContext.SpellDuration,
					("power", (int)power), ("outcome", (int)outcome));
				if (!ValidLifetime(seconds.Value, out error)) return false;
			}
			application = new NativeCreation(Guid.NewGuid(), this, template, caster, location, grade, CharacterInstanceIdentityComparer.IdentityId(caster),
				seconds, native.InvocationOriginId);
			return true;
		}
		catch (Exception ex) { error = "NPC lifetime could not be prepared: " + ex.Message; return false; }
	}

	private sealed record LegacyCreation(CreateNPCEffect Effect, ICharacter Caster, IPerceivable Target,
		OpposedOutcomeDegree Outcome, SpellPower Power) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => Effect.GetOrApplyEffect(Caster, Target, Outcome, Power, parent, []);
	}

	private sealed record NativeCreation(Guid LifecycleId, CreateNPCEffect Effect, INPCTemplate Template, ICharacter Caster,
		SpatialLocation Location, int Grade, long CreatorId, double? Seconds, Guid? Invocation) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent)
		{
			var now = RuntimeClock.UtcNow;
			var origin = new SpellLifecycleOrigin(LifecycleId, Effect.Spell.Id, Grade, CreatorId, Effect.LifecycleFamily,
				Effect.LifecycleMode!.Value, now, Seconds is { } seconds ? now.AddSeconds(seconds) : null,
				$"native-createnpc; template={Template.Id}/{Template.RevisionNumber}; invocation={Invocation}; parent={(parent as MagicSpellParent)?.Identity}");
			var character = Template.CreateSpellOwnedCharacter(Location, origin);
			try { return Effect.FinishLoading(character, Template, Caster); }
			catch (Exception ex)
			{
				var store = new SpellOwnedLifecycleStore(); var lifecycle = store.Find(LifecycleId)!;
				if (lifecycle.State != SpellLifecycleState.Completed)
				{
					var diagnostic = "Native on-load callback failed after committed creation; do not replay: " + ex.Message;
					store.Hold(LifecycleId, lifecycle.Version, diagnostic[..Math.Min(diagnostic.Length, 2048)],
						RuntimeClock.UtcNow < lifecycle.UpdatedUtc ? lifecycle.UpdatedUtc : RuntimeClock.UtcNow);
				}
				throw;
			}
		}
	}

    public IMagicSpellEffectTemplate Clone()
    {
        return new CreateNPCEffect(SaveToXml(), Spell);
    }

    #region Implementation of IEditableItem

    public const string HelpText = @"You can use the following options with this effect:

	#3npc <proto>#0 - sets the NPC template to be loaded
	#3prog <which>#0 - sets the NPC on-load program
	#3prog none#0 - clears the on-load program
	#3lifecycle legacy|permanent|temporarycleanup|deathonexpiry#0 - sets explicit creation policy
	#3family <name>#0 - sets the lifecycle family
	#3lifetime <expression>#0 - absolute lifetime in real seconds, independent of control duration";

    public string Show(ICharacter actor)
    {
        return SpellEffectPresentation.Describe(actor, "Create NPC",
            ("Template", NPCTemplate?.EditHeader() ?? "nothing".ColourError()),
            ("On Load", _onLoadProg?.MXPClickableFunctionName() ?? "none".ColourError()),
			("Lifecycle", LifecycleMode?.ToString() ?? "legacy"), ("Family", LifecycleFamily), ("Lifetime Seconds", _lifetimeFormula ?? "none"));
    }

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
			case "lifecycle":
				var modeText = command.SafeRemainingArgument;
				if (modeText.EqualTo("legacy")) { LifecycleMode = null; LifetimeExpression = null; _lifetimeFormula = null; }
				else if (Enum.TryParse<SpellLifecycleMode>(modeText, true, out var mode) && Enum.IsDefined(mode))
				{
					LifecycleMode = mode;
					if (mode == SpellLifecycleMode.Permanent) { LifetimeExpression = null; _lifetimeFormula = null; }
				}
				else { actor.OutputHandler.Send("Specify legacy, permanent, temporarycleanup or deathonexpiry."); return false; }
				_loadError = null; _unreadableLifecycle = null;
				Spell.Changed = true; actor.OutputHandler.Send("NPC lifecycle policy updated."); return true;
			case "family":
				if (string.IsNullOrWhiteSpace(command.SafeRemainingArgument) || command.SafeRemainingArgument.Length > 128)
				{ actor.OutputHandler.Send("Specify a lifecycle family of at most 128 characters."); return false; }
				LifecycleFamily = command.SafeRemainingArgument; Spell.Changed = true;
				actor.OutputHandler.Send("NPC lifecycle family updated."); return true;
			case "lifetime":
				var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
				if (expression.HasErrors()) { actor.OutputHandler.Send("Invalid lifetime expression: " + expression.Error); return false; }
				LifetimeExpression = expression; _lifetimeFormula = command.SafeRemainingArgument; Spell.Changed = true;
				actor.OutputHandler.Send("NPC lifetime expression updated."); return true;
            case "npc":
                return BuildingCommandNPC(actor, command);
            case "prog":
                return BuildingCommandProg(actor, command);
        }

        actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
        return false;
    }

    private bool BuildingCommandProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must either specify a prog to execute when the NPC is loaded or use #3none#0 to clear it.".SubstituteANSIColour());
            return false;
        }

        IFutureProg prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Void, [
            [ProgVariableTypes.Character],
            [ProgVariableTypes.Character, ProgVariableTypes.Character],
            [ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.MagicSpell],
        ]).LookupProg();
        if (prog is null)
        {
            return false;
        }

        _onLoadProg = prog;
        Spell.Changed = true;
        actor.OutputHandler.Send($"This spell effect will now execute the {prog.MXPClickableFunctionName()} prog when loading the NPC.");
        return true;
    }

    private bool BuildingCommandNPC(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which NPC prototype should this spell effect load?");
            return false;
        }

        INPCTemplate proto = Gameworld.NpcTemplates.GetByIdOrName(command.SafeRemainingArgument);
        if (proto is null)
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid NPC prototype.");
            return false;
        }

        if (proto.Status != RevisionStatus.Current)
        {
            actor.OutputHandler.Send($"The NPC prototype {proto.EditHeader()} is not approved for use.");
            return false;
        }

        _npcPrototypeId = proto.Id;
        Spell.Changed = true;
        actor.OutputHandler.Send($"This spell effect will now load the NPC {proto.EditHeader()}.");
        return true;
    }
    #endregion
}
