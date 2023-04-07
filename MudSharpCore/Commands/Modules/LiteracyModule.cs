using System;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Commands.Modules;

public class LiteracyModule : Module<ICharacter>
{
	private LiteracyModule()
		: base("Literacy")
	{
		IsNecessary = true;
	}

	public static LiteracyModule Instance { get; } = new();

	[PlayerCommand("Scripts", "scripts")]
	protected static void Scripts(ICharacter actor, string command)
	{
		if (!actor.IsLiterate)
		{
			actor.Send("You're illiterate. You don't know anything about fancy squiggly lines.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("You know the following scripts:");
		foreach (var script in actor.Scripts)
		{
			sb.AppendLine(
				$"\t{script.Name.Colour(Telnet.Cyan)}: {script.KnownScriptDescription}{(script == actor.CurrentScript ? $" ({"Current".Colour(Telnet.Green)})" : "")}");
		}

		actor.Send(sb.ToString());
	}

	[PlayerCommand("Read", "read")]
	protected static void Read(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to read?");
			return;
		}

		var target = actor.TargetItem(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anything like that to read.");
			return;
		}

		var targetAsReadable = target.GetItemType<IReadable>();
		if (targetAsReadable == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be read.");
			return;
		}

		var targetAsOpenable = target.GetItemType<IOpenable>();
		if (targetAsOpenable != null && !targetAsOpenable.IsOpen)
		{
			actor.Send($"{target.HowSeen(actor, true)} must first be opened before it can be read.");
			return;
		}

		var sb = new StringBuilder();
		if (ss.IsFinished)
		{
			var textNumber = 1;
			foreach (var item in targetAsReadable.Readables)
			{
				if (item is IWriting writingItem)
				{
					sb.AppendLine($"Writing #{textNumber++:N0}: {actor.GetWritingHeader(writingItem)}"
						.ColourIncludingReset(Telnet.Yellow));
					if (actor.CanRead(writingItem))
					{
						sb.AppendLine(new string(actor.Account.UseUnicode ? '═' : '=', actor.InnerLineFormatLength));
						sb.AppendLine(writingItem.ParseFor(actor));
					}
					else
					{
						sb.AppendLine($"***{actor.WhyCannotRead(writingItem)}***".Colour(Telnet.Red));
					}
				}
				else if (item is IDrawing drawingItem)
				{
					sb.AppendLine(
						$"Drawing #{(textNumber++).ToString("N0", actor)}: {drawingItem.ShortDescription} ({drawingItem.DrawingSize.DescribeEnum()})"
							.ColourIncludingReset(Telnet.Yellow));
					sb.AppendLine(new string(actor.Account.UseUnicode ? '═' : '=', actor.InnerLineFormatLength));
					sb.AppendLine(drawingItem.ParseFor(actor));
				}

				sb.AppendLine();
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		if (!int.TryParse(ss.Pop(), out var value))
		{
			actor.Send(
				"If you specify an additional argument for read, it must be the number of the writing or drawing on that item that you wish to view.");
			return;
		}

		var readable = targetAsReadable.Readables.ElementAtOrDefault(value - 1);
		if (readable == null)
		{
			actor.Send(
				$"There are only {targetAsReadable.Readables.Count():N0} writings or drawings available to read on that item.");
			return;
		}

		if (readable is IWriting writing)
		{
			sb.AppendLine($"Writing #{value:N0}: {actor.GetWritingHeader(writing)}");
			if (actor.CanRead(writing))
			{
				sb.AppendLine(new string(actor.Account.UseUnicode ? '═' : '=', actor.InnerLineFormatLength));
				sb.AppendLine(readable.ParseFor(actor));
			}
			else
			{
				sb.AppendLine($"***{actor.WhyCannotRead(writing)}***".Colour(Telnet.Red));
			}
		}
		else if (readable is IDrawing drawing)
		{
			sb.AppendLine(
				$"Drawing #{value.ToString("N0", actor)}: {drawing.ShortDescription} ({drawing.DrawingSize.DescribeEnum()})");
			sb.AppendLine(new string(actor.Account.UseUnicode ? '═' : '=', actor.InnerLineFormatLength));
			sb.AppendLine(drawing.ParseFor(actor));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	[PlayerCommand("Write", "write")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Write(ICharacter actor, string command)
	{
		if (!actor.IsLiterate)
		{
			actor.Send("You're illiterate. You won't be writing anything.");
			return;
		}

		if (actor.CurrentScript == null)
		{
			actor.Send("You must first decide what script you will use when writing, before you can write.");
			actor.Send(
				$"See {"help script".FluentTagMXP("send", "href='help script'")} for more information".Colour(
					Telnet.Yellow));
			return;
		}

		if (actor.CurrentWritingLanguage == null)
		{
			actor.Send("You must first decide what language you will use when writing, before you can write.");
			actor.Send(
				$"See {"help script".FluentTagMXP("send", "href='help script'")} for more information".Colour(
					Telnet.Yellow));
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What do you want to write on?");
			return;
		}

		var target = actor.TargetItem(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anything like that to write on.");
			return;
		}

		var targetAsWritable = target.GetItemType<IWriteable>();
		if (targetAsWritable == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be written on.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var implement = actor.Body.HeldOrWieldedItems.SelectNotNull(x => x.GetItemType<IWritingImplement>())
		                     .FirstOrDefault();
		if (implement == null)
		{
			actor.Send(
				"You must be holding a writing implement of some description that matches the thing you want to write on.");
			return;
		}

		if (!targetAsWritable.CanWrite(actor, implement, null))
		{
			actor.Send(targetAsWritable.WhyCannotWrite(actor, implement, null));
			return;
		}

		actor.EditorMode((text, handler, pars) =>
			{
				actor.RemoveAllEffects(x => x.IsEffectType<StoredEditorText>());
				actor.AddEffect(new StoredEditorText(actor, text), TimeSpan.FromMinutes(30));

				// First, check they still have the writing implement and can still see the writeable
				if (!actor.Body.HeldOrWieldedItems.Contains(implement.Parent))
				{
					handler.Send(
						"You seem to have lost your writing implement while you were pondering what to write.");
					return;
				}

				if ((!actor.CanSee(target) && target.TrueLocations.All(x => x.Location != actor.Location)) ||
				    target.Destroyed)
				{
					handler.Send("The thing you were writing on is no longer there.");
					return;
				}

				var writing = new SimpleWriting(actor.Gameworld, actor, text)
				{
					WritingColour = implement.WritingImplementColour, ImplementType = implement.WritingImplementType
				};
				if (!targetAsWritable.CanWrite(actor, implement, writing))
				{
					handler.Send(targetAsWritable.WhyCannotWrite(actor, implement, writing));
					actor.Gameworld.SaveManager.Abort(writing);
					return;
				}

				targetAsWritable.Write(actor, implement, writing);
				handler.Handle(new EmoteOutput(new Emote("@ write|writes on $0 with $1.", actor, target,
					implement.Parent)));
			}, (handler, pars) => { handler.Send("You decide not to write anything."); },
			actor.CurrentScript.DocumentLengthModifier);
	}

	[PlayerCommand("DrawPicture", "drawpicture")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void DrawPicture(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What do you want to draw on?");
			return;
		}

		var target = actor.TargetItem(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anything like that to draw on.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var targetAsWritable = target.GetItemType<IWriteable>();
		if (targetAsWritable == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be drawn on.");
			return;
		}

		var implement = actor.Body.HeldOrWieldedItems.SelectNotNull(x => x.GetItemType<IWritingImplement>())
		                     .FirstOrDefault();
		if (implement == null)
		{
			actor.Send(
				"You must be holding a writing implement of some description that matches the thing you want to draw on.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a size of the drawing that you want to draw. The valid sizes and their equivalent amount of document length consumption are: {Enum.GetValues(typeof(DrawingSize)).OfType<DrawingSize>().Select(x => $"{x.DescribeEnum().ColourName()} ({x.DocumentLength().ToString("N0", actor).ColourValue()})").ListToString()}.");
			return;
		}

		if (!ss.PopSpeech().TryParseEnum<DrawingSize>(out var size))
		{
			actor.OutputHandler.Send(
				$"That is not a valid drawing size. The valid sizes and their equivalent amount of document length consumption are: {Enum.GetValues(typeof(DrawingSize)).OfType<DrawingSize>().Select(x => $"{x.DescribeEnum().ColourName()} ({x.DocumentLength().ToString("N0", actor).ColourValue()})").ListToString()}.");
			return;
		}

		var dummy = new DummyDrawing
			{ DrawingSize = size, ImplementType = implement.WritingImplementType, Author = actor };

		if (!targetAsWritable.CanDraw(actor, implement, dummy))
		{
			actor.Send(targetAsWritable.WhyCannotDraw(actor, implement, dummy));
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify a short description for your picture, in a format like \"a picture of a beautiful flower\"");
			return;
		}

		var sdesc = ss.PopSpeech();
		if (sdesc.Length > 80)
		{
			actor.OutputHandler.Send("The maximum length for short descriptions is 80 characters.");
			return;
		}

		actor.EditorMode((text, handler, pars) =>
		{
			actor.RemoveAllEffects(x => x.IsEffectType<StoredEditorText>());
			actor.AddEffect(new StoredEditorText(actor, text), TimeSpan.FromMinutes(30));
			// First, check they still have the writing implement and can still see the writeable
			if (!actor.Body.HeldOrWieldedItems.Contains(implement.Parent))
			{
				handler.Send("You seem to have lost your writing implement while you were pondering what to draw.");
				return;
			}

			if ((!actor.CanSee(target) && target.TrueLocations.All(x => x.Location != actor.Location)) ||
			    target.Destroyed)
			{
				handler.Send("The thing you were drawing on is no longer there.");
				return;
			}

			var drawing = new Drawing(actor,
				actor.TraitValue(actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("DrawingTraitId"))), sdesc,
				text, implement.WritingImplementType, size);
			if (!targetAsWritable.CanDraw(actor, implement, drawing))
			{
				handler.Send(targetAsWritable.WhyCannotDraw(actor, implement, drawing));
				actor.Gameworld.SaveManager.Abort(drawing);
				return;
			}

			targetAsWritable.Draw(actor, implement, drawing);
			handler.Handle(new EmoteOutput(new Emote("@ draw|draws on $0 with $1.", actor, target, implement.Parent)));
		}, (handler, pars) => { handler.Send("You decide not to draw anything."); }, 1.0);
	}

	[PlayerCommand("Title", "title")]
	protected static void Title(ICharacter actor, string command)
	{
		if (!actor.IsLiterate)
		{
			actor.Send("You're illiterate, you don't know anything about titling works.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("help") || ss.Peek().EqualTo("?"))
		{
			actor.Send($"The syntax for this command is {"title <item> <title>".Colour(Telnet.Yellow)}");
			return;
		}

		var target = actor.TargetItem(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You don't see anything like that to title.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var targetAsWritable = target.GetItemType<IWriteable>();
		if (targetAsWritable == null)
		{
			actor.Send("{0} is not something that can be given titles.", target.HowSeen(actor));
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What title do you want to give to {0}?", target.HowSeen(actor));
			return;
		}

		if (!targetAsWritable.CanGiveTitle(actor, ss.SafeRemainingArgument))
		{
			actor.Send(targetAsWritable.WhyCannotGiveTitle(actor, ss.SafeRemainingArgument));
			return;
		}

		var oldTitle = targetAsWritable.Title ?? string.Empty;
		targetAsWritable.GiveTitle(actor, ss.SafeRemainingArgument);
		if (oldTitle.Equals(string.Empty))
		{
			actor.Send($"You give the title of \"{targetAsWritable.Title}\" to {{0}}", target.HowSeen(actor));
		}
		else
		{
			actor.Send(
				$"You change the title from \"{oldTitle.Colour(Telnet.Green)}\" to \"{targetAsWritable.Title.Colour(Telnet.Green)}\"");
		}
	}
}