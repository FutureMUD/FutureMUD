#nullable enable

using MudSharp.Community;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class MilitaryStandardGameItemComponent : GameItemComponent, IMilitaryStandard
{
	private MilitaryStandardGameItemComponentProto _prototype;
	private string? _identityKeyOverride;
	private string? _identityNameOverride;
	private string? _designOverride;
	private MilitaryStandardAssociationType? _associationTypeOverride;
	private string? _associationKeyOverride;
	private string? _associationNameOverride;
	private bool _isPlanted;
	private MilitaryStandardCustodyState _custodyState;
	private int _captureCount;

	public MilitaryStandardGameItemComponent(MilitaryStandardGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public MilitaryStandardGameItemComponent(MudSharp.Models.GameItemComponent component,
		MilitaryStandardGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private MilitaryStandardGameItemComponent(MilitaryStandardGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_identityKeyOverride = rhs._identityKeyOverride;
		_identityNameOverride = rhs._identityNameOverride;
		_designOverride = rhs._designOverride;
		_associationTypeOverride = rhs._associationTypeOverride;
		_associationKeyOverride = rhs._associationKeyOverride;
		_associationNameOverride = rhs._associationNameOverride;
		_isPlanted = false;
		_custodyState = MilitaryStandardCustodyState.Unclaimed;
		_captureCount = 0;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public MilitaryStandardFamily Family => _prototype.Family;
	public string IdentityKey => _identityKeyOverride ?? _prototype.IdentityKey;
	public string IdentityName => _identityNameOverride ?? _prototype.IdentityName;
	public string Design => _designOverride ?? _prototype.Design;
	public MilitaryStandardAssociationType AssociationType =>
		_associationTypeOverride ?? _prototype.AssociationType;
	public string AssociationKey => _associationKeyOverride ?? _prototype.AssociationKey;
	public string AssociationName => _associationNameOverride ?? _prototype.AssociationName;
	public bool IsPlanted => _isPlanted;
	public MilitaryStandardCustodyState CustodyState => _custodyState;
	public int CaptureCount => _captureCount;
	public IReadOnlyCollection<string> SignalPatterns => _prototype.SignalPatterns;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MilitaryStandardGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (MilitaryStandardGameItemComponentProto)newProto;
	}

	private void LoadFromXml(XElement root)
	{
		_identityKeyOverride = EmptyToNull(root.Element("IdentityKeyOverride")?.Value);
		_identityNameOverride = EmptyToNull(root.Element("IdentityNameOverride")?.Value);
		_designOverride = EmptyToNull(root.Element("DesignOverride")?.Value);
		_associationTypeOverride =
			root.Element("AssociationTypeOverride")?.Value.TryParseEnum<MilitaryStandardAssociationType>(
				out var association) == true
				? association
				: null;
		_associationKeyOverride = EmptyToNull(root.Element("AssociationKeyOverride")?.Value);
		_associationNameOverride = EmptyToNull(root.Element("AssociationNameOverride")?.Value);
		_isPlanted = bool.TryParse(root.Element("IsPlanted")?.Value, out var planted) && planted;
		_custodyState =
			root.Element("CustodyState")?.Value.TryParseEnum<MilitaryStandardCustodyState>(out var custody) == true
				? custody
				: MilitaryStandardCustodyState.Unclaimed;
		_captureCount = int.TryParse(root.Element("CaptureCount")?.Value, out var count) ? Math.Max(0, count) : 0;
	}

	private static string? EmptyToNull(string? value)
	{
		return string.IsNullOrWhiteSpace(value) ? null : value;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("IdentityKeyOverride", _identityKeyOverride ?? string.Empty),
			new XElement("IdentityNameOverride", _identityNameOverride ?? string.Empty),
			new XElement("DesignOverride", _designOverride ?? string.Empty),
			new XElement("AssociationTypeOverride", _associationTypeOverride?.ToString() ?? string.Empty),
			new XElement("AssociationKeyOverride", _associationKeyOverride ?? string.Empty),
			new XElement("AssociationNameOverride", _associationNameOverride ?? string.Empty),
			new XElement("IsPlanted", IsPlanted),
			new XElement("CustodyState", CustodyState),
			new XElement("CaptureCount", CaptureCount)).ToString();
	}

	public bool IsAuthorisedBearer(ICharacter actor)
	{
		if (_prototype.CanBearProg is not null)
		{
			return _prototype.CanBearProg.ExecuteBool(false, actor, Parent);
		}

		if (!Parent.HasOwner)
		{
			return false;
		}

		if (Parent.IsOwnedBy(actor))
		{
			return true;
		}

		return Parent.Owner is IClan clan &&
		       actor.ClanMemberships.Any(x => x.Clan == clan);
	}

	public void ReevaluateCustody(ICharacter? actor)
	{
		if (actor is null || IsPlanted)
		{
			return;
		}

		if (!Parent.HasOwner && _prototype.CanBearProg is null)
		{
			TransitionCustody(actor, MilitaryStandardCustodyState.Unclaimed);
			return;
		}

		TransitionCustody(actor,
			IsAuthorisedBearer(actor)
				? MilitaryStandardCustodyState.Friendly
				: MilitaryStandardCustodyState.Captured);
	}

	private void TransitionCustody(ICharacter actor, MilitaryStandardCustodyState newState)
	{
		var oldState = _custodyState;
		if (oldState == newState)
		{
			return;
		}

		if (newState == MilitaryStandardCustodyState.Captured &&
		    oldState != MilitaryStandardCustodyState.Captured)
		{
			_captureCount++;
		}

		_custodyState = newState;
		Changed = true;
		if (newState == MilitaryStandardCustodyState.Captured)
		{
			_prototype.OnCapturedProg?.Execute(actor, Parent, _captureCount);
		}
		else if (newState == MilitaryStandardCustodyState.Friendly &&
		         oldState == MilitaryStandardCustodyState.Captured)
		{
			_prototype.OnRecoveredProg?.Execute(actor, Parent, _captureCount);
		}

		_prototype.OnCustodyChangedProg?.Execute(actor, Parent, oldState.DescribeEnum(), newState.DescribeEnum());
	}

	public bool IsRecognisedBy(ICharacter actor)
	{
		return IsAuthorisedBearer(actor) ||
		       actor.EffectsOfType<RecognisedMilitaryStandard>().Any(x => x.StandardId == Parent.Id);
	}

	public bool Recognise(ICharacter actor)
	{
		if (IsRecognisedBy(actor))
		{
			actor.OutputHandler.Send(
				$"You recognise {Parent.HowSeen(actor)} as {IdentityName.ColourName()}.");
			return true;
		}

		if (_prototype.CanRecogniseProg?.ExecuteBool(false, actor, Parent) == false)
		{
			actor.OutputHandler.Send("You cannot identify that standard.");
			return false;
		}

		var outcome = _prototype.RecognitionTrait is null ||
		              _prototype.RecognitionDifficulty == Difficulty.Automatic
			? Outcome.Pass
			: Gameworld.GetCheck(CheckType.GenericSkillCheck)
			           .Check(actor, _prototype.RecognitionDifficulty, _prototype.RecognitionTrait)
			           .Outcome;
		if (outcome.IsFail())
		{
			actor.OutputHandler.Send("You cannot identify that standard's provenance.");
			return false;
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.RecogniseEmote, actor, actor, Parent)));
		actor.AddEffect(new RecognisedMilitaryStandard(actor, Parent.Id));
		_prototype.OnRecogniseProg?.Execute(actor, Parent);
		actor.OutputHandler.Send($"You identify it as {IdentityName.ColourName()}.\n\n{Design}");
		return true;
	}

	public bool Plant(ICharacter actor, IEmote? playerEmote = null)
	{
		if (IsPlanted)
		{
			actor.OutputHandler.Send($"{Parent.HowSeen(actor, true)} is already planted.");
			return false;
		}

		if (!actor.Body.HeldOrWieldedItems.Contains(Parent))
		{
			actor.OutputHandler.Send($"You must be holding {Parent.HowSeen(actor)} to plant it.");
			return false;
		}

		if (!actor.Body.CanDrop(Parent, 0))
		{
			actor.OutputHandler.Send(actor.Body.WhyCannotDrop(Parent, 0));
			return false;
		}

		actor.Body.Drop(Parent, silent: true);
		_isPlanted = true;
		Changed = true;
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.PlantEmote, actor, actor, Parent)).Append(playerEmote));
		_prototype.OnPlantProg?.Execute(actor, Parent);
		return true;
	}

	public bool TakeUp(ICharacter actor, IEmote? playerEmote = null)
	{
		if (!IsPlanted)
		{
			actor.OutputHandler.Send($"{Parent.HowSeen(actor, true)} is not planted.");
			return false;
		}

		if (Parent.Location != actor.Location || !actor.Body.CanGet(Parent, 0))
		{
			actor.OutputHandler.Send($"You cannot take up {Parent.HowSeen(actor)}.");
			return false;
		}

		actor.Body.Get(Parent, silent: true);
		_isPlanted = false;
		Changed = true;
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.TakeUpEmote, actor, actor, Parent)).Append(playerEmote));
		ReevaluateCustody(actor);
		_prototype.OnTakeUpProg?.Execute(actor, Parent);
		return true;
	}

	public bool Signal(ICharacter actor, string pattern, IEmote? playerEmote = null)
	{
		var actual = SignalPatterns.FirstOrDefault(x => x.EqualTo(pattern));
		if (actual is null)
		{
			actor.OutputHandler.Send($"{Parent.HowSeen(actor, true)} has no visual signal named {pattern.ColourName()}.");
			return false;
		}

		if (!actor.Body.HeldOrWieldedItems.Contains(Parent))
		{
			actor.OutputHandler.Send($"You must be holding {Parent.HowSeen(actor)} to signal with it.");
			return false;
		}

		actor.OutputHandler.Handle(new MixedEmoteOutput(
			new Emote($"@ wave|waves $1 in the {actual} signal.", actor, actor, Parent)).Append(playerEmote));
		_prototype.OnSignalProg?.Execute(actor, Parent, actual);
		return true;
	}

	public void SetIdentityOverride(string key, string name)
	{
		_identityKeyOverride = EmptyToNull(key);
		_identityNameOverride = EmptyToNull(name);
		Changed = true;
	}

	public void SetDesignOverride(string design)
	{
		_designOverride = EmptyToNull(design);
		Changed = true;
	}

	public void SetAssociationOverride(MilitaryStandardAssociationType type, string key, string name)
	{
		_associationTypeOverride = type;
		_associationKeyOverride = EmptyToNull(key);
		_associationNameOverride = EmptyToNull(name);
		Changed = true;
	}

	public void ResetOverrides()
	{
		_identityKeyOverride = null;
		_identityNameOverride = null;
		_designOverride = null;
		_associationTypeOverride = null;
		_associationKeyOverride = null;
		_associationNameOverride = null;
		Changed = true;
	}

	public void SetCustody(MilitaryStandardCustodyState state)
	{
		_custodyState = state;
		Changed = true;
	}

	public void SetCaptureCount(int count)
	{
		_captureCount = Math.Max(0, count);
		Changed = true;
	}

	public void ResetCaptureCount()
	{
		_captureCount = 0;
		Changed = true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.ItemGotten:
			case EventType.ItemGiven:
			case EventType.ItemWielded:
				_isPlanted = false;
				Changed = true;
				ReevaluateCustody(FindCharacter(arguments));
				break;
			case EventType.ItemOwnershipChanged:
				ReevaluateCustody(Parent.InInventoryOf?.Actor ?? FindCharacter(arguments));
				break;
		}

		return base.HandleEvent(type, arguments);
	}

	private static ICharacter? FindCharacter(dynamic[] arguments)
	{
		return arguments.Cast<object>().OfType<ICharacter>().LastOrDefault();
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Short or DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Short && IsPlanted)
		{
			return $"{description} {"(planted)".FluentColour(Telnet.Green, colour)}";
		}

		if (type != DescriptionType.Full)
		{
			return description;
		}

		var identity = voyeur is ICharacter actor && IsRecognisedBy(actor)
			? $"\n\nIt is {IdentityName.ColourName()}. {Design}"
			: "\n\nIts precise identity is not known to you.";
		var association = AssociationType == MilitaryStandardAssociationType.None
			? string.Empty
			: $"\nIt is associated with {AssociationName.ColourName()}.";
		return
			$"{description}{identity}{association}\nIts custody is {CustodyState.DescribeEnum().ColourName()}, it is {(IsPlanted ? "planted" : "not planted")}, and it has been captured {CaptureCount.ToString("N0", voyeur).ColourValue()} time{(CaptureCount == 1 ? "" : "s")}.";
	}
}
