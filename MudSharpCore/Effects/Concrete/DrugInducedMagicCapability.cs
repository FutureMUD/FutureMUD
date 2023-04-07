using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Concrete;

public class DrugInducedMagicCapability : Effect, IGiveMagicCapabilityEffect
{
	public IBody BodyOwner { get; protected set; }

	public DrugInducedMagicCapability(IBody owner) : base(owner)
	{
		BodyOwner = owner;
	}

	public List<IMagicCapability> InducedCapabilities { get; } = new();
	public IEnumerable<IMagicCapability> Capabilities => InducedCapabilities;

	public void NewCapabilities(IEnumerable<IMagicCapability> capabilities)
	{
		var changes = false;
		foreach (var capability in capabilities)
		{
			if (InducedCapabilities.Contains(capability))
			{
				continue;
			}

			AddCapability(capability);
			changes = true;
		}

		foreach (var capability in InducedCapabilities.ToList())
		{
			if (!capabilities.Contains(capability))
			{
				RemoveCapability(capability);
				changes = true;
			}
		}

		if (changes)
		{
			BodyOwner.Actor.CheckResources();
		}
	}

	public void AddCapability(IMagicCapability capability)
	{
		if (!BodyOwner.Actor.Capabilities.Contains(capability))
		{
			BodyOwner.Actor.OutputHandler.Send(
				$"You feel as if you are now {capability.Name.TitleCase().A_An().Colour(capability.School.PowerListColour)}.\nType {$"{capability.School.SchoolVerb} powers".ColourCommand()} to see your powers.");
		}

		InducedCapabilities.Add(capability);
	}

	public void RemoveCapability(IMagicCapability capability)
	{
		InducedCapabilities.Remove(capability);
		if (!BodyOwner.Actor.Capabilities.Contains(capability))
		{
			BodyOwner.Actor.OutputHandler.Send(
				$"You feel as if you are no longer {capability.Name.TitleCase().A_An().Colour(capability.School.PowerListColour)}.");
		}
	}

	public override void RemovalEffect()
	{
		base.RemovalEffect();
		foreach (var capability in Capabilities.ToList())
		{
			RemoveCapability(capability);
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Drug-Induced Magic Capabilities for the following: {InducedCapabilities.Select(x => x.Name.Colour(x.School.PowerListColour)).ListToString()}";
	}

	protected override string SpecificEffectType => "DrugInducedMagicCapability";
}