#nullable enable

using System.Globalization;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class CreateShelterEffect : IMagicSpellEffectTemplate, IMagicSpellEffectAdmission, IMagicSpellEffectPreparedSelection
{
	private XElement _definition;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public const string HelpText = @"Shelter effect options:
	#3kind SpringHaven|BurrowRefuge|SandShelter|SeveringRefuge#0
	#3ward school <school>#0 / #3ward tag <invocation tag>#0 - toggle an explicit Severing Refuge selector
	#3ward coverage Incoming|Outgoing|Both#0 / #3ward subschools true|false#0
	#3template <room>#0 - approved indoor room whose overlay supplies the new space
	#3terrain <terrain>#0 - toggle an admitted source terrain
	#3fallback <room>#0 - permanent ground-level recovery destination
	#3lifetime <seconds per grade>#0 - authored real-time duration
	#3capacity <1-128>#0 - physical occupant limit, including offline instances
	#3depth <1-128>#0 - Burrow Refuge's depth below the source in native grid levels
	#3water <item prototype>#0 - Spring Haven's immovable finite container
	#3liquid <liquid>#0 - Spring Haven's initial supply
	#3litres <litres per grade>#0 - one initial fill; no regeneration";

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("createshelter", (root, spell) => new CreateShelterEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("createshelter", (_, spell) =>
			(new CreateShelterEffect(Definition(new(SpellShelterKind.SandShelter, 0, [], 0, 120, 8)), spell), ""),
			"Creates an occupied temporary shelter with exact topology retirement", HelpText, true, true,
			StandaloneSpellEffectTemplateHelper.RoomTriggerTypes);
	}

	public CreateShelterEffect(XElement root, IMagicSpell spell) { Spell = spell; _definition = new(root); }
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new CreateShelterEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => StandaloneSpellEffectTemplateHelper.IsRoomTarget(trigger.TargetTypes);

	public static XElement Definition(SpellShelterConfiguration c) => new("Effect", new XAttribute("type", "createshelter"),
		new XAttribute("version", 1), new XElement("Kind", c.Kind), new XElement("TemplateRoom", c.TemplateRoomId),
		new XElement("FallbackRoom", c.FallbackRoomId), new XElement("SecondsPerGrade", c.SecondsPerGrade),
		new XElement("MaximumOccupants", c.MaximumOccupants), c.AllowedTerrainIds.Order().Select(x => new XElement("Terrain", x)),
		new XElement("WaterPrototype", c.WaterPrototypeId), new XElement("Liquid", c.LiquidId), new XElement("LitresPerGrade", c.LitresPerGrade),
		new XElement("UndergroundDepth", c.UndergroundDepth), c.Ward?.Save());

	internal SpellShelterConfiguration Configuration()
	{
		if ((string?)_definition.Attribute("version") != "1") throw new FormatException("Unsupported shelter effect version.");
		if (_definition.Elements().Where(x => x.Name != "Terrain").GroupBy(x => x.Name).Any(x => x.Count() != 1) ||
			_definition.Elements().Any(x => !new[] { "Kind", "TemplateRoom", "FallbackRoom", "SecondsPerGrade", "MaximumOccupants", "Terrain", "WaterPrototype", "Liquid", "LitresPerGrade", "UndergroundDepth", "Ward" }.Contains(x.Name.LocalName)))
			throw new FormatException("Unknown or duplicate shelter configuration fields.");
		return new(Enum.Parse<SpellShelterKind>(_definition.Element("Kind")!.Value), (long)_definition.Element("TemplateRoom")!,
			_definition.Elements("Terrain").Select(x => (long)x).Distinct().ToArray(), (long)_definition.Element("FallbackRoom")!,
			(double)_definition.Element("SecondsPerGrade")!, (int)_definition.Element("MaximumOccupants")!,
			(long?)_definition.Element("WaterPrototype") ?? 0, (long?)_definition.Element("Liquid") ?? 0,
			(double?)_definition.Element("LitresPerGrade") ?? 0, (int?)_definition.Element("UndergroundDepth") ?? 1,
			_definition.Element("Ward") is { } ward ? SpellShelterWardConfiguration.Load(ward) : null);
	}

	public string? DefinitionError
	{
		get { try { Configuration(); return null; } catch (Exception ex) { return "Invalid shelter configuration: " + ex.Message; } }
	}

	private bool Validate(ICharacter caster, IPerceivable target, out SpellShelterConfiguration? c, out int grade, out string? error)
	{
		c = null; grade = 0; error = DefinitionError;
		if (error is not null) return false;
		if (target is not IRoom anchor || Spell is not MagicSpell { InvocationGrade: { } selected } || Gameworld.SpellOwnedShelters is null)
		{ error = "Shelter creation needs a selected-grade casting invocation targeting the creator's room."; return false; }
		grade = selected; c = Configuration();
		error = Gameworld.SpellOwnedShelters.AdmissionError(caster, anchor, c, grade);
		return error is null;
	}

	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error);
			return existing;
		}
		if (!Validate(caster, recipient, out var c, out var grade, out var diagnostic)) throw new InvalidOperationException(diagnostic);
		var anchor = (IRoom)recipient;
		var template = Gameworld.Rooms.Get(c!.TemplateRoomId) ?? throw new InvalidOperationException("The admitted shelter template disappeared.");
		var fallback = Gameworld.Rooms.Get(c.FallbackRoomId) ?? throw new InvalidOperationException("The admitted shelter fallback disappeared.");
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var anchorOverlay = anchor.CurrentOverlay; var templateOverlay = template.CurrentOverlay;
		var position = caster.RoutePositionMetres; var layer = caster.RoomLayer; var instance = caster.InstanceId;
		var templateDefinition = OverlayDefinition(templateOverlay);
		var anchorDefinition = OverlayDefinition(anchorOverlay);
		var waterPrototype = Gameworld.ItemProtos.Get(c.WaterPrototypeId);
		var waterComponent = waterPrototype?.Components.SingleOrDefault() as LiquidContainerGameItemComponentProto;
		var waterCapacity = waterComponent?.LiquidCapacity;
		var liquid = Gameworld.Liquids.Get(c.LiquidId);
		var conversion = Gameworld.UnitManager.BaseFluidToLitres;
		var selection = new Selection(caster, recipient, c, grade, definition, () =>
			ReferenceEquals(caster.Location, anchor) && caster.InstanceId == instance && caster.RoutePositionMetres == position &&
			caster.RoomLayer == layer && ReferenceEquals(anchor.CurrentOverlay, anchorOverlay) && OverlayDefinition(anchorOverlay) == anchorDefinition &&
			ReferenceEquals(template.CurrentOverlay, templateOverlay) && OverlayDefinition(templateOverlay) == templateDefinition &&
			ReferenceEquals(Gameworld.Rooms.Get(c.FallbackRoomId), fallback) &&
			(c.Kind != SpellShelterKind.SpringHaven || ReferenceEquals(Gameworld.ItemProtos.Get(c.WaterPrototypeId), waterPrototype) &&
				ReferenceEquals(waterPrototype!.Components.SingleOrDefault(), waterComponent) && waterComponent!.LiquidCapacity == waterCapacity &&
				ReferenceEquals(Gameworld.Liquids.Get(c.LiquidId), liquid) && Gameworld.UnitManager.BaseFluidToLitres == conversion) &&
			SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		_selections.Add(recipient, selection);
		return selection;
	}

	private static string OverlayDefinition(IRoomOverlay overlay) =>
		$"{overlay.Id}/{overlay.Terrain.Id}/{overlay.Package.Id}/{overlay.Package.RevisionNumber}/{overlay.Package.Status}/" +
		$"{overlay.OutdoorsType}/{overlay.RoomName}/{overlay.RoomDescription}/{overlay.Atmosphere?.Id}/{overlay.AmbientLightFactor:R}/{overlay.AddedLight:R}/{overlay.HearingProfile?.Id}";

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared shelter frame or native bindings changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Recipient, recipient) ||
			!token.IsCurrent || SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition) return false;
		if (!Validate(caster, recipient, out _, out var grade, out error) || grade != token.Grade) return false;
		_selections[recipient] = token; return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared shelter frame or native bindings changed.";
		return _selections.TryGetValue(recipient, out var token) && ReferenceEquals(token.Caster, caster) && token.IsCurrent &&
			Validate(caster, recipient, out _, out var grade, out error) && grade == token.Grade;
	}

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		if (!Validate(caster, target, out var c, out var grade, out error)) return false;
		if (_selections.ContainsKey(target) && !TryConfirmPreparedSelection(caster, target, out error)) return false;
		application = new Creation(Guid.NewGuid(), this, caster, (IRoom)target, c!, grade); return true;
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null || !TryPrepareApplication(caster, target, outcome, power, TimeSpan.Zero, out var application, out var error))
			throw new InvalidOperationException("Shelter application was not admitted.");
		return application!.Create(parent);
	}

	private sealed record Selection(ICharacter Caster, IPerceivable Recipient, SpellShelterConfiguration Configuration,
		int Grade, string Definition, Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{ public bool IsCurrent => Check(); }

	private sealed record Creation(Guid Id, CreateShelterEffect Effect, ICharacter Caster, IRoom Anchor,
		SpellShelterConfiguration Configuration, int Grade) : IMagicSpellEffectApplicationOperation
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => Apply(parent).Effect!;
		public MagicEffectOperation Apply(IMagicSpellEffectParent parent)
		{
			if (Effect._selections.ContainsKey(Anchor) && !Effect.TryConfirmPreparedSelection(Caster, Anchor, out var error))
				throw new InvalidOperationException(error);
			var now = RuntimeClock.UtcNow;
			var metadata = new SpellShelterAnchor(Configuration.Kind, Anchor.Id, Configuration.FallbackRoomId,
				Caster.RoutePositionMetres, Configuration.MaximumOccupants, Anchor.CurrentOverlay.Id, Configuration.Ward);
			Effect.Gameworld.SpellOwnedShelters!.Create(Caster, Anchor, Configuration,
				new SpellLifecycleOrigin(Id, Effect.Spell.Id, Grade, CharacterInstanceIdentityComparer.IdentityId(Caster),
					SpellShelterAnchor.Family, SpellLifecycleMode.TemporaryCleanup, now,
					now.AddSeconds(Configuration.SecondsPerGrade * Grade), metadata.Save()));
			return new(MagicEffectOperationStatus.Applied, null);
		}
	}

	public string Show(ICharacter actor)
	{
		if (DefinitionError is { } error) return error.ColourError();
		var c = Configuration();
		return $"{c.Kind.DescribeEnum().ColourName()} using room {c.TemplateRoomId.ToString("N0", actor).ColourValue()}, " +
			$"fallback {c.FallbackRoomId.ToString("N0", actor).ColourValue()}, capacity {c.MaximumOccupants.ToString("N0", actor).ColourValue()}, " +
			$"lifetime {TimeSpan.FromSeconds(c.SecondsPerGrade).Describe(actor).ColourValue()} per grade; " +
			$"admitted terrains {c.AllowedTerrainIds.Select(x => x.ToString("N0", actor)).ListToCommaSeparatedValues().ColourValue()}. " +
			(c.Ward is { } ward ? $"Ward: {ward.Coverage.DescribeEnum().ColourName()}, schools {ward.SchoolIds.Select(x => x.ToString("N0", actor)).ListToCommaSeparatedValues().ColourValue()}, invocation tags {ward.Tags.ListToCommaSeparatedValues().ColourValue()}, subschools {ward.IncludesSubschools.ToColouredString()}. " : "") +
			(c.Kind == SpellShelterKind.BurrowRefuge ? $"Depth: {c.UndergroundDepth.ToString("N0", actor).ColourValue()} native grid levels. " : "") +
			(c.Kind == SpellShelterKind.SpringHaven ? $"Finite supply: prototype {c.WaterPrototypeId.ToString("N0", actor)}, liquid {c.LiquidId.ToString("N0", actor)}, {c.LitresPerGrade.ToString("N2", actor)} litres per grade." : "");
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PeekSpeech().EqualTo("ward")) return BuildingCommandWard(actor, command);
		var field = command.PopSpeech().ToLowerInvariant(); var text = command.SafeRemainingArgument;
		string? name = null; object? value = null;
		switch (field)
		{
			case "kind":
				if (!Enum.TryParse<SpellShelterKind>(text, true, out var kind) || !Enum.IsDefined(kind)) break;
				name = "Kind"; value = kind;
				if (kind != SpellShelterKind.SeveringRefuge) _definition.Element("Ward")?.Remove();
				if (kind != SpellShelterKind.SpringHaven)
				{ _definition.SetElementValue("WaterPrototype", 0); _definition.SetElementValue("Liquid", 0); _definition.SetElementValue("LitresPerGrade", 0); }
				break;
			case "template": case "fallback":
				var room = Gameworld.Rooms.GetByIdOrName(text);
				if (room is null) break;
				name = field == "template" ? "TemplateRoom" : "FallbackRoom"; value = room.Id; break;
			case "terrain":
				var terrain = Gameworld.Terrains.GetByIdOrName(text); if (terrain is null) break;
				var match = _definition.Elements("Terrain").SingleOrDefault(x => (long)x == terrain.Id);
				if (match is null) _definition.Add(new XElement("Terrain", terrain.Id)); else match.Remove();
				Spell.Changed = true; actor.OutputHandler.Send("Source terrain admission updated."); return true;
			case "water":
				var proto = Gameworld.ItemProtos.GetByIdOrName(text); if (proto is null) break;
				name = "WaterPrototype"; value = proto.Id; break;
			case "liquid":
				var liquid = Gameworld.Liquids.GetByIdOrName(text); if (liquid is null) break;
				name = "Liquid"; value = liquid.Id; break;
			case "depth":
				if (!int.TryParse(text, out var depth) || depth is < 1 or > 128) break;
				name = "UndergroundDepth"; value = depth; break;
			case "capacity":
				if (!int.TryParse(text, out var count) || count is < 1 or > 128) break;
				name = "MaximumOccupants"; value = count; break;
			case "lifetime": case "litres":
				if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) || !double.IsFinite(number) || number <= 0) break;
				name = field == "lifetime" ? "SecondsPerGrade" : "LitresPerGrade"; value = number; break;
		}
		if (name is null) { actor.OutputHandler.Send(HelpText.SubstituteANSIColour()); return false; }
		_definition.SetElementValue(name, value); Spell.Changed = true;
		actor.OutputHandler.Send("Shelter configuration updated."); return true;
	}

	private bool BuildingCommandWard(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		var field = command.PopSpeech().ToLowerInvariant(); var text = command.SafeRemainingArgument;
		if ((string?)_definition.Element("Kind") != nameof(SpellShelterKind.SeveringRefuge))
		{ actor.OutputHandler.Send("Only Severing Refuge owns this ward.".ColourError()); return false; }
		var ward = _definition.Element("Ward") is { } existing ? new XElement(existing) :
			new XElement("Ward", new XAttribute("version", 1), new XElement("Coverage", MagicInterdictionCoverage.Both), new XElement("IncludesSubschools", true));
		switch (field)
		{
			case "school":
				var school = Gameworld.MagicSchools.GetByIdOrName(text); if (school is null) return false;
				var selectedSchool = ward.Elements("School").SingleOrDefault(x => (long)x == school.Id);
				if (selectedSchool is null) ward.Add(new XElement("School", school.Id)); else selectedSchool.Remove(); break;
			case "tag":
				if (string.IsNullOrWhiteSpace(text) || text.Length > 128) return false;
				var selectedTag = ward.Elements("Tag").SingleOrDefault(x => x.Value.EqualTo(text));
				if (selectedTag is null) ward.Add(new XElement("Tag", text)); else selectedTag.Remove(); break;
			case "coverage":
				if (!Enum.TryParse<MagicInterdictionCoverage>(text, true, out var coverage) || !Enum.IsDefined(coverage)) return false;
				ward.SetElementValue("Coverage", coverage); break;
			case "subschools":
				if (!bool.TryParse(text, out var children)) return false;
				ward.SetElementValue("IncludesSubschools", children); break;
			default: actor.OutputHandler.Send(HelpText.SubstituteANSIColour()); return false;
		}
		try { SpellShelterWardConfiguration.Load(ward); }
		catch (ArgumentException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); return false; }
		_definition.Element("Ward")?.Remove(); _definition.Add(ward); Spell.Changed = true;
		actor.OutputHandler.Send("Shelter ward configuration updated."); return true;
	}
}
