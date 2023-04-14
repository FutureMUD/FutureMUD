using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models;

public class GPTThread
{
	public GPTThread()
	{
		Messages = new HashSet<GPTMessage>();
	}

	public long Id { get; set; }
	public string Name { get; set; }
	public string Prompt { get; set; }
	public string Model { get; set; }
	public double Temperature { get; set; }
	public virtual ICollection<GPTMessage> Messages { get; set; }
}