using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Effects.Concrete;

public class AdjacentToExit : Effect, IEffectSubtype, IRemoveOnMovementEffect, IScoreAddendumEffect, IRemoveOnGet
{
	private static TimeSpan? _defaultEffectTimeSpan;

	public static TimeSpan DefaultEffectTimeSpan
	{
		get
		{
			if (_defaultEffectTimeSpan == null)
			{
				_defaultEffectTimeSpan =
					TimeSpan.FromSeconds(
						Futuremud.Games.First().GetStaticDouble("DefaultAdjacentToExitLengthInSeconds"));
			}

			return _defaultEffectTimeSpan.Value;
		}
	}

	public ICellExit Exit { get; set; }

	public AdjacentToExit(IPerceivable owner, ICellExit exit, IFutureProg applicabilityProg = null) : base(owner,
		applicabilityProg)
	{
		Exit = exit;
	}

	protected override string SpecificEffectType => "AdjacentToExit";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Is adjacent to the exit {Exit.DescribeFor(voyeur ?? (IPerceiver)Owner, false).Colour(Telnet.Green)}.";
	}

	public bool ShowInScore => true;

	public bool ShowInHealth => false;

	public string ScoreAddendum =>
		$"You are close to the exit {Exit.DescribeFor((IPerceiver)Owner, false).Colour(Telnet.Yellow)}.";

	bool IRemoveOnMovementEffect.ShouldRemove()
	{
		return true;
	}

	#region Overrides of Effect

	/// <inheritdoc />
	public override bool Applies(object target)
	{
		if (target is ICellExit exit)
		{
			return Exit == exit;
		}

		return base.Applies(target);
	}

	#endregion
}