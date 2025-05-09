﻿using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Merchandise
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ShopId { get; set; }
        public bool AutoReordering { get; set; }
        public decimal AutoReorderPrice { get; set; }
        public decimal BasePrice { get; set; }
        public bool DefaultMerchandiseForItem { get; set; }
        public long ItemProtoId { get; set; }
        public long? PreferredDisplayContainerId { get; set; }
        public string ListDescription { get; set; }
        public int MinimumStockLevels { get; set; }
        public double MinimumStockLevelsByWeight { get; set; }
        public bool PreserveVariablesOnReorder { get; set; }
        public long? SkinId { get; set; }
        public bool WillSell { get; set; }
        public bool WillBuy { get; set; }
        public decimal BaseBuyModifier { get; set; }
        public decimal? SalesMarkupMultiplier { get; set; }
        public double MinimumConditionToBuy { get; set; }
        public int MaximumStockLevelsToBuy { get; set; }
        public bool IgnoreMarketPricing { get; set; }
        public bool PermitItemDecayOnStockedItems { get; set; }

        public virtual GameItem PreferredDisplayContainer { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
