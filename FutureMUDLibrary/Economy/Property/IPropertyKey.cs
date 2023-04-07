using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Property
{
	public interface IPropertyKey : IFrameworkItem, ISaveable
	{
		IProperty Property { get; set; }
		IGameItem GameItem { get; set; }
		MudDateTime AddedToPropertyOnDate { get; set; }
		decimal CostToReplace { get; set; }
		bool IsReturned { get; set; }
		void Delete();
	}
}
