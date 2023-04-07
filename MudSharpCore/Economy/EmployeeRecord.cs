using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character.Name;

namespace MudSharp.Economy;

public class EmployeeRecord : IEmployeeRecord
{
	public override string ToString()
	{
		return $"Employee Record [{Name.GetName(NameStyle.FullName)}] - Id {EmployeeCharacterId}";
	}

	public long EmployeeCharacterId { get; set; }
	public IPersonalName Name { get; set; }
	public TimeAndDate.MudDateTime EmployeeSince { get; set; }
	public bool ClockedIn { get; set; }
	public bool IsManager { get; set; }
	public bool IsProprietor { get; set; }

	public XElement SaveToXml()
	{
		return new XElement("Record",
			new XElement("Id", EmployeeCharacterId),
			new XElement("Name", Name.SaveToXml()),
			new XElement("NameCulture", Name.Culture.Id),
			new XElement("EmployeeSince", new XCData(EmployeeSince.GetDateTimeString())),
			new XElement("ClockedIn", ClockedIn),
			new XElement("IsManager", IsManager),
			new XElement("IsProprietor", IsProprietor)
		);
	}

	public EmployeeRecord(ICharacter actor)
	{
		EmployeeCharacterId = actor.Id;
		Name = actor.CurrentName;
		EmployeeSince = actor.Location.DateTime();
		ClockedIn = false;
		IsManager = false;
		IsProprietor = false;
	}

	public EmployeeRecord(XElement root, IFuturemud gameworld)
	{
		EmployeeCharacterId = long.Parse(root.Element("Id").Value);
		var nameCulture = gameworld.NameCultures.Get(long.Parse(root.Element("NameCulture").Value));
		Name = new PersonalName(nameCulture, root.Element("Name"));
		EmployeeSince = new TimeAndDate.MudDateTime(root.Element("EmployeeSince").Value, gameworld);
		ClockedIn = bool.Parse(root.Element("ClockedIn").Value);
		IsManager = bool.Parse(root.Element("IsManager").Value);
		IsProprietor = bool.Parse(root.Element("IsProprietor").Value);
	}
}