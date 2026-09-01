#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class BiometricScannerGameItemComponent : AccessControlReaderGameItemComponent, IBiometricScanner
{
	private const int MaximumAuthorisedPeople = 256;
	private readonly List<BiometricAuthorisation> _authorisedPeople = [];
	private BiometricScannerGameItemComponentProto _prototype;

	public BiometricScannerGameItemComponent(BiometricScannerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public BiometricScannerGameItemComponent(MudSharp.Models.GameItemComponent component,
		BiometricScannerGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		foreach (var element in XElement.Parse(component.Definition).Element("AuthorisedPeople")?.Elements("Person") ?? [])
		{
			if (long.TryParse(element.Attribute("id")?.Value, out var id) && id > 0)
			{
				_authorisedPeople.Add(new BiometricAuthorisation(id, element.Attribute("name")?.Value ?? $"#{id:N0}"));
			}
		}
	}

	public BiometricScannerGameItemComponent(BiometricScannerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_authorisedPeople.AddRange(rhs._authorisedPeople);
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public IReadOnlyCollection<BiometricAuthorisation> AuthorisedPeople => _authorisedPeople.AsReadOnly();
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new BiometricScannerGameItemComponent(this, newParent, temporary);

	public override bool DescriptionDecorator(DescriptionType type) => type == DescriptionType.Full;
	public override int DecorationPriority => 1000;
	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags) =>
		$"{description}\n\nIt has a biometric scanner for {_prototype.BodypartShape.Name.ColourName()}s, used with {"access <item> [<severed bodypart>]".ColourCommand()}. It is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())} and {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}.";

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (BiometricScannerGameItemComponentProto)newProto;
	}

	protected override XElement SaveAccessSubtypeToXml(XElement root)
	{
		root.Add(new XElement("AuthorisedPeople", _authorisedPeople.Select(x => new XElement("Person",
			new XAttribute("id", x.CharacterId), new XAttribute("name", x.Name)))));
		return root;
	}

	public bool AddAuthorisedPerson(ICharacter character, out string error)
	{
		var id = CharacterInstanceIdentityComparer.IdentityId(character);
		if (id <= 0)
		{
			error = "That character has no stable identity.";
			return false;
		}
		if (IsAuthorised(id))
		{
			error = "That person is already authorised.";
			return false;
		}
		if (_authorisedPeople.Count >= MaximumAuthorisedPeople)
		{
			error = $"This scanner cannot store more than {MaximumAuthorisedPeople:N0} people.";
			return false;
		}
		_authorisedPeople.Add(new BiometricAuthorisation(id, character.Name));
		Changed = true;
		error = string.Empty;
		return true;
	}

	public bool RemoveAuthorisedPerson(long characterId, out string error)
	{
		var entry = _authorisedPeople.FirstOrDefault(x => x.CharacterId == characterId);
		if (entry is null)
		{
			error = "That person is not authorised.";
			return false;
		}
		_authorisedPeople.Remove(entry);
		Changed = true;
		error = string.Empty;
		return true;
	}

	public bool ClearAuthorisedPeople()
	{
		if (!_authorisedPeople.Any())
		{
			return false;
		}
		_authorisedPeople.Clear();
		Changed = true;
		return true;
	}

	public bool IsAuthorised(long characterId) => _authorisedPeople.Any(x => x.CharacterId == characterId);

	public bool CanScan(ICharacter actor, IGameItem? severedBodypart, out long identityId, out string error)
	{
		identityId = 0L;
		if (!SwitchedOn || !IsPowered)
		{
			error = "The biometric scanner is not powered and ready.";
			return false;
		}
		if (severedBodypart is not null)
		{
			var severed = severedBodypart.GetItemType<ISeveredBodypart>();
			if (severed is null || !severed.Parts.Any(x => x.Shape.Id == _prototype.BodypartShape.Id))
			{
				error = $"That is not a severed bodypart containing a {_prototype.BodypartShape.Name}.";
				return false;
			}
			identityId = severed.OriginalCharacterId;
		}
		else
		{
			var part = actor.Body.ExposedBodyparts.FirstOrDefault(x =>
				x.Shape.Id == _prototype.BodypartShape.Id &&
				actor.Body.CanUseBodypart(x) == CanUseBodypartResult.CanUse);
			if (part is null)
			{
				error = $"You have no exposed, usable {_prototype.BodypartShape.Name} to scan.";
				return false;
			}
			identityId = CharacterInstanceIdentityComparer.IdentityId(actor);
		}
		if (!IsAuthorised(identityId))
		{
			error = "The biometric scanner rejects the presented identity.";
			return false;
		}
		if (!ActivateAccessSignal())
		{
			error = "The biometric scanner does not respond.";
			return false;
		}
		error = string.Empty;
		return true;
	}
}
