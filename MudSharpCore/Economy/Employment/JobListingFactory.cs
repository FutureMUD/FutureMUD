using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Economy.Employment;

public static class JobListingFactory
{
	public static IJobListing Load(Models.JobListing listing, IFuturemud gameworld)
	{
		switch (listing.JobListingType)
		{
			case "ongoing":
				return new OngoingJobListing(listing, gameworld);
			default:
				throw new ApplicationException($"Unknown job listing type {listing.JobListingType} in {nameof(Load)}");
		}
	}
}