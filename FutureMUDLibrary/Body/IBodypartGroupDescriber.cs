﻿using System.Collections.Generic;
using MudSharp.Body.Grouping;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.Body {
	/// <summary>
	///     An IBodypartGroupDescriber is used to
	/// </summary>
	public interface IBodypartGroupDescriber : IEditableItem, ISaveable {
		string Comment { get; }
		string DescribedAs { get; }
		IBodyPrototype BodyPrototype { get; }
		BodypartGroupResult Match(IEnumerable<IBodypart> parts);
		void FinaliseLoad(Models.BodypartGroupDescriber describer, IFuturemud gameworld);
		IBodypartGroupDescriber Clone();
	}
}