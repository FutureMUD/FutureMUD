using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Movement;

namespace MudSharp.Effects.Concrete;
public class PreFleeSpeed : Effect, IRemoveOnCombatEnd
{
	/// <inheritdoc />
	public PreFleeSpeed(ICharacter owner, IEnumerable<IMoveSpeed> speeds) : base(owner, null)
	{
		Speeds = speeds;
	}


	/// <inheritdoc />
	public override string Describe(IPerceiver voyeur)
	{
		return $"Remembering to go back to {Speeds.Select(x => x.PresentParticiple).ListToColouredString()} at combat end.";
	}

	/// <inheritdoc />
	protected override string SpecificEffectType => "PreFleeSpeed";

	public IEnumerable<IMoveSpeed> Speeds { get; }

	/// <inheritdoc />
	public override void RemovalEffect()
	{
		var ch = (ICharacter)Owner;
		foreach (var speed in Speeds)
		{
			ch.CurrentSpeeds[speed.Position] = speed;
		}
	}
}
