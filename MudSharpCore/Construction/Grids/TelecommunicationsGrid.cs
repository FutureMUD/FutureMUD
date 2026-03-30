#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Construction.Grids;

public class TelecommunicationsGrid : GridBase, ITelecommunicationsGrid
{
	private readonly List<(long ItemId, string Number)> _loadedAssignments = [];
	private readonly Dictionary<ITelephone, string> _phoneNumbers = [];
	private readonly Dictionary<string, ITelephone> _phonesByNumber = new(StringComparer.InvariantCultureIgnoreCase);

	public TelecommunicationsGrid(Models.Grid grid, IFuturemud gameworld) : base(grid, gameworld)
	{
		var root = XElement.Parse(grid.Definition);
		Prefix = root.Element("Prefix")?.Value ?? "555";
		NumberLength = int.Parse(root.Element("NumberLength")?.Value ?? "4");
		NextNumber = long.Parse(root.Element("NextNumber")?.Value ?? "1");
		foreach (var element in root.Elements("Phone"))
		{
			var itemId = long.Parse(element.Attribute("id")!.Value);
			var number = element.Attribute("number")!.Value;
			_loadedAssignments.Add((itemId, number));
		}
	}

	public TelecommunicationsGrid(IFuturemud gameworld, ICell? initialLocation, string prefix, int numberLength) : base(gameworld, initialLocation)
	{
		Prefix = prefix;
		NumberLength = numberLength;
		NextNumber = 1;
	}

	public TelecommunicationsGrid(ITelecommunicationsGrid rhs) : base(rhs)
	{
		Prefix = rhs.Prefix;
		NumberLength = rhs.NumberLength;
		NextNumber = rhs is TelecommunicationsGrid grid ? grid.NextNumber : 1;
	}

	public override string GridType => "Telecommunications";
	public string Prefix { get; }
	public int NumberLength { get; }
	private long NextNumber { get; set; }

	public override void LoadTimeInitialise()
	{
		base.LoadTimeInitialise();
		foreach (var (itemId, number) in _loadedAssignments)
		{
			var phone = Locations.SelectMany(x => x.GameItems).FirstOrDefault(x => x.Id == itemId)
			            ?.GetItemType<ITelephone>();
			if (phone == null)
			{
				continue;
			}

			ConnectPhone(phone, number);
		}

		_loadedAssignments.Clear();
	}

	protected override XElement SaveDefinition()
	{
		var root = base.SaveDefinition();
		root.Add(new XElement("Prefix", Prefix));
		root.Add(new XElement("NumberLength", NumberLength));
		root.Add(new XElement("NextNumber", NextNumber));
		foreach (var (phone, number) in _phoneNumbers.OrderBy(x => x.Value))
		{
			root.Add(new XElement("Phone",
				new XAttribute("id", phone.Parent.Id),
				new XAttribute("number", number)
			));
		}

		return root;
	}

	public void JoinGrid(ITelephone phone)
	{
		if (_phoneNumbers.ContainsKey(phone))
		{
			return;
		}

		if (_loadedAssignments.Any(x => x.ItemId == phone.Parent.Id))
		{
			return;
		}

		RequestNumber(phone, phone.PreferredNumber);
	}

	public void LeaveGrid(ITelephone phone)
	{
		if (phone.IsEngaged)
		{
			phone.EndCall(phone.ConnectedPhone);
		}

		ReleaseNumber(phone);
	}

	public bool TryStartCall(ITelephone caller, string number, out string error)
	{
		error = string.Empty;
		var normalised = Normalise(number);
		if (!_phonesByNumber.TryGetValue(normalised, out var receiver))
		{
			error = "That number is not connected.";
			return false;
		}

		if (receiver == caller)
		{
			error = "You cannot call the same telephone you are using.";
			return false;
		}

		if (!receiver.CanReceiveCalls)
		{
			error = "That line cannot receive calls right now.";
			return false;
		}

		if (receiver.IsEngaged)
		{
			error = "That line is currently busy.";
			return false;
		}

		caller.BeginOutgoingCall(receiver, normalised);
		receiver.ReceiveIncomingCall(caller);
		return true;
	}

	public bool TryResolvePhone(string number, out ITelephone? phone)
	{
		return _phonesByNumber.TryGetValue(Normalise(number), out phone);
	}

	public string? GetPhoneNumber(ITelephone phone)
	{
		return _phoneNumbers.GetValueOrDefault(phone);
	}

	public bool RequestNumber(ITelephone phone, string? preferredNumber)
	{
		ReleaseNumber(phone);
		if (!string.IsNullOrWhiteSpace(preferredNumber))
		{
			var normalised = Normalise(preferredNumber);
			if (!_phonesByNumber.ContainsKey(normalised))
			{
				ConnectPhone(phone, normalised);
				return true;
			}
		}

		var number = GenerateNumber();
		ConnectPhone(phone, number);
		return true;
	}

	public void ReleaseNumber(ITelephone phone)
	{
		if (!_phoneNumbers.Remove(phone, out var number))
		{
			phone.AssignPhoneNumber(null);
			return;
		}

		_phonesByNumber.Remove(number);
		phone.AssignPhoneNumber(null);
		Changed = true;
	}

	private void ConnectPhone(ITelephone phone, string number)
	{
		number = Normalise(number);
		_phoneNumbers[phone] = number;
		_phonesByNumber[number] = phone;
		phone.AssignPhoneNumber(number);
		Changed = true;
	}

	private string GenerateNumber()
	{
		while (true)
		{
			var number = $"{Prefix}{NextNumber.ToString().PadLeft(NumberLength, '0')}";
			NextNumber++;
			if (_phonesByNumber.ContainsKey(number))
			{
				continue;
			}

			return number;
		}
	}

	private static string Normalise(string number)
	{
		return new string(number.Where(char.IsDigit).ToArray());
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Grid #{Id.ToString("N0", actor)}");
		sb.AppendLine($"Type: {"Telecommunications".Colour(Telnet.BoldBlue)}");
		sb.AppendLine($"Locations: {Locations.Count().ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Prefix: {Prefix.ColourValue()}");
		sb.AppendLine($"Subscriber Digits: {NumberLength.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Connected Phones: {_phoneNumbers.Count.ToString("N0", actor).ColourValue()}");
		return sb.ToString();
	}
}
