using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Form.Characteristics;

public class CharacteristicValueRemovalProposal : Proposal, IProposal
{
	protected ICharacter _proponent;
	protected ICharacteristicValue _value;

	public CharacteristicValueRemovalProposal(ICharacter proponent, ICharacteristicValue definition)
	{
		_proponent = proponent;
		_value = definition;
	}

	#region IProposal Members

	public override void Accept(string message = "")
	{
		using (new FMDB())
		{
			_proponent.Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.CharacteristicValues.Find(_value.Id);
			if (dbitem != null)
			{
				FMDB.Context.CharacteristicValues.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		_proponent.Gameworld.Destroy(_value);

		foreach (
			var value in _proponent.Gameworld.CharacteristicProfiles.Where(x => x.ContainsCharacteristic(_value)))
		{
			value.ExpireCharacteristic(_value);
		}

		foreach (var item in _proponent.Gameworld.Items.SelectNotNull(x => x.GetItemType<IVariable>()))
		{
			item.ExpireValue(_value);
		}

		_proponent.OutputHandler.Send("You remove the Characteristic Value \"" + _value.Name + "\" ID " + _value.Id +
		                              " and all its associated profiles.");
	}

	public override void Reject(string message = "")
	{
		_proponent.OutputHandler.Send("You decide not to delete Characteristic Value \"" + _value.Name + "\" ID " +
		                              _value.Id + ".");
	}

	public override void Expire()
	{
		_proponent.OutputHandler.Send("You decide not to delete Characteristic Value \"" + _value.Name + "\" ID " +
		                              _value.Id + ".");
	}

	public override string Describe(IPerceiver voyeur)
	{
		return _proponent.HowSeen(voyeur, true) + " is proposing to delete Characteristic Definition \"" +
		       _value.Name + "\" ID " + _value.Id + ".";
	}

	#endregion

	#region IKeyworded Members

	protected static List<string> _keywords = new() { "characteristic", "value", "delete", "remove" };
	public override IEnumerable<string> Keywords => _keywords;

	#endregion
}