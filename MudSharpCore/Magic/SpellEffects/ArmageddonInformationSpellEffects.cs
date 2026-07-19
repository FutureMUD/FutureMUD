#nullable enable

using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.GameItems;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

internal static class ArmageddonInformationSpellEffectTemplateHelpers
{
	private static readonly ProgVariableTypes[][] IdentifyParameters =
	[
		[ProgVariableTypes.Character],
		[ProgVariableTypes.Character, ProgVariableTypes.Character]
	];

	private static readonly ProgVariableTypes[][] ReciteParameters =
	[
		[ProgVariableTypes.Character],
		[ProgVariableTypes.Character, ProgVariableTypes.Character]
	];

	private static readonly ProgVariableTypes[][] DeadSpeakParameters =
	[
		[ProgVariableTypes.Character],
		[ProgVariableTypes.Character, ProgVariableTypes.Item]
	];

	public static IFutureProg? ResolveIdentifyProg(IFuturemud gameworld, string input, out string error)
	{
		return ResolveProg(gameworld, input, ProgVariableTypes.Text, IdentifyParameters,
			"a text prog accepting (target) or (target, caster)", out error);
	}

	public static IFutureProg? ResolveReciteLinkProg(IFuturemud gameworld, string input, out string error)
	{
		return ResolveProg(gameworld, input, ProgVariableTypes.Character, ReciteParameters,
			"a character prog accepting (caster) or (caster, proxy)", out error);
	}

	public static IFutureProg? ResolveDeadSpeakLinkProg(IFuturemud gameworld, string input, out string error)
	{
		return ResolveProg(gameworld, input, ProgVariableTypes.Character, DeadSpeakParameters,
			"a character prog accepting (caster) or (caster, corpse)", out error);
	}

	public static IFutureProg? LookupIdentifyProg(ICharacter actor, StringStack command)
	{
		return new ProgLookupFromBuilderInput(actor.Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Text,
			IdentifyParameters).LookupProg();
	}

	public static IFutureProg? LookupReciteLinkProg(ICharacter actor, StringStack command)
	{
		return new ProgLookupFromBuilderInput(actor.Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Character,
			ReciteParameters).LookupProg();
	}

	public static IFutureProg? LookupDeadSpeakLinkProg(ICharacter actor, StringStack command)
	{
		return new ProgLookupFromBuilderInput(actor.Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Character,
			DeadSpeakParameters).LookupProg();
	}

	public static bool ProgStillValid(IFutureProg? prog, ProgVariableTypes returnType,
		IEnumerable<IEnumerable<ProgVariableTypes>> parameters)
	{
		return prog?.ReturnType.CompatibleWith(returnType) == true &&
		       parameters.Any(x => prog.MatchesParameters(x));
	}

	public static ICharacter? ResolveReciteLink(IFutureProg prog, ICharacter caster, ICharacter proxy)
	{
		return prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character])
			? prog.Execute(caster, proxy) as ICharacter
			: prog.Execute(caster) as ICharacter;
	}

	public static ICharacter? ResolveDeadSpeakLink(IFutureProg prog, ICharacter caster, IGameItem corpse)
	{
		return prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Item])
			? prog.Execute(caster, corpse) as ICharacter
			: prog.Execute(caster) as ICharacter;
	}

	public static bool BuildingCommandChance(ICharacter actor, IMagicSpell spell, StringStack command,
		Action<double> setter)
	{
		if (command.IsFinished ||
		    !command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value) ||
		    value < 0.0 || value > 1.0)
		{
			actor.OutputHandler.Send(
				$"You must enter a relay chance between {0.ToStringP2Colour(actor)} and {1.ToStringP2Colour(actor)}.");
			return false;
		}

		setter(value);
		spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now relay speech {(value).ToString("P2", actor).ColourValue()} of the time.");
		return true;
	}

	private static IFutureProg? ResolveProg(IFuturemud gameworld, string input, ProgVariableTypes returnType,
		IEnumerable<IEnumerable<ProgVariableTypes>> parameters, string description, out string error)
	{
		error = string.Empty;
		var prog = gameworld.FutureProgs.GetByIdOrName(input);
		if (prog is null)
		{
			error = $"There is no such prog identified by {input.ColourCommand()}.";
			return null;
		}

		if (!prog.ReturnType.CompatibleWith(returnType))
		{
			error = $"You must specify {description}; {prog.FunctionName.ColourName()} returns {prog.ReturnType.Describe().ColourName()}.";
			return null;
		}

		if (!parameters.Any(x => prog.MatchesParameters(x)))
		{
			error = $"You must specify {description}; {prog.FunctionName.ColourName()} has parameters {prog.Parameters.Select(x => x.Describe().ColourName()).ListToString()}.";
			return null;
		}

		return prog;
	}
}

public sealed class IdentifySpellEffect : CharacterSpellEffectTemplateBase
{
	private long _identifyProgId;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("identify", (root, spell) => new IdentifySpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("identify", BuilderFactory,
			"Adds prog-driven lines to LOOK output for the affected viewer.",
			HelpText,
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		if (commands.IsFinished)
		{
			return (null!, "You must specify a text prog accepting (target) or (target, caster).");
		}

		var prog = ArmageddonInformationSpellEffectTemplateHelpers.ResolveIdentifyProg(spell.Gameworld,
			commands.SafeRemainingArgument, out var error);
		if (prog is null)
		{
			return (null!, error);
		}

		return (new IdentifySpellEffect(new XElement("Effect",
			new XAttribute("type", "identify"),
			new XElement("IdentifyProgId", prog.Id)), spell), string.Empty);
	}

	private IdentifySpellEffect(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	private IFutureProg? IdentifyProg => Gameworld.FutureProgs.Get(_identifyProgId);

	protected override string BuilderEffectType => "identify";
	protected override string ShowText => "Identify";

	protected override void LoadFromXml(XElement root)
	{
		_identifyProgId = long.Parse(root.Element("IdentifyProgId")?.Value ?? root.Element("ProgId")?.Value ?? "0");
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("IdentifyProgId", _identifyProgId));
	}

	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		if (IdentifyProg is not { } prog ||
		    !ArmageddonInformationSpellEffectTemplateHelpers.ProgStillValid(prog, ProgVariableTypes.Text,
			    [[ProgVariableTypes.Character], [ProgVariableTypes.Character, ProgVariableTypes.Character]]))
		{
			caster.OutputHandler.Send("This identify effect does not have a valid text prog configured.");
			return null;
		}

		return new SpellIdentifyEffect(target, parent, CharacterInstanceIdentityComparer.IdentityId(caster),
			caster.InstanceId, prog);
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "prog":
			case "identifyprog":
			case "textprog":
				return BuildingCommandProg(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public override string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Identify",
			("Prog", IdentifyProg?.MXPClickableFunctionNameWithId() ?? $"Missing #{_identifyProgId.ToString("N0", actor)}".ColourError()));
	}

	public override IMagicSpellEffectTemplate Clone()
	{
		return new IdentifySpellEffect(SaveToXml(), Spell);
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which text prog should this identify effect use?");
			return false;
		}

		var prog = ArmageddonInformationSpellEffectTemplateHelpers.LookupIdentifyProg(actor, command);
		if (prog is null)
		{
			return false;
		}

		_identifyProgId = prog.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now add LOOK lines from {prog.MXPClickableFunctionNameWithId()}.");
		return true;
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3prog <prog>#0 - sets the text prog; must accept (target) or (target, caster)";
}

public sealed class ReciteProxySpellEffect : CharacterSpellEffectTemplateBase
{
	private long _linkProgId;
	private double _relayChance = 1.0;
	private string _targetEcho = "";
	private string _reciteEcho = ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("reciteproxy",
			(root, spell) => new ReciteProxySpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("reciteproxy", BuilderFactory,
			"Causes a character to recite a linked character's speech.",
			HelpText,
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		if (commands.IsFinished)
		{
			return (null!, "You must specify a character-returning link prog accepting (caster) or (caster, proxy).");
		}

		var prog = ArmageddonInformationSpellEffectTemplateHelpers.ResolveReciteLinkProg(spell.Gameworld,
			commands.SafeRemainingArgument, out var error);
		if (prog is null)
		{
			return (null!, error);
		}

		return (new ReciteProxySpellEffect(new XElement("Effect",
			new XAttribute("type", "reciteproxy"),
			new XElement("LinkProgId", prog.Id),
			new XElement("RelayChance", 1.0),
			new XElement("TargetEcho", new XCData("")),
			new XElement("ReciteEcho", new XCData(ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho))), spell),
			string.Empty);
	}

	private ReciteProxySpellEffect(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	private IFutureProg? LinkProg => Gameworld.FutureProgs.Get(_linkProgId);
	protected override string BuilderEffectType => "reciteproxy";
	protected override string ShowText => "Recite Proxy";

	protected override void LoadFromXml(XElement root)
	{
		_linkProgId = long.Parse(root.Element("LinkProgId")?.Value ?? "0");
		_relayChance = double.Parse(root.Element("RelayChance")?.Value ?? "1.0");
		_targetEcho = root.Element("TargetEcho")?.Value ?? "";
		_reciteEcho = root.Element("ReciteEcho")?.Value ?? ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho;
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(
			new XElement("LinkProgId", _linkProgId),
			new XElement("RelayChance", _relayChance),
			new XElement("TargetEcho", new XCData(_targetEcho)),
			new XElement("ReciteEcho", new XCData(_reciteEcho))
		);
	}

	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		if (LinkProg is not { } prog ||
		    !ArmageddonInformationSpellEffectTemplateHelpers.ProgStillValid(prog, ProgVariableTypes.Character,
			    [[ProgVariableTypes.Character], [ProgVariableTypes.Character, ProgVariableTypes.Character]]))
		{
			caster.OutputHandler.Send("This recite proxy effect does not have a valid character link prog configured.");
			return null;
		}

		var linked = ArmageddonInformationSpellEffectTemplateHelpers.ResolveReciteLink(prog, caster, target);
		if (linked is null)
		{
			caster.OutputHandler.Send("The recite proxy link prog did not return a character.");
			return null;
		}

		if (CharacterInstanceIdentityComparer.SamePhysicalInstance(target, linked))
		{
			caster.OutputHandler.Send("A character cannot recite their own speech through this effect.");
			return null;
		}

		if (!string.IsNullOrWhiteSpace(_targetEcho))
		{
			target.OutputHandler.Send(_targetEcho.SubstituteANSIColour());
		}

		return new SpellReciteProxyEffect(
			target,
			parent,
			CharacterInstanceIdentityComparer.IdentityId(caster),
			caster.InstanceId,
			CharacterInstanceIdentityComparer.IdentityId(linked),
			linked.InstanceId,
			_relayChance,
			_reciteEcho);
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "linkprog":
			case "prog":
				return BuildingCommandLinkProg(actor, command);
			case "chance":
			case "relay":
			case "relaychance":
				return ArmageddonInformationSpellEffectTemplateHelpers.BuildingCommandChance(actor, Spell, command,
					value => _relayChance = value);
			case "targetecho":
			case "target":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "target",
					"", value => _targetEcho = value);
			case "reciteecho":
			case "recite":
			case "echo":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "recite",
					ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho, value => _reciteEcho = value);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public override string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Recite Proxy",
			("Link Prog", LinkProg?.MXPClickableFunctionNameWithId() ?? $"Missing #{_linkProgId.ToString("N0", actor)}".ColourError()),
			("Relay Chance", _relayChance.ToString("P2", actor).ColourValue()),
			("Target Echo", DirectPossessionBuilderHelpers.DescribeEcho(_targetEcho, "")),
			("Recite Echo", DirectPossessionBuilderHelpers.DescribeEcho(_reciteEcho,
				ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho)));
	}

	public override IMagicSpellEffectTemplate Clone()
	{
		return new ReciteProxySpellEffect(SaveToXml(), Spell);
	}

	private bool BuildingCommandLinkProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which character-returning link prog should this recite proxy use?");
			return false;
		}

		var prog = ArmageddonInformationSpellEffectTemplateHelpers.LookupReciteLinkProg(actor, command);
		if (prog is null)
		{
			return false;
		}

		_linkProgId = prog.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now link to speech from {prog.MXPClickableFunctionNameWithId()}.");
		return true;
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3linkprog <prog>#0 - sets the character prog; must accept (caster) or (caster, proxy)
	#3chance <percent>#0 - sets the chance that heard speech is relayed
	#3targetecho <text>|default|none#0 - sets optional text shown to the proxy when the spell lands
	#3reciteecho <text>|default|none#0 - sets the language preamble used when the proxy recites";
}

public sealed class DeadSpeakSpellEffect : IMagicSpellEffectTemplate
{
	private long _linkProgId;
	private bool _allowPcs = true;
	private bool _allowNpcs = true;
	private bool _allowAdmins;
	private bool _allowFinal = true;
	private bool _allowSkeletal;
	private double _relayChance = 1.0;
	private string _targetEcho = ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakTargetEcho;
	private string _roomEcho = ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRoomEcho;
	private string _collapseEcho = ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakCollapseEcho;
	private string _restoreEcho = ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRestoreEcho;
	private string _reciteEcho = ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("deadspeak", (root, spell) => new DeadSpeakSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("deadspeak", BuilderFactory,
			"Animates a corpse as a speech proxy for a linked character.",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "item")
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		if (commands.IsFinished)
		{
			return (null!, "You must specify a character-returning link prog accepting (caster) or (caster, corpse).");
		}

		var prog = ArmageddonInformationSpellEffectTemplateHelpers.ResolveDeadSpeakLinkProg(spell.Gameworld,
			commands.SafeRemainingArgument, out var error);
		if (prog is null)
		{
			return (null!, error);
		}

		return (new DeadSpeakSpellEffect(new XElement("Effect",
			new XAttribute("type", "deadspeak"),
			new XElement("LinkProgId", prog.Id),
			new XElement("AllowPCs", true),
			new XElement("AllowNPCs", true),
			new XElement("AllowAdmins", false),
			new XElement("AllowFinal", true),
			new XElement("AllowSkeletal", false),
			new XElement("RelayChance", 1.0),
			new XElement("TargetEcho", new XCData(ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakTargetEcho)),
			new XElement("RoomEcho", new XCData(ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRoomEcho)),
			new XElement("CollapseEcho", new XCData(ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakCollapseEcho)),
			new XElement("RestoreEcho", new XCData(ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRestoreEcho)),
			new XElement("ReciteEcho", new XCData(ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho))
		), spell), string.Empty);
	}

	private DeadSpeakSpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		_linkProgId = long.Parse(root.Element("LinkProgId")?.Value ?? "0");
		_allowPcs = bool.Parse(root.Element("AllowPCs")?.Value ?? "true");
		_allowNpcs = bool.Parse(root.Element("AllowNPCs")?.Value ?? "true");
		_allowAdmins = bool.Parse(root.Element("AllowAdmins")?.Value ?? "false");
		_allowFinal = bool.Parse(root.Element("AllowFinal")?.Value ?? "true");
		_allowSkeletal = bool.Parse(root.Element("AllowSkeletal")?.Value ?? "false");
		_relayChance = double.Parse(root.Element("RelayChance")?.Value ?? "1.0");
		_targetEcho = root.Element("TargetEcho")?.Value ?? ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakTargetEcho;
		_roomEcho = root.Element("RoomEcho")?.Value ?? ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRoomEcho;
		_collapseEcho = root.Element("CollapseEcho")?.Value ??
		                ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakCollapseEcho;
		_restoreEcho = root.Element("RestoreEcho")?.Value ??
		               ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRestoreEcho;
		_reciteEcho = root.Element("ReciteEcho")?.Value ?? ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	private IFutureProg? LinkProg => Gameworld.FutureProgs.Get(_linkProgId);

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "deadspeak"),
			new XElement("LinkProgId", _linkProgId),
			new XElement("AllowPCs", _allowPcs),
			new XElement("AllowNPCs", _allowNpcs),
			new XElement("AllowAdmins", _allowAdmins),
			new XElement("AllowFinal", _allowFinal),
			new XElement("AllowSkeletal", _allowSkeletal),
			new XElement("RelayChance", _relayChance),
			new XElement("TargetEcho", new XCData(_targetEcho)),
			new XElement("RoomEcho", new XCData(_roomEcho)),
			new XElement("CollapseEcho", new XCData(_collapseEcho)),
			new XElement("RestoreEcho", new XCData(_restoreEcho)),
			new XElement("ReciteEcho", new XCData(_reciteEcho))
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return trigger.TargetTypes == "item";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not IGameItem corpseItem || corpseItem.GetItemType<ICorpse>() is not { } corpse)
		{
			return null;
		}

		if (corpse.OriginalBody is null)
		{
			caster.OutputHandler.Send("That corpse has no original body to speak through.");
			return null;
		}

		if (corpseItem.ContainedIn is not null || corpseItem.InInventoryOf is not null ||
		    corpseItem.TrueLocations.FirstOrDefault() is not { } location)
		{
			caster.OutputHandler.Send("That corpse must be visibly present in a room to speak through.");
			return null;
		}

		if (corpseItem.AffectedBy<ICorpsePossessionEffect>() || corpseItem.AffectedBy<IAnimatedCorpseEffect>())
		{
			caster.OutputHandler.Send("That corpse is already animated by magic.");
			return null;
		}

		if (!_allowFinal && corpse.RepresentsFinalCharacterDeath)
		{
			caster.OutputHandler.Send("This spell is not configured to animate final-death corpses.");
			return null;
		}

		if (!_allowSkeletal && corpse.Decay == DecayState.Skeletal)
		{
			caster.OutputHandler.Send("This spell is not configured to animate skeletal remains.");
			return null;
		}

		if (corpse.OriginalCharacter.IsPlayerCharacter && (!_allowPcs || corpse.OriginalCharacter.IsGuest))
		{
			caster.OutputHandler.Send("This spell is not configured to animate player-character corpses.");
			return null;
		}

		if (!corpse.OriginalCharacter.IsPlayerCharacter && !_allowNpcs)
		{
			caster.OutputHandler.Send("This spell is not configured to animate non-player corpses.");
			return null;
		}

		if (!_allowAdmins && DirectPossessionSecurity.HasProtectedStaffAuthority(corpse.OriginalCharacter))
		{
			caster.OutputHandler.Send("This spell is not configured to animate administrator avatars.");
			return null;
		}

		if (LinkProg is not { } prog ||
		    !ArmageddonInformationSpellEffectTemplateHelpers.ProgStillValid(prog, ProgVariableTypes.Character,
			    [[ProgVariableTypes.Character], [ProgVariableTypes.Character, ProgVariableTypes.Item]]))
		{
			caster.OutputHandler.Send("This dead speak effect does not have a valid character link prog configured.");
			return null;
		}

		var linked = ArmageddonInformationSpellEffectTemplateHelpers.ResolveDeadSpeakLink(prog, caster, corpseItem);
		if (linked is null)
		{
			caster.OutputHandler.Send("The dead speak link prog did not return a character.");
			return null;
		}

		if (!string.IsNullOrWhiteSpace(_targetEcho))
		{
			corpseItem.Handle(_targetEcho.SubstituteANSIColour());
		}

		var originalLayer = corpseItem.RoomLayer;
		var spawn = CharacterInstanceService.SpawnAnimatedCorpseInstance(
			corpse.OriginalCharacter,
			corpse.OriginalBody,
			location,
			originalLayer,
			CharacterInstanceIdentityComparer.IdentityId(caster),
			caster.InstanceId,
			corpseItem.Id,
			Spell.Id,
			Array.Empty<IArtificialIntelligence>());
		if (!spawn.Success || spawn.Instance is not ICharacter animated)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not spawn dead speak actor for corpse #{corpseItem.Id.ToString("N0")}: {spawn.Message}",
				true);
			caster.OutputHandler.Send("The corpse fails to speak.");
			return null;
		}

		location.Extract(corpseItem);
		animated.AddEffect(new CorpseAnimationDispelProxyEffect(animated, corpseItem.Id));
		return new SpellDeadSpeakEffect(
			corpseItem,
			parent,
			CharacterInstanceIdentityComparer.IdentityId(caster),
			caster.InstanceId,
			corpseItem.Id,
			CharacterInstanceIdentityComparer.IdentityId(corpse.OriginalCharacter),
			corpse.OriginalBody.Id,
			animated.InstanceId,
			location.Id,
			(int)originalLayer,
			Spell.Id,
			CharacterInstanceIdentityComparer.IdentityId(linked),
			linked.InstanceId,
			_relayChance,
			CharacterInstancePersistencePolicy.TemporaryEffectBound,
			_roomEcho,
			_collapseEcho,
			_restoreEcho,
			_reciteEcho);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new DeadSpeakSpellEffect(SaveToXml(), Spell);
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "linkprog":
			case "prog":
				return BuildingCommandLinkProg(actor, command);
			case "chance":
			case "relay":
			case "relaychance":
				return ArmageddonInformationSpellEffectTemplateHelpers.BuildingCommandChance(actor, Spell, command,
					value => _relayChance = value);
			case "targetecho":
			case "target":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "target",
					ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakTargetEcho, value => _targetEcho = value);
			case "roomecho":
			case "room":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "room",
					ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRoomEcho, value => _roomEcho = value);
			case "collapseecho":
			case "collapse":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "collapse",
					ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakCollapseEcho, value => _collapseEcho = value);
			case "restoreecho":
			case "restore":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "restore",
					ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRestoreEcho, value => _restoreEcho = value);
			case "reciteecho":
			case "recite":
			case "echo":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "recite",
					ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho, value => _reciteEcho = value);
			case "allowpcs":
			case "pcs":
				_allowPcs = !_allowPcs;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowPcs.NowNoLonger()} allow player-character corpses.");
				return true;
			case "allownpcs":
			case "npcs":
				_allowNpcs = !_allowNpcs;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowNpcs.NowNoLonger()} allow non-player corpses.");
				return true;
			case "allowadmins":
			case "admins":
				_allowAdmins = !_allowAdmins;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowAdmins.NowNoLonger()} allow administrator corpses.");
				return true;
			case "allowfinal":
			case "final":
				_allowFinal = !_allowFinal;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowFinal.NowNoLonger()} allow final-death corpses.");
				return true;
			case "allowskeletal":
			case "skeletal":
				_allowSkeletal = !_allowSkeletal;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowSkeletal.NowNoLonger()} allow skeletal corpses.");
				return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Dead Speak",
			("Link Prog", LinkProg?.MXPClickableFunctionNameWithId() ?? $"Missing #{_linkProgId.ToString("N0", actor)}".ColourError()),
			("Relay Chance", _relayChance.ToString("P2", actor).ColourValue()),
			("Allow PCs", _allowPcs.ToColouredString()),
			("Allow NPCs", _allowNpcs.ToColouredString()),
			("Allow Admins", _allowAdmins.ToColouredString()),
			("Allow Final", _allowFinal.ToColouredString()),
			("Allow Skeletal", _allowSkeletal.ToColouredString()),
			("Target Echo", DirectPossessionBuilderHelpers.DescribeEcho(_targetEcho,
				ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakTargetEcho)),
			("Room Echo", DirectPossessionBuilderHelpers.DescribeEcho(_roomEcho,
				ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRoomEcho)),
			("Collapse Echo", DirectPossessionBuilderHelpers.DescribeEcho(_collapseEcho,
				ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakCollapseEcho)),
			("Restore Echo", DirectPossessionBuilderHelpers.DescribeEcho(_restoreEcho,
				ArmageddonInformationSpellEffectDefaults.DefaultDeadSpeakRestoreEcho)),
			("Recite Echo", DirectPossessionBuilderHelpers.DescribeEcho(_reciteEcho,
				ArmageddonInformationSpellEffectDefaults.DefaultReciteEcho))
		);
	}

	private bool BuildingCommandLinkProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which character-returning link prog should this dead speak effect use?");
			return false;
		}

		var prog = ArmageddonInformationSpellEffectTemplateHelpers.LookupDeadSpeakLinkProg(actor, command);
		if (prog is null)
		{
			return false;
		}

		_linkProgId = prog.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now link dead speech from {prog.MXPClickableFunctionNameWithId()}.");
		return true;
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3linkprog <prog>#0 - sets the character prog; must accept (caster) or (caster, corpse)
	#3chance <percent>#0 - sets the chance that heard speech is relayed through the corpse
	#3targetecho <text>|default|none#0 - sets optional text emitted through the corpse item before animation
	#3roomecho <text>|default|none#0 - sets the room echo when the corpse rises
	#3collapseecho <text>|default|none#0 - sets the room echo when the animation collapses
	#3restoreecho <text>|default|none#0 - sets the room echo when the corpse item is restored
	#3reciteecho <text>|default|none#0 - sets the language preamble used when the corpse recites
	#3allowpcs#0 - toggles whether player-character corpses are allowed
	#3allownpcs#0 - toggles whether non-player corpses are allowed
	#3allowadmins#0 - toggles whether administrator avatars are allowed
	#3allowfinal#0 - toggles whether final-death corpses are allowed
	#3allowskeletal#0 - toggles whether skeletal corpses are allowed";
}
