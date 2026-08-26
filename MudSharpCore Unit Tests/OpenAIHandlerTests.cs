#nullable enable
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.OpenAI;

namespace MudSharpCore_Unit_Tests;

[TestClass]
public class OpenAIHandlerTests
{
	[TestMethod]
	public async Task RunGptRequestAsync_Success_InvokesSuccessCallback()
	{
		string? response = null;
		Exception? failure = null;

		await OpenAIHandler.RunGptRequestAsync(_ => Task.FromResult("response"),
			text => response = text,
			exception => failure = exception,
			TimeSpan.FromSeconds(1));

		Assert.AreEqual("response", response);
		Assert.IsNull(failure);
	}

	[TestMethod]
	public async Task RunGptRequestAsync_RequestThrows_InvokesErrorCallback()
	{
		bool successCalled = false;
		Exception? failure = null;

		await OpenAIHandler.RunGptRequestAsync(_ => throw new InvalidOperationException("failure"),
			_ => successCalled = true,
			exception => failure = exception,
			TimeSpan.FromSeconds(1));

		Assert.IsFalse(successCalled);
		Assert.IsInstanceOfType(failure, typeof(InvalidOperationException));
	}

	[TestMethod]
	public async Task RunGptRequestAsync_RequestTimesOut_InvokesErrorCallback()
	{
		bool successCalled = false;
		Exception? failure = null;

		await OpenAIHandler.RunGptRequestAsync(
			async cancellationToken =>
			{
				await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
				return string.Empty;
			},
			_ => successCalled = true,
			exception => failure = exception,
			TimeSpan.FromMilliseconds(20));

		Assert.IsFalse(successCalled);
		Assert.IsInstanceOfType(failure, typeof(OperationCanceledException));
	}

	[TestMethod]
	public void DescribeGptFailure_NestedNetworkFailure_ReportsSecureConnectionProblem()
	{
		Exception exception = new AggregateException(
			new HttpRequestException("transport", new System.Security.Authentication.AuthenticationException("TLS")));

		string result = OpenAIHandler.DescribeGptFailure(exception, "client-reference");

		StringAssert.Contains(result, "secure network connection");
		StringAssert.Contains(result, "client-reference");
	}
}
