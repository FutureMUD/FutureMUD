#nullable enable

using MudSharp.Character;

namespace MudSharp.Magic.Powers;

public abstract partial class MagicPowerBase
{
	private readonly HashSet<string> _powerTags = new(StringComparer.Ordinal);
	// The existing explicit classification contributes its native tag, never a school/name inference.
	public IReadOnlyCollection<string> PowerTags => _powerTags.Concat(IsPsionic ? ["psionic"] : Array.Empty<string>())
		.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
	internal static bool ValidPowerTag(string tag) => tag.Length is >= 1 and <= 64 &&
		tag.All(x => char.IsAsciiLetterOrDigit(x) || x is '_' or '-' or '.' or ':');
	private void LoadPowerTags(XElement root)
	{
		var tags = root.Element("PowerTags")?.Elements("Tag").Select(x => x.Value.Trim().ToLowerInvariant()).ToArray() ?? [];
		if (tags.Length > 64 || tags.Any(x => !ValidPowerTag(x)) || tags.Distinct(StringComparer.Ordinal).Count() != tags.Length)
			throw new FormatException("Power tags must be distinct bounded native identifiers.");
		_powerTags.UnionWith(tags);
	}
	private bool TagsAllowInvocation(ICharacter actor) => !actor.Effects.OfType<IMagicPowerTagSuppression>()
		.Any(effect => effect.IsActiveFor(actor) && effect.SuppressedPowerTags.Intersect(PowerTags, StringComparer.Ordinal).Any());

	/// <summary>One ordinary activation admission: explicit tag policy, the existing prog, then tag recheck.</summary>
	public bool CanInvokePower(ICharacter actor, params object?[] arguments)
	{
		if (!IsPsionic && MudSharp.Construction.RoomPolicyRules.MagicDenial(actor, arguments.OfType<IPerceivable>()) is not null) return false;
		if (!TagsAllowInvocation(actor) || !BodilyCostReady(actor)) return false;
		var parameters = new object?[] { actor }.Concat(arguments).ToArray();
		return CanInvokePowerProg?.ExecuteBool(parameters) != false && TagsAllowInvocation(actor) &&
		       (IsPsionic || MudSharp.Construction.RoomPolicyRules.MagicDenial(actor, arguments.OfType<IPerceivable>()) is null);
	}
	private bool BuildingCommandPowerTag(ICharacter actor, StringStack command)
	{
		var tag = command.PopSpeech().Trim().ToLowerInvariant();
		if (!command.IsFinished || !ValidPowerTag(tag))
		{ actor.OutputHandler.Send("Specify one power tag of 1-64 letters, digits, underscores, hyphens, dots or colons."); return false; }
		if (tag == "psionic" && IsPsionic)
		{ actor.OutputHandler.Send("The explicit psionic classification supplies that tag. Use the psionic setting to change it."); return false; }
		if (!_powerTags.Remove(tag))
		{
			if (_powerTags.Count >= 64) { actor.OutputHandler.Send("This power already has 64 explicit tags."); return false; }
			_powerTags.Add(tag);
		}
		Changed = true; actor.OutputHandler.Send("The explicit power tag has been toggled."); return true;
	}
}
