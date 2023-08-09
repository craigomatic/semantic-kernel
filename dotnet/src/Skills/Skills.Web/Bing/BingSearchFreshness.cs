// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Globalization;

namespace Microsoft.SemanticKernel.Skills.Web.Bing;

public sealed class BingSearchFreshness
{
    public DateTime? RangeStart { get; set; }

    public DateTime? RangeEnd { get; set; }

    public BingSearchFreshnessType Type { get; set; } = BingSearchFreshnessType.Default;

    public override string ToString()
    {
        switch (this.Type)
        {
            case BingSearchFreshnessType.Day:
                return "freshness=day";
            case BingSearchFreshnessType.Week:
                return "freshness=week";
            case BingSearchFreshnessType.Month:
                return "freshness=month";
            case BingSearchFreshnessType.DateRange:
                return $"freshness={this.RangeStart?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}..{this.RangeEnd?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
        }

        return string.Empty;
    }
}

public enum BingSearchFreshnessType
{
    Default,
    DateRange,
    Day,
    Week,
    Month
}
