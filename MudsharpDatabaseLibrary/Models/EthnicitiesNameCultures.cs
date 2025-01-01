namespace MudSharp.Models;

public partial class EthnicitiesNameCultures
{
	public long EthnicityId { get; set; }
	public long NameCultureId { get; set; }
	public short Gender { get; set; }

	public virtual Ethnicity Ethnicity { get; set; }
	public virtual NameCulture NameCulture { get; set; }
}