#nullable enable

using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class KeycardGameItemComponent : GameItemComponent, IKeycard
{
	private readonly List<string> _codes = [];
	private KeycardGameItemComponentProto _prototype;

	public KeycardGameItemComponent(KeycardGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		_codes.AddRange(AccessCredentialUtilities.NormaliseCodes(proto.InitialCodes));
	}

	public KeycardGameItemComponent(MudSharp.Models.GameItemComponent component, KeycardGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_codes.AddRange(AccessCredentialUtilities.NormaliseCodes(
			XElement.Parse(component.Definition).Element("Codes")?.Elements("Code").Select(x => x.Value) ?? []));
	}
	public KeycardGameItemComponent(KeycardGameItemComponent rhs, IGameItem parent, bool temporary = false)
		: base(rhs, parent, temporary)
	{
		_prototype = rhs._prototype;
		_codes.AddRange(rhs._codes);
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public IReadOnlyCollection<string> Codes => _codes.AsReadOnly();
	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new KeycardGameItemComponent(this, newParent, temporary);
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto) =>
		_prototype = (KeycardGameItemComponentProto)newProto;
	protected override string SaveToXml() =>
		new XElement("Definition", new XElement("Codes", _codes.Select(x => new XElement("Code", new XCData(x)))))
			.ToString();

	public bool AddCode(string code, out string error)
	{
		if (!AccessCredentialUtilities.TryNormaliseCode(code, out var value, out error))
		{
			return false;
		}
		if (_codes.Contains(value, StringComparer.Ordinal))
		{
			error = "That code is already on the keycard.";
			return false;
		}
		if (_codes.Count >= AccessCredentialUtilities.MaximumCodes)
		{
			error = "That keycard cannot store any more codes.";
			return false;
		}

		_codes.Add(value);
		Changed = true;
		error = string.Empty;
		return true;
	}

	public bool RemoveCode(string code, out string error)
	{
		if (!AccessCredentialUtilities.TryNormaliseCode(code, out var value, out error))
		{
			return false;
		}
		var existing = _codes.FirstOrDefault(x => x.Equals(value, StringComparison.Ordinal));
		if (existing is null)
		{
			error = "That code is not on the keycard.";
			return false;
		}

		_codes.Remove(existing);
		Changed = true;
		error = string.Empty;
		return true;
	}

	public bool ClearCodes()
	{
		if (!_codes.Any())
		{
			return false;
		}
		_codes.Clear();
		Changed = true;
		return true;
	}

	public bool HasCode(string code) => _codes.Contains(code?.Trim() ?? string.Empty, StringComparer.Ordinal);
}
