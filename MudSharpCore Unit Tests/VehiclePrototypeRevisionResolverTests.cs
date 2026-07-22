#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Commands.Modules;
using MudSharp.Framework.Revision;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehiclePrototypeRevisionResolverTests
{
	[TestMethod]
	public void ResolveForApproval_CurrentAndPending_ReturnsNewestPendingRevision()
	{
		var revisions = new[]
		{
			CreatePrototype(10, 1, "Test Train", RevisionStatus.Current),
			CreatePrototype(10, 2, "Test Train", RevisionStatus.PendingRevision),
			CreatePrototype(10, 3, "Test Train", RevisionStatus.PendingRevision)
		};

		var result = VehicleModule.ResolveVehiclePrototypeRevisionForApproval(revisions, "10");

		Assert.IsNotNull(result);
		Assert.AreEqual(3, result.RevisionNumber);
		Assert.AreEqual(RevisionStatus.PendingRevision, result.Status);
	}

	[TestMethod]
	public void ResolveForApproval_OnlyCurrent_ReturnsNull()
	{
		var revisions = new[]
		{
			CreatePrototype(10, 1, "Test Train", RevisionStatus.Current)
		};

		var result = VehicleModule.ResolveVehiclePrototypeRevisionForApproval(revisions, "Test Train");

		Assert.IsNull(result);
	}

	[TestMethod]
	public void ResolveForApproval_ExactNameIsCaseInsensitive_ReturnsPendingRevision()
	{
		var pending = CreatePrototype(10, 2, "Test Train", RevisionStatus.PendingRevision);

		var result = VehicleModule.ResolveVehiclePrototypeRevisionForApproval([pending], "test train");

		Assert.AreSame(pending, result);
	}

	[TestMethod]
	public void ResolveForApproval_NamePrefixOnly_ReturnsNull()
	{
		var revisions = new[]
		{
			CreatePrototype(10, 2, "Test Train", RevisionStatus.PendingRevision)
		};

		var result = VehicleModule.ResolveVehiclePrototypeRevisionForApproval(revisions, "Test");

		Assert.IsNull(result);
	}

	[TestMethod]
	public void ResolveForApproval_AmbiguousExactNameAcrossIds_ReturnsNull()
	{
		var revisions = new[]
		{
			CreatePrototype(10, 2, "Test Train", RevisionStatus.PendingRevision),
			CreatePrototype(20, 4, "Test Train", RevisionStatus.PendingRevision)
		};

		var result = VehicleModule.ResolveVehiclePrototypeRevisionForApproval(revisions, "Test Train");

		Assert.IsNull(result);
	}

	[TestMethod]
	public void ResolveForEditing_CurrentAndEditableRevisions_ReturnsNewestEditableRevision()
	{
		var revisions = new[]
		{
			CreatePrototype(10, 1, "Test Train", RevisionStatus.Current),
			CreatePrototype(10, 2, "Test Train", RevisionStatus.UnderDesign),
			CreatePrototype(10, 3, "Test Train", RevisionStatus.Rejected)
		};

		var result = VehicleModule.ResolveVehiclePrototypeRevisionForEditing(revisions, "10");

		Assert.IsNotNull(result);
		Assert.AreEqual(3, result.RevisionNumber);
		Assert.AreEqual(RevisionStatus.Rejected, result.Status);
	}

	[TestMethod]
	public void ResolveForEditing_OnlyCurrent_ReturnsCurrentForRevisionCloning()
	{
		var current = CreatePrototype(10, 1, "Test Train", RevisionStatus.Current);

		var result = VehicleModule.ResolveVehiclePrototypeRevisionForEditing([current], "Test Train");

		Assert.AreSame(current, result);
	}

	[TestMethod]
	public void ResolveForEditing_PendingRevisionTakesPriorityOverCurrentRevision()
	{
		var current = CreatePrototype(10, 1, "Test Train", RevisionStatus.Current);
		var pending = CreatePrototype(10, 2, "Test Train", RevisionStatus.PendingRevision);

		var result = VehicleModule.ResolveVehiclePrototypeRevisionForEditing([current, pending], "10");

		Assert.AreSame(pending, result);
	}

	[TestMethod]
	public void ResolveForEditing_NoEditableOrCurrentRevision_ReturnsNull()
	{
		var revisions = new[]
		{
			CreatePrototype(10, 1, "Test Train", RevisionStatus.Revised),
			CreatePrototype(10, 2, "Test Train", RevisionStatus.Obsolete)
		};

		var result = VehicleModule.ResolveVehiclePrototypeRevisionForEditing(revisions, "10");

		Assert.IsNull(result);
	}

	private static IVehiclePrototype CreatePrototype(
		long id,
		int revision,
		string name,
		RevisionStatus status)
	{
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Id).Returns(id);
		prototype.SetupGet(x => x.RevisionNumber).Returns(revision);
		prototype.SetupGet(x => x.Name).Returns(name);
		prototype.SetupGet(x => x.Status).Returns(status);
		return prototype.Object;
	}
}
