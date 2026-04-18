using MudSharp.Health;
using MudSharp.TimeAndDate;
using System.Xml.Linq;

namespace MudSharp.Body.Disfigurements
{
	public enum ScarFreshness
	{
		Fresh,
		Recent,
		Old
	}

	public interface IScar : IDisfigurement
	{
		ScarFreshness Freshness { get; }
		MudDateTime TimeOfScarring { get; }
		int Distinctiveness { get; }
		int SizeSteps { get; }
		bool HasSpecialScarCharacteristicOverride { get; }
		string SpecialScarCharacteristicOverride(bool withForm);
		bool IsSurgical { get; }
		DamageType DamageType { get; }
		WoundSeverity Severity { get; }
		SurgicalProcedureType? SurgicalProcedureType { get; }
		XElement SaveToXml();
	}
}
