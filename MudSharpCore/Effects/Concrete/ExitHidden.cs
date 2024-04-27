using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class ExitHidden : Effect, IExitHiddenEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("ExitHidden", (effect, owner) => new ExitHidden(effect, owner));
	}

	public ExitHidden(IPerceivable owner, IExit exit, IFutureProg applicabilityProg = null) : base(owner,
		applicabilityProg)
	{
		Exit = exit;
	}

	protected ExitHidden(XElement root, IPerceivable owner) : base(root, owner)
	{
		_exitId = long.Parse(root.Element("Effect").Element("Exit").Value);
	}

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Exit", Exit.Id)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		var cellOwner = (ICell)Owner;
		var cellExit = Exit.CellExitFor(cellOwner);
		return $"Exit {Exit.Id.ToString("N0", voyeur)} ({cellExit.OutboundDirectionDescription})";
	}

	protected override string SpecificEffectType => "ExitHidden";
	private long _exitId;
	private IExit _exit;

	public IExit Exit
	{
		get
		{
			if (_exit == null)
			{
				Gameworld.ExitManager.InitialiseCell((ICell)Owner, null);
				_exit = Gameworld.ExitManager.GetExitByID(_exitId);
			}

			return _exit;
		}
		init
		{
			_exit = value;
			_exitId = value.Id;
		}
	}

	public PerceptionTypes HiddenTypes => PerceptionTypes.AllVisual;
}