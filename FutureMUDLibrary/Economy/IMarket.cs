﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.Economy;
#nullable enable

public interface IMarket : ISaveable, IEditableItem
{
	IEconomicZone EconomicZone { get; }
	string Description { get; }
	IEnumerable<IMarketInfluence> MarketInfluences { get; }
	double PriceMultiplierForCategory(IMarketCategory category);
	void ApplyMarketInfluence(IMarketInfluence influence);
	void RemoveMarketInfluence(IMarketInfluence influence);
}