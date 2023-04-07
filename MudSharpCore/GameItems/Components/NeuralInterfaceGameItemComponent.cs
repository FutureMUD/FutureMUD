using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class NeuralInterfaceGameItemComponent : ImplantBaseGameItemComponent, IImplantNeuralLink
{
	protected NeuralInterfaceGameItemComponentProto _neuralInterfacePrototype;
	public override IGameItemComponentProto Prototype => _neuralInterfacePrototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_neuralInterfacePrototype = (NeuralInterfaceGameItemComponentProto)newProto;
	}

	#region Constructors

	public NeuralInterfaceGameItemComponent(NeuralInterfaceGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_neuralInterfacePrototype = proto;
	}

	public NeuralInterfaceGameItemComponent(MudSharp.Models.GameItemComponent component,
		NeuralInterfaceGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_neuralInterfacePrototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public NeuralInterfaceGameItemComponent(NeuralInterfaceGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_neuralInterfacePrototype = rhs._neuralInterfacePrototype;
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		_pendingImplantConnections.AddRange(root.Elements("ConnectedImplant").Select(x => long.Parse(x.Value)));
	}


	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new NeuralInterfaceGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		FinaliseLoadTimeConnectedImplants();
		var basevalue = SaveToXmlNoTextConversion();
		basevalue.Add(from implant in _connectedImplants select new XElement("ConnectedImplant", implant.Id));
		return basevalue.ToString();
	}

	#endregion

	#region IImplantNeuralLink Implementation

	public bool PermitsAudio => _neuralInterfacePrototype.PermitsAudio;
	public bool PermitsVisual => _neuralInterfacePrototype.PermitsVisual;

	public void IssueCommand(string alias, string command, StringStack arguments)
	{
		FinaliseLoadTimeConnectedImplants();
		var implant = _connectedImplants.OfType<IImplantRespondToCommands>()
		                                .FirstOrDefault(x => x.AliasForCommands.EqualTo(alias));
		if (implant == null)
		{
			InstalledBody.Actor.OutputHandler.Send("You don't have any implant with that alias to issue a command to.");
			return;
		}

		if (command.EqualToAny("help", "?"))
		{
			InstalledBody.Actor.OutputHandler.Send(implant.CommandHelp);
			return;
		}

		if (command.EqualTo("alias"))
		{
			if (arguments.IsFinished)
			{
				InstalledBody.Actor.OutputHandler.Send("You must specify a new alias to give to that implant.");
				return;
			}

			var newalias = arguments.PopSpeech().ToLowerInvariant();
			if (InstalledBody.Implants.OfType<IImplantRespondToCommands>()
			                 .Any(x => x.AliasForCommands.EqualTo(newalias)))
			{
				InstalledBody.Actor.OutputHandler.Send(
					"You already have an implant with that alias. You must give each implant a unique alias.");
				return;
			}

			implant.AliasForCommands = newalias;
			InstalledBody.Actor.OutputHandler.Send(
				$"You change the alias for your implant {implant.Parent.HowSeen(InstalledBody.Actor)} to {newalias.Colour(Telnet.BoldWhite)}");
		}

		if (!implant.Commands.Any(x => x.EqualTo(command) || x.StartsWith(command)))
		{
			InstalledBody.Actor.OutputHandler.Send(
				$"That is not a valid command for that implant. The valid commands are {implant.Commands.Select(x => x.ColourCommand()).ListToString()}. You can also issue the HELP command to see more information.");
			return;
		}

		implant.IssueCommand(command, arguments);
	}

	private readonly List<long> _pendingImplantConnections = new();
	private readonly List<IImplant> _connectedImplants = new();

	private void FinaliseLoadTimeConnectedImplants()
	{
		foreach (var id in _pendingImplantConnections)
		{
			var implant = InstalledBody.Implants.FirstOrDefault(x => x.Id == id);
			if (implant != null && !_connectedImplants.Contains(implant))
			{
				_connectedImplants.Add(implant);
			}
		}

		_pendingImplantConnections.Clear();
	}

	public void AddLink(IImplant implant)
	{
		FinaliseLoadTimeConnectedImplants();
		if (!_connectedImplants.Contains(implant))
		{
			_connectedImplants.Add(implant);
			Changed = true;
		}
	}

	public void RemoveLink(IImplant implant)
	{
		FinaliseLoadTimeConnectedImplants();
		_connectedImplants.Remove(implant);
		Changed = true;
	}

	public bool IsLinkedTo(IImplant implant)
	{
		FinaliseLoadTimeConnectedImplants();
		return _connectedImplants.Contains(implant);
	}

	public bool DNIConnected => _powered; // TODO - more complex reasons why this would not be so

	public void DoReportStatus()
	{
		FinaliseLoadTimeConnectedImplants();
		var sb = new StringBuilder();
		sb.AppendLine($"Status report for neurally-linked implant {Parent.HowSeen(InstalledBody.Actor)}:");
		foreach (var implant in _connectedImplants.OfType<IImplantReportStatus>())
		{
			// TODO - should implants have their own "internal" sdesc separate to their regular item one?
			var iirtc = implant as IImplantRespondToCommands;
			sb.AppendLine();
			sb.AppendLine(
				$"{implant.Parent.HowSeen(InstalledBody.Actor, true)}{(iirtc?.AliasForCommands != null ? $" [{iirtc.AliasForCommands.Colour(Telnet.BoldWhite)}]" : "")}:");
			sb.Append(implant.ReportStatus());
		}

		InstalledBody.Actor.OutputHandler.Send(sb.ToString());
	}

	#endregion
}