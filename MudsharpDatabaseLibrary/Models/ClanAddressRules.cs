#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public class RankFormOfAddress
{
	public Guid Id { get; set; }
	public long RankId { get; set; }
	public int Order { get; set; }
	public string? Text { get; set; }
	public int Relations { get; set; }
	public bool IsPublic { get; set; }
	public long? FutureProgId { get; set; }
	public string? TemplateKey { get; set; }
	public virtual Rank Rank { get; set; } = null!;
	public virtual FutureProg? FutureProg { get; set; }
}

public class AppointmentFormOfAddress
{
	public Guid Id { get; set; }
	public long AppointmentId { get; set; }
	public int Order { get; set; }
	public string? Text { get; set; }
	public int Relations { get; set; }
	public bool IsPublic { get; set; }
	public long? FutureProgId { get; set; }
	public string? TemplateKey { get; set; }
	public virtual Appointment Appointment { get; set; } = null!;
	public virtual FutureProg? FutureProg { get; set; }
}

public partial class Rank
{
	public virtual ICollection<RankFormOfAddress> FormsOfAddress { get; set; } = new List<RankFormOfAddress>();
}

public partial class Appointment
{
	public int AddressPriority { get; set; }
	public virtual ICollection<AppointmentFormOfAddress> FormsOfAddress { get; set; } = new List<AppointmentFormOfAddress>();
}

public partial class Clan
{
	public bool ShowFormsOfAddressInLook { get; set; } = true;
}
