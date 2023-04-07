using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Server;

internal class FutureMUDConsoleWriter : IDisposable
{
	private FileStream fileStream;
	private StreamWriter fileWriter;
	private TextWriter doubleWriter;
	private TextWriter oldOut;

	private class DoubleWriter : TextWriter
	{
		private TextWriter one;
		private TextWriter two;

		public DoubleWriter(TextWriter one, TextWriter two)
		{
			this.one = one;
			this.two = two;
		}

		public override Encoding Encoding => one.Encoding;

		public override void Flush()
		{
			one.Flush();
			two.Flush();
		}

		public override void Write(char value)
		{
			one.Write(value);
			two.Write(value);
		}
	}

	public FutureMUDConsoleWriter(string path)
	{
		oldOut = Console.Out;

		try
		{
			fileStream = File.Create(path);

			fileWriter = new StreamWriter(fileStream);
			fileWriter.AutoFlush = true;

			doubleWriter = new DoubleWriter(fileWriter, oldOut);
		}
		catch (Exception e)
		{
			Console.WriteLine("Cannot open file for writing");
			Console.WriteLine(e.Message);
			return;
		}

		Console.SetOut(doubleWriter);
	}

	public void Dispose()
	{
		Console.SetOut(oldOut);
		if (fileWriter != null)
		{
			fileWriter.Flush();
			fileWriter.Close();
			fileWriter = null;
		}

		if (fileStream != null)
		{
			fileStream.Close();
			fileStream = null;
		}
	}
}