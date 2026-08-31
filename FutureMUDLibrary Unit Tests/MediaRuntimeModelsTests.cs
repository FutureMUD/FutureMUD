#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Computers;

namespace FutureMUDLibrary_Unit_Tests.Computers;

[TestClass]
public class MediaRuntimeModelsTests
{
	[TestMethod]
	public void MediaEndpointAddress_IsValidOnlyWithStableItemComponentAndKey()
	{
		var address = new MediaEndpointAddress(42L, 19L, "camera-out");

		Assert.IsTrue(address.IsValid);
		Assert.IsFalse(MediaEndpointAddress.Empty.IsValid);
		Assert.IsFalse(new MediaEndpointAddress(42L, 0L, "camera-out").IsValid);
		Assert.IsFalse(new MediaEndpointAddress(42L, 19L, " ").IsValid);
	}

	[TestMethod]
	public void MediaPacket_WithProvenance_PreservesStructuredLanguageAndOriginalPacket()
	{
		var source = new MediaEndpointAddress(42L, 19L, "camera-out");
		var sink = new MediaEndpointAddress(99L, 77L, "monitor-in", MediaEndpointDirection.Input);
		var payload = new MediaLanguagePayload(false, 11L, 12L, "Hello there", 5, 72, 33L,
			"Ava", 1, "quietly", "smiling");
		var packet = new MediaPacket(Guid.NewGuid(), 7L, DateTime.UtcNow,
			MediaCapabilities.Audio | MediaCapabilities.Video, MediaEventKind.AudioVideo, source,
			new[] { source }, payload);

		var forwarded = packet.WithProvenance(sink);

		Assert.AreEqual(1, packet.Provenance.Count);
		Assert.AreEqual(2, forwarded.Provenance.Count);
		Assert.IsFalse(packet.HasVisited(sink));
		Assert.IsTrue(forwarded.HasVisited(sink));
		Assert.AreEqual(payload, forwarded.Payload);
		Assert.AreEqual("Hello there", ((MediaLanguagePayload)forwarded.Payload).RawText);
		Assert.AreEqual(11L, ((MediaLanguagePayload)forwarded.Payload).LanguageId);
		Assert.AreEqual(12L, ((MediaLanguagePayload)forwarded.Payload).AccentOrVarietyId);
	}
}
