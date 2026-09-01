#nullable enable

using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class KeycardScannerGameItemComponent : AccessControlReaderGameItemComponent, IKeycardScanner
{
	private readonly List<string> _acceptedCodes = [];
	private KeycardScannerGameItemComponentProto _prototype;

	public KeycardScannerGameItemComponent(KeycardScannerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
		_acceptedCodes.AddRange(AccessCredentialUtilities.NormaliseCodes(proto.InitialCodes));
	}

	public KeycardScannerGameItemComponent(MudSharp.Models.GameItemComponent component,
		KeycardScannerGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_acceptedCodes.AddRange(AccessCredentialUtilities.NormaliseCodes(XElement.Parse(component.Definition)
			.Element("AcceptedCodes")?.Elements("Code").Select(x => x.Value) ?? []));
	}

	public KeycardScannerGameItemComponent(KeycardScannerGameItemComponent rhs, IGameItem parent,
		bool temporary = false) : base(rhs, parent, temporary)
	{
		_prototype = rhs._prototype;
		_acceptedCodes.AddRange(rhs._acceptedCodes);
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public IReadOnlyCollection<string> AcceptedCodes => _acceptedCodes.AsReadOnly();
	public override int DecorationPriority => 1000;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new KeycardScannerGameItemComponent(this, newParent, temporary);

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (KeycardScannerGameItemComponentProto)newProto;
	}

	protected override XElement SaveAccessSubtypeToXml(XElement root)
	{
		root.Add(new XElement("AcceptedCodes",
			_acceptedCodes.Select(x => new XElement("Code", new XCData(x)))));
		return root;
	}

	public override bool DescriptionDecorator(DescriptionType type) => type == DescriptionType.Full;

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags) =>
		$"{description}\n\nIt has a keycard scanner used with {"access <item> <keycard>".ColourCommand()}. It is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())} and {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}.";

	public bool AddAcceptedCode(string code, out string error)
	{
		if (!AccessCredentialUtilities.TryNormaliseCode(code, out var value, out error))
		{
			return false;
		}
		if (_acceptedCodes.Contains(value, StringComparer.Ordinal))
		{
			error = "That reader already accepts that code.";
			return false;
		}
		if (_acceptedCodes.Count >= AccessCredentialUtilities.MaximumCodes)
		{
			error = "That reader cannot store any more codes.";
			return false;
		}

		_acceptedCodes.Add(value);
		Changed = true;
		error = string.Empty;
		return true;
	}

	public bool RemoveAcceptedCode(string code, out string error)
	{
		if (!AccessCredentialUtilities.TryNormaliseCode(code, out var value, out error))
		{
			return false;
		}
		var existing = _acceptedCodes.FirstOrDefault(x => x.Equals(value, StringComparison.Ordinal));
		if (existing is null)
		{
			error = "That reader does not accept that code.";
			return false;
		}

		_acceptedCodes.Remove(existing);
		Changed = true;
		error = string.Empty;
		return true;
	}

	public bool ClearAcceptedCodes()
	{
		if (!_acceptedCodes.Any())
		{
			return false;
		}
		_acceptedCodes.Clear();
		Changed = true;
		return true;
	}

	public bool AcceptsCode(string code) =>
		_acceptedCodes.Contains(code?.Trim() ?? string.Empty, StringComparer.Ordinal);

	public bool TryCard(IKeycard card, out string error)
	{
		if (!SwitchedOn || !IsPowered)
		{
			error = "The keycard reader is not powered and ready.";
			return false;
		}
		if (!card.Codes.Any(AcceptsCode))
		{
			error = "The keycard reader rejects that card.";
			return false;
		}
		if (!ActivateAccessSignal())
		{
			error = "The keycard reader does not respond.";
			return false;
		}
		error = string.Empty;
		return true;
	}
}
