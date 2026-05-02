#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Magic;

namespace MudSharp.Effects.Interfaces;

public interface IMagicPortalExit
{
	IExit Exit { get; }
	ICell Source { get; }
	ICell Destination { get; }
	ICharacter? Caster { get; }
	IMagicSpell? Spell { get; }
	IEffect? SourceEffect { get; }
	string Verb { get; }
	string OutboundKeyword { get; }
	string InboundKeyword { get; }
}
