using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.Effects.Concrete;

public class AdminSpy : Effect, IRemoteObservationEffect
{
	public ICharacter AdminOwner { get; set; }

	public AdminSpy(IPerceivable owner, ICharacter adminOwner, IFutureProg applicabilityProg = null) : base(owner,
		applicabilityProg)
	{
		AdminOwner = adminOwner;
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"{AdminOwner.HowSeen(voyeur, true)} is spying on this location.";
	}

	public void HandleOutput(string text, ILocation location)
	{
		if (location != AdminOwner.Location)
		{
			AdminOwner.OutputHandler.Send($"@[{location.HowSeen(AdminOwner)} ({location.Id.ToStringN0(AdminOwner)})]\r\n{text}");
		}
	}

	public void HandleOutput(IOutput output, ILocation location)
	{
		if (location != AdminOwner.Location)
		{
			AdminOwner.OutputHandler.Send(new PrependOutputWrapper(output,
				$"@[{location.HowSeen(AdminOwner)} ({location.Id.ToStringN0(AdminOwner)})]\r\n"));
		}
	}

	protected override string SpecificEffectType => "AdminSpy";

	#endregion
}