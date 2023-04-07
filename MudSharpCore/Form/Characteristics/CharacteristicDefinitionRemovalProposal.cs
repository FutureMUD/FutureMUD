using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Form.Characteristics;

public class CharacteristicDefinitionRemovalProposal : Proposal, IProposal
{
	protected ICharacteristicDefinition _definition;

	protected ICharacter _proponent;

	public CharacteristicDefinitionRemovalProposal(ICharacter proponent, ICharacteristicDefinition definition)
	{
		_proponent = proponent;
		_definition = definition;
	}

	#region IProposal Members

	public override void Accept(string message = "")
	{
		using (new FMDB())
		{
			_proponent.Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.CharacteristicDefinitions.Find(_definition.Id);
			if (dbitem != null)
			{
				FMDB.Context.CharacteristicDefinitions.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		_proponent.Gameworld.Destroy(_definition);
		foreach (
			var value in _proponent.Gameworld.CharacteristicValues.Where(x => x.Definition == _definition).ToList())
		{
			_proponent.Gameworld.Destroy(value);
		}

		foreach (
			var value in
			_proponent.Gameworld.CharacteristicProfiles.Where(x => x.TargetDefinition == _definition).ToList())
		{
			_proponent.Gameworld.Destroy(value);
		}

		foreach (var ch in _proponent.Gameworld.Bodies.Where(x => x.CharacteristicDefinitions.Contains(_definition))
		        )
		{
			ch.ExpireDefinition(_definition);
		}

		foreach (var comp in _proponent.Gameworld.ItemComponentProtos.OfType<VariableGameItemComponentProto>())
		{
			comp.ExpireDefinition(_definition);
		}

		foreach (var item in _proponent.Gameworld.Items.SelectNotNull(x => x.GetItemType<IVariable>()))
		{
			item.ExpireDefinition(_definition);
		}

		_proponent.OutputHandler.Send("You remove the Characteristic Definition \"" + _definition.Name + "\" ID " +
		                              _definition.Id + " and all its associated profiles and values.");
	}

	public override void Reject(string message = "")
	{
		_proponent.OutputHandler.Send("You decide not to delete Characteristic Definition \"" + _definition.Name +
		                              "\" ID " + _definition.Id + ".");
	}

	public override void Expire()
	{
		_proponent.OutputHandler.Send("You decide not to delete Characteristic Definition \"" + _definition.Name +
		                              "\" ID " + _definition.Id + ".");
	}

	public override string Describe(IPerceiver voyeur)
	{
		return _proponent.HowSeen(voyeur, true) + " is proposing to delete Characteristic Definition \"" +
		       _definition.Name + "\" ID " + _definition.Id + ".";
	}

	#endregion

	#region IKeyworded Members

	protected static List<string> _keywords = new() { "characteristic", "definition", "delete", "remove" };
	public override IEnumerable<string> Keywords => _keywords;

	#endregion
}