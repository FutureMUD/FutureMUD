#nullable enable

using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class KeypadGameItemComponent : AccessControlReaderGameItemComponent, IKeypad
{
	private const int MaximumFailedAttemptsBeforeLockout = 5;
	private static readonly TimeSpan FailedAttemptLockoutDuration = TimeSpan.FromSeconds(30);
	private KeypadGameItemComponentProto _prototype;
	private string? _runtimeCode;
	private DateTime? _lockedUntil;
	private int _failedAttempts;

	public KeypadGameItemComponent(KeypadGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public KeypadGameItemComponent(MudSharp.Models.GameItemComponent component, KeypadGameItemComponentProto proto,
		IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_runtimeCode = XElement.Parse(component.Definition).Element("RuntimeCode")?.Value;
	}

	public KeypadGameItemComponent(KeypadGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_runtimeCode = rhs._runtimeCode;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public string Code => _runtimeCode ?? _prototype.Code;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new KeypadGameItemComponent(this, newParent, temporary);

	public override bool DescriptionDecorator(DescriptionType type) => type == DescriptionType.Full;
	public override int DecorationPriority => 1000;

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		return
			$"{description}\n\nIt has an electronic keypad used with {"access <item> <digits>".ColourCommand()} or {"select <item> <digits>".ColourCommand()}. It is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())} and {(IsPowered ? "powered".ColourValue() : "not powered".ColourError())}.";
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (KeypadGameItemComponentProto)newProto;
	}

	protected override XElement SaveAccessSubtypeToXml(XElement root)
	{
		if (_runtimeCode is not null)
		{
			root.Add(new XElement("RuntimeCode", new XCData(_runtimeCode)));
		}
		return root;
	}

	public bool CanSelect(ICharacter character, string argument) => IsNumericCode(argument);

	public bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)
	{
		if (!silent)
		{
			character.OutputHandler.Handle(new MixedEmoteOutput(
				new Emote(_prototype.EntryEmote, character, character, Parent),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
		}
		if (!TryCode(argument, out var error))
		{
			character.Send(error);
			return false;
		}
		return true;
	}

	public bool TryCode(string code, out string error)
	{
		if (!IsNumericCode(code))
		{
			error = "You must enter a numeric code on that keypad.";
			return false;
		}
		if (!SwitchedOn)
		{
			error = "The keypad is switched off.";
			return false;
		}
		if (!IsPowered)
		{
			error = "The keypad does not appear to be powered.";
			return false;
		}
		if (_lockedUntil is not null && _lockedUntil > DateTime.UtcNow)
		{
			error = "The keypad refuses further entries for a short time.";
			return false;
		}
		if (_lockedUntil is not null)
		{
			_lockedUntil = null;
			_failedAttempts = 0;
		}
		if (!code.Trim().Equals(Code, StringComparison.Ordinal))
		{
			_failedAttempts++;
			if (_failedAttempts >= MaximumFailedAttemptsBeforeLockout)
			{
				_failedAttempts = 0;
				_lockedUntil = DateTime.UtcNow + FailedAttemptLockoutDuration;
			}
			error = "Nothing happens.";
			return false;
		}
		_failedAttempts = 0;
		_lockedUntil = null;
		if (!ActivateAccessSignal())
		{
			error = "The keypad does not respond.";
			return false;
		}
		error = string.Empty;
		return true;
	}

	public bool TrySetCode(string code, out string error)
	{
		var value = code?.Trim() ?? string.Empty;
		if (!IsNumericCode(value))
		{
			error = "Keypad codes must contain digits only.";
			return false;
		}
		if (value == Code)
		{
			error = "That is already the keypad code.";
			return false;
		}
		_runtimeCode = value;
		Changed = true;
		error = string.Empty;
		return true;
	}

	private static bool IsNumericCode(string? argument) =>
		!string.IsNullOrWhiteSpace(argument) && argument.Trim().All(char.IsDigit);
}
