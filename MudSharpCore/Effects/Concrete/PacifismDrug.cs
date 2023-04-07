using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Concrete;

public class PacifismDrug : Effect, IPacifismEffect, IScoreAddendumEffect
{
	public ICharacter CharacterOwner { get; set; }
	public IBody BodyOwner { get; set; }

	public bool IsPeaceful => _intensityPerGramMass > 5.0;
	public bool IsSuperPeaceful => _intensityPerGramMass > 10.0;

	private string StatusDescription
	{
		get
		{
			if (_intensityPerGramMass >= 10.0)
			{
				return "You feel completely and totally at peace; with yourself, with the cosmos, with everything.";
			}

			if (_intensityPerGramMass >= 5.0)
			{
				return "You feel peaceful and whole, with a deep connection to all life and living things.";
			}

			if (_intensityPerGramMass >= 3.0)
			{
				return "You feel relaxed and at ease, like nothing in the world is a problem.";
			}

			if (_intensityPerGramMass >= 2.0)
			{
				return "You feel relaxed and well, with a mild sense of euphoria.";
			}

			if (_intensityPerGramMass >= 1.0)
			{
				return "You feel...good. You're not sure why.";
			}

			return "";
		}
	}

	private double _intensityPerGramMass;

	public double IntensityPerGramMass
	{
		get => _intensityPerGramMass;
		set
		{
			if (value >= 10.0 && _intensityPerGramMass < 10.0)
			{
				Owner?.OutputHandler?.Send(
					"You feel completely and totally at peace; with yourself, with the cosmos, with everything.");
			}
			else if (value >= 5.0 && _intensityPerGramMass < 5.0)
			{
				Owner?.OutputHandler?.Send(
					"You feel peaceful and whole, with a deep connection to all life and living things.");
			}
			else if (value >= 3.0 && _intensityPerGramMass < 3.0)
			{
				Owner?.OutputHandler?.Send("You feel relaxed and at ease, like nothing in the world is a problem.");
			}
			else if (value >= 2.0 && _intensityPerGramMass < 2.0)
			{
				Owner?.OutputHandler?.Send("You feel relaxed and well, with a mild sense of euphoria.");
			}
			else if (value >= 1.0 && _intensityPerGramMass < 1.0)
			{
				Owner?.OutputHandler?.Send("You feel...good. You're not sure why.");
			}
			else if (value < 5.0 && _intensityPerGramMass >= 5.0)
			{
				Owner?.OutputHandler?.Send(
					"You feel your connection to the universe slip away, though you still feel connected to the web of living things.");
			}
			else if (value < 5.0 && _intensityPerGramMass >= 5.0)
			{
				Owner?.OutputHandler?.Send(
					"You feel the connection to the web of living things still itself and then quiet, leaving you calm but a little empty.");
			}
			else if (value < 1.5 && _intensityPerGramMass >= 1.5)
			{
				Owner?.OutputHandler?.Send(
					"Your euphoria is beginning to fade, and though you still feel good you can feel the worries and anxieties of the real world scratching at the edges of your psyche.");
			}
			else if (value < 1.0 && _intensityPerGramMass >= 1.0)
			{
				Owner?.OutputHandler?.Send("You feel normal again, the good feeling now but a memory.");
			}

			_intensityPerGramMass = value;
		}
	}

	public PacifismDrug(IBody owner, double intensity) : base(owner)
	{
		_intensityPerGramMass = intensity;
		BodyOwner = owner;
		CharacterOwner = owner.Actor;
	}

	protected override string SpecificEffectType => "PacifismDrug";

	public bool ShowInScore => _intensityPerGramMass >= 1.0;
	public bool ShowInHealth => ShowInScore;

	public string ScoreAddendum => StatusDescription.Colour(Telnet.BoldBlue);

	public override string Describe(IPerceiver voyeur)
	{
		return $"Has an intensity of {_intensityPerGramMass:N2} of pacifism.";
	}
}